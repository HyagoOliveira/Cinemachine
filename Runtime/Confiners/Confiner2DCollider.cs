using UnityEngine;
using System.Collections.Generic;

namespace ActionCode.Cinemachine
{
    /// <summary>
    /// Creates and edits Rectangle Areas in your Scene.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class Confiner2DCollider : MonoBehaviour
    {
        [field: SerializeField, ContextMenuItem("Setup", nameof(SetupCurrentBounds))]
        public PolygonCollider2D CurrentBounds { get; internal set; }
        public List<Rect> areas = new();

        private void Reset()
        {
            CreateFirstArea();
            SetupCurrentBounds();
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
            if (IsEmpty() || target == null) return areas[0];

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

        public void SetupCurrentBounds()
        {
            CurrentBounds = GetOrCreateCurrentBounds();
            CurrentBounds.gameObject.layer = LayerMask.NameToLayer("TransparentFX");
            UpdateCurrentBounds(areas[0]);

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(gameObject);
#endif
        }

        public void UpdateCurrentBounds(Rect area) => CurrentBounds.points = new Vector2[4]
        {
            area.BottomLeft(),
            area.BottomRight(),
            area.TopRight(),
            area.TopLeft()
        };

        private PolygonCollider2D GetOrCreateCurrentBounds()
        {
            const string name = "CurrentBounds";

            var instance = transform.Find(name);
            if (instance == null)
            {
                instance = new GameObject(name).transform;
                instance.SetParent(transform);
            }

            var hasCollider = instance.TryGetComponent(out PolygonCollider2D polyCollider);
            return hasCollider ? polyCollider : instance.gameObject.AddComponent<PolygonCollider2D>();
        }
    }
}