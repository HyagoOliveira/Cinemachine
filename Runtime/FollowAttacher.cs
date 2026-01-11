using UnityEngine;
using Unity.Cinemachine;

namespace ActionCode.Cinemachine
{
    /// <summary>
    /// Follow Attacher for Cinemachine.
    /// </summary>
    /// <remarks>Attaches a Transform to be followed by the local VirtualCamera.</remarks>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CinemachineCamera))]
    public sealed class FollowAttacher : MonoBehaviour
    {
        [SerializeField] private CinemachineCamera virtualCamera;
        [TagField, Tooltip("If set, it'll search and attach the first GameObject with this tag on Awake function.")]
        public string tagOnAwake = string.Empty;

        private void Reset() => virtualCamera = GetComponent<CinemachineCamera>();
        private void Awake() => Attach(tagOnAwake);

        /// <summary>
        /// Attaches the given target GameObject to be followed by the VirtualCamera.
        /// </summary>
        /// <param name="target">A GameObject to be followed.</param>
        public void Attach(GameObject target) => Attach(target.transform);

        /// <summary>
        /// Attaches the given target to be followed by the VirtualCamera.
        /// </summary>
        /// <param name="target">A Transform to be followed.</param>
        public void Attach(Transform target) => virtualCamera.Follow = target;

        /// <summary>
        /// Attaches the first GameObject with the given tag to be followed by the VirtualCamera.
        /// </summary>
        /// <param name="tag">A Tag to search for a GameObject.</param>
        public void Attach(string tag)
        {
            var invalidTag = string.IsNullOrEmpty(tag);
            if (invalidTag) return;

            var target = GameObject.FindWithTag(tag);
            if (target) Attach(target);
        }
    }
}