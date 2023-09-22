using System;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Mita
{
    public class GameController : NetworkBehaviour, INetworkRunnerCallbacks
    {
        [SerializeField] private NetworkPrefabRef flagPrefab;
        [SerializeField] private float levelTime = 20f;
        [SerializeField] private float flagDistance = 20f;

        private static GameController instance;
        public static GameController Instance => instance;

        private NetworkObject currentFlag;

        [Networked]
        private bool IsGameStarted { get; set; }

        [Networked]
        private TickTimer LevelTimer { get; set; }
        
        [Networked]
        private int Level { get; set; }

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

        public override void Spawned()
        {
            Runner.AddCallbacks(this);
        }

        private void StartLevel()
        {
            if (Object.HasStateAuthority)
            {
                Level++;
                LevelTimer = TickTimer.CreateFromSeconds(Runner, levelTime);
                IsGameStarted = true;
                
                SpawnFlag();
            }
            
            LevelChangedEvent.Trigger(Level);
            GameEvent.Trigger(Constants.EVENT_GAME_START);
        }

        public void ResetLevel()
        {
            RPC_ResetLevel();
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_ResetLevel()
        {
            StartLevel();
        }

        public void EndLevel(Player player)
        {
            if (Object.HasStateAuthority)
            {
                IsGameStarted = false;
                RPC_End(player);
            }
        }
        
        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_End(Player player)
        {
            Debug.Log($"Game Ended");
            GameEndedEvent.Trigger(player);
        }

        private void Draw()
        {
            if (Object.HasStateAuthority)
            {
                IsGameStarted = false;
                Runner.Despawn(currentFlag);
                RPC_Draw();
            }
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_Draw()
        {
            Debug.Log($"Game Draw");
            GameEvent.Trigger(Constants.EVENT_GAME_DRAW);
        }

        private void SpawnFlag()
        {
            float randomAngle = Random.Range(0f, 2f * Mathf.PI);
            float spawnX = transform.position.x + flagDistance * Mathf.Cos(randomAngle);
            float spawnZ = transform.position.z + flagDistance * Mathf.Sin(randomAngle);
            
            var spawnPosition = new Vector3(spawnX, 0f, spawnZ);
            currentFlag = Runner.Spawn(flagPrefab, spawnPosition, Quaternion.identity);
        }

        public override void FixedUpdateNetwork()
        {
            if (LevelTimer.Expired(Runner) && IsGameStarted)
            {
                Draw();
            }
        }
        
        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            Debug.Log($"Player joined. Current players: {runner.ActivePlayers.Count()}");
            if (runner.IsServer && runner.ActivePlayers.Count() == 2)
            {
                StartLevel();
            }
        }
        
        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            
        }

        #region Unused Network Behaviour Callbacks

        public void OnInput(NetworkRunner runner, NetworkInput input) { }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }

        public void OnConnectedToServer(NetworkRunner runner) { }

        public void OnDisconnectedFromServer(NetworkRunner runner) { }

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }

        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }

        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }

        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken){ }

        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }

        public void OnSceneLoadDone(NetworkRunner runner) { }

        public void OnSceneLoadStart(NetworkRunner runner) { }
        
        #endregion
    }
}