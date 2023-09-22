namespace Mita
{
    public struct LevelChangedEvent
    {
        public int level;

        public LevelChangedEvent(int level)
        {
            this.level = level;
        }

        private static LevelChangedEvent gameEvent;

        public static void Trigger(int level)
        {
            gameEvent.level = level;
            EventManager.TriggerEvent(gameEvent);
        }
    }
}