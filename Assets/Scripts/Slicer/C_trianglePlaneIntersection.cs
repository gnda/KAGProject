using System.Collections.Generic;
using UnityEngine;

public class C_trianglePlaneIntersection
{
    #region attribut
    private C_meshColumn a_meshColumn;
    private C_mesh a_mesh; //3 first
    #endregion

    #region getter/setter
    public C_mesh m_mesh { get => a_mesh; set => a_mesh = value; }
    public C_meshColumn m_meshColumn { get => a_meshColumn; set => a_meshColumn = value; }
    #endregion

    #region constructor
    public C_trianglePlaneIntersection()
    {
        this.m_mesh = new C_mesh(3);
        this.a_meshColumn = new C_meshColumn(3);
    }
    #endregion

    #region triangle plane intersection
    public bool m_ifTrianglePlaneIntersection(List<Vector3> p_points, List<Vector2> p_uvs, List<int> p_triangles, int p_beginIndice, ref Plane p_plane, C_mesh p_faceMesh, C_mesh p_backMesh, Vector3[] p_intersectVectors)
    {
        bool to_return = false;
        // We put triangle, point and uv from the beginIndices on the mesh
        this.m_addElement(p_points, p_uvs, p_triangles, p_beginIndice);
        p_faceMesh.m_containsKeys(p_triangles, p_beginIndice, this.m_meshColumn.m_isFace);
        to_return = this.m_meshColumn.m_isAllSameSide(ref p_plane, p_faceMesh, p_backMesh, p_intersectVectors);
        return to_return;
    }
    #endregion

    #region add element triangle/point/uv 
    private void m_addElement(List<Vector3> p_points, List<Vector2> p_uvs, List<int> p_triangles, int p_beginIndice)
    {
        for (int i = 0; i < 3; i++)
        {
            this.m_meshColumn.m_triangles[i] = p_triangles[p_beginIndice + i];
            this.m_meshColumn.m_points[i] = p_points[this.m_meshColumn.m_triangles[i]];
            this.m_meshColumn.m_uvs[i] = p_uvs[this.m_meshColumn.m_triangles[i]];
            this.m_mesh.m_addTriangle(p_triangles[p_beginIndice + i]);
            this.m_mesh.m_addVertex(p_points[this.m_mesh.m_triangles[i]], p_uvs[this.m_mesh.m_triangles[i]]);
        }
    }
    #endregion
}