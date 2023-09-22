using Fusion;

namespace Mita
{
    public class Flag : NetworkBehaviour, IEventListener<GameEvent>
    {
        public override void Spawned()
        {
            Player.LocalPlayer.UI.SetTarget(transform);
        }

        private void OnDestroy()
        {
            Player.LocalPlayer.UI.SetTarget(null);
        }

        public void OnEvent(GameEvent e)
        {
            if (e.eventName == Constants.EVENT_GAME_DRAW)
            {
                Runner.Despawn(Object);
            }
        }
    }
}
