using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;

namespace Mita
{
    [RequireComponent(typeof(PlayerInput))]
    public class Player : NetworkBehaviour, INetworkRunnerCallbacks, IEventListener<GameEvent>, IEventListener<GameEndedEvent>
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

        private void OnEnable()
        {
            EventManager.AddListener<GameEvent>(this);
            EventManager.AddListener<GameEndedEvent>(this);
        }

        private void OnDisable()
        {
            EventManager.RemoveListener<GameEvent>(this);
            EventManager.RemoveListener<GameEndedEvent>(this);
        }

        public override void Spawned()
        {
            Runner.AddCallbacks(this);
            
            if (Object.HasInputAuthority)
            {
                SetCameraFollowPlayer();
                LocalPlayer = this;
                Input.AllowInput = false;

                gameObject.name = $"Player{Object.Id}";
                
                var ui = Instantiate(playerUI);
                UI = ui;
                
                UI.SetLocalPLayer(this);
            }

            if (Runner.IsServer && Object.HasInputAuthority)
            {
                Runner.Spawn(gameController);
            }

            GameController = GameController.Instance;
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
                GameController.Instance.EndLevel(this);
            }
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        private void RPC_Teleport(Vector3 position)
        {
            transform.position = position;
        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            if (Object.HasInputAuthority)
            {
                UI.UpdateNotification();
            }
        }
        
        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            if (Object.HasInputAuthority)
            {
                UI.UpdateNotification();
            }
        }
        
        public void OnEvent(GameEvent e)
        {
            if (e.eventName == Constants.EVENT_GAME_DRAW)
            {
                Input.AllowInput = false;
            }

            if (e.eventName == Constants.EVENT_GAME_START)
            {
                Input.AllowInput = true;
                RPC_Teleport(Vector3.zero);
            }
        }
        
        public void OnEvent(GameEndedEvent e)
        {
            Input.AllowInput = false;
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
