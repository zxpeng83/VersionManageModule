namespace GameConfig
{
    /// <summary>
    /// ��Ϸģʽ
    /// </summary>
    public enum GameMode
    {
        AStar,
    }

    /// <summary>
    /// ��ͼ�Ϸ��õ���������
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
    /// ��ͼ����ϵx��z��ķ�Χ
    /// </summary>
    public class RangeXYZ
    {
        public static int minX = 1;
        public static int maxX = 21;
        public static int minZ = 1;
        public static int maxZ = 21;
    }

    /// <summary>
    /// ��ɫ���ƶ�����
    /// </summary>
    public class MoveDirec
    {
        public static int[] dx = { -1, -1, 0, 1, 1, 1, 0, -1 };
        public static int[] dy = { 0, 1, 1, 1, 0, -1, -1, -1 };
    }
}