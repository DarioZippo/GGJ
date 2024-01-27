using UnityEngine;

namespace Pearl
{

    public class Dynamic2DSortingLayerManager : DynamicSortingLayerManager
    {
        [SerializeField]
        private SemiAxis2DEnum currentAxis = SemiAxis2DEnum.Up;

        private static readonly int _maxSortingLayer = SpriteRenderExtend.MaxSortingLayer;

        public override int CalculateDistance(SortingOrderData sorter)
        {
            if (cam == null)
            {
                return 0;
            }

            float currentAxisValue;
            Vector2 viewportPoint = cam.WorldToViewportPoint(sorter.transform.position);

            currentAxisValue = currentAxis == SemiAxis2DEnum.Up || currentAxis == SemiAxis2DEnum.Down ? viewportPoint.y : viewportPoint.x;
            float a = currentAxis == SemiAxis2DEnum.Up || currentAxis == SemiAxis2DEnum.Right ? _maxSortingLayer : -_maxSortingLayer;
            float b = currentAxis == SemiAxis2DEnum.Up || currentAxis == SemiAxis2DEnum.Right ? -_maxSortingLayer : _maxSortingLayer;

            float aux = Mathf.Lerp(a, b, currentAxisValue);
            return Mathf.FloorToInt(aux);
        }
    }
}