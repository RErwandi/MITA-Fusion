using UnityEngine;

namespace Mita
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 offset;
        [SerializeField] private float smoothness = 5f;

        private void LateUpdate()
        {
            if (target != null)
            {
                var desiredPosition = target.position + offset;

                transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * smoothness);
            }
        }

        public void SetTarget(Transform target)
        {
            this.target = target;
        }
    }
}
