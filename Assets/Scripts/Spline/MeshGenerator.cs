using System;
using System.Collections.Generic;
using System.Linq;
using TriangleNet;
using TriangleNet.Geometry;
using UnityEngine;
using Vert = TriangleNet.Geometry.Vertex;
using Tri = TriangleNet.Topology.Triangle;

namespace Spline
{
    public static class MeshGenerator {
        public static Mesh GenerateTriangulatedMesh(List<Vert> points, 
            int perimeterTolerancePercentage, float uvScale)
        {
            Polygon poly = new Polygon();

            foreach (var vertex in points)
            {
                poly.Add(vertex);
            }

            float threshold = 0;

            TriangleNetMesh triangleNetMesh = (TriangleNetMesh) poly.Triangulate();
            
            List<Tri> trianglesNet = triangleNetMesh.Triangles.ToList();

            foreach (Tri triangle in triangleNetMesh.Triangles)
            {
                Vert v0 = triangle.GetVertex(0);
                Vert v1 = triangle.GetVertex(1);
                Vert v2 = triangle.GetVertex(2);

                Vector2 vect0 = new Vector2(v0.X, v0.Y);
                Vector2 vect1 = new Vector2(v1.X, v1.Y);
                Vector2 vect2 = new Vector2(v2.X, v2.Y);

                var a = Math.Abs(Vector2.Distance(vect0, vect1));
                var b = Math.Abs(Vector2.Distance(vect0, vect2));

                threshold += (Math.Abs(Vector2.Distance(vect0, vect1)) +
                              Math.Abs(Vector2.Distance(vect1, vect2)) +
                              Math.Abs(Vector2.Distance(vect2, vect0))) / 3;
            }

            threshold /= triangleNetMesh.Triangles.Count;
            threshold += (threshold / 100) * perimeterTolerancePercentage;

            foreach (Tri triangle in triangleNetMesh.Triangles)
            {
                Vert v0 = triangle.GetVertex(0);
                Vert v1 = triangle.GetVertex(1);
                Vert v2 = triangle.GetVertex(2);

                Vector2 vect0 = new Vector2(v0.X, v0.Y);
                Vector2 vect1 = new Vector2(v1.X, v1.Y);
                Vector2 vect2 = new Vector2(v2.X, v2.Y);

                if ((Math.Abs(Vector2.Distance(vect0, vect1)) > threshold) ||
                    (Math.Abs(Vector2.Distance(vect1, vect2)) > threshold) ||
                    (Math.Abs(Vector2.Distance(vect2, vect0)) > threshold))
                {
                    trianglesNet.Remove(triangle);
                }
            }

            Mesh mesh = new Mesh();

            List<Vert> tVertices = new List<Vert>();

            foreach (var t in trianglesNet)
            {
                if (!tVertices.Contains(t.GetVertex(0)))
                    tVertices.Add(t.GetVertex(0));
                if (!tVertices.Contains(t.GetVertex(1)))
                    tVertices.Add(t.GetVertex(1));
                if (!tVertices.Contains(t.GetVertex(2)))
                    tVertices.Add(t.GetVertex(2));
            }

            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();

            for (int i = 0; i < trianglesNet.Count; i++)
            {
                // Get the current triangle
                Tri triangle = trianglesNet[i];

                // For the triangles to be right-side up, they need
                // to be wound in the opposite direction
                Vert tv2 = triangle.GetVertex(0);
                Vert tv1 = triangle.GetVertex(1);
                Vert tv0 = triangle.GetVertex(2);

                Vert vt0 = tVertices.Find(v =>
                    (Math.Abs(v.X - tv0.X) < 0.000001) && (Math.Abs(v.Y - tv0.Y) < 0.000001));
                Vert vt1 = tVertices.Find(v =>
                    (Math.Abs(v.X - tv1.X) < 0.000001) && (Math.Abs(v.Y - tv1.Y) < 0.000001));
                Vert vt2 = tVertices.Find(v =>
                    (Math.Abs(v.X - tv2.X) < 0.000001) && (Math.Abs(v.Y - tv2.Y) < 0.000001));

                Vector2 v0 = new Vector2(vt0.X, vt0.Y);
                Vector2 v1 = new Vector2(vt1.X, vt1.Y);
                Vector2 v2 = new Vector2(vt2.X, vt2.Y);

                // This triangle is made of the next three vertices to be added
                triangles.Add(vertices.Count);
                triangles.Add(vertices.Count + 1);
                triangles.Add(vertices.Count + 2);

                // Add the vertices
                vertices.Add(v0);
                vertices.Add(v1);
                vertices.Add(v2);
            }

            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            mesh.uv = GenerateUv(mesh.vertices, uvScale);

            return mesh;
        }
        
        private static Vector2[] GenerateUv(Vector3[] vertices, float uvScale = 1f)
        {
            Vector2[] uvs = new Vector2[vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                uvs[i] = new Vector2(vertices[i].x * uvScale, vertices[i].y * uvScale);
            }

            return uvs;
        }
    }
}