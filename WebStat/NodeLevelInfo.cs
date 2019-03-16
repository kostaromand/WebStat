namespace WebStat
{
    public class NodeLevelInfo
    {
        public LevelType LevelType { get; private set; }
        public PopupLevelType PopupType { get; private set; }
        public int Value { get; private set; }
        public NodeLevelInfo(LevelType levelType, PopupLevelType popupType,int value)
        {
            this.LevelType = levelType;
            this.PopupType = popupType;
            this.Value = value;
        }
    }
}