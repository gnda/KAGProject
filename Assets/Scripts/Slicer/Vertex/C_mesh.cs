using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class C_mesh
{
    #region Attribut 
    private List<C_vertex>          a_vertices  ;
    private int                     a_size      ;
    private List<int>               a_triangles ;
    private Dictionary<int, int>    a_dictionary; 
    private float                   a_surface   ;
    #endregion

    #region getter and setter
    public  List<C_vertex>      m_vertices      { get => a_vertices     ; set => a_vertices     = value; }
    public  List<int>           m_triangles     { get => a_triangles    ; set => a_triangles    = value; }
    private int                 m_size          { get => a_size         ; set => a_size         = value; }
    private Dictionary<int,int> m_dictionary    { get => a_dictionary   ; set => a_dictionary   = value; }
    public  float               m_surface       { get => a_surface      ; set => a_surface      = value; }
    #endregion

    #region constuctor
    public C_mesh(int p_size)
    {
        this.a_size = p_size;
        this.a_vertices = new List<C_vertex>(this.m_size);
        this.a_triangles = new List<int>(this.m_size * 3);
        this.a_dictionary = new Dictionary<int, int>(this.m_size);
        this.a_surface = 0.0f;
    }
    #endregion

    #region clear
    public void Clear()
    {
        this.m_vertices.Clear();
        this.m_triangles.Clear();
        this.m_dictionary.Clear();
        this.m_surface = 0.0f;
    }
    #endregion

    #region vertex and triangle
    private void m_addVertexTriangle(Vector3 p_point, Vector3 p_normal, Vector2 p_uv)
    {
        this.m_addTriangle(this.m_vertices.Count);
        this.m_addVertex(p_point, p_normal, p_uv);
    }
    public void m_addVertex(Vector3 p_point, Vector2 p_uv)
    {
        //Debug.Log("add vertex me ");
        C_vertex v_vertex = new C_vertex(p_point, p_uv);
        this.m_vertices.Add(v_vertex);

    }
    public void m_addVertex(Vector3 p_point, Vector3 p_normal, Vector2 p_uv)
    {
        //Debug.Log("add vertex me ");
        C_vertex v_vertex = new C_vertex(p_point, p_normal, p_uv); 
        this.m_vertices.Add(v_vertex);
        
    }

    public void m_addTriangle(int p_triangle)
    {
        this.m_triangles.Add(p_triangle); 
    }
    #endregion

    #region normal cross
    private Vector3 m_normalCross(Vector3 p_v2, Vector3 p_v1, Vector3 p_v3)
    {
        return Vector3.Cross(p_v2-p_v1,p_v3-p_v2).normalized;
    }
    #endregion

    #region add sliced triangle 
    private Vector3 m_addPartedTriangle(int p_indice, Vector3 p_v2, Vector3 p_uv2, Vector3 p_v3)
    {
        Vector3 v_normal = this.m_normalCross(p_v2, this.m_vertices[this.m_dictionary[p_indice]].m_point, p_v3);
        this.m_addTriangle(this.m_dictionary[p_indice]);
        this.m_addVertexTriangle(p_v2, v_normal, p_uv2);
        return v_normal;
    }
    public void m_addSlicedTriangle(int p_indice, Vector3 p_v2, Vector2 p_uv2, int p_indice3)
    {
        this.m_addPartedTriangle(p_indice, p_v2, p_uv2, this.m_vertices[this.m_dictionary[p_indice3]].m_point);
        this.m_addTriangle(this.m_dictionary[p_indice3]); 
        this.m_updateSurfaceArea();
    }
    public void m_addSlicedTriangle(int p_indice, Vector3 p_v2, Vector3 p_v3, Vector2 p_uv2, Vector2 p_uv3)
    {
        Vector3 v_normal = this.m_addPartedTriangle(p_indice, p_v2, p_uv2, p_v3);
        this.m_addVertexTriangle(p_v3, v_normal, p_uv3);
        this.m_updateSurfaceArea();
    }
    #endregion

    #region add original triangle
    public void AddOgTriangle(int[] p_indice)
    {
        int v_value;
        for (int i = 0; i < 3; i++)
        {
            v_value = this.m_dictionary[p_indice[i]];
            this.m_addTriangle(v_value);
        }
        this.m_updateSurfaceArea();
    }
    #endregion

    #region add initial triangle
    public void AddInitialTriangle(List<int> p_indice)
    {
        int v_triangle;
        for (int i = 0; i < 3; i++)
        {
            v_triangle = this.m_dictionary[p_indice[i]];
            this.m_addTriangle(v_triangle);
        }
        this.m_updateSurfaceArea();
    }
    #endregion

    #region add triangle
    public void AddTriangle(Vector3[] p_points)
    {
        Vector3 v_normal = this.m_normalCross(p_points[1], p_points[0], p_points[2]);
        for (int i = 0; i < p_points.Length; i++) 
        {
            this.m_addVertexTriangle(p_points[i], v_normal, Vector2.zero);
        }
        this.m_updateSurfaceArea();
    }
    #endregion

    #region update surface area
    private void m_updateSurfaceArea()
    {
        this.m_surface = this.m_surface + this.m_getTriangleArea(this.m_triangles.Count - 3);
    }
    #endregion

    #region contains Keys
    //storage vertex
    public void m_containsKeys(List<int> p_triangles, int p_indiceBegin, bool[] p_face)
    {
        //Debug.Log("************************** begin contains key ");
        //if (p_positive.Length == p_triangles.Count)
        //{
        bool b;

        for (int i = 0; i < 3; i++)
        {
            b = this.m_dictionary.ContainsKey(p_triangles[p_indiceBegin + i]);
            //Debug.Log("p_triangles ... : "+p_triangles[p_indiceBegin + i] + " b: "+b);
            p_face[i] = this.m_dictionary.ContainsKey(p_triangles[p_indiceBegin + i]);
        }
        //}
        //else
        //{
        //error
        //}
        //Debug.Log("******************* end contains key ");
    }
    #endregion

    #region dictionary Tuple
    public void m_dictionayTuple(int p_indice)
    {
        //Debug.Log("dictionay tuple");
        this.m_dictionary[p_indice] = this.m_vertices.Count;
    }
    #endregion

    #region get triangle area
    private float m_getTriangleArea(int p_indice)
    {
        //Debug.Log("triangle area");
        var v_va = this.m_vertices[this.m_triangles[p_indice + 2]].m_point - this.m_vertices[this.m_triangles[p_indice]].m_point;
        var v_vb = this.m_vertices[this.m_triangles[p_indice + 1]].m_point - this.m_vertices[this.m_triangles[p_indice]].m_point;
        //Vector3.Angle(from,to) from/to which the angular difference is measured
        float v_gamma = Mathf.Deg2Rad * Vector3.Angle(v_vb, v_va);
        return v_va.magnitude * v_vb.magnitude * Mathf.Sin(v_gamma) / 2;
    }
    #endregion

    #region all point
    public List<Vector3> m_allPoint()
    {

        List<Vector3> v_list = new List<Vector3>(this.m_size);
        foreach (C_vertex v_vertex in this.m_vertices)
        {
            v_list.Add(v_vertex.m_point);
        }
        Debug.Log("m_all_point : " + v_list.Count+" m_size: "+this.m_size);
        return v_list;
    }
    #endregion

    #region all uv
    public List<Vector2> m_allUv()
    {
        List<Vector2> v_list = new List<Vector2>(this.m_size);
        foreach (C_vertex v_vertex in this.m_vertices)
        {
            v_list.Add(v_vertex.m_uv);
        }
        Debug.Log("m_all_uv : " + v_list.Count);
        return v_list;
    }
    #endregion

    #region all normal
    public List<Vector3> m_allNormal()
    {
        List<Vector3> v_list = new List<Vector3>(this.m_size);
        foreach (C_vertex v_vertex in this.m_vertices)
        {
            v_list.Add(v_vertex.m_normal);
        }
        Debug.Log("m_all_normal : " + v_list.Count);
        return v_list;
    }
    #endregion

    #region is key on the object
    public void m_isKeyOn(List<int> p_triangles, int p_indiceBegin, bool[] p_face)
    {
        //Debug.Log("************************** begin contains key ");
        //if (p_positive.Length == p_triangles.Count)
        //{
        bool b;

        for (int i = 0; i < 3; i++)
        {
            b = this.m_dictionary.ContainsKey(p_triangles[p_indiceBegin + i]);

            //Debug.Log("p_triangles ... : " + p_triangles[p_indiceBegin + i] + " b: " + b);
            p_face[i] = this.m_dictionary.ContainsKey(p_triangles[p_indiceBegin + i]);
        }
        //}
        //else
        //{
        //error
        //}
        //Debug.Log("******************* end contains key ");
    }
    #endregion

    #region setmesh
    public Mesh m_setMesh(Mesh p_mesh)
    {
        p_mesh.Clear();
        p_mesh.SetVertices(this.m_allPoint());
        p_mesh.SetTriangles(this.m_triangles, 0);
        p_mesh.SetNormals(this.m_allNormal());
        p_mesh.SetUVs(0, this.m_allUv());
        p_mesh.RecalculateTangents();
        return p_mesh;
    }
    #endregion

    #region getmesh
    public Mesh m_getMesh(Mesh p_mesh)
    {
        p_mesh.Clear();
        p_mesh.SetVertices(this.m_allPoint());
        p_mesh.SetTriangles(this.m_triangles, 0);
        p_mesh.SetNormals(this.m_allNormal());
        p_mesh.SetUVs(0, this.m_allUv());
        p_mesh.RecalculateTangents();
        return p_mesh;
    }
    #endregion

    #region print
    public String m_println()
    {
        return "m_vertices : "+this.m_vertices+" | triangle: "+this.m_triangles;
    }
    #endregion
}