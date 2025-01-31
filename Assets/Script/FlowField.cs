using GameConfig;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FlowField
{
    private static FlowField ins = null;
    public static FlowField instance
    {
        get
        {
            if (ins == null)
            {
                ins = new FlowField();
            }

            return ins;
        }
    }

    private static int[] dir = { 0, 2, 4, 6, 1, 3, 5, 7 };

    private int[][] flowFieldGraph = null;

    public int[][] getGraph()
    {
        if (flowFieldGraph == null)
        {
            this.startNavigation();
        }

        return flowFieldGraph;
    }

    public void startNavigation()
    {
        if (CharMgr.charList.Count <= 0) return;

        ///��ͼ
        int[][] graph = GraphMgr.Instance.getGraph();
        Queue<(int x, int z, int step)> que = new Queue<(int x, int z, int step)>();
        
        ///�յ�
        Vector2Int target = Vector2Int.zero;

        for (int i = 0; i < graph.Length; i++)
        {
            for (int j = 0; j < graph[i].Length; j++)
            {
                if (graph[i][j] == (int)GraphObjType.Target)
                {
                    target = new Vector2Int(i, j);
                }
            }
        }

        if (target == Vector2Int.zero)
        {
            //GraphMgr.Instance.prin();
            Debug.LogError("FlowFieldѰ·ʧ��,��ͼ���Ҳ����յ�");
            this.flowFieldGraph = null;
            return;
        }

        this.resetFFGraph();

        que.Enqueue((target.x, target.y, 0));

        ///��ʼFlowFieldѰ·
        while (que.Count > 0)
        {
            var top = que.Dequeue();

            int x = top.x;
            int z = top.z;
            int step = top.step;

            for(int ii = 0; ii < FlowField.dir.Length; ii++)
            {
                int i= FlowField.dir[ii];

                int xx = x + MoveDirec.dx[i];
                int zz = z + MoveDirec.dy[i];

                //�Ѿ�������
                if (this.flowFieldGraph[xx][zz] >= 0) continue;

                var type = GraphMgr.Instance.getVal(xx, zz);
                if (!(type == (int)GraphObjType.None || type == (int)GraphObjType.Char)) continue;
                ///��ʱ������ None || Char


                ///�Խ��������ж�
                bool flag = true;
                if (i % 2 == 1)
                {
                    int diagonal1 = (i - 1) % 8;
                    int diagonal2 = (i + 1) % 8;
                    List<int> diagonals = new List<int>();
                    diagonals.Add(diagonal1);
                    diagonals.Add(diagonal2);

                    foreach (var item in diagonals)
                    {
                        int temx = x + MoveDirec.dx[item];
                        int temz = z + MoveDirec.dy[item];

                        if (graph[temx][temz] == (int)GraphObjType.Wall || graph[temx][temz] == (int)GraphObjType.Barrier)
                        {
                            flag = false;
                            break;
                        }
                    }
                }
                if (!flag) continue;

                //��¼����
                this.flowFieldGraph[xx][zz] = (i + 4) % 8;

                que.Enqueue((xx, zz, step + 1));
            }
        }

        ///������һ�㵱�����
        for (int ii = 0; ii < FlowField.dir.Length; ii++)
        {
            int i = FlowField.dir[ii];

            int xx = target.x + MoveDirec.dx[i];
            int zz = target.y + MoveDirec.dy[i];

            var type = GraphMgr.Instance.getVal(xx, zz);

            if (type == (int)GraphObjType.None || type == (int)GraphObjType.Char)
            {
                this.flowFieldGraph[xx][zz] = -1;
            }
        }

    }

    private void resetFFGraph()
    {
        this.flowFieldGraph = new int[GraphMgr.Instance.getGraph().Length][];
        for(int i = 0; i < GraphMgr.Instance.getGraph().Length; i++)
        {
            this.flowFieldGraph[i] = new int[GraphMgr.Instance.getGraph()[i].Length];
        }

        for(int i = 0; i < this.flowFieldGraph.Length; i++)
        {
            for(int j = 0; j < this.flowFieldGraph[i].Length; j++)
            {
                this.flowFieldGraph[i][j] = -1;
            }
        }
    }

    //TODO debug�õĴ�ӡ,��ɾ
    public void prin()
    {
        string str = "";
        for (int i = this.flowFieldGraph[0].Length - 1; i >= 0; i--)
        {
            for (int j = 0; j <= this.flowFieldGraph.Length - 1; j++)
            {
                str += this.flowFieldGraph[j][i];
            }
            str += "\n";
        }
        Debug.LogError(str);
    }
}
