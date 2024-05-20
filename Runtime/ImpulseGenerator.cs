using Cinemachine;
using UnityEngine;

namespace ActionCode.Cinemachine
{
    /// <summary>
    /// Generates an impulse using the local <see cref="CinemachineImpulseSource"/> component when enabled.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CinemachineImpulseSource))]
    public sealed class ImpulseGenerator : MonoBehaviour
    {
        [SerializeField] private CinemachineImpulseSource source;

        private void Reset() => source = GetComponent<CinemachineImpulseSource>();
        private void OnEnable() => source.GenerateImpulse();
    }
}