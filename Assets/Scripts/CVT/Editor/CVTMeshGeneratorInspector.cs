using System.Collections.Generic;
using System.IO;
using Spline;
using TheTide.utils;
using TriangleNet.Tools;
using UnityEditor;
using UnityEngine;
using Vertex = TriangleNet.Geometry.Vertex;

namespace Editor
{
    [CustomEditor(typeof(CVTMeshGenerator))]
    public class CVTMeshGeneratorInspector : UnityEditor.Editor
    {
        private bool generateCVTMesh;

        private void HorizontalSlider(string labelTxt, int min, int max, ref int val)
        {
            GUILayout.Label(labelTxt + " : " + val.ToString());
            int oldVal = val;
            val = (int) GUILayout.HorizontalSlider(val, min, max);
            if (oldVal != val)
            {
                EditorUtility.SetDirty(target);
            }
            GUILayout.BeginHorizontal();
                var defaultAlignment = GUI.skin.label.alignment;
                GUILayout.Label(min.ToString());
                GUI.skin.label.alignment = TextAnchor.UpperRight;
                GUILayout.Label(max.ToString());
                GUI.skin.label.alignment = defaultAlignment;
            GUILayout.EndHorizontal();
        }

        private void OnSceneGUI()
        {
            if (generateCVTMesh)
            {
                GenerateCVTMesh();
                generateCVTMesh = !generateCVTMesh;
            }

            SceneView.RepaintAll();
        }

        public override void OnInspectorGUI()
        {
            CVTMeshGenerator cvtGen = ((CVTMeshGenerator) target);
            base.OnInspectorGUI();

            // CVT Part
            GUILayout.Label("CVT2D Parameters", new GUIStyle(){fontStyle = FontStyle.Bold});
            HorizontalSlider("Process Number", 1, 100, ref cvtGen.processNumber);
            HorizontalSlider("Generator Number", 2, 1000, ref cvtGen.generatorNumber);
            HorizontalSlider("Iteration Number", 1, 200, ref cvtGen.iterationNumber);
            HorizontalSlider("Irregular Perimeter Tolerance Percentage", 0, 100, 
                ref cvtGen.perimeterTolerancePercentage);
            generateCVTMesh = GUILayout.Toggle(generateCVTMesh, "Generate Mesh using CVT", "Button");
        }

        private void GenerateCVTMesh()
        {
            CVTMeshGenerator cvtGen = ((CVTMeshGenerator) target);

            if (cvtGen.useControlPoints)
            {
                writeInputUsingControlPoints();
            }
            else
            {
                writeInputUsingSteps();
            }

            if (cvtGen.RegenerateMesh)
            {
                CVT.run(cvtGen.processNumber, cvtGen.generatorNumber, 
                    cvtGen.iterationNumber);
            }

            Vertex[] outputVertices = readOutput().ToArray();
            VertexSorter.Sort(outputVertices);

            if (cvtGen.OverwriteCVTOutput && cvtGen.transform.Find("CVTOutput") != null)
            {
                DestroyImmediate(cvtGen.transform.Find("CVTOutput").gameObject);
            }

            GameObject cvtOutput = new GameObject("CVTOutput");
            cvtOutput.transform.SetParent(cvtGen.transform);

            var mf = cvtOutput.AddComponent<MeshFilter>();
            var mr = cvtOutput.AddComponent<MeshRenderer>();

            mf.sharedMesh = MeshGenerator.
                GenerateTriangulatedMesh(readOutput(), cvtGen.perimeterTolerancePercentage, 
                     cvtGen.uvScale);

            if (cvtGen.materialToApply == null)
            {
                cvtGen.materialToApply = new Material(Shader.Find("Unlit/Color"));
                cvtGen.materialToApply.color = Color.magenta;
            }
            
            mr.material = cvtGen.materialToApply;

            cvtOutput.AddComponent<MeshCollider>();
            var serializeMesh = cvtOutput.AddComponent<SerializeMesh>();
            serializeMesh.Serialize();

            EditorUtility.SetDirty(cvtOutput);
        }

        private void writeInputUsingControlPoints()
        {
            CVTMeshGenerator cvtGen = ((CVTMeshGenerator) target);
            string cvt2DPath = Application.dataPath + "/CVT2D/";
            string inputPath = cvt2DPath + "input/" + "input.boundary";
            
            BezierSpline outerSpline = cvtGen.outerSpline;
            BezierSpline[] innerSplines = cvtGen.innerSplines;
            
            StreamWriter writer = new StreamWriter(inputPath, false);

            writer.WriteLine("#Outer");

            for (int i = outerSpline.ControlPointCount - 1; i >= 0; i--)
            {
                Vector3 splinePoint = outerSpline.GetControlPoint(i);
                Vector3 point = outerSpline.transform.position + splinePoint;
                
                writer.WriteLine(point.x.ToString().Replace(",", ".") + " "
                    + point.y.ToString().Replace(",", "."));
            }

            foreach (var innerSpline in innerSplines)
            {
                writer.WriteLine("#Inner");

                for (int i = innerSpline.ControlPointCount - 1; i >= 0; i--)
                {
                    Vector3 splinePoint = innerSpline.GetControlPoint(i);
                    Vector3 point = innerSpline.transform.position + splinePoint;
                    
                    writer.WriteLine(point.x.ToString().Replace(",", ".") + " "
                        + point.y.ToString().Replace(",", "."));
                }
            }
            
            writer.Close();
        }
        
        // Needs to be simplified (using delegate ?)
        private void writeInputUsingSteps()
        {
            CVTMeshGenerator cvtGen = ((CVTMeshGenerator) target);
            string cvt2DPath = Application.dataPath + "/CVT2D/";
            string inputPath = cvt2DPath + "input/" + "input.boundary";
            
            BezierSpline outerSpline = cvtGen.outerSpline;
            BezierSpline[] innerSplines = cvtGen.innerSplines;
            
            StreamWriter writer = new StreamWriter(inputPath, false);

            writer.WriteLine("#Outer");

            for (int i = outerSpline.splineSteps; i > 0; i--)
            {
                Vector3 splinePoint = outerSpline.
                    GetPoint(i / (float) outerSpline.splineSteps);;
                Vector3 point = outerSpline.transform.position + splinePoint;
                
                writer.WriteLine(point.x.ToString().Replace(",", ".") + " "
                    + point.y.ToString().Replace(",", "."));
            }
            
            Vector3 lastOuterPoint = outerSpline.transform.position + 
                                     outerSpline.GetPoint(0);
            writer.WriteLine(lastOuterPoint.x.ToString().Replace(",", ".") + " "
                + lastOuterPoint.y.ToString().Replace(",", "."));
            
            foreach (var innerSpline in innerSplines)
            {
                writer.WriteLine("#Inner");

                for (int i = innerSpline.splineSteps; i > 0; i--)
                {
                    Vector3 splinePoint = innerSpline.
                        GetPoint(i / (float) innerSpline.splineSteps);
                    Vector3 point = innerSpline.transform.position + splinePoint;
                    
                    writer.WriteLine(point.x.ToString().Replace(",", ".") + " "
                        + point.y.ToString().Replace(",", "."));
                }
                
                Vector3 lastInnerPoint = outerSpline.transform.position + 
                                         outerSpline.GetPoint(0);
                writer.WriteLine(lastInnerPoint.x.ToString().Replace(",", ".") + " "
                    + lastInnerPoint.y.ToString().Replace(",", "."));
            }
            
            writer.Close();
        }
        
        private List<Vertex> readOutput()
        {
            string cvt2DPath = Application.dataPath + "/CVT2D/";
            string outputPath = cvt2DPath + "output/" + "finalState.txt";
            
            StreamReader reader = new StreamReader(outputPath);
            string line;

            List<Vertex> outputVerticesList = new List<Vertex>();

            while ((line = reader.ReadLine()) != null)
            {
                string[] values = line.Split(',');
                if (values.Length == 2)
                {
                    outputVerticesList.Add(
                        new Vertex(float.Parse(values[0].Replace(".", ",")),
                            float.Parse(values[1].Replace(".", ","))));
                }
            }
            
            reader.Close();

            return outputVerticesList;
        }
    }
}
