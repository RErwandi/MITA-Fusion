using Fusion;

namespace Mita
{
    public class Flag : NetworkBehaviour
    {
        public override void Spawned()
        {
            Player.LocalPlayer.UI.SetTarget(transform);
        }

        private void OnDestroy()
        {
            Player.LocalPlayer.UI.SetTarget(null);
        }
    }
}
