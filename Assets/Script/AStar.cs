using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameConfig;
using UnityEngine;
using UnityEngine.UIElements;

public class AStar
{
    private static AStar ins = null;
    public static AStar instance
    {
        get
        {
            if(ins == null)
            {
                ins = new AStar();
            }

            return ins;
        }
    }

    public List<Vector2Int> startNavigation()
    {
        if (CharMgr.charList.Count <= 0) return null;

        var charac = CharMgr.charList[0].getGraphIdx();
        if (!charac.flag) return null;

        ///��ͼ
        int[][] graph = GraphMgr.Instance.getGraph();
        ///ʹ���ֵ���ģ���(key:��ǰ�ƶ�����+��ֵ����  val:��ǰλ��)
        var heap = new SortedDictionary<float, List<Vector2Int>>();
        ///�ƶ���Vector2�ľ���Ϊfloat
        Dictionary<Vector2Int, float> dist = new Dictionary<Vector2Int, float>();
        ///·����¼ key��ǰһ��Ϊval
        Dictionary<Vector2Int, Vector2Int> path = new Dictionary<Vector2Int, Vector2Int>();

        ///���
        Vector2Int start = Vector2Int.zero;
        ///�յ�
        Vector2Int target = Vector2Int.zero;

        for (int i = 0; i < graph.Length; i++)
        {
            for(int j = 0; j < graph[i].Length; j++)
            {
                //Debug.LogError(graph[i][j] + " " + (int)GraphObjType.Target);
                if (graph[i][j] == (int)GraphObjType.Char)
                {
                    start = new Vector2Int(i, j);
                }
                if (graph[i][j] == (int)GraphObjType.Target)
                {
                    target = new Vector2Int(i, j);
                }
            }
        }

        if(start == Vector2Int.zero ||  target == Vector2Int.zero)
        {
            //GraphMgr.Instance.prin();
            Debug.LogError("AStarѰ·ʧ��,��ͼ���Ҳ��������յ�");
            return null;
        }

        ///�������
        List<Vector2Int> l = new List<Vector2Int>();
        l.Add(start);
        heap.Add(0, l);
        dist[start] = 0;

        ///��ʼAStarѰ·
        while(heap.Count > 0)
        {
            ///ȡ���Ѷ�Ԫ��
            var topAll = heap.First();
            var topList = topAll.Value;
            var topListVal = topList.First();
            topList.RemoveAt(0);

            if(topList.Count <= 0)
            {
                heap.Remove(topAll.Key);
            }

            Vector2Int preIdx = topListVal;
            float preDis = dist[preIdx];

            //�����յ�
            if(preIdx == target)
            {
                break;
            }

            for(int i = 0; i < MoveDirec.dx.Length; i++)
            {
                int xx = preIdx.x + MoveDirec.dx[i];
                int yy = preIdx.y + MoveDirec.dy[i];

                if (graph[xx][yy] == (int)GraphObjType.Wall || graph[xx][yy] == (int)GraphObjType.Barrier)
                {
                    continue;
                }

                bool flag = true;
                ///�Խ��������ж�
                if(i%2 == 1)
                {
                    int diagonal1 = (i - 1) % 8;
                    int diagonal2 = (i + 1) % 8;
                    List<int> diagonals = new List<int>();
                    diagonals.Add(diagonal1);
                    diagonals.Add(diagonal2);

                    foreach (var item in diagonals)
                    {
                        int temx = preIdx.x + MoveDirec.dx[item];
                        int temy = preIdx.y + MoveDirec.dy[item];

                        if (graph[temx][temy] == (int)GraphObjType.Wall || graph[temx][temy] == (int)GraphObjType.Barrier)
                        {
                            flag = false;
                            break;
                        }
                    }
                }
                if (!flag) continue;

                Vector2Int nexIdx = new Vector2Int(xx, yy);
                float nexDis = float.MaxValue;

                if(!dist.TryGetValue(nexIdx, out nexDis) || dist[nexIdx] > preDis + MathTool.getEuclideanDisV2(preIdx, nexIdx))
                {
                    nexDis = preDis + MathTool.getEuclideanDisV2(preIdx, nexIdx);
                    dist[nexIdx] = nexDis;
                    path[nexIdx] = preIdx;
                    if(!heap.TryGetValue(dist[nexIdx] + MathTool.getEuclideanDisV2(nexIdx, target), out var list))
                    {
                        list = new List<Vector2Int>();
                        list.Add(nexIdx);
                        heap.Add(dist[nexIdx] + MathTool.getEuclideanDisV2(nexIdx, target), list);
                    }
                    else
                    {
                        list.Add(nexIdx);
                    }
                }
            }
        }

        List<Vector2Int> ans = new List<Vector2Int>();
        Vector2Int curPoint = target;
        while(path.TryGetValue(curPoint, out Vector2Int prePoint))
        {
            ans.Add(prePoint);
            curPoint = prePoint;
        }

        if(ans.Count > 0 && ans[0] == target)
        {
            ans.RemoveAt(0);
        }

        return ans;
    }

    
}
