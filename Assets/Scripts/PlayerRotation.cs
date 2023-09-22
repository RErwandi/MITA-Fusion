using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mita
{
    public class PlayerRotation : MonoBehaviour
    {
        private Camera mainCamera;

        private void Start()
        {
            mainCamera = Camera.main;
        }

        private void Update()
        {
            LookAtCursorPosition();
        }

        private void LookAtCursorPosition()
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

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