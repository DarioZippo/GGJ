using Pearl;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static System.Collections.Specialized.BitVector32;

namespace Pearl
{
    [RequireComponent(typeof(Camera))]
    public class CompositionGridCamera : MonoBehaviour
    {
        public enum CompositionStyleEnum { RuleeOfThirds, GoldenSection, CustomGrid, Null }

        #region Inspector Fields
        [SerializeField]
        private CompositionStyleEnum compositionStyle = CompositionStyleEnum.RuleeOfThirds;
        [SerializeField]
        private Color colorForGrid = default;
        [SerializeField, ConditionalField("@compositionStyle == GoldenSection")]
        private bool inverseX = false;
        [SerializeField, ConditionalField("@compositionStyle == GoldenSection")]
        private bool inverseY = false;
        [SerializeField, ConditionalField("@compositionStyle == GoldenSection"), Range(1, 30)]
        private int numberOfGoldenSection = 6;

        [SerializeField, ConditionalField("@compositionStyle == CustomGrid")]
        private int numberColumn = 2;
        [SerializeField, ConditionalField("@compositionStyle == CustomGrid")]
        private int numberRow = 2;
        [SerializeField, ConditionalField("@compositionStyle == CustomGrid")]
        private Vector2 spacingElement;
        #endregion

        #region Private Fields
        private Camera _camera;
        private const float goldenRatio = 1.618f;
        #endregion

        #region Unity Callbacks
        private void OnDrawGizmos()
        {
            if (_camera == null)
            {
                _camera = GetComponent<Camera>();
            }

            Bounds bound = _camera.OrthographicBounds();
            Gizmos.color = colorForGrid;

            if (compositionStyle == CompositionStyleEnum.RuleeOfThirds)
            {
                float deltaX = bound.max.x - bound.min.x;
                float sectionX = deltaX / 3;

                float deltaY = bound.max.y - bound.min.y;
                float sectionY = deltaY / 3;

                for (int i = 0; i < 2; i++)
                {
                    float auxX = bound.min.x + (sectionX * (i + 1));
                    float auxY = bound.min.y + (sectionY * (i + 1));
                    Gizmos.DrawLine(new Vector3(auxX, bound.min.y, 0), new Vector3(auxX, bound.max.y, 0));
                    Gizmos.DrawLine(new Vector3(bound.min.x, auxY, 0), new Vector3(bound.max.x, auxY, 0));
                }
            }
            else if (compositionStyle == CompositionStyleEnum.GoldenSection)
            {
                Bounds newBounds = bound;
                bool alternatedX = inverseX;
                bool alternatedY = inverseY;

                bool landscape = _camera.aspect >= 1;
                float sizeBezier = 2;


                var startPointBezier =  inverseX ? new Vector3(newBounds.max.x, newBounds.min.y, 0) : new Vector3(newBounds.min.x, newBounds.min.y, 0);


                for (int i = 0; i < numberOfGoldenSection; i++)
                {
                    if ( (landscape && i % 2 == 0) || (!landscape && i % 2 != 0))
                    {
                        float deltaX = newBounds.max.x - newBounds.min.x;
                        float deltaY = newBounds.max.y - newBounds.min.y;

                        float sectionX = deltaX / goldenRatio;
                        float auxX = alternatedX ? newBounds.min.x + (deltaX - sectionX) : newBounds.min.x + (sectionX);
                        Gizmos.DrawLine(new Vector3(auxX, newBounds.min.y, 0), new Vector3(auxX, newBounds.max.y, 0));
       

                        float centerNewBoundsX = alternatedX ? newBounds.min.x + (deltaX - sectionX) * 0.5f : auxX + (deltaX - sectionX) * 0.5f;
                        Vector3 centerNewBounds = new(centerNewBoundsX, newBounds.center.y, newBounds.center.z);
                        Vector3 sizeNewBounds = new(deltaX - sectionX, newBounds.size.y, newBounds.size.z);
                        var auxBounds = new Bounds(centerNewBounds, sizeNewBounds);


                        centerNewBoundsX = alternatedX ? newBounds.min.x + auxBounds.size.x + (deltaX - auxBounds.size.x) * 0.5f : newBounds.min.x + (deltaX - auxBounds.size.x) * 0.5f;
                        centerNewBounds = new(centerNewBoundsX, newBounds.center.y, newBounds.center.z);
                        sizeNewBounds = new(deltaX - sizeNewBounds.x, newBounds.size.y, newBounds.size.z);
                        var alternedBounds = new Bounds(centerNewBounds, sizeNewBounds);

                        var endPointBezier = alternedBounds.OppositivePointsInbounds(startPointBezier);
                        CubicBezier.GetDirectionTanForLSmoothCurve(startPointBezier, endPointBezier, out Vector3 directionStartTan, out Vector3 directionEndTan, TypeCurveEnum.Concave);
                        var startTangent = startPointBezier + directionStartTan * deltaY / sizeBezier;
                        var endTangent = endPointBezier + directionEndTan * deltaX / sizeBezier;
                        Handles.DrawBezier(startPointBezier, endPointBezier, startTangent, endTangent, Color.black, null, 5);
                        startPointBezier = endPointBezier;
                        

                        newBounds = auxBounds;
                        alternatedX = !alternatedX;
                    }
                    else
                    {
                        float deltaX = newBounds.max.x - newBounds.min.x;
                        float deltaY = newBounds.max.y - newBounds.min.y;
                        float sectionY = deltaY / goldenRatio;
                        float auxY = alternatedY ? newBounds.min.y + (sectionY) : newBounds.min.y + (deltaY - sectionY);
                        Gizmos.DrawLine(new Vector3(newBounds.min.x, auxY, 0), new Vector3(newBounds.max.x, auxY, 0));

                        float centerNewBoundsY = alternatedY ? auxY + (deltaY - sectionY) * 0.5f : newBounds.min.y + (deltaY - sectionY) * 0.5f;
                        Vector3 centerNewBounds = new(newBounds.center.x, centerNewBoundsY, newBounds.center.z);
                        Vector3 sizeNewBounds = new(newBounds.size.x, deltaY - sectionY, newBounds.size.z);
                        var auxBounds = new Bounds(centerNewBounds, sizeNewBounds);


                        centerNewBoundsY = alternatedY ? newBounds.min.y + (deltaY - auxBounds.size.y) * 0.5f : newBounds.min.y + auxBounds.size.y + (deltaY - auxBounds.size.y) * 0.5f;
                        centerNewBounds = new(newBounds.center.x, centerNewBoundsY, newBounds.center.z);
                        sizeNewBounds = new(newBounds.size.x, deltaY - sizeNewBounds.y, newBounds.size.z);
                        var alternedBounds = new Bounds(centerNewBounds, sizeNewBounds);

                        var endPointBezier = alternedBounds.OppositivePointsInbounds(startPointBezier);
                        CubicBezier.GetDirectionTanForLSmoothCurve(startPointBezier, endPointBezier, out Vector3 directionStartTan, out Vector3 directionEndTan, TypeCurveEnum.Concave);
                        var startTangent = startPointBezier + directionStartTan * deltaX / sizeBezier;
                        var endTangent = endPointBezier + directionEndTan * deltaY / sizeBezier;
                        Handles.DrawBezier(startPointBezier, endPointBezier, startTangent, endTangent, Color.black, null, 5);
                        startPointBezier = endPointBezier;


                        newBounds = auxBounds;
                        alternatedY = !alternatedY;
                    }
                }
            }
            else if (compositionStyle == CompositionStyleEnum.CustomGrid)
            {
                float deltaX = bound.max.x - bound.min.x;
                deltaX -= spacingElement.x * numberColumn;
                float sectionX = deltaX / numberColumn;

                float deltaY = bound.max.y - bound.min.y;
                deltaY -= spacingElement.y * numberRow;
                float sectionY = deltaY / numberRow;

                for (int i = 0; i < numberColumn - 1; i++)
                {
                    float auxX = bound.min.x + (sectionX * (i + 1) + spacingElement.x * i);
                    Gizmos.DrawLine(new Vector3(auxX, bound.min.y, 0), new Vector3(auxX, bound.max.y, 0));
                    Gizmos.DrawLine(new Vector3(auxX + spacingElement.x, bound.min.y, 0), new Vector3(auxX + spacingElement.x, bound.max.y, 0));
                }

                for (int i = 0; i < numberRow - 1; i++)
                {
                    float auxY = bound.min.y + (sectionY * (i + 1) + spacingElement.y * i);
                    Gizmos.DrawLine(new Vector3(bound.min.x, auxY, 0), new Vector3(bound.max.x, auxY, 0));
                    Gizmos.DrawLine(new Vector3(bound.min.x, auxY + spacingElement.y, 0), new Vector3(bound.max.x, auxY + spacingElement.y, 0));
                }
            }
        }
        #endregion
    }
}
