using Fusion;
using UnityEngine;

namespace Mita
{
    public class GameController : NetworkBehaviour
    {
        [SerializeField] private NetworkPrefabRef flagPrefab;
        [SerializeField] private float levelTime = 20f;
        [SerializeField] private float flagDistance = 20f;

        private static GameController instance;
        public static GameController Instance => instance;

        private bool isGameStarted;

        [Networked]
        private TickTimer LevelTimer { get; set; }

        public float RemainingTime
        {
            get
            {
                if (LevelTimer.IsRunning)
                {
                    return (float)LevelTimer.RemainingTime(Runner);
                }

                return 0f;
            }
        }

        private void Awake()
        {
            instance = this;
        }

        public void StartLevel()
        {
            if (Object.HasStateAuthority)
            {
                Debug.Log("Start Level");
                LevelTimer = TickTimer.CreateFromSeconds(Runner, levelTime);
                isGameStarted = true;
                
                SpawnFlag();
            }
        }

        private void Draw()
        {
            Debug.Log("Draw");
            isGameStarted = false;
        }

        private void SpawnFlag()
        {
            float randomAngle = Random.Range(0f, 2f * Mathf.PI);
            float spawnX = transform.position.x + flagDistance * Mathf.Cos(randomAngle);
            float spawnZ = transform.position.z + flagDistance * Mathf.Sin(randomAngle);
            
            var spawnPosition = new Vector3(spawnX, 0f, spawnZ);
            Runner.Spawn(flagPrefab, spawnPosition, Quaternion.identity);
        }

        public override void FixedUpdateNetwork()
        {
            if (LevelTimer.Expired(Runner) && isGameStarted)
            {
                Draw();
            }
        }
    }
}