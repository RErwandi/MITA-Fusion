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

        [Networked]
        private bool IsGameStarted { get; set; }

        [Networked]
        private TickTimer LevelTimer { get; set; }

        public float RemainingTime
        {
            get
            {
                if (!IsGameStarted)
                {
                    return 0f;
                }

                if (LevelTimer.IsRunning)
                {
                    return (float)LevelTimer.RemainingTime(Runner);
                }

                return 0f;
            }
        }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Runner.Despawn(Object);
            }
        }

        public void StartLevel()
        {
            if (Object.HasStateAuthority)
            {
                Debug.Log("Start Level");
                LevelTimer = TickTimer.CreateFromSeconds(Runner, levelTime);
                IsGameStarted = true;
                
                SpawnFlag();
                RPC_AllowPlayerMovement(true);
            }
        }

        public void ResetLevel()
        {
            RPC_ResetLevel();
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_ResetLevel()
        {
            Debug.Log("Reset");
            Player.LocalPlayer.Teleport(Vector3.zero);
        }

        public void WinLevel(Player player)
        {
            if (Object.HasStateAuthority)
            {
                IsGameStarted = false;
                RPC_Win();
            }
        }
        
        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_Win()
        {
            Debug.Log("Win");
            RPC_AllowPlayerMovement(false);
        }

        private void Draw()
        {
            if (Object.HasStateAuthority)
            {
                IsGameStarted = false;
                RPC_Draw();
            }
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_Draw()
        {
            Debug.Log("Draw");
            RPC_AllowPlayerMovement(false);
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
            if (LevelTimer.Expired(Runner) && IsGameStarted)
            {
                Draw();
            }
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_AllowPlayerMovement(bool value)
        {
            Player.LocalPlayer.Input.SetAllowInput(value);
        }
    }
}