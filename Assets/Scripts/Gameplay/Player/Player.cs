using System;
using Fusion;
using UnityEngine;

namespace Mita
{
    [RequireComponent(typeof(PlayerInput))]
    public class Player : NetworkBehaviour
    {
        [SerializeField] private NetworkPrefabRef gameController;
        [SerializeField] private PlayerUI playerUI;
        
        public PlayerInput Input { get; set; }
        public GameController GameController { get; set; }

        private void Awake()
        {
            Input = GetComponent<PlayerInput>();
        }

        public override void Spawned()
        {
            if (Object.HasInputAuthority)
            {
                SetCameraFollowPlayer();
            }

            if (Runner.IsServer)
            {
                Runner.Spawn(gameController);
            }

            var ui = Instantiate(playerUI);
            ui.SetLocalPLayer(this);
            
            GameController = GameController.Instance;
        }

        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Space))
            {
                GameController.StartLevel();
            }
        }

        private void SetCameraFollowPlayer()
        {
            var cameraFollow = Camera.main.GetComponent<CameraFollow>();
            cameraFollow.SetTarget(transform);
        }
    }
}
