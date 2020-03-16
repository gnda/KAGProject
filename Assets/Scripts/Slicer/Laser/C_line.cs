using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_line : MonoBehaviour
{

    #region attribut

    public delegate void LineSYS(Vector3 begin, Vector3 end, Vector3 depth);
    public event LineSYS a_lineSYS;

    private bool a_isDragged;
    private Vector3 a_begin;
    private Vector3 a_end;
    private Camera a_camera;

    public Material lineMaterial;
    #endregion

    #region getter/setter
    public Camera m_camera { get => a_camera; set => a_camera = value; }
    public Vector3 m_end { get => a_end; set => a_end = value; }
    public Vector3 m_begin { get => a_begin; set => a_begin = value; }
    public bool m_isDragged { get => a_isDragged; set => a_isDragged = value; }
    #endregion

    #region start
    private void Start()
    {
        a_camera = Camera.main;
         this.m_isDragged = false;
    }
    #endregion
    #region on enable
    private void OnEnable()
    {
        Camera.onPostRender += m_GL;
    }
    #endregion
    #region on disable
    private void OnDisable()
    {
        Camera.onPostRender -= m_GL;
    }
    #endregion
    #region update
    void Update()
    {
        if (! this.m_isDragged && Input.GetMouseButtonDown(0))
        {
            this.m_begin = this.m_camera.ScreenToViewportPoint(Input.mousePosition);
             this.m_isDragged = true;
        }
        if ( this.m_isDragged)
        {
            this.m_end = this.m_camera.ScreenToViewportPoint(Input.mousePosition);
        }
        if ( this.m_isDragged && Input.GetMouseButtonUp(0))
        {
            this.m_end = this.m_camera.ScreenToViewportPoint(Input.mousePosition);
             this.m_isDragged = false;

            var startRay = this.m_camera.ViewportPointToRay(this.m_begin);
            var endRay = this.m_camera.ViewportPointToRay(this.m_end);
            a_lineSYS?.Invoke(startRay.GetPoint(this.m_camera.nearClipPlane),endRay.GetPoint(this.m_camera.nearClipPlane),endRay.direction.normalized);
        }
    }
    #endregion
    #region GL
    //multiple of GL lines to have "épaisseur"
    private void m_GL(Camera camera)
    {
        //C_GL.m_onRenderObject(this.m_isDrag,m_line,m_end,m_begin);
        if ( this.m_isDragged && lineMaterial)
        {
            GL.PushMatrix();
            lineMaterial.SetPass(0);
            GL.LoadOrtho();
            GL.Begin(GL.LINES);
            for (float i = -0.009f; i < 0.009f; i = i + 0.001f)
            {
                //float a = i / (float)100;
                //  float angle = a * Mathf.PI * 2;
                // Vertex colors change from red to green
                GL.Color(Color.red);

                //GL.Vertex3(0, 0, 0);
                //GL.Vertex3(Mathf.Cos(angle) * 3f, Mathf.Sin(angle) * 3f, 0);
                //}
                //GL.Color(Color.white);
                //float a = i / (float)lineCount;

                GL.Vertex3(this.m_begin.x + i, this.m_begin.y + i, this.m_begin.z + i);
                GL.Vertex3(this.m_end.x + i, this.m_end.y + i, this.m_end.z + i);
            }
            GL.End();
            GL.PopMatrix();
        }

    }
    #endregion
}