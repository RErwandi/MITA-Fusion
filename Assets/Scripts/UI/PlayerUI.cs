using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mita
{
    public class PlayerUI : MonoBehaviour, IEventListener<GameEvent>, IEventListener<GameEndedEvent>, IEventListener<LevelChangedEvent>
    {
        [SerializeField] private Transform target;
        [SerializeField] private RectTransform flagRect;
        [SerializeField] private RectTransform canvasRect;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private TextMeshProUGUI notificationText;
        [SerializeField] private Button retryButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private GameObject buttonContainer;
        
        private Player localPlayer;
        private Camera mainCam;

        private void OnEnable()
        {
            EventManager.AddListener<GameEvent>(this);
            EventManager.AddListener<GameEndedEvent>(this);
            EventManager.AddListener<LevelChangedEvent>(this);
            
            retryButton.onClick.AddListener(Retry);
            quitButton.onClick.AddListener(Quit);
        }

        private void OnDisable()
        {
            EventManager.RemoveListener<GameEvent>(this);
            EventManager.RemoveListener<GameEndedEvent>(this);
            EventManager.RemoveListener<LevelChangedEvent>(this);
            
            retryButton.onClick.RemoveListener(Retry);
            quitButton.onClick.RemoveListener(Quit);
        }

        private void Retry()
        {
            GameController.Instance.ResetLevel();
        }
        
        private void Quit()
        {
            Application.Quit();
        }

        private void Start()
        {
            mainCam = Camera.main;
        }

        public void SetLocalPLayer(Player player)
        {
            localPlayer = player;

            if (!player.Object.HasStateAuthority)
            {
                retryButton.interactable = false;
            }
            
            UpdateNotification();
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
                    edgePosition.x = Mathf.Clamp(edgePosition.x, 0.05f, 0.95f);
                    edgePosition.y = Mathf.Clamp(edgePosition.y, 0.05f, 0.95f);
                    
                    edgePosition.x *= canvasRect.sizeDelta.x;
                    edgePosition.y *= canvasRect.sizeDelta.y;

                    flagRect.anchoredPosition = edgePosition;
                    
                    var angle = Mathf.Atan2(screenPos.y - 0.5f, screenPos.x - 0.5f) * Mathf.Rad2Deg;
                    flagRect.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
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

        public void SetTarget(Transform target)
        {
            this.target = target;
        }

        private void Win()
        {
            notificationText.text = Constants.TEXT_WIN;
            buttonContainer.SetActive(true);
        }

        private void Lose()
        {
            notificationText.text = Constants.TEXT_LOSE;
            buttonContainer.SetActive(true);
        }

        private void Draw()
        {
            notificationText.text = Constants.TEXT_DRAW;
            buttonContainer.SetActive(true);
        }

        public void UpdateNotification()
        {
            notificationText.text = NetworkManager.Instance.HasEnoughPlayer ? string.Empty : Constants.TEXT_WAITING;
            buttonContainer.SetActive(false);
        }

        public void OnEvent(GameEvent e)
        {
            if (e.eventName == Constants.EVENT_GAME_DRAW)
            {
                Draw();
            }

            if (e.eventName == Constants.EVENT_GAME_START)
            {
                UpdateNotification();
            }
        }

        public void OnEvent(GameEndedEvent e)
        {
            if (e.winner == localPlayer)
            {
                Win();
            }
            else
            {
                Lose();
            }
        }

        public void OnEvent(LevelChangedEvent e)
        {
            levelText.text = $"Level: {e.level}";
        }
    }
}
