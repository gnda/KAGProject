using System;
using UnityEngine;

public class C_meshColumn
{
    #region attribut
    private int[] a_triangles;
    private Vector2[] a_uvs;
    private Vector3[] a_points;
    private Vector3[] a_normals;
    private int a_size;
    private bool[] a_isFace;
    private Ray a_edgeRay; // https://answers.unity.com/questions/1393405/x-ray-shader-cutting-a-hole-through-a-mesh.html | https://forum.unity.com/threads/splitting-a-mesh-in-two.452057/
    #endregion

    #region constructor
    public C_meshColumn(int p_size)
    {
        this.m_size = p_size;
        this.m_points = new Vector3[this.m_size];
        this.m_uvs = new Vector2[this.m_size];
        this.m_normals = new Vector3[this.m_size];
        this.m_triangles = new int[this.m_size];
        this.m_isFace = new bool[this.m_size];

    }
    #endregion

    #region getter/setter
    public int[] m_triangles { get => a_triangles; set => a_triangles = value; }
    public Vector2[] m_uvs { get => a_uvs; set => a_uvs = value; }
    public Vector3[] m_points { get => a_points; set => a_points = value; }
    public Vector3[] m_normals { get => a_normals; set => a_normals = value; }
    public bool[] m_isFace { get => a_isFace; set => a_isFace = value; }
    public int m_size { get => a_size; set => a_size = value; }
    public Ray m_edgeRay { get => a_edgeRay; set => a_edgeRay = value; }
    #endregion

    #region start
    // Start is called before the first frame update
    void Start()
    {
    }
    #endregion

    #region update
    // Update is called once per frame
    void Update()
    {
    }
    #endregion

    #region all is on the face or not 
    public bool m_isAllFace()
    {
        bool to_return = true;
        int i = 0;
        while ((i < (this.m_size - 1)) && to_return)
        {
            if (this.m_isFace[i] == this.m_isFace[i + 1])
            {
                to_return = true;
            }
            else
            {
                to_return = false;
            }
            i++;
        }
        return to_return;
    }
    #endregion

    #region is all are on the same side
    public bool m_isAllSameSide(ref Plane p_plane, C_mesh p_faceMesh, C_mesh p_backMesh, Vector3[] p_intersectVectors)
    {
        bool to_return;
        if (this.m_isAllFace())
        {
            (this.m_isFace[0] ? p_faceMesh : p_backMesh).AddOgTriangle(this.m_triangles);
            to_return = false;
        }
        else
        {
            this.m_sliceByFace(p_faceMesh, p_backMesh, p_intersectVectors, ref p_plane);
            to_return = true;
        }
        return to_return;
    }
    #endregion

    #region find points
    public int[] m_findPoints()
    {
        // Find point
        int[] v_value = new int[3];
        v_value[1] = 0;
        if (this.m_isFace[0] != this.m_isFace[1])
        {
            v_value[1] = this.m_isFace[0] != this.m_isFace[2] ? 0 : 1;
        }
        else
        {
            v_value[1] = 2;
        }
        v_value[0] = v_value[1] - 1;
        if (v_value[0] == -1) v_value[0] = 2;
        v_value[2] = v_value[1] + 1;
        if (v_value[2] == 3) v_value[2] = 0;
        return v_value;
    }
    #endregion

    #region slice by face
    public void m_sliceByFace(C_mesh p_faceMesh, C_mesh p_backMesh, Vector3[] p_intersectVectors, ref Plane p_plane)
    {
        int[] v_point = new int[3];
        v_point = this.m_findPoints();

        ValueTuple<Vector3, Vector2> v_newPointPrev = this.m_findIntersection(p_plane, this.m_points[v_point[1]], this.m_points[v_point[0]], this.m_uvs[v_point[1]], this.m_uvs[v_point[0]]);
        ValueTuple<Vector3, Vector2> v_newPointNext = this.m_findIntersection(p_plane, this.m_points[v_point[1]], this.m_points[v_point[2]], this.m_uvs[v_point[1]], this.m_uvs[v_point[2]]);

        (this.m_isFace[v_point[1]] ? p_faceMesh : p_backMesh).m_addSlicedTriangle(this.m_triangles[v_point[1]], v_newPointNext.Item1, v_newPointPrev.Item1, v_newPointNext.Item2, v_newPointPrev.Item2);
        (this.m_isFace[v_point[0]] ? p_faceMesh : p_backMesh).m_addSlicedTriangle(this.m_triangles[v_point[0]], v_newPointPrev.Item1, v_newPointPrev.Item2, this.m_triangles[v_point[2]]);
        (this.m_isFace[v_point[0]] ? p_faceMesh : p_backMesh).m_addSlicedTriangle(this.m_triangles[v_point[2]], v_newPointPrev.Item1, v_newPointNext.Item1, v_newPointPrev.Item2, v_newPointNext.Item2);
        this.m_intersectionVectors(p_intersectVectors, v_point, v_newPointPrev, v_newPointNext);
    }
    #endregion

    #region find intersection
    private ValueTuple<Vector3, Vector2> m_findIntersection(Plane p_plane, Vector3 p_point1, Vector3 p_point2, Vector2 p_uv1, Vector2 p_uv2)
    {
        a_edgeRay.origin = p_point1;
        a_edgeRay.direction = (p_point2 - p_point1).normalized;
        float dist;
        float maxDist = Vector3.Distance(p_point1, p_point2);
        p_plane.Raycast(m_edgeRay, out dist);
        var returnVal = new ValueTuple<Vector3, Vector2>{ Item1 = m_edgeRay.GetPoint(dist)};
        var relativeDist = dist / maxDist;
        returnVal.Item2.x = Mathf.Lerp(p_uv1.x, p_uv2.x, relativeDist);
        returnVal.Item2.y = Mathf.Lerp(p_uv1.y, p_uv2.y, relativeDist);
        return returnVal;
    }
    #endregion

    #region intersection vectors
    private Vector3[] m_intersectionVectors(Vector3[] p_intersectVectors, int[] p_point, ValueTuple<Vector3, Vector2> p_newPointPrev, ValueTuple<Vector3, Vector2> p_newPointNext)
    {
        if (this.m_isFace[p_point[0]])
        {
            p_intersectVectors[0] = p_newPointPrev.Item1;
            p_intersectVectors[1] = p_newPointNext.Item1;
        }
        else
        {
            p_intersectVectors[0] = p_newPointNext.Item1;
            p_intersectVectors[1] = p_newPointPrev.Item1;
        }
        return p_intersectVectors;
    }

    #endregion
}
