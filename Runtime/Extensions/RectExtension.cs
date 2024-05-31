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
    }
}