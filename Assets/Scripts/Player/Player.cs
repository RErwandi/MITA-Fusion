using Fusion;
using UnityEngine;

namespace Mita
{
    [RequireComponent(typeof(PlayerInput))]
    public class Player : NetworkBehaviour
    {
        public PlayerInput Input { get; set; }

        private void Awake()
        {
            Input = GetComponent<PlayerInput>();
        }

        public override void Spawned()
        {
            if (Object.HasInputAuthority)
            {
                var cameraFollow = Camera.main.GetComponent<CameraFollow>();
                cameraFollow.SetTarget(transform);
            }
        }
    }
}
