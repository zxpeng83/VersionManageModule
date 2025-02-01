using GameConfig;
using System.Collections.Generic;

/// <summary>
/// 可持久化2维线段树（实现类似git的版本管理）
/// </summary>
public class PersistentSegmentTree2D
{
    private static PersistentSegmentTree2D ins = null;
    public static PersistentSegmentTree2D instance
    {
        get
        {
            if (ins == null)
            {
                ins = new PersistentSegmentTree2D();
            }

            return ins;
        }
    }

    private static int nodeCount = 40000;
    private static int versionCount = 40;
    private TreeNode[] tree = new TreeNode[PersistentSegmentTree2D.nodeCount];
    private int[] versionRoot = new int[PersistentSegmentTree2D.versionCount];
    /// <summary>
    /// 节点下标
    /// </summary>
    private int nodeIdx = 0;
    /// <summary>
    /// 版本下标
    /// </summary>
    private int versionIdx = 0;

    /// <summary>
    /// 重置
    /// </summary>
    public void reset()
    {
        this.nodeIdx = 0;
        this.versionIdx = 0;
        this.versionRoot[this.versionIdx] = this.build(1, 20, 1, 20, true);
    }

    /// <summary>
    /// 放置方块
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <param name="type"></param>
    public void putCube(int x, int z, GraphObjType type)
    {
        int preVersionIdx = this.versionIdx;
        this.versionIdx++;
        this.versionRoot[this.versionIdx] = insert(preVersionIdx, 1, 20, 1, 20, x, z, type, true);
    }

    /// <summary>
    /// 建立线段树
    /// </summary>
    /// <param name="xLeft">当前节点对应的区间范围</param>
    /// <param name="xRight">当前节点对应的区间范围</param>
    /// <param name="zBottom">当前节点对应的区间范围</param>
    /// <param name="zTop">当前节点对应的区间范围</param>
    /// <param name="isXAxis">当前是否对x轴进行划分</param>
    /// <returns></returns>
    private int build(int xLeft, int xRight, int zBottom, int zTop, bool isXAxis)
    {
        //返回标志值
        int p = nodeIdx++;

        tree[p].xLeft = xLeft;
        tree[p].xRight = xRight;
        tree[p].zBottom = zBottom;
        tree[p].zTop = zTop;
        if (xLeft == xRight && zBottom == zTop)
        {   //到叶子节点了
            tree[p].count = 0;
            tree[p].type = GraphObjType.None;
            tree[p].lTree = -1;
            tree[p].rTree = -1;
            return p;
        }

        if (isXAxis)
        {
            //划分x轴
            int xMid = (xLeft + xRight) >> 1;
            tree[p].lTree = this.build(xLeft, xMid, zBottom, zTop, !isXAxis);
            tree[p].rTree = this.build(xRight, xMid+1, zBottom, zTop, !isXAxis);
        }
        else
        {
            //划分z轴
            int zMid = (xLeft + xRight) >> 1;
            tree[p].lTree = this.build(xLeft, xRight, zBottom, zMid, !isXAxis);
            tree[p].rTree = this.build(xLeft, xRight, zMid+1, zTop, !isXAxis);
        }
        tree[p].count = tree[tree[p].lTree].count + tree[tree[p].rTree].count;

        return p;
    }

    /// <summary>
    /// 放置方块
    /// </summary>
    /// <param name="preNode">当前区间对应上一个版本的节点</param>
    /// <param name="xLeft">当前区间范围</param>
    /// <param name="xRight">当前区间范围</param>
    /// <param name="zBottom">当前区间范围</param>
    /// <param name="zTop">当前区间范围</param>
    /// <param name="x">放置方块的位置</param>
    /// <param name="z">放置方块的位置</param>
    /// <param name="type">放置的方块类型</param>
    /// <param name="isXAxis">当前是否划分x轴</param>
    /// <returns></returns>
    private int insert(int preNode, int xLeft, int xRight, int zBottom, int zTop, int x, int z, GraphObjType type, bool isXAxis)
    {
        //返回值是新版本树节点的标志值
        int curNode = nodeIdx++;
        tree[curNode] = tree[preNode];  //先将上一个版本信息复制过来
        if (xLeft == xRight && zBottom == zTop)
        {  
            //遍历到叶子节点，同时找到了要插入的位置，cnt++
            tree[curNode].count = 1;
            tree[curNode].type = type;
            return curNode;
        }

        //接着找要插入的位置
        if (isXAxis)
        {
            int xMid = (xLeft + xRight) >> 1;
            if (x <= xMid)
            {
                tree[curNode].lTree = this.insert(tree[curNode].lTree, xLeft, xMid, zBottom, zTop, x, z, type, !isXAxis);
            }
            else
            {
                tree[curNode].rTree = this.insert(tree[curNode].rTree, xMid + 1, xRight, zBottom, zTop, x, z, type, !isXAxis);
            }
        }
        else
        {
            int zMid = (zBottom + zTop) >> 1;
            if(z <= zMid)
            {
                tree[curNode].lTree = this.insert(tree[curNode].lTree, xLeft, xRight, zBottom, zMid, x, z, type, !isXAxis);
            }
            else
            {
                tree[curNode].rTree = this.insert(tree[curNode].rTree, xLeft, xRight, zMid+1, zBottom, x, z, type, !isXAxis);
            }
        }
        
        tree[curNode].count = tree[tree[curNode].lTree].count + tree[tree[curNode].rTree].count;

        return curNode;
    }
}
