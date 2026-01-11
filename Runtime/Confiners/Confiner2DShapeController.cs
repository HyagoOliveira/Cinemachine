using UnityEngine;
using Unity.Cinemachine;

namespace ActionCode.Cinemachine
{
    /// <summary>
    /// Controls and updates the 2D camera confining shape for a Cinemachine virtual camera 
    /// based on the current area defined by a Confiner2DCollider.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CinemachineCamera))]
    [RequireComponent(typeof(CinemachineConfiner2D))]
    public sealed class Confiner2DShapeController : MonoBehaviour
    {
        [SerializeField] private CinemachineCamera virtualCamera;
        [SerializeField] private CinemachineConfiner2D confiner;

        [Space]
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
        [Tooltip("The collider areas confining the Camera.")]
        public Confiner2DCollider collider;
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
        [SerializeField, Tooltip("Whether to find the above collider on Awake.")]
        private bool findColliderOnAwake = true;

        public Rect CurrentArea { get; private set; }

        public PolygonCollider2D BoundingShape
        {
            get => boundingShape;
            private set
            {
                boundingShape = value;
                confiner.BoundingShape2D = boundingShape;
                confiner.InvalidateBoundingShapeCache();
            }
        }

        private PolygonCollider2D boundingShape;

        private void Reset()
        {
            confiner = GetComponent<CinemachineConfiner2D>();
            virtualCamera = GetComponent<CinemachineCamera>();
        }

        private void Awake()
        {
            if (findColliderOnAwake) FindCollider();
        }

        private void Start()
        {
            UpdateCurrentArea();
            UpdateBoundingShape();
        }

        private void LateUpdate() => CheckNewArea();

        public void FindCollider() => collider = FindFirstObjectByType<Confiner2DCollider>();
        public void UpdateCurrentArea() => CurrentArea = collider.FindArea(virtualCamera.Follow);

        private void CheckNewArea()
        {
            var lastArea = CurrentArea;
            UpdateCurrentArea();

            var isNewArea = lastArea != CurrentArea;
            if (isNewArea) UpdateBoundingShape();
        }

        private void UpdateBoundingShape()
        {
            collider.UpdateCurrentBounds(CurrentArea);
            BoundingShape = collider.CurrentBounds;
        }
    }
}