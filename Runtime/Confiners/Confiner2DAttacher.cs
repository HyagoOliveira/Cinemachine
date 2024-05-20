using UnityEngine;
using Cinemachine;

namespace ActionCode.Cinemachine
{
    /// <summary>
    /// Attaches a Bounding Shape 2D to a local confiner on Awake.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CinemachineConfiner2D))]
    public sealed class Confiner2DAttacher : MonoBehaviour
    {
        [SerializeField] private CinemachineConfiner2D confiner;

        private void Reset() => confiner = GetComponent<CinemachineConfiner2D>();
        private void Awake() => TryAttach();

        public void TryAttach()
        {
            var collider = FindObjectOfType<Confiner2DCollider>();
            if (collider) confiner.m_BoundingShape2D = collider.GetCollider();
        }
    }
}