using Spline;

namespace UnityEngine
{
    public class CVTMeshGenerator : MonoBehaviour
    {
        [Header("Splines")]
        public BezierSpline outerSpline;
        public BezierSpline[] innerSplines;
        
        [Header("Mesh Texturing")]
        public Material materialToApply;
        public float uvScale = 0.5f;
        
        [Header("Mesh Generation")]
        public bool useControlPoints = true;
        public bool RegenerateMesh = true;
        public bool OverwriteCVTOutput = false;
        
        [HideInInspector]
        public int processNumber = 1;
        [HideInInspector]
        public int generatorNumber = 20;
        [HideInInspector]
        public int iterationNumber = 10;
        [HideInInspector] 
        public int perimeterTolerancePercentage = 30;
    }
}