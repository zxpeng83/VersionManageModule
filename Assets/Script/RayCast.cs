using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCast
{
    private static RayCast ins = null;
    public static RayCast instance
    {
        get
        {
            if (ins == null)
            {
                ins = new RayCast();
            }

            return ins;
        }
    }

    /// <summary>
    /// 角色路径
    /// </summary>
    private LinkedList<Vector3> path = new LinkedList<Vector3>();
    private int maxCount = 10;

    /// <summary>
    /// 获取角色路径
    /// </summary>
    /// <returns></returns>
    public LinkedList<Vector3> getPath()
    {
        return path;
    }

    /// <summary>
    /// 添加角色新位置节点
    /// </summary>
    /// <param name="idxV3"></param>
    public void addIdxV3(Vector3 idxV3)
    {
        if(path == null)
        {
            path = new LinkedList<Vector3>();
        }
        if(path.Count > 0)
        {
            var first = path.First.Value;

            if(MathTool.getEuclideanDisV3(first, idxV3) < 0.05)
            {
                return;
            }
        }

        path.AddFirst(idxV3);

        while(path.Count > this.maxCount)
        {
            path.RemoveLast();
        }
    }

    public void reset()
    {
        path.Clear();
    }
}
