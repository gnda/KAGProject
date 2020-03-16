using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_vertex
{
    #region attribut
    private Vector3 a_point;
    private Vector3 a_normal;
    private Vector2 a_uv    ;
    #endregion

    #region getter setter
    public Vector3 m_point  { get => a_point;   set => a_point = value;  }
    public Vector3 m_normal { get => a_normal;  set => a_normal = value; }
    public Vector2 m_uv     { get => a_uv;      set => a_uv = value;     }
    #endregion

    #region constructor
    public C_vertex(Vector3 p_point, Vector3 p_normal, Vector2 p_uv )
    {
        this.a_normal   = p_normal  ;
        this.a_point    = p_point   ;
        this.a_uv       = p_uv      ; 
    }

    public C_vertex(Vector3 p_point, Vector2 p_uv)
    {
        this.a_point = p_point;
        this.a_uv = p_uv;
    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
