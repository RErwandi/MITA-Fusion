using TMPro;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Mita
{
    public class PlayerUI : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private RectTransform flagRect;
        [SerializeField] private RectTransform canvasRect;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI timerText;
        
        private Player localPlayer;
        private Camera mainCam;

        private void Start()
        {
            mainCam = Camera.main;
        }

        public void SetLocalPLayer(Player player)
        {
            localPlayer = player;
        }

        private void Update()
        {
            UpdateTimer();
            UpdateIndicator();
        }

        private void UpdateTimer()
        {
            if (localPlayer != null)
            {
                var time = localPlayer.GameController.RemainingTime;
                timerText.text = time <= 0 ? $"" : $"Time: {time:n2}";
            }
        }

        private void UpdateIndicator()
        {
            if (target != null)
            {
                var screenPos = mainCam.WorldToViewportPoint(target.position);
                
                // Check if the target is outside the screen
                if (screenPos.x < 0f || screenPos.x > 1f || screenPos.y < 0f || screenPos.y > 1f)
                {
                    flagRect.gameObject.SetActive(true);
                    
                    // Calculate the position of the indicator on the edge of the screen
                    Vector2 edgePosition = screenPos;

                    // Ensure the indicator stays within the screen boundaries
                    edgePosition.x = Mathf.Clamp01(edgePosition.x);
                    edgePosition.y = Mathf.Clamp01(edgePosition.y);
                    
                    edgePosition.x *= Screen.width - flagRect.sizeDelta.x;
                    edgePosition.y *= Screen.height - flagRect.sizeDelta.y;
                    
                    flagRect.anchoredPosition = edgePosition;
                }
                else
                {
                    // Hide the indicator if the target is inside the screen
                    flagRect.gameObject.SetActive(false);
                }
            }
            else
            {
                // Hide the indicator if there's no target object
                flagRect.gameObject.SetActive(false);
            }
        }
    }
}
