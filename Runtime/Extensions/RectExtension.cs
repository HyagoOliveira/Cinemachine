using UnityEngine;

namespace ActionCode.Cinemachine
{
    public static class RectExtension
    {
        public static Vector3 ClosestPoint(this Rect rect, Vector3 point)
        {
            var bounds = new Bounds(rect.center, rect.size);
            return bounds.ClosestPoint(point);
        }

        public static Rect ToRect(this RectInt rect) => new(rect.position, rect.size);
        public static RectInt ToRectInt(this Rect rect) => new(ToVector2Int(rect.position), ToVector2Int(rect.size));

        public static Vector2 TopLeft(this Rect rect) => new(rect.xMin, rect.yMax);
        public static Vector2 TopCenter(this Rect rect) => new(rect.center.x, rect.yMax);
        public static Vector2 TopRight(this Rect rect) => rect.max;
        public static Vector2 LeftCenter(this Rect rect) => new(rect.xMin, rect.center.y);
        public static Vector2 RightCenter(this Rect rect) => new(rect.xMax, rect.center.y);
        public static Vector2 BottomLeft(this Rect rect) => rect.min;
        public static Vector2 BottomCenter(this Rect rect) => new(rect.center.x, rect.yMin);
        public static Vector2 BottomRight(this Rect rect) => new(rect.xMax, rect.yMin);

        private static Vector2Int ToVector2Int(Vector2 value) => new(Mathf.FloorToInt(value.x), Mathf.FloorToInt(value.y));
    }
}