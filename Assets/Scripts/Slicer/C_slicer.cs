using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_slicer
{
    #region attribut
    private C_mesh[] a_mesh = new C_mesh[3];
    private C_trianglePlaneIntersection a_trianglePlaneIntersection;
    private Vector3[] a_intersectionPair;
    private int a_size;

    private List<Vector3> a_addedPairs;
    private Vector3[] a_tempTriangle;
    private float a_threshold = 1e-6f;

    private List<Vector3> a_oldPoints;
    private List<int> a_oldTriangles;
    private List<Vector3> a_oldNormals;
    private List<Vector2> a_oldUvs;

    //private ;
    #endregion

    #region getter/setter
    public C_mesh[] m_mesh { get => a_mesh; set => a_mesh = value; }
    public Vector3[] m_intersectionPair { get => a_intersectionPair; set => a_intersectionPair = value; }
    public int m_size { get => a_size; set => a_size = value; }
    public C_trianglePlaneIntersection m_trianglePlaneIntersection { get => a_trianglePlaneIntersection; set => a_trianglePlaneIntersection = value; }
    public List<Vector3> m_addedPairs { get => a_addedPairs; set => a_addedPairs = value; }
    public Vector3[] m_tempTriangle { get => a_tempTriangle; set => a_tempTriangle = value; }
    public List<Vector3> m_oldPoints { get => a_oldPoints; set => a_oldPoints = value; }
    #endregion

    #region constructor
    public C_slicer(int p_size)
    {
        this.a_size = p_size;
        this.a_mesh[0] = new C_mesh(this.m_size);
        this.a_mesh[1] = new C_mesh(this.m_size);
        this.a_mesh[2] = new C_mesh(this.m_size);

        this.a_intersectionPair = new Vector3[2];
        this.a_tempTriangle = new Vector3[3];
        this.a_trianglePlaneIntersection = new C_trianglePlaneIntersection();
        this.a_addedPairs = new List<Vector3>(this.m_size);

        this.a_oldPoints = new List<Vector3>(this.m_size);
        this.a_oldNormals = new List<Vector3>(this.m_size);
        this.a_oldUvs = new List<Vector2>(this.m_size);
        this.a_oldTriangles = new List<int>(this.m_size * 3);
    }
    #endregion

    #region clear (face mesh / back mesh / added pairs)
    private void m_clear()
    {
        this.m_addedPairs.Clear();
        this.m_mesh[0].Clear(); //face
        this.m_mesh[1].Clear(); //back
    }
    #endregion

    #region intersection bound plane
    private bool m_intersectionBoundPlane(Mesh p_mesh, ref Plane p_slice)
    {
        return C_defineCalculIntersection.m_isMeshIntersectPlane(p_mesh, ref p_slice);
    }
    #endregion

    #region vertex for separatation
    private void m_meshVertexSeparation()
    {

    }
    #endregion

    #region Separate old vertices in new meshes 
    private void m_separateOldVerticesOnNewMesh(ref Plane p_slice)
    {
        for (int i = 0; i < this.a_oldPoints.Count; i++)
        {
            if (p_slice.GetDistanceToPoint(this.a_oldPoints[i]) >= 0)
            {
                this.m_mesh[0].m_dictionayTuple(i);
                this.m_mesh[0].m_addVertex(this.a_oldPoints[i], this.a_oldNormals[i], this.a_oldUvs[i]);
            }
            else
            {
                this.m_mesh[1].m_dictionayTuple(i);
                this.m_mesh[1].m_addVertex(this.a_oldPoints[i], this.a_oldNormals[i], this.a_oldUvs[i]);
            }
        }
    }
    #endregion

    #region separate triangle cut intersection
    private void m_separateTriangleCutIntersection(ref Plane p_slice)
    {
        for (int i = 0; i < this.a_oldTriangles.Count; i = i + 3)
        {
            bool v_intersection = this.m_trianglePlaneIntersection.m_ifTrianglePlaneIntersection(this.a_oldPoints, this.a_oldUvs, this.a_oldTriangles, i, ref p_slice, this.m_mesh[0], this.m_mesh[1], this.m_intersectionPair); //
            if (v_intersection)
            {
                this.m_addedPairs.AddRange(this.m_intersectionPair);
            }
        }
    }
    #endregion



    #region is have vertices 
    private bool m_isHaveVertice(ref Plane p_slice)
    {
        bool to_return = false;
        if (!(this.m_mesh[1].m_vertices.Count == 0 || this.m_mesh[0].m_vertices.Count == 0))
        {
            // 3. Separate triangles and cut those that intersect the plane
            this.m_separateTriangleCutIntersection(ref p_slice);
            if (this.m_addedPairs.Count > 0)
            {
                this.m_fillBoundaryFace(this.m_addedPairs);
                to_return = true;
            }
        }
        return to_return;
    }
    #endregion

    #region slice mesh
    public bool m_sliceMesh(Mesh p_mesh, ref Plane p_slice)
    {
        bool to_return = false;
        p_mesh.GetVertices(this.a_oldPoints);
        if (this.m_intersectionBoundPlane(p_mesh, ref p_slice))
        {
            p_mesh.GetTriangles(this.a_oldTriangles, 0);
            p_mesh.GetNormals(this.a_oldNormals);
            p_mesh.GetUVs(0, this.a_oldUvs);
            this.m_clear();
            this.m_separateOldVerticesOnNewMesh(ref p_slice);
            to_return = this.m_isHaveVertice(ref p_slice);
        }
        return to_return;
    }
    #endregion

    #region Boundary fill method

    #region fill boundary general
    private void FillBoundaryGeneral(List<Vector3> p_added)
    {
        m_reorderPoints(p_added);
        Vector3 center = m_detectCenterPolygon(p_added);
        this.a_tempTriangle[2] = center;
        for (int i = 0; i < p_added.Count; i += 2)
        {
            this.a_tempTriangle[0] = p_added[i];
            this.a_tempTriangle[1] = p_added[i + 1];
            m_mesh[0].AddTriangle(this.a_tempTriangle);
            this.a_tempTriangle[0] = p_added[i + 1];
            this.a_tempTriangle[1] = p_added[i];
            this.m_mesh[2].AddTriangle(this.a_tempTriangle);
        }
    }
    #endregion

    #region fill boundary face
    private void m_fillBoundaryFace(List<Vector3> p_added)
    {
        m_reorderPoints(p_added);
        var face = this.m_findRealPolygon(p_added);
        int v_t1 = 0;
        int v_t2 = face.Count - 1;
        int v_t3 = 1;
        bool incr_t1 = true;
        while (v_t3 != v_t1 && v_t3 != v_t2)
        {
            this.m_addTriangle(face, v_t2, v_t1, v_t3);
            if (incr_t1) v_t1 = v_t3;
            else v_t2 = v_t3;

            incr_t1 = !incr_t1;
            v_t3 = incr_t1 ? v_t1 + 1 : v_t2 - 1;
        }
    }
    #endregion

    #region find real polygon
    private List<Vector3> m_findRealPolygon(List<Vector3> p_pairs)
    {
        List<Vector3> v_vertices = new List<Vector3>();
        Vector3 v_edge1;
        Vector3 v_edge2;
        for (int i = 0; i < p_pairs.Count; i += 2)
        {
            v_edge1 = (p_pairs[i + 1] - p_pairs[i]);
            if (i == p_pairs.Count - 2)
            {
                v_edge2 = p_pairs[1] - p_pairs[0];
            }
            else
            {
                v_edge2 = p_pairs[i + 3] - p_pairs[i + 2];
            }
            v_edge1.Normalize();
            v_edge2.Normalize();

            if (Vector3.Angle(v_edge1, v_edge2) > this.a_threshold)
            {
                v_vertices.Add(p_pairs[i + 1]);
            }
        }

        return v_vertices;
    }
    #endregion

    #region add triangle
    private void m_addTriangle(List<Vector3> face, int t1, int t2, int t3)
    {
        this.a_tempTriangle[0] = face[t1];
        this.a_tempTriangle[1] = face[t2];
        this.a_tempTriangle[2] = face[t3];
        this.m_mesh[0].AddTriangle(this.a_tempTriangle);
        this.a_tempTriangle[1] = face[t3];
        this.a_tempTriangle[2] = face[t2];
        this.m_mesh[1].AddTriangle(this.a_tempTriangle);
    }
    #endregion

    #endregion

    #region detect center polygon
    public static Vector3 m_detectCenterPolygon(List<Vector3> p_pairs)
    {
        Vector3 v_center = Vector3.zero;
        int v_count = 0;
        for (int i = 0; i < p_pairs.Count; i += 2)
        {
            v_center += p_pairs[i];
            v_count++;
        }
        return v_center / v_count;
    }
    #endregion

    #region reorder points
    public static void m_reorderPoints(List<Vector3> p_pairs)
    {
        int v_nbFaces = 0;
        int v_faceStart = 0;
        int i = 0;

        while (i < p_pairs.Count)
        {
            for (int j = i + 2; j < p_pairs.Count; j += 2)
            {
                if (p_pairs[j] == p_pairs[i + 1])
                {
                    m_inversePoints(p_pairs, i + 2, j);
                    break;
                }
            }

            if (p_pairs[i + 3] == p_pairs[v_faceStart])
            {
                v_nbFaces++;
                i += 4;
                v_faceStart = i;
            }
            else if (!((i + 3) >= p_pairs.Count))
            {
                i += 2;
            }
        }
    }
    #endregion

    #region inverse points with tempo 
    private static void m_inversePoints(List<Vector3> p_points, int p_pos1, int p_pos2)
    {
        if (p_pos1.Equals(p_pos2)) return;
        Vector3 v_tempon1 = p_points[p_pos1];
        Vector3 v_tempon2 = p_points[p_pos1 + 1];
        p_points[p_pos1] = p_points[p_pos2];
        p_points[p_pos1 + 1] = p_points[p_pos2 + 1];
        p_points[p_pos2] = v_tempon1;
        p_points[p_pos2 + 1] = v_tempon2;
    }
    #endregion
}