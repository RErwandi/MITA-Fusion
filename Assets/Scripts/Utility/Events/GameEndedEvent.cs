namespace Mita
{
    public struct GameEndedEvent
    {
        public Player winner;

        public GameEndedEvent(Player winner)
        {
            this.winner = winner;
        }

        private static GameEndedEvent gameEvent;

        public static void Trigger(Player winner)
        {
            gameEvent.winner = winner;
            EventManager.TriggerEvent(gameEvent);
        }
    }
}