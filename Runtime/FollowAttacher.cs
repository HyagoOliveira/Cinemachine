using UnityEngine;
using Unity.Cinemachine;

namespace ActionCode.Cinemachine
{
    /// <summary>
    /// Follow Attacher for Cinemachine.
    /// <para>It will attach a Transform to be followed by the local VirtualCamera.</para>
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CinemachineCamera))]
    public sealed class FollowAttacher : MonoBehaviour
    {
        [SerializeField] private CinemachineCamera virtualCamera;
        [TagField, Tooltip("If set, it'll search and attach the first GameObject with this tag on Start function.")]
        public string tagOnStart = string.Empty;

        private void Reset() => virtualCamera = GetComponent<CinemachineCamera>();
        private void Start() => Attach(tagOnStart);

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
            var invalidTag = tag.Length == 0;
            if (invalidTag) return;

            var target = GameObject.FindWithTag(tagOnStart);
            if (target) Attach(target);
        }
    }
}