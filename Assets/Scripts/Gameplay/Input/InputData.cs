using Fusion;
using UnityEngine;

namespace Mita
{
    public struct InputData : INetworkInput
    {
        public Vector2 moveDirection;
        public Vector2 mousePosition;
    }
}