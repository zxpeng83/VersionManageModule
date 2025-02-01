using GameConfig;
using System.Collections.Generic;

/// <summary>
/// �ɳ־û�2ά�߶�����ʵ������git�İ汾����
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
    /// �ڵ��±�
    /// </summary>
    private int nodeIdx = 0;
    /// <summary>
    /// �汾�±�
    /// </summary>
    private int versionIdx = 0;

    /// <summary>
    /// ����
    /// </summary>
    public void reset()
    {
        this.nodeIdx = 0;
        this.versionIdx = 0;
        this.versionRoot[this.versionIdx] = this.build(1, 20, 1, 20, true);
    }

    /// <summary>
    /// ���÷���
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
    /// �����߶���
    /// </summary>
    /// <param name="xLeft">��ǰ�ڵ��Ӧ�����䷶Χ</param>
    /// <param name="xRight">��ǰ�ڵ��Ӧ�����䷶Χ</param>
    /// <param name="zBottom">��ǰ�ڵ��Ӧ�����䷶Χ</param>
    /// <param name="zTop">��ǰ�ڵ��Ӧ�����䷶Χ</param>
    /// <param name="isXAxis">��ǰ�Ƿ��x����л���</param>
    /// <returns></returns>
    private int build(int xLeft, int xRight, int zBottom, int zTop, bool isXAxis)
    {
        //���ر�־ֵ
        int p = nodeIdx++;

        tree[p].xLeft = xLeft;
        tree[p].xRight = xRight;
        tree[p].zBottom = zBottom;
        tree[p].zTop = zTop;
        if (xLeft == xRight && zBottom == zTop)
        {   //��Ҷ�ӽڵ���
            tree[p].count = 0;
            tree[p].type = GraphObjType.None;
            tree[p].lTree = -1;
            tree[p].rTree = -1;
            return p;
        }

        if (isXAxis)
        {
            //����x��
            int xMid = (xLeft + xRight) >> 1;
            tree[p].lTree = this.build(xLeft, xMid, zBottom, zTop, !isXAxis);
            tree[p].rTree = this.build(xRight, xMid+1, zBottom, zTop, !isXAxis);
        }
        else
        {
            //����z��
            int zMid = (xLeft + xRight) >> 1;
            tree[p].lTree = this.build(xLeft, xRight, zBottom, zMid, !isXAxis);
            tree[p].rTree = this.build(xLeft, xRight, zMid+1, zTop, !isXAxis);
        }
        tree[p].count = tree[tree[p].lTree].count + tree[tree[p].rTree].count;

        return p;
    }

    /// <summary>
    /// ���÷���
    /// </summary>
    /// <param name="preNode">��ǰ�����Ӧ��һ���汾�Ľڵ�</param>
    /// <param name="xLeft">��ǰ���䷶Χ</param>
    /// <param name="xRight">��ǰ���䷶Χ</param>
    /// <param name="zBottom">��ǰ���䷶Χ</param>
    /// <param name="zTop">��ǰ���䷶Χ</param>
    /// <param name="x">���÷����λ��</param>
    /// <param name="z">���÷����λ��</param>
    /// <param name="type">���õķ�������</param>
    /// <param name="isXAxis">��ǰ�Ƿ񻮷�x��</param>
    /// <returns></returns>
    private int insert(int preNode, int xLeft, int xRight, int zBottom, int zTop, int x, int z, GraphObjType type, bool isXAxis)
    {
        //����ֵ���°汾���ڵ�ı�־ֵ
        int curNode = nodeIdx++;
        tree[curNode] = tree[preNode];  //�Ƚ���һ���汾��Ϣ���ƹ���
        if (xLeft == xRight && zBottom == zTop)
        {  
            //������Ҷ�ӽڵ㣬ͬʱ�ҵ���Ҫ�����λ�ã�cnt++
            tree[curNode].count = 1;
            tree[curNode].type = type;
            return curNode;
        }

        //������Ҫ�����λ��
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
