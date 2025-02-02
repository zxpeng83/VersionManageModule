using GameConfig;
using System.Collections.Generic;
using UnityEngine;
using GameConfig;
using System;

/// <summary>
/// 可持久化2维线段树（实现版本管理）
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
    private static int versionCount = 400;
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
    /// 当前展示的版本
    /// </summary>
    private int showVersionIdx = 0;

    /// <summary>
    /// 重置
    /// </summary>
    public void reset()
    {
        this.nodeIdx = 0;
        this.versionIdx = 0;
        this.showVersionIdx = 0;
        GraphMgr.Instance.removeCube(GraphObjType.Blue);
        GraphMgr.Instance.removeCube(GraphObjType.Red);
        GraphMgr.Instance.removeCube(GraphObjType.Green);
        this.versionRoot[this.versionIdx] = this.build(1, 20, 1, 20, true);
    }

    /// <summary>
    /// 获取总共版本id
    /// </summary>
    /// <returns></returns>
    public int getVersionIdx()
    {
        return versionIdx;
    }

    /// <summary>
    /// 获取当前展示的版本id
    /// </summary>
    /// <returns></returns>
    public int getshowVersionIdx()
    {
        return showVersionIdx;
    }

    /// <summary>
    /// 转跳到指定版本
    /// </summary>
    /// <param name="version"></param>
    public void jump2Version(int versionIdx)
    {
        if(this.showVersionIdx == versionIdx)
        {
            return;
        }
        if(versionIdx < 0 || versionIdx > this.versionIdx)
        {
            return;
        }

        this.showVersionIdx = versionIdx;

        GraphMgr.Instance.removeCube(GraphObjType.Blue);
        GraphMgr.Instance.removeCube(GraphObjType.Red);
        GraphMgr.Instance.removeCube(GraphObjType.Green);

        this.dfsTree(this.versionRoot[versionIdx], (node) =>
        {
            if (node.lTree < 0 && node.rTree < 0 && node.xLeft == node.xRight && node.zBottom == node.zTop)
            {
                //叶子节点
                GraphMgr.Instance.putCube(new Vector3(node.xLeft + 0.5f, 0, node.zBottom + 0.5f), node.type);
            }
        });
    }

    private void dfsTree(int parent, Action<TreeNode> callback)
    {
        if(parent < 0)
        {
            return;
        }

        callback(tree[parent]);

        if (tree[parent].lTree >= 0)
        {
            this.dfsTree(tree[parent].lTree, callback);
        }
        if (tree[parent].rTree >= 0)
        {
            this.dfsTree(tree[parent].rTree, callback);
        }
    }

    /// <summary>
    /// 放置方块
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <param name="type"></param>
    public void putCube(int x, int z, GraphObjType type)
    {
        if(this.versionIdx != this.showVersionIdx)
        {
            this.versionIdx = this.showVersionIdx;
            this.nodeIdx = this.findMaxNodeIdx(this.versionRoot[this.versionIdx]) + 1;
        }

        int preVersionIdx = this.versionIdx;
        this.versionIdx++;
        this.showVersionIdx = this.versionIdx;
        this.versionRoot[this.versionIdx] = insert(this.versionRoot[preVersionIdx], 1, 20, 1, 20, x, z, type, true);
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
        //Debug.LogError(xLeft + "  " +  xRight + "  " + zBottom + "  " + zTop + "  " + isXAxis);
        //返回标志值
        int p = nodeIdx++;

        if (tree[p] == null)
        {
            tree[p] = new TreeNode();
        }

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

        if (isXAxis && xLeft != xRight)
        {
            //划分x轴
            int xMid = (xLeft + xRight) >> 1;
            tree[p].lTree = this.build(xLeft, xMid, zBottom, zTop, !isXAxis);
            tree[p].rTree = this.build(xMid+1, xRight, zBottom, zTop, !isXAxis);
        }
        else if(!isXAxis && zBottom != zTop)
        {
            //划分z轴
            int zMid = (zBottom + zTop) >> 1;
            tree[p].lTree = this.build(xLeft, xRight, zBottom, zMid, !isXAxis);
            tree[p].rTree = this.build(xLeft, xRight, zMid+1, zTop, !isXAxis);
        }
        else if(xLeft != xRight)
        {
            //划分x轴
            int xMid = (xLeft + xRight) >> 1;
            tree[p].lTree = this.build(xLeft, xMid, zBottom, zTop, !isXAxis);
            tree[p].rTree = this.build(xMid + 1, xRight, zBottom, zTop, !isXAxis);
        }
        else if(zBottom != zTop)
        {
            //划分z轴
            int zMid = (zBottom + zTop) >> 1;
            tree[p].lTree = this.build(xLeft, xRight, zBottom, zMid, !isXAxis);
            tree[p].rTree = this.build(xLeft, xRight, zMid + 1, zTop, !isXAxis);
        }
        //Debug.LogErrorFormat("ltree:{0}  rtree:{0}", tree[p].lTree, tree[p].rTree);
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
        
        tree[curNode] = new TreeNode(tree[preNode]);  //先将上一个版本信息复制过来
        if (xLeft == xRight && zBottom == zTop)
        {  
            //遍历到叶子节点，同时找到了要插入的位置，cnt++
            tree[curNode].count = 1;
            tree[curNode].type = type;
            return curNode;
        }

        //接着找要插入的位置
        if (isXAxis && xLeft != xRight)
        {
            int xMid = (xLeft + xRight) >> 1;
            if (x <= xMid)
            {
                tree[curNode].lTree = this.insert(tree[preNode].lTree, xLeft, xMid, zBottom, zTop, x, z, type, !isXAxis);
            }
            else
            {
                tree[curNode].rTree = this.insert(tree[preNode].rTree, xMid + 1, xRight, zBottom, zTop, x, z, type, !isXAxis);
            }
        }
        else if(!isXAxis && zBottom != zTop)
        {
            int zMid = (zBottom + zTop) >> 1;
            if(z <= zMid)
            {
                tree[curNode].lTree = this.insert(tree[preNode].lTree, xLeft, xRight, zBottom, zMid, x, z, type, !isXAxis);
            }
            else
            {
                tree[curNode].rTree = this.insert(tree[preNode].rTree, xLeft, xRight, zMid+1, zTop, x, z, type, !isXAxis);
            }
        }
        else if(xLeft != xRight)
        {
            int xMid = (xLeft + xRight) >> 1;
            if (x <= xMid)
            {
                tree[curNode].lTree = this.insert(tree[preNode].lTree, xLeft, xMid, zBottom, zTop, x, z, type, !isXAxis);
            }
            else
            {
                tree[curNode].rTree = this.insert(tree[preNode].rTree, xMid + 1, xRight, zBottom, zTop, x, z, type, !isXAxis);
            }
        }
        else if(zBottom != zTop)
        {
            int zMid = (zBottom + zTop) >> 1;
            if (z <= zMid)
            {
                tree[curNode].lTree = this.insert(tree[preNode].lTree, xLeft, xRight, zBottom, zMid, x, z, type, !isXAxis);
            }
            else
            {
                tree[curNode].rTree = this.insert(tree[preNode].rTree, xLeft, xRight, zMid + 1, zTop, x, z, type, !isXAxis);
            }
        }
        
        tree[curNode].count = tree[tree[curNode].lTree].count + tree[tree[curNode].rTree].count;

        return curNode;
    }

    /// <summary>
    /// 寻找改版本树使用的最大树节点下标
    /// </summary>
    /// <param name="version"></param>
    /// <returns></returns>
    private int findMaxNodeIdx(int version)
    {
        int maxIdx = version;
        if (tree[version].lTree >= 0)
        {
            maxIdx = Mathf.Max(maxIdx, this.findMaxNodeIdx(tree[version].lTree));
        }
        if (tree[version].rTree >= 0)
        {
            maxIdx = Mathf.Max(maxIdx, this.findMaxNodeIdx(tree[version].rTree));
        }

        return maxIdx;
    }
}
