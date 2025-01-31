namespace GameConfig
{
    /// <summary>
    /// 游戏模式
    /// </summary>
    public enum GameMode
    {
        AStar,
    }

    /// <summary>
    /// 地图上放置的物体类型
    /// </summary>
    public enum GraphObjType
    {
        None = 0,

        Target = -1,
        Barrier = 1,
        Wall = 2,
        Char = 3,
        Fake = 4,
        Pet = 5,
    }

    /// <summary>
    /// 地图坐标系x和z轴的范围
    /// </summary>
    public class RangeXYZ
    {
        public static int minX = 1;
        public static int maxX = 21;
        public static int minZ = 1;
        public static int maxZ = 21;
    }

    /// <summary>
    /// 角色各移动方向
    /// </summary>
    public class MoveDirec
    {
        public static int[] dx = { -1, -1, 0, 1, 1, 1, 0, -1 };
        public static int[] dy = { 0, 1, 1, 1, 0, -1, -1, -1 };
    }
}