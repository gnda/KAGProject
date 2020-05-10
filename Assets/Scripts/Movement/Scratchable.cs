using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Scratchable : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.GetComponent<CanScratch>() != null)
        {
            HandleTriangleDestruction(other);
        }
    }
    
    private void OnCollisionStay(Collision other)
    {
        if (other.transform.GetComponent<CanScratch>() != null)
        {
            HandleTriangleDestruction(other);
        }
    }
    
    // Search the right triangle according to each contact points
    private void HandleTriangleDestruction(Collision other)
    {
        var mc = GetComponent<MeshCollider>();
        List<ContactPoint> contacts = new List<ContactPoint>();
        other.GetContacts(contacts);

        foreach (var c in contacts)
        {
            var point = c.point;

            List<Vector3> vertices = mc.sharedMesh.vertices.ToList();
            List<int> triangles = mc.sharedMesh.triangles.ToList();
            
            Vector3 p0 = Vector3.zero;
            Vector3 p1 = Vector3.zero;
            Vector3 p2 = Vector3.zero;

            for (int i = 0; i < triangles.Count - 2; i++)
            {
                Vector3 tv0 = vertices[triangles[(i * 3 + 0) % (triangles.Count)]];
                Vector3 tv1 = vertices[triangles[(i * 3 + 1) % (triangles.Count)]];
                Vector3 tv2 = vertices[triangles[(i * 3 + 2) % (triangles.Count)]];

                var testBar = new Barycentric(tv0, tv1, tv2, point);

                if (testBar.IsInside)
                {
                    p0 = tv0;
                    p1 = tv1;
                    p2 = tv2;

                    break;
                }
            }

            if (p0 == Vector3.zero && p1 == Vector3.zero && p2 == Vector3.zero)
                return;

            RemoveTriangle(p0, p1, p2);
        }
    }

    // Destroys the triangle in the mesh described by p0, p1 and p2
    private void RemoveTriangle(Vector3 p0, Vector3 p1, Vector3 p2)
    {
        var vertices = GetComponent<MeshCollider>().sharedMesh.vertices.ToList();
        var triangles = GetComponent<MeshCollider>().sharedMesh.triangles.ToList();
        var mfSharedMesh = GetComponent<MeshFilter>().sharedMesh;
        var mc = GetComponent<MeshCollider>();

        int i = 0;

        while (!(vertices[triangles[i]] == p0 && vertices[triangles[i + 1]] == p1 
             && vertices[triangles[i + 2]] == p2) && (i < triangles.Count - 2))
        {
            i++;
        }
        
        var remInd = i;

        if (remInd + 2 >= triangles.Count)
        {
            if (remInd + 1 >= triangles.Count)
            {
                triangles.RemoveAt(remInd);
                triangles.RemoveAt(remInd);
                triangles.RemoveAt(remInd);
            }
            else
            {
                triangles.RemoveAt(remInd + 1);
                triangles.RemoveAt(remInd);
                triangles.RemoveAt(remInd - 1);
            }
        }
        else
        {
            triangles.RemoveAt(remInd + 2);
            triangles.RemoveAt(remInd + 1);
            triangles.RemoveAt(remInd);
        }
            

        mfSharedMesh.triangles = triangles.ToArray();
        mc.sharedMesh = mfSharedMesh;
    }
}