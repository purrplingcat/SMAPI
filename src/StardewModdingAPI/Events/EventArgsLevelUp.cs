using System;

namespace StardewModdingAPI.Events
{
    public class EventArgsLevelUp : EventArgs
    {
        public enum LevelType
        {
            Combat,
            Farming,
            Fishing,
            Foraging,
            Mining,
            Luck
        }

        public EventArgsLevelUp(LevelType type, int newLevel)
        {
            Type = type;
            NewLevel = newLevel;
        }

        public LevelType Type { get; private set; }
        public int NewLevel { get; private set; }
    }
}