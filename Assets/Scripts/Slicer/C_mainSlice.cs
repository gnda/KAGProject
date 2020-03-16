using System.Collections.Generic;
using UnityEngine;

public class C_mainSlice : MonoBehaviour
{
    #region attribut
    public GameObject a_plane;
    public Transform a_ObjectContainer;
    public float a_separation;
    private Plane a_slicePlane = new Plane();
    public bool a_drawPlane;
    public C_line a_line;
    private C_slicer a_slicer;
    private C_mesh[] a_mesh;
    private C_mesh biggerMesh;
    private C_mesh smallerMesh;
    #endregion

    #region constructor
    public C_slicer m_slicer { get => a_slicer; set => a_slicer = value; }
    public C_mesh[] m_mesh { get => a_mesh; set => a_mesh = value; }
    public C_line m_line { get => a_line; set => a_line = value; }
    public GameObject m_gameObjectPlane { get => a_plane; set => a_plane = value; }
    public Transform m_ObjectContainer { get => a_ObjectContainer; set => a_ObjectContainer = value; }
    public float m_separation { get => a_separation; set => a_separation = value; }
    public bool m_isNeedDrawPlane { get => a_drawPlane; set => a_drawPlane = value; }
    public Plane m_slicePlane { get => a_slicePlane; set => a_slicePlane = value; }
    #endregion

    #region draw plane

    private void m_drawPlane(Vector3 p_begin, Vector3 p_end, Vector3 p_normal)
    {
        this.m_gameObjectPlane.transform.localRotation = Quaternion.FromToRotation(Vector3.up, p_normal);
        this.m_gameObjectPlane.transform.position = (p_end + p_begin) / 2;
        this.m_gameObjectPlane.SetActive(true);
    }

    #endregion

    #region start
    void Start()
    {
        this.a_slicer = new C_slicer(256);

    }
    #endregion

    #region on enable
    private void OnEnable()
    {
        a_line.a_lineSYS += m_SYSLine;
    }
    #endregion

    #region on disable
    private void OnDisable()
    {
        a_line.a_lineSYS -= m_SYSLine;
    }
    #endregion

    #region SYS line 
    public void m_SYSLine(Vector3 p_begin, Vector3 p_end, Vector3 p_depth)
    {
        var v_tangante = (p_end - p_begin).normalized;
        if (v_tangante == Vector3.zero){
            v_tangante = Vector3.right;
        }
        var v_normal = Vector3.Cross(p_depth, v_tangante);
        if (this.m_isNeedDrawPlane) {
            this.m_drawPlane(p_begin, p_end, v_normal);
        }
        this.m_sliceObjects(p_begin, v_normal);
    }
    #endregion

    #region slice objecs with begin point
    void m_sliceObjects(Vector3 p_beginPoint, Vector3 p_normal)
    {
        var v_toSlice = GameObject.FindObjectsOfType<Sliceable>();
        List<Transform> v_positive = new List<Transform>();
        List<Transform> v_negative = new List<Transform>();
        GameObject      v_gameObject;
        bool            v_isSliced = false;
        for (int i = 0; i < v_toSlice.Length; i++)
        {
            v_gameObject = v_toSlice[i].gameObject;
            var transformedNormal = ((Vector3)(v_gameObject.transform.localToWorldMatrix.transpose * p_normal)).normalized;
            a_slicePlane.SetNormalAndPosition(transformedNormal, v_gameObject.transform.InverseTransformPoint(p_beginPoint));
            v_isSliced = this.m_sliceObject(ref a_slicePlane, v_gameObject, v_positive, v_negative) || v_isSliced;
            
            if (v_isSliced == true)
            {
                this.m_ecarteMesh(v_positive, v_negative, p_normal);
                //C_score.a_score += 100;
            }
        }
    }
    #endregion

    #region slice object
    private bool m_sliceObject(ref Plane p_slicePlane, GameObject p_gameObject, List<Transform> p_positiveObjects, List<Transform> p_negativeObjects)
    {
        bool to_return = false;
        bool v_posBigger = false;
        var v_mesh = p_gameObject.GetComponent<MeshFilter>().mesh;
        if (!this.a_slicer.m_sliceMesh(v_mesh, ref a_slicePlane))
        {
            if (a_slicePlane.GetDistanceToPoint(this.a_slicer.m_oldPoints[0]) >= 0)
            {
                this.m_addObject(ref p_positiveObjects, p_gameObject);
            }
            else
            {
                this.m_addObject(ref p_negativeObjects, p_gameObject);
            }
            to_return = false;
        }
        else
        {
            if (this.a_slicer.m_mesh[0].m_surface > this.a_slicer.m_mesh[1].m_surface)
            {
                biggerMesh = this.a_slicer.m_mesh[0];
                smallerMesh = this.a_slicer.m_mesh[1];
                v_posBigger = true;
            }
            else
            {
                biggerMesh = this.a_slicer.m_mesh[1];
                smallerMesh = this.a_slicer.m_mesh[0];
                v_posBigger = false;
            }
            GameObject v_gameObject = Instantiate(p_gameObject, this.a_ObjectContainer);
            v_gameObject.transform.SetPositionAndRotation(p_gameObject.transform.position, p_gameObject.transform.rotation);
            var v_objMesh = v_gameObject.GetComponent<MeshFilter>().mesh;
            this.m_replaceMesh(v_mesh, biggerMesh);
            this.m_replaceMesh(v_objMesh, smallerMesh);
            if (v_posBigger == true)
            {
                this.m_addObject(ref p_positiveObjects, p_gameObject);
                this.m_addObject(ref p_negativeObjects, v_gameObject);
            }
            else
            {
                this.m_addObject(ref p_negativeObjects, p_gameObject);
                this.m_addObject(ref p_positiveObjects, v_gameObject);
            }

            to_return = true;
        }
        return to_return;
    }
    #endregion

    private void m_isPosBigger()
    {

    }

    #region add object
    private void m_addObject(ref List<Transform> p_object, GameObject p_GO)
    {
        p_object.Add(p_GO.transform);
    }
    #endregion

    #region replace mesh
    private void m_replaceMesh(Mesh p_mesh, C_mesh p_tempMesh)
    {
        MeshCollider v_meshCollider = null;
        p_mesh.Clear();
        p_mesh = p_tempMesh.m_setMesh(p_mesh);

        if (v_meshCollider != null && v_meshCollider.enabled)
        {
            v_meshCollider.sharedMesh = p_mesh;
            v_meshCollider.convex = true;
        }
    }
    #endregion

    #region ecarte mesh unitarly
    void m_ecarteMesh(Transform p_first, Transform p_second, Vector3 p_normalLocal)
    {
        Vector3 v_ecarte = ((Vector3)(p_first.worldToLocalMatrix.transpose * p_normalLocal)).normalized * this.m_separation;
        p_first.position += v_ecarte;
        p_second.position -= v_ecarte;
    }
    #endregion

    #region transfom position 
    private void m_transformPosition(ref List<Transform> p_list, Vector3 p_ecarte)
    {
        for (int i = 0; i < p_list.Count; i++)
        {
            p_list[i].transform.position += p_ecarte;
        }
    }
    #endregion

    #region ecarte mesh with list
    void m_ecarteMesh(List<Transform> p_obj1, List<Transform> p_obj2, Vector3 p_normalWorld)
    {
        var v_ecarte = p_normalWorld * this.m_separation;
        this.m_transformPosition(ref p_obj1, v_ecarte);
        this.m_transformPosition(ref p_obj2, v_ecarte);
    }
    #endregion
}