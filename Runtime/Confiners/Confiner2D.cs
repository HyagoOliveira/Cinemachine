using Cinemachine;
using UnityEngine;
using Cinemachine.Utility;

namespace ActionCode.Cinemachine
{
    /// <summary>
    /// Cinemachine Extension that confines the camera final position according to the areas from <see cref="Confiner2DCollider"/>. 
    /// This will work for orthographic or perspective cameras.
    /// <para>
    /// This component was inspired by <see cref="CinemachineConfiner2D"/>, removing the mandatory use of <see cref="PolygonCollider2D"/> 
    /// and using a list of <see cref="Rect"/> as areas. This makes the code more performative and simple to use.
    /// </para>
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class Confiner2D : CinemachineExtension
    {
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
        [Tooltip("The collider areas confining the Camera.")]
        public Confiner2DCollider collider;
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
        [SerializeField, Tooltip("Whether to find the above collider on Awake.")]
        private bool findColliderOnAwake = true;
        [SerializeField, Range(0F, maxDampingSpeed), Tooltip("The damping speed when moving between areas.")]
        private float dampingSpeed = 0.25F;

        public Rect CurrentArea { get; private set; }

        /// <summary>
        /// The damping speed when moving between areas.
        /// </summary>
        public float DampingSpeed
        {
            get => dampingSpeed;
            set => dampingSpeed = Mathf.Clamp(value, 0F, maxDampingSpeed);
        }

        private Camera mainCamera;
        private Vector3 dampedDisplacement;
        private Vector3 previousDisplacement;

        private const float maxDampingSpeed = 5f;
        private const float cornerAngleThreshold = 10f;

        protected override void Awake()
        {
            base.Awake();
            mainCamera = Camera.main;
            if (findColliderOnAwake) FindCollider();
        }

        public void FindCollider() => collider = FindObjectOfType<Confiner2DCollider>();

        protected override void PostPipelineStageCallback(
            CinemachineVirtualCameraBase vcam,
            CinemachineCore.Stage stage,
            ref CameraState state,
            float deltaTime
        )
        {
            var isInvalidStage = stage != CinemachineCore.Stage.Body;
            if (isInvalidStage || collider == null) return;

            CurrentArea = collider.FindArea(vcam.Follow);

            var displacement = ConfineScreenEdges(ref state);
            displacement -= GetTransitionDamping(displacement, deltaTime);

            state.PositionCorrection += displacement;
        }

        private Vector3 ConfineScreenEdges(ref CameraState state)
        {
            const int kMaxIter = 12;

            var rot = Quaternion.Inverse(state.CorrectedOrientation);
            var dy = mainCamera.orthographicSize;
            var dx = dy * mainCamera.aspect;
            var vx = rot * Vector3.right * dx;
            var vy = rot * Vector3.up * dy;
            var displacement = Vector3.zero;
            var camPos = state.CorrectedPosition;
            var lastD = Vector3.zero;

            for (int i = 0; i < kMaxIter; ++i)
            {
                var d = ConfinePoint(camPos - vy - vx);
                if (d.AlmostZero()) d = ConfinePoint(camPos + vy + vx);
                if (d.AlmostZero()) d = ConfinePoint(camPos - vy + vx);
                if (d.AlmostZero()) d = ConfinePoint(camPos + vy - vx);
                if (d.AlmostZero()) break;

                if ((d + lastD).AlmostZero())
                {
                    displacement += d * 0.5f;  // confiner too small: center it
                    break;
                }

                displacement += d;
                camPos += d;
                lastD = d;
            }

            return displacement;
        }

        private Vector3 ConfinePoint(Vector3 camPos)
        {
            if (CurrentArea.Contains(camPos)) return Vector3.zero;

            var closest = CurrentArea.ClosestPoint(camPos);
            closest.z = camPos.z;

            return closest - camPos;
        }

        private Vector3 GetTransitionDamping(Vector3 displacement, float deltaTime)
        {
            var prev = previousDisplacement;
            previousDisplacement = displacement;

            if (!VirtualCamera.PreviousStateIsValid || deltaTime < 0 || dampingSpeed <= 0F)
                return Vector3.zero;

            // If a big change from previous frame's desired displacement is detected, 
            // assume we are going around a corner and extract that difference for damping
            if (prev.sqrMagnitude > 10f && Vector2.Angle(prev, displacement) > cornerAngleThreshold)
                dampedDisplacement += displacement - prev;

            dampedDisplacement -= Damper.Damp(dampedDisplacement, dampingSpeed, deltaTime);

            return dampedDisplacement;
        }
    }
}