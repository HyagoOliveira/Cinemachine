using UnityEditor;
using UnityEngine;
using UnityEditor.IMGUI.Controls;

namespace ActionCode.Cinemachine.Editor
{
    [CustomEditor(typeof(Confiner2DCollider))]
    public sealed class Confiner2DColliderEditor : UnityEditor.Editor
    {
        private Rect SelectedArea
        {
            get => confiner.areas[selectedAreaIndex];
            set => confiner.areas[selectedAreaIndex] = value;
        }

        private GUIStyle sceneLabelStyle;
        private int selectedAreaIndex = -1;
        private bool clampCurrentAreaToInt = true;
        private Confiner2DCollider confiner;
        private BoxBoundsHandle currentAreaHandle;

        private static readonly Vector2 sceneButtonSize = Vector2.one * 2F;
        private static readonly Color createButtonColor = Color.green * 1.8F;
        private static readonly Color deleteButtonColor = Color.red * 0.9F;
        private static readonly Color areaOutlineColor = new(0.1568628F, 0.9411765F, 0.1490196F);
        private static readonly Color areaFaceColor = areaOutlineColor * 0.16F;

        private void OnEnable()
        {
            confiner = (Confiner2DCollider)target;
            currentAreaHandle = new BoxBoundsHandle()
            {
                axes = PrimitiveBoundsHandle.Axes.X | PrimitiveBoundsHandle.Axes.Y
            };

            InitializeGUIStyles();
            if (!confiner.IsEmpty()) selectedAreaIndex = 0;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();

            if (confiner.IsEmpty()) DrawCreateFirstAreaButton();
            else DrawSelectedAreaGUI();
        }

        private void OnSceneGUI()
        {
            if (confiner.IsEmpty()) return;

            var lastHandlesColor = Handles.color;

            DrawAreas();
            if (HasSelectedArea())
            {
                HandleCurrentArea();
                DrawCurrentAreaCreateButtons();
                DrawCurrentAreaDeleteButton();
            }

            Handles.color = lastHandlesColor;
        }

        private bool HasSelectedArea() => selectedAreaIndex > -1 && selectedAreaIndex < confiner.areas.Count;

        private void InitializeGUIStyles()
        {
            sceneLabelStyle = new GUIStyle()
            {
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.LowerLeft,
                fontSize = 32
            };
            sceneLabelStyle.normal.textColor = areaOutlineColor * 2F;
        }

        private void DrawCreateFirstAreaButton()
        {
            if (GUILayout.Button("Create First Area"))
            {
                confiner.CreateFirstArea();
                UpdateEditorGUI();
            }
        }

        private void DrawSelectedAreaGUI()
        {
            if (confiner.IsEmpty() || !HasSelectedArea()) return;

            EditorGUILayout.BeginVertical(GUI.skin.box);
            clampCurrentAreaToInt = EditorGUILayout.Toggle("Use Integers", clampCurrentAreaToInt);

            EditorGUI.BeginChangeCheck();

            SelectedArea = DrawSelectedAreaField();

            var hasChanges = EditorGUI.EndChangeCheck();
            if (hasChanges) UpdateEditorGUI();

            EditorGUILayout.EndVertical();
        }

        private Rect DrawSelectedAreaField()
        {
            const string label = "Selected Area";
            return clampCurrentAreaToInt ?
                EditorGUILayout.RectIntField(label, SelectedArea.ToRectInt()).ToRect() :
                EditorGUILayout.RectField(label, SelectedArea);
        }

        private void DrawAreas()
        {
            Handles.color = areaOutlineColor;

            for (var i = 0; i < confiner.areas.Count; i++)
            {
                var identifier = $" {i}";
                var area = confiner.areas[i];

                Handles.Label(area.BottomLeft(), identifier, sceneLabelStyle);

                var hasSelectedArea = SceneButton.RectButton(area);
                if (hasSelectedArea)
                {
                    selectedAreaIndex = i;
                    UpdateEditorGUI();
                }
            }
        }

        private void HandleCurrentArea()
        {
            currentAreaHandle.center = SelectedArea.center;
            currentAreaHandle.size = SelectedArea.size;

            EditorGUI.BeginChangeCheck();
            currentAreaHandle.DrawHandle();

            var hasChanges = EditorGUI.EndChangeCheck();
            if (hasChanges)
            {
                SelectedArea = new Rect()
                {
                    size = currentAreaHandle.size,
                    center = currentAreaHandle.center
                };

                UpdateEditorGUI();
            }

            Handles.DrawSolidRectangleWithOutline(SelectedArea, areaFaceColor, areaOutlineColor);
        }

        private void DrawCurrentAreaCreateButtons()
        {
            const float SKIN = 2F;

            var topPos = SelectedArea.TopCenter() + Vector2.up * SKIN;
            var leftPos = SelectedArea.LeftCenter() + Vector2.left * SKIN;
            var rightPos = SelectedArea.RightCenter() + Vector2.right * SKIN;
            var bottomPos = SelectedArea.BottomCenter() + Vector2.down * SKIN;

            Handles.color = createButtonColor;

            TryDrawCreateButton(topPos, direction: Vector2.up, angle: 90F, distance: SelectedArea.height);
            TryDrawCreateButton(leftPos, direction: Vector2.left, angle: 180F, distance: SelectedArea.width);
            TryDrawCreateButton(rightPos, direction: Vector2.right, angle: 0F, distance: SelectedArea.width);
            TryDrawCreateButton(bottomPos, direction: Vector2.down, angle: 270F, distance: SelectedArea.height);
        }

        private void DrawCurrentAreaDeleteButton()
        {
            var position = SelectedArea.TopRight() - sceneButtonSize;

            Handles.color = deleteButtonColor;
            var deleteButtonDown = SceneButton.CrossButton(position, sceneButtonSize, 0F);

            if (deleteButtonDown) DeleteArea();
        }

        private void TryDrawCreateButton(Vector2 position, Vector2 direction, float angle, float distance)
        {
            var isAvailable = !confiner.Contains(position);
            var isDown = isAvailable && SceneButton.ArrowButton(position, sceneButtonSize, angle);
            if (isDown) CreateArea(direction, distance);
        }

        private void CreateArea(Vector2 direction, float distance)
        {
            var area = new Rect(SelectedArea);
            area.position += direction * distance;

            confiner.areas.Add(area);
            selectedAreaIndex = confiner.areas.Count - 1;

            UpdateEditorGUI();
        }

        private void DeleteArea()
        {
            confiner.areas.RemoveAt(selectedAreaIndex);
            selectedAreaIndex = Mathf.Max(0, selectedAreaIndex - 1);

            UpdateEditorGUI();
        }

        private void UpdateEditorGUI() => EditorUtility.SetDirty(target);
    }
}