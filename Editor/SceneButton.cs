using UnityEditor;
using UnityEngine;

namespace ActionCode.Cinemachine.Editor
{
    /// <summary>
    /// Draws button on the Scene window.
    /// <para>
    /// This class was created using <see cref="UnityEditorInternal.Button"/> and <see cref="Handles"/> as references.
    /// This was necessary since the original ones didn't support drawing rectangles with different width and height.
    /// </para>
    /// </summary>
    public static class SceneButton
    {
        private static readonly int rectButtonHash = "RectButtonHash".GetHashCode();
        private static readonly int arrowButtonHash = "ArrowButtonHash".GetHashCode();
        private static readonly int crossButtonHash = "CrossButtonHash".GetHashCode();

        private static readonly Vector3[] rectangleHandlePointsCache = new Vector3[5];
        private static readonly Vector3[] arrowHandlePointsCache = new Vector3[8];
        private static readonly Vector3[] crossHandlePointsCache = new Vector3[13];

        private delegate void DrawRectCapFunction(int controlID, Rect area, Rect collision, float angle, EventType eventType);

        public static bool RectButton(Rect area, float angle = 0F)
        {
            const float SKIN = 0.1F;
            var halfSize = new Vector2(area.width * SKIN, area.height * SKIN);
            var collision = area;
            collision.min += halfSize;
            collision.max -= halfSize;
            return RectButton(area, collision, angle);
        }

        public static bool RectButton(Rect area, Rect collision, float angle = 0F)
        {
            var id = GUIUtility.GetControlID(rectButtonHash, FocusType.Passive);
            return Do(id, area, collision, angle, DrawRectangleHandleCap);
        }

        public static bool ArrowButton(Vector2 center, Vector2 size, float angle)
        {
            var position = center - Vector2.one * size * 0.5F;
            var rectPos = new Rect(position, size);
            return ArrowButton(rectPos, angle);
        }

        public static bool ArrowButton(Rect area, float angle)
        {
            var id = GUIUtility.GetControlID(arrowButtonHash, FocusType.Passive);
            return Do(id, area, area, angle, DrawArrowHandleCap);
        }

        public static bool CrossButton(Vector2 center, Vector2 size, float angle)
        {
            var position = center - Vector2.one * size * 0.5F;
            var rectPos = new Rect(position, size);
            return CrossButton(rectPos, angle);
        }

        public static bool CrossButton(Rect area, float angle)
        {
            var id = GUIUtility.GetControlID(crossButtonHash, FocusType.Passive);
            return Do(id, area, area, angle, DrawCrossHandleCap);
        }

        private static bool Do(int id, Rect area, Rect collision, float angle, DrawRectCapFunction drawFunction)
        {
            var currentEvent = Event.current;
            var hasNearestControl = HandleUtility.nearestControl == id;
            var wasMouseLeftClick = currentEvent.button == 0;

            switch (currentEvent.GetTypeForControl(id))
            {
                case EventType.Layout:
                    if (GUI.enabled)
                    {
                        drawFunction(id, area, collision, angle, EventType.Layout);
                    }
                    break;

                case EventType.MouseMove:
                    if (hasNearestControl && wasMouseLeftClick)
                    {
                        HandleUtility.Repaint();
                    }
                    break;

                case EventType.MouseDown:
                    if (hasNearestControl && wasMouseLeftClick)
                    {
                        // Grab mouse focus
                        GUIUtility.hotControl = id;
                        currentEvent.Use();
                    }
                    break;

                case EventType.MouseUp:
                    if (GUIUtility.hotControl == id && wasMouseLeftClick)
                    {
                        GUIUtility.hotControl = 0;
                        currentEvent.Use();
                        return hasNearestControl;
                    }
                    break;

                case EventType.Repaint:
                    Color origColor = Handles.color;
                    if (hasNearestControl && GUI.enabled && GUIUtility.hotControl == 0)
                    {
                        Handles.color = Handles.preselectionColor;
                    }

                    drawFunction(id, area, collision, angle, EventType.Repaint);
                    Handles.color = origColor;
                    break;
            }

            return false;
        }

        private static void DrawRectangleHandleCap(int controlID, Rect area, Rect collision, float angle, EventType eventType)
        {
            switch (eventType)
            {
                case EventType.Layout:
                case EventType.MouseMove:
                    HandleUtility.AddControl(controlID, DistanceToRectangle(collision, angle));
                    break;
                case (EventType.Repaint):
                    UpdateRectangleHandlePointsCache(area, angle);
                    Handles.DrawPolyLine(rectangleHandlePointsCache);
                    break;
            }
        }

        private static void DrawArrowHandleCap(int controlID, Rect area, Rect collision, float angle, EventType eventType)
        {
            switch (eventType)
            {
                case EventType.Layout:
                case EventType.MouseMove:
                    HandleUtility.AddControl(controlID, DistanceToRectangle(collision, angle));
                    break;
                case (EventType.Repaint):
                    UpdateArrowHandlePointsCache(area, angle);
                    Handles.DrawPolyLine(arrowHandlePointsCache);
                    break;
            }
        }

        private static void DrawCrossHandleCap(int controlID, Rect area, Rect collision, float angle, EventType eventType)
        {
            switch (eventType)
            {
                case EventType.Layout:
                case EventType.MouseMove:
                    HandleUtility.AddControl(controlID, DistanceToRectangle(collision, angle));
                    break;
                case (EventType.Repaint):
                    UpdateCrossHandlePointsCache(area, angle);
                    Handles.DrawPolyLine(crossHandlePointsCache);
                    break;
            }
        }

        private static float DistanceToRectangle(Rect collision, float angle)
        {
            UpdateRectangleHandlePointsCache(collision, angle);
            var points = new Vector3[rectangleHandlePointsCache.Length];

            for (int i = 0; i < points.Length; i++)
            {
                points[i] = HandleUtility.WorldToGUIPoint(rectangleHandlePointsCache[i]);
            }

            var mousePos = Event.current.mousePosition;
            var oddNodes = false;
            var j = 4;

            for (int i = 0; i < 5; i++)
            {
                if ((points[i].y > mousePos.y) != (points[j].y > mousePos.y))
                {
                    if (mousePos.x < (points[j].x - points[i].x) * (mousePos.y - points[i].y) / (points[j].y - points[i].y) + points[i].x)
                    {
                        oddNodes = !oddNodes;
                    }
                }
                j = i;
            }

            if (!oddNodes)
            {
                // Distance to closest edge (not so fast)
                float closestDist = -1f;
                j = 1;
                for (int i = 0; i < 4; i++)
                {
                    var dist = HandleUtility.DistancePointToLineSegment(mousePos, points[i], points[j++]);
                    if (dist < closestDist || closestDist < 0)
                        closestDist = dist;
                }
                return closestDist;
            }
            return 0;
        }

        private static void UpdateRectangleHandlePointsCache(Rect area, float angle)
        {
            var topLeftPos = area.position + Vector2.up * area.height;
            var bottomRightPos = area.position + Vector2.right * area.width;

            rectangleHandlePointsCache[0] = area.min;
            rectangleHandlePointsCache[1] = bottomRightPos;
            rectangleHandlePointsCache[2] = area.max;
            rectangleHandlePointsCache[3] = topLeftPos;

            var applyRotation = Mathf.Abs(angle) > 0f;
            if (applyRotation)
            {
                for (int i = 0; i < 4; i++)
                {
                    rectangleHandlePointsCache[i] = RotateAroundPivot(rectangleHandlePointsCache[i], area.center, angle);
                }
            }

            rectangleHandlePointsCache[4] = rectangleHandlePointsCache[0];
        }

        private static void UpdateArrowHandlePointsCache(Rect area, float angle)
        {
            var halfHeight = area.height * 0.5F;
            var quarterHeight = halfHeight * 0.5f;
            var halfWidth = area.width * 0.5F;

            arrowHandlePointsCache[0] = area.min + Vector2.up * quarterHeight;
            arrowHandlePointsCache[1] = arrowHandlePointsCache[0] + Vector3.right * halfWidth;
            arrowHandlePointsCache[2] = area.min + Vector2.right * halfWidth;
            arrowHandlePointsCache[3] = area.max + Vector2.down * halfHeight;

            arrowHandlePointsCache[4] = arrowHandlePointsCache[2] + Vector3.up * area.height;
            arrowHandlePointsCache[5] = arrowHandlePointsCache[4] + Vector3.down * quarterHeight;
            arrowHandlePointsCache[6] = arrowHandlePointsCache[5] + Vector3.left * halfWidth;

            var applyRotation = Mathf.Abs(angle) > 0f;
            if (applyRotation)
            {
                for (int i = 0; i < 7; i++)
                {
                    arrowHandlePointsCache[i] = RotateAroundPivot(arrowHandlePointsCache[i], area.center, angle);
                }
            }

            arrowHandlePointsCache[7] = arrowHandlePointsCache[0];
        }

        private static void UpdateCrossHandlePointsCache(Rect area, float angle)
        {
            var halfSize = area.size * 0.5F;
            var quarterSize = halfSize * 0.5F;

            crossHandlePointsCache[0] = area.min + Vector2.up * quarterSize.y;
            crossHandlePointsCache[1] = area.min + Vector2.right * quarterSize.x;
            crossHandlePointsCache[2] = area.min + new Vector2(halfSize.x, quarterSize.y);
            crossHandlePointsCache[3] = crossHandlePointsCache[1] + Vector3.right * halfSize.x;
            crossHandlePointsCache[4] = crossHandlePointsCache[0] + Vector3.right * area.width;
            crossHandlePointsCache[5] = area.max - new Vector2(quarterSize.x, halfSize.y);
            crossHandlePointsCache[6] = area.max + Vector2.down * quarterSize.y;
            crossHandlePointsCache[7] = area.max + Vector2.left * quarterSize.x;
            crossHandlePointsCache[8] = crossHandlePointsCache[2] + Vector3.up * halfSize.y;
            crossHandlePointsCache[9] = crossHandlePointsCache[7] + Vector3.left * halfSize.x;
            crossHandlePointsCache[10] = crossHandlePointsCache[0] + Vector3.up * halfSize.y;
            crossHandlePointsCache[11] = area.min + new Vector2(quarterSize.x, halfSize.y);

            var applyRotation = Mathf.Abs(angle) > 0f;
            if (applyRotation)
            {
                for (int i = 0; i < 12; i++)
                {
                    crossHandlePointsCache[i] = RotateAroundPivot(crossHandlePointsCache[i], area.center, angle);
                }
            }

            crossHandlePointsCache[12] = crossHandlePointsCache[0];
        }

        private static Vector2 RotateAroundPivot(Vector2 point, Vector2 pivot, float angle)
        {
            const float TO_RADIANS = Mathf.PI / 180F;
            angle *= TO_RADIANS;
            Vector2 dir = point - pivot;
            float cosAngle = Mathf.Cos(angle);
            float sinAngle = Mathf.Sin(angle);
            point.x = cosAngle * dir.x - sinAngle * dir.y + pivot.x;
            point.y = sinAngle * dir.x + cosAngle * dir.y + pivot.y;
            return point;
        }
    }
}