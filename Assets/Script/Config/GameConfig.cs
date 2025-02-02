using System;
using Unity.VisualScripting;

namespace GameConfig
{
    /// <summary>
    /// 地图上放置的物体类型
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
        //当前节点对应的二维区间
        public int xLeft = -1;
        public int xRight = -1;
        public int zBottom = -1;
        public int zTop = -1;

        /// <summary>
        /// 左子树
        /// </summary>
        public int lTree = -1;
        /// <summary>
        /// 右子树
        /// </summary>
        public int rTree = -1;

        /// <summary>
        /// 当前子树放的方块数量
        /// </summary>
        public int count = 0;

        /// <summary>
        /// 当前位置的方块类型，只有子节点才有意义
        /// </summary>
        public GraphObjType type = GraphObjType.None;
    }
}