using UnityEngine;
using System.Collections.Generic;

namespace ActionCode.Cinemachine
{
    /// <summary>
    /// Facilitates the creation of PolygonCollider2D rectangles.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class Confiner2DCollider : MonoBehaviour
    {
        [SerializeField] private List<Rect> areas;

        //private static readonly Vector2 defaultAreaSize = new(30, 15);

        private void Reset() => gameObject.layer = LayerMask.NameToLayer("TransparentFX");

        public bool IsEmpty() => areas.Count == 0;

        public void AddArea(Vector2 position, Vector2 size)
        {
            var area = new Rect(position, size);
            AddArea(area);
        }

        public void AddArea(Rect area) => areas.Add(area);

        public Rect GetArea(int index) => areas[index];

        public Rect FindArea(Transform target)
        {
            if (IsEmpty() || target == null) return default;

            foreach (var area in areas)
            {
                if (area.Contains(target.position))
                    return area;
            }

            // Target is outside from any area.
            return FindClosestArea(target.position);
        }

        public Rect FindClosestArea(Vector3 position)
        {
            var closestArea = new Rect();
            var closestDistance = Mathf.Infinity;

            foreach (var area in areas)
            {
                var closestPosition = area.ClosestPoint(position);
                var distance = Vector3.Distance(position, closestPosition);
                var isClosest = distance < closestDistance;

                if (isClosest)
                {
                    closestArea = area;
                    closestDistance = distance;
                }
            }

            return closestArea;
        }
    }
}