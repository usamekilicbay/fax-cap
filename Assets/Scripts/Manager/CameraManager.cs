using DG.Tweening;
using UnityEngine;

namespace FaxCap.Manager
{
    public class CameraManager : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Reference to the UI root's RectTransform.")]
        private RectTransform uiRootTransform;

        [SerializeField]
        [Tooltip("Duration of the shake effect in seconds.")]
        private float duration = 0.5f;

        [SerializeField]
        [Tooltip("Strength of the shake effect.")]
        private float strength = 1f;


        public void ShakeCamera()
        {
            uiRootTransform.DOShakeAnchorPos(duration, strength);
        }
    }
}
