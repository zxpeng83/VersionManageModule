using System;
using Unity.VisualScripting;

namespace GameConfig
{
    /// <summary>
    /// ��ͼ�Ϸ��õ���������
    /// </summary>
    public enum GraphObjType
    {
        None = 0,
        Wall = -1,

        Red = 1,
        Green = 2,
        Blue = 3,
        Fake = 4,
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

    public class TreeNode
    {
        public TreeNode()
        {
            
        }
        public TreeNode(TreeNode node)
        {
            this.xLeft = node.xLeft;
            this.xRight = node.xRight;
            this.zBottom = node.zBottom;
            this.zTop = node.zTop;

            this.lTree = node.lTree;
            this.rTree = node.rTree;

            this.count = node.count;

            this.type = node.type;
        }
        //��ǰ�ڵ��Ӧ�Ķ�ά����
        public int xLeft = -1;
        public int xRight = -1;
        public int zBottom = -1;
        public int zTop = -1;

        /// <summary>
        /// ������
        /// </summary>
        public int lTree = -1;
        /// <summary>
        /// ������
        /// </summary>
        public int rTree = -1;

        /// <summary>
        /// ��ǰ�����ŵķ�������
        /// </summary>
        public int count = 0;

        /// <summary>
        /// ��ǰλ�õķ������ͣ�ֻ���ӽڵ��������
        /// </summary>
        public GraphObjType type = GraphObjType.None;
    }
}