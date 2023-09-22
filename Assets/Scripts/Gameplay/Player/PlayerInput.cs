using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;

namespace Mita
{
    public class PlayerInput : NetworkBehaviour, INetworkRunnerCallbacks
    {
        [SerializeField] private bool allowInput = true;
        public event Action<InputData> OnGetInput;

        private PlayerInputActions playerInputActions;
        private InputData inputData;

        private void Awake()
        {
            playerInputActions = new PlayerInputActions();
        }

        public override void Spawned()
        {
            playerInputActions.Player.Enable();
            Runner.AddCallbacks(this);
        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
            if (!allowInput)
            {
                inputData.moveDirection = Vector2.zero;
                inputData.mousePosition = Vector2.zero;
            }
            else
            {
                inputData.moveDirection = playerInputActions.Player.MoveDirection.ReadValue<Vector2>();
                inputData.mousePosition = playerInputActions.Player.MousePosition.ReadValue<Vector2>();
            }
            
            input.Set(inputData);
            inputData = default;
        }

        public override void FixedUpdateNetwork()
        {
            if (GetInput<InputData>(out var input) == false) return;

            OnGetInput?.Invoke(input);
        }

        public void SetAllowInput(bool value)
        {
            allowInput = value;
        }

        #region Unused Network Runner Callbacks
        
        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            
        }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
        {
            
        }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
            
        }

        public void OnConnectedToServer(NetworkRunner runner)
        {
            
        }

        public void OnDisconnectedFromServer(NetworkRunner runner)
        {
            
        }

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
        {
            
        }

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
        {
           
        }

        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
        {
            
        }

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
        {
            
        }

        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
        {
            
        }

        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
        {
            
        }

        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
        {
            
        }

        public void OnSceneLoadDone(NetworkRunner runner)
        {
            
        }

        public void OnSceneLoadStart(NetworkRunner runner)
        {
            
        }
        #endregion
    }

}