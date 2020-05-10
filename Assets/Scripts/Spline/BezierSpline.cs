using System;
using System.Collections.Generic;
using UnityEngine;

/*
 * Code heavily inspired by
 * https://catlikecoding.com/unity/tutorials/curves-and-splines/
 */
namespace Spline
{
    public class BezierSpline : MonoBehaviour
    {
        [SerializeField] private List<Vector3> points;
        [SerializeField] private BezierControlPointMode[] modes;
        
        [HideInInspector] public int splineSteps = 20;

        public int ControlPointCount
        {
            get { return points.Count; }
        }

        public int CurveCount
        {
            get { return (points.Count - 1) / 3; }
        }

        private bool loop;

        public bool Loop
        {
            get => loop;
            set
            {
                loop = value;
                if (value != true) return;
                modes[modes.Length - 1] = modes[0];
                SetControlPoint(0, points[0]);
            }
        }

        [Header("Axis Locking While Drawing")]
        public bool lockXAxis = false;
        public bool lockYAxis = false;
        public bool lockZAxis = true;

        [Header("Lines Rendering In Editor")]
        public bool drawLines = true;
        public bool drawBezier = false;
        public Material linesMaterial;

        public void Reset()
        {
            transform.position = Vector3.zero;
            points = new List<Vector3>() {};
            modes = new[] {BezierControlPointMode.Free};
        }

        /**
         * Returns the point of the curve at the given t time
         */
        public Vector3 GetPoint(float t)
        {
            int i;
            GetProgress(out i, ref t);
            return Bezier.GetPoint(points[i], points[i + 1], points[i + 2], points[i + 3], t);
        }

        public Quaternion GetOrientation(float t)
        {
            int i;
            GetProgress(out i, ref t);
            return Bezier.GetOrientation(points[i], points[i + 1], points[i + 2], points[i + 3], t, Vector3.down);
        }

        public Vector3 GetVelocity(float t)
        {
            int i;
            GetProgress(out i, ref t);
            return Bezier.GetFirstDerivative(points[i], points[i + 1], points[i + 2],
                       points[i + 3], t) - transform.position;
        }

        /**
         * Get the index of the curve i at the t time
         */
        public void GetProgress(out int i, ref float t)
        {
            if (t >= 1f)
            {
                t = 1f;
                i = points.Count - 4;
            }
            else
            {
                t = Mathf.Clamp01(t) * CurveCount;
                i = (int) t;
                t -= i;
                i *= 3;
            }
        }

        public Vector3 GetDirection(float t)
        {
            return GetVelocity(t).normalized;
        }

        public Vector3 GetControlPoint(int index)
        {
            return points[index];
        }

        public void SetControlPoint(int index, Vector3 point)
        {
            if (index % 3 == 0)
            {
                Vector3 delta = point - points[index];
                if (loop)
                {
                    if (index == 0)
                    {
                        points[1] += delta;
                        points[points.Count - 2] += delta;
                        points[points.Count - 1] = point;
                    }
                    else if (index == points.Count - 1)
                    {
                        points[0] = point;
                        points[1] += delta;
                        points[index - 1] += delta;
                    }
                    else
                    {
                        points[index - 1] += delta;
                        points[index + 1] += delta;
                    }
                }
                else
                {
                    if (index > 0)
                        points[index - 1] += delta;
                    if (index + 1 < points.Count)
                        points[index + 1] += delta;
                }
            }

            points[index] = point;
            EnforceMode(index);
        }

        /**
         * Fake loop for CVT2D
         */
        public void FakeLoop(Vector3 offset)
        {
            modes[modes.Length - 1] = modes[0];
            points[ControlPointCount - 1] = points[0] + offset;
        }

        public BezierControlPointMode GetControlPointMode(int index)
        {
            return modes[(index + 1) / 3];
        }

        public void SetControlPointMode(int index, BezierControlPointMode mode)
        {
            int modeIndex = (index + 1) / 3;
            modes[modeIndex] = mode;
            if (loop)
            {
                if (modeIndex == 0)
                {
                    modes[modes.Length - 1] = mode;
                }
                else if (modeIndex == modes.Length - 1)
                {
                    modes[0] = mode;
                }
            }

            EnforceMode(index);
        }

        private void EnforceMode(int index)
        {
            int modeIndex = (index + 1) / 3;
            BezierControlPointMode mode = modes[modeIndex];

            if (mode == BezierControlPointMode.Free || !loop &&
                (modeIndex == 0 || modeIndex == modes.Length - 1))
            {
                return;
            }

            int middleIndex = modeIndex * 3;
            int fixedIndex, enforcedIndex;
            if (index <= middleIndex)
            {
                fixedIndex = middleIndex - 1;
                if (fixedIndex < 0)
                    fixedIndex = points.Count - 2;
                enforcedIndex = middleIndex + 1;
                if (enforcedIndex >= points.Count)
                    enforcedIndex = 1;
            }
            else
            {
                fixedIndex = middleIndex + 1;
                if (fixedIndex >= points.Count)
                    fixedIndex = 1;
                enforcedIndex = middleIndex - 1;
                if (enforcedIndex < 0)
                    enforcedIndex = points.Count - 2;
            }

            Vector3 middle = points[middleIndex];
            Vector3 enforcedTangent = middle - points[fixedIndex];
            if (mode == BezierControlPointMode.Aligned)
            {
                enforcedTangent = enforcedTangent.normalized *
                                  Vector3.Distance(middle, points[enforcedIndex]);
            }

            points[enforcedIndex] = middle + enforcedTangent;
        }

        public float GetTotalLength()
        {
            float length = 0f;

            for (int i = 0; i < points.Count - 1; i++)
            {
                length += Math.Abs(Vector3.Distance(points[i], points[i + 1]));
            }

            return length;
        }

        public void AddPoint(Vector3 point)
        {
            point.x = lockXAxis ? 0 : point.x;
            point.y = lockYAxis ? 0 : point.y;
            point.z = lockZAxis ? 0 : point.z;
            points.Add(point);

            if (points.Count % 2 == 0)
            {
                Array.Resize(ref modes, modes.Length + 1);
                modes[modes.Length - 1] = BezierControlPointMode.Free;
            }
        }

        public void RemovePoint(Vector3 point)
        {
            points.Remove(point);
        }

        private void OnDrawGizmos()
        {
            if ((ControlPointCount > 0) && drawLines)
            {
                if (linesMaterial == null)
                {
                    linesMaterial = new Material(Shader.Find("Unlit/Color"));
                    linesMaterial.color = Color.magenta;
                }

                linesMaterial.SetPass(0);
                GL.PushMatrix();
                GL.Begin(GL.LINES);
                for (int i = 0; i < ControlPointCount - 1; ++i)
                {
                    GL.Vertex(GetControlPoint(i) + transform.position);
                    GL.Vertex(GetControlPoint(i + 1) + transform.position);
                }

                GL.Vertex(GetControlPoint(ControlPointCount - 1));

                GL.End();
                GL.PopMatrix();
            }
            if ((splineSteps > 0) && (ControlPointCount > 0) && drawBezier)
            {
                if (linesMaterial == null)
                {
                    linesMaterial = new Material(Shader.Find("Unlit/Color"));
                    linesMaterial.color = Color.magenta;
                }

                linesMaterial.SetPass(0);
                GL.PushMatrix();
                GL.Begin(GL.LINES);
                
                GL.Vertex(GetPoint(0) + transform.position);
                GL.Vertex(GetPoint(1 / (float) splineSteps) + transform.position);
                
                for (int i = 1; i < splineSteps; i++)
                {
                    GL.Vertex(GetPoint(i / (float) splineSteps) + transform.position);
                    GL.Vertex(GetPoint((i + 1) / (float) splineSteps) + transform.position);
                }

                GL.End();
                GL.PopMatrix();
            }
        }
    }
}