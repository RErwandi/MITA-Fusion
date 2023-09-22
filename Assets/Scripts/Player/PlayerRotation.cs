using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using Object = System.Object;

namespace Mita
{
    public class PlayerRotation : NetworkBehaviour
    {
        private Camera mainCamera;
        private Player player;
        private PlayerInput playerInput;
        
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
            mainCamera = Camera.main;
        }

        private void OnGetPlayerInput(InputData input)
        {
            LookAtCursorPosition(input.mousePosition);
        }

        private void LookAtCursorPosition(Vector2 mousePos)
        {
            Ray ray = mainCamera.ScreenPointToRay(mousePos);

            if (Physics.Raycast(ray, out var hit))
            {
                transform.LookAt(hit.point);
                
                Vector3 eulerAngles = transform.rotation.eulerAngles;
                eulerAngles.x = 0f;
                eulerAngles.z = 0f;

                transform.rotation = Quaternion.Euler(eulerAngles);
            }
        }
    }
}