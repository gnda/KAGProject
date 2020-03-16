using UnityEngine;

public static class C_defineCalculIntersection
{
    #region calcul Absice
    private static float m_absice(float p_planeNormalValue)
    {
        float to_return = Mathf.Abs(p_planeNormalValue);
        return to_return;
    }
    #endregion

    #region absice * mesh
    public static float m_absiceMultMesh(float p_meshValue, float p_absiceValue)
    {
        return p_meshValue * p_absiceValue;
    }
    #endregion

    //projection interval radius of b (AABB) onto L(t) = b.c + t*p.n p is the plane 
    public static float m_radius(Mesh p_mesh, ref Plane p_plane)
    {
        float v_meshX = p_mesh.bounds.extents.x; //AABB
        float v_meshY = p_mesh.bounds.extents.y; //AABB
        float v_meshZ = p_mesh.bounds.extents.z; //AABB
        float v_absX = m_absice(p_plane.normal.x);
        float v_absY = m_absice(p_plane.normal.y);
        float v_absZ = m_absice(p_plane.normal.z);
        float v_radius = m_absiceMultMesh(v_meshX, v_absX) + m_absiceMultMesh(v_meshY, v_absY) + m_absiceMultMesh(v_meshZ, v_absZ);

        return v_radius;
    }

    #region if mesh intersect the plane
    public static bool m_isMeshIntersectPlane(Mesh p_mesh, ref Plane p_plane)
    {
        // distance of mesh center from plane
        float v_distance = Vector3.Dot(p_plane.normal, p_mesh.bounds.center) - (-p_plane.distance);
        // Intersection occurs when distance falls within [-v_radius,+v_radius] interval
        return Mathf.Abs(v_distance) <= m_radius(p_mesh, ref p_plane);
    }
    #endregion
}
