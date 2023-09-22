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
        public PlayerUI UI { get; set; }
        public GameController GameController { get; set; }
        public static Player LocalPlayer { get; set; }

        private void Awake()
        {
            Input = GetComponent<PlayerInput>();
        }

        public override void Spawned()
        {
            if (Object.HasInputAuthority)
            {
                SetCameraFollowPlayer();
                LocalPlayer = this;
                var ui = Instantiate(playerUI);
                ui.SetLocalPLayer(this);
                UI = ui;
            }

            if (Runner.IsServer && Object.HasInputAuthority)
            {
                Runner.Spawn(gameController);
            }

            GameController = GameController.Instance;
        }

        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Space) && Object.HasInputAuthority)
            {
                GameController.StartLevel();
            }
            
            if (UnityEngine.Input.GetKeyDown(KeyCode.R) && Object.HasInputAuthority)
            {
                GameController.ResetLevel();
            }
        }

        private void SetCameraFollowPlayer()
        {
            var cameraFollow = Camera.main.GetComponent<CameraFollow>();
            cameraFollow.SetTarget(transform);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Flag"))
            {
                var flag = other.GetComponent<Flag>();
                Runner.Despawn(flag.Object);
                GameController.Instance.WinLevel(this);
            }
        }

        public void Teleport(Vector3 position)
        {
            transform.position = position;
        }
    }
}
