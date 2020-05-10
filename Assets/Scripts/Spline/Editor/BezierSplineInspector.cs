using Spline;
using UnityEditor;
using UnityEngine;

/*
 * Code heavily inspired by
 * https://catlikecoding.com/unity/tutorials/curves-and-splines/
 */
namespace Editor
{
    [CustomEditor(typeof(BezierSpline))]
    public class BezierSplineInspector : UnityEditor.Editor
    {
        public BezierSpline spline;
        public Transform handleTransform;
        public Quaternion handleRotation;
        
        private const float handleSize = 0.04f;
        private const float pickSize = 0.06f;
	
        private int selectedIndex = -1;

        private bool addMode, loop;

        private static Color[] modeColors =
        {
            Color.white,
            Color.yellow,
            Color.cyan
        };

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
            Event e = Event.current;
            spline = target as BezierSpline;
            handleTransform = spline.transform;
            handleRotation = Tools.pivotRotation == PivotRotation.Local ? 
                handleTransform.rotation : Quaternion.identity;

            if (addMode) {
                Ray worldRay = HandleUtility.GUIPointToWorldRay (Event.current.mousePosition);
                RaycastHit h;
                
                if (!Physics.Raycast(worldRay, out h))
                {
                    Handles.color = Color.gray;
                    if (spline.ControlPointCount > 0)
                    {
                        Handles.DrawLine(ShowPoint(spline.ControlPointCount - 1), worldRay.origin);
                    }

                    if ((e.type == EventType.MouseDrag || e.type == EventType.MouseDown) && e.button == 0)
                    {
                        EditorGUI.BeginChangeCheck();
                        spline.AddPoint(worldRay.origin - spline.transform.position);
                        if (EditorGUI.EndChangeCheck()) {
                            Undo.RecordObject(spline, "Add Points Mode");
                            EditorUtility.SetDirty(spline);
                        }
                        
                        e.Use();
                    } else if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Escape)
                    {
                        addMode = !addMode;
                        Selection.activeGameObject = spline.gameObject;
                    }
                }
            }

            if (spline.ControlPointCount <= 0) return;
            
            for (int i = 1; i < spline.ControlPointCount; i++)
            {
                Handles.color = Color.gray;
                Handles.DrawLine(ShowPoint(i - 1), ShowPoint(i));
            }

            if (loop)
            {
                spline.FakeLoop(Vector2.right);
                EditorUtility.SetDirty(spline);
                loop = !loop;
            }

            SceneView.RepaintAll();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            Event e = Event.current;   
            spline = target as BezierSpline;
            
            loop = GUILayout.Toggle(loop, "Loop", "Button");

            addMode = GUILayout.Toggle(addMode, "Add Points Mode", "Button");
            ActiveEditorTracker.sharedTracker.isLocked = addMode;

            if ((e.type == EventType.KeyDown && e.keyCode == KeyCode.Escape) && addMode)
            {
                addMode = !addMode;
                Selection.activeGameObject = spline.gameObject;
            }

            HorizontalSlider("Spline Steps", 1, 100, ref spline.splineSteps);

            if (selectedIndex >= 0 && selectedIndex < spline.ControlPointCount)
                DrawSelectedPointInspector();
            if (addMode)
            {
                Undo.RecordObject(spline, "Add Points Mode");
            }
        }

        private void DrawSelectedPointInspector()
        {
            GUILayout.Label("Selected Point");
            EditorGUI.BeginChangeCheck();
            Vector3 point = EditorGUILayout.Vector3Field("Position", 
                spline.GetControlPoint(selectedIndex));
            if (EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(spline, "Move Point");
                EditorUtility.SetDirty(spline);
                spline.SetControlPoint(selectedIndex, point);
            }
            EditorGUI.BeginChangeCheck();
            BezierControlPointMode mode = (BezierControlPointMode)
                EditorGUILayout.EnumPopup("Mode", spline.GetControlPointMode(selectedIndex));
            if (EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(spline, "Change Point Mode");
                spline.SetControlPointMode(selectedIndex, mode);
                EditorUtility.SetDirty(spline);
            }
        }

        private Vector3 ShowPoint(int index)
        {
            Vector3 point = handleTransform.position + spline.GetControlPoint(index);

            float size = HandleUtility.GetHandleSize(point);
            if (index == 0) {
                size *= 2f;
            }
            Handles.color = modeColors[(int)spline.GetControlPointMode(index)];
            if (Handles.Button(point, handleRotation, size * handleSize,
                size * pickSize, Handles.DotHandleCap)) {
                selectedIndex = index;
                Repaint();
            }
            if (selectedIndex == index) {
                EditorGUI.BeginChangeCheck();
                point = Handles.DoPositionHandle(point, handleRotation);
                if (Event.current != null && 
                    Event.current.isKey && 
                    Event.current.type.Equals(EventType.KeyDown) && 
                    Event.current.keyCode == KeyCode.Delete)
                {
                    spline.RemovePoint(point);
                    GUIUtility.hotControl = 0;
                    Event.current.Use();
                }
                if (EditorGUI.EndChangeCheck()) {
                    Undo.RecordObject(spline, "Move Point");
                    EditorUtility.SetDirty(spline);
                    spline.SetControlPoint(index, 
                        handleTransform.InverseTransformPoint(point));
                }
            }
            
            return point;
        }
    }
}