using UnityEngine;

namespace ActionCode.Cinemachine
{
    /// <summary>
    /// Facilitates the creation of PolygonCollider2D rectangles.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PolygonCollider2D))]
    public sealed class Confiner2DCollider : MonoBehaviour
    {
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
        [SerializeField] private PolygonCollider2D collider;
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword

        [Space]
        [SerializeField] private int index;
        [SerializeField] private Rect area = defaultArea;

        private static readonly Color outlineColor = new(0.1568628F, 0.9411765F, 0.1490196F);
        private static readonly Color faceColor = outlineColor * 0.16F;

        private int lastIndex = -1;
        private static readonly Rect defaultArea = new(0, 0, 30, 15);

        private void Reset()
        {
            collider = GetComponent<PolygonCollider2D>();
            collider.isTrigger = true;

            var isRect = collider.GetPath(0).Length == 4;
            if (!isRect) collider.SetPath(0, GetPathFrom(area));

            gameObject.layer = LayerMask.NameToLayer("TransparentFX");
        }

        private void OnValidate()
        {
            if (collider == null) return;

            index = Mathf.Clamp(index, 0, collider.pathCount - 1);

            var hasIndexChanged = index != lastIndex;
            if (hasIndexChanged) UpdateAreaUsingIndex();

            UpdatePathUsingIndex();
            lastIndex = index;
        }

        private void OnDrawGizmosSelected() => DrawEditRectangle();

        public PolygonCollider2D GetCollider() => collider;

        [ContextMenu("Add Area To Right")]
        public void AddAreaToRight() => AddArea(GetBottomRightPosition(index));

        [ContextMenu("Add Area To Up")]
        public void AddAreaToUp() => AddArea(GetTopLeftPosition(index));

        public void AddArea(Vector2 position)
        {
            var area = defaultArea;
            area.position = position;
            TryAddNewPath(GetPathFrom(area));
        }

        public Vector2 GetBottomRightPosition(int index)
        {
            var area = GetRectFrom(collider.GetPath(index));
            return new Vector2(area.xMax, area.yMin);
        }

        public Vector2 GetTopLeftPosition(int index)
        {
            var area = GetRectFrom(collider.GetPath(index));
            return new Vector2(area.xMin, area.yMax);
        }

        private void UpdatePathUsingIndex() => collider.SetPath(index, GetPathFrom(area));
        private void UpdateAreaUsingIndex() => area = GetRectFrom(collider.GetPath(index));

        private void TryAddNewPath(Vector2[] path)
        {
            var newIndex = index + 1;
            var isInvalidIndex = newIndex > collider.pathCount - 1;

            if (isInvalidIndex)
            {
                Debug.LogError($"Cannot add new area since collider has only {collider.pathCount} Path Elements. Increase it manually.");
                return;
            }
            // There isn't any collider.AddPath() and cannot possible to set collider.points property
            collider.SetPath(newIndex, path);
        }

        private void DrawEditRectangle()
        {
#if UNITY_EDITOR
            UnityEditor.Handles.DrawSolidRectangleWithOutline(area, faceColor, outlineColor);
#endif
        }

        private static Vector2[] GetPathFrom(Rect area) => new Vector2[4]
        {
            area.min,
            new(area.max.x, area.min.y),
            area.max,
            new(area.min.x, area.max.y),
        };

        private static Rect GetRectFrom(Vector2[] path)
        {
            var min = path[0];
            var max = path[2];
            return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
        }
    }
}