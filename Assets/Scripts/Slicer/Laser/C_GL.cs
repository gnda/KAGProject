using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class C_GL{

    // Will be called after all regular rendering is done
    public static void m_onRenderObject(bool p_drag, Material p_line, Vector3 p_end, Vector3 p_begin)
    {
        if (p_drag && p_line)
        {
            GL.PushMatrix();
            p_line.SetPass(0);
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

                GL.Vertex3(p_begin.x + i, p_begin.y + i, p_begin.z + i);
                GL.Vertex3(p_end.x + i, p_end.y + i, p_end.z + i);
            }
            GL.End();
            GL.PopMatrix();
        }
    }
}
