using System;
using Fusion;
using UnityEngine;

namespace Mita
{
    [RequireComponent(typeof(Player))]
    public class PlayerMovement : NetworkBehaviour
    {
        [SerializeField] private float moveSpeed = 5f;

        private Player player;
        private PlayerInput playerInput;
        private Transform cameraTransform;

        private void Awake()
        {
            player = GetComponent<Player>();
            playerInput = player.Input;
        }
        
        private void OnEnable()
        {
            playerInput.OnGetInput += OnGetPlayerInput;
        }

        private void OnDisable()
        {
            playerInput.OnGetInput -= OnGetPlayerInput;
        }

        public override void Spawned()
        {
            cameraTransform = Camera.main.transform;
        }

        private void OnGetPlayerInput(InputData input)
        {
            MoveRelativeToCamera(input.moveDirection);
        }

        private void MoveRelativeToCamera(Vector2 dir)
        {
            var cameraForward = cameraTransform.up;
            var cameraRight = cameraTransform.right;
            
            cameraForward.y = 0f;
            cameraRight.y = 0f;
            
            cameraForward.Normalize();
            cameraRight.Normalize();

            var verticalInput = dir.y;
            var horizontalInput = dir.x;
            
            var movement = (cameraForward * verticalInput + cameraRight * horizontalInput) * moveSpeed * Time.deltaTime;

            transform.Translate(movement, Space.World);
        }
    }
}
