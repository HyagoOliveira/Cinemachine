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
        public List<Rect> areas = new();

        private void Reset()
        {
            CreateFirstArea();
            gameObject.layer = LayerMask.NameToLayer("TransparentFX");
        }

        public bool IsEmpty() => areas.Count == 0;

        public bool Contains(Vector2 position)
        {
            foreach (var area in areas)
            {
                if (area.Contains(position)) return true;
            }

            return false;
        }

        public void AddArea(Vector2 position, Vector2 size)
        {
            var area = new Rect(position, size);
            areas.Add(area);
        }

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

        public Vector2 GetTopLeftPosition(int index) => new(areas[index].xMin, areas[index].yMax);

        public void CreateFirstArea()
        {
            var area = new Rect(position: Vector2.zero, size: new Vector2(30f, 15f));
            areas.Add(area);
        }
    }
}