using GameConfig;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 地图管理单例
/// </summary>
public class GraphMgr: MonoBehaviour
{
    public static GraphMgr Instance;

    /// <summary>
    /// 地图抽象到代码的障碍物图 1为障碍物 0为平地
    /// </summary>
    private int[][] graph;
    /// <summary>
    /// 地图坐标0点,同时也是障碍物容器
    /// </summary>
    public GameObject graphAnchor;
    /// <summary>
    /// 地图上要放置物体时的预瞄物体
    /// </summary>
    private GameObject fakeObj = null;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        this.buildAndScanGraph();
    }

    public int[][] getGraph()
    {
        return graph;
    }

    /// <summary>
    /// 建图并扫描地图
    /// </summary>
    public void buildAndScanGraph()
    {
        // -1 是地图x轴格子数, +2 是x轴两边的边界, 这样写以便理解
        this.graph = new int[GameConfig.RangeXYZ.maxX - 1 + 2][];
        for(int i = 0; i < this.graph.Length; i++)
        {
            this.graph[i] = new int[GameConfig.RangeXYZ.maxZ - 1 + 2];
        }

        //初始化围墙
        for(int i = 0; i < this.graph.Length; i++)
        {
            this.graph[i][0] = this.graph[i][this.graph[0].Length-1] = (int)GraphObjType.Wall;
        }
        for(int i = 0; i < this.graph[0].Length; i++)
        {
            this.graph[0][i] = this.graph[this.graph.Length-1][i] = (int)GraphObjType.Wall;
        }

        this.scanGraph();
    }

    /// <summary>
    /// 扫描并刷新地图
    /// </summary>
    public void scanGraph()
    {
        GameObject[] objs = ObjTool.instance.getChildWithL1(this.graphAnchor);

        ///遍历非墙壁置0
        for(int i = 1; i <= this.graph.Length - 2; i++)
        {
            for(int j = 1; j <= this.graph[i].Length - 2; j++)
            {
                this.graph[i][j] = (int)GraphObjType.None;
            }
        }

        //扫描动态物体
        foreach (var item in objs)
        {
            Debug.LogError(item.name);
            string[] str = item.name.Split("-");
            if (str.Length < 3) continue;

            int xx = int.Parse(str[1]);
            int zz = int.Parse(str[2]);
            string type = str[0];

            if (!Enum.TryParse(type, out GraphObjType eType)) continue;

            // 需要记录进地图的物体:  Red方块                  Green方块              Blue方块
            if(eType == GraphObjType.Red || eType == GraphObjType.Green || eType == GraphObjType.Blue)
            {
                this.graph[xx][zz] = (int)eType;
            }
        }
    }

    /// <summary>
    /// 在地图上放置预瞄物体
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="type"></param>
    public void putFakeObj(Vector3 pos, GraphObjType type)
    {
        if(type == GraphObjType.Fake)
        {
            if (!this.fakeObj)
            {
                this.fakeObj = ObjPool.instance.getObj(type.ToString(), (obj) =>
                {
                    ObjTool.instance.setNameWithChild(obj, obj.name);
                });
            }
            ObjTool.instance.setLayWithChild(this.fakeObj, 2);
            this.fakeObj.transform.parent = this.graphAnchor.transform;
            this.fakeObj.transform.localPosition = pos;
            this.fakeObj.name = type.ToString() + "-" + Mathf.FloorToInt(pos.x) + "-" + Mathf.FloorToInt(pos.z);
        }
    }

    /// <summary>
    /// 清除所有预瞄物体
    /// </summary>
    public void clearAllFakeObj()
    {
        Debug.Log("清除所有预瞄物体");
        if (this.fakeObj)
        {
            ObjTool.instance.setLayWithChild(this.fakeObj, 0);
            ObjPool.instance.backObj(this.fakeObj, (obj) =>
            {
                ObjTool.instance.setNameWithChild(obj, obj.name);
            });
            this.fakeObj = null;
        }
    }

    /// <summary>
    /// 在地图上放置Red、Green、Blue方块
    /// </summary>
    /// <param name="pos">地图的格子坐标</param>
    /// <param name="type"></param>
    public bool putCube(Vector3 pos, GraphObjType type)
    {
        if (type != GraphObjType.Red && type != GraphObjType.Green || type != GraphObjType.Blue) return false;

        GameObject obj = ObjPool.instance.getObj(type.ToString(), (obj) =>
        {
            ObjTool.instance.setNameWithChild(obj, obj.name);
        });

        if (obj)
        {
            obj.transform.parent = this.graphAnchor.transform;
            ObjTool.instance.setLayWithChild(obj, 0);
            obj.transform.localPosition = pos;
            string Newname = obj.name + "-" + Mathf.FloorToInt(pos.x) + "-" + Mathf.FloorToInt(pos.z);
            ObjTool.instance.setNameWithChild(obj, Newname);

            this.setVal(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.z), (int)type);

            return true;
        }
        else
        {
            Debug.LogError("生成物体错误");
            return false;
        }
    }

    /// <summary>
    /// 移除已经放置的方块
    /// </summary>
    /// <param name="type">要清除的类型,全部清除</param>
    public void removeCube(GraphObjType type)
    {
        if (type != GraphObjType.Red && type != GraphObjType.Green || type != GraphObjType.Blue) return;

        GameObject[] gos = new GameObject[this.graphAnchor.transform.childCount];

        for(int i = 0; i < this.graphAnchor.transform.childCount; i++)
        {
            GameObject child = this.graphAnchor.transform.GetChild(i).gameObject;
            gos[i] = child;
        }

        for(int i=0;i<gos.Length;i++)
        {
            GameObject child = gos[i];
            if(!child) continue;

            if(child.name.IndexOf(type.ToString()) != -1){
                string[] tem = child.name.Split("-");
                if (tem.Length != 3) continue;

                int xx = int.Parse(tem[1]);
                int zz = int.Parse(tem[2]);
                this.setVal(xx, zz, (int)GraphObjType.None);

                ObjPool.instance.backObj(child, (obj) => { 
                    ObjTool.instance.setNameWithChild(obj, obj.name); 
                });
            }
        }
    }

    /// <summary>
    /// 将地图的本地坐标转化为世界坐标
    /// </summary>
    /// <param name="localPos"></param>
    /// <returns></returns>
    public Vector3 localPos2WorldPos(Vector3 localPos)
    {
        Vector3 rtnPos = graphAnchor.transform.TransformPoint(localPos);
        return rtnPos;
    }

    /// <summary>
    /// 将世界坐标转化为地图上的本地坐标(即this.graphAnchor的本地坐标)
    /// </summary>
    /// <param name="pos">世界坐标</param>
    /// <returns>
    /// flag:是否在地图范围内
    /// pos:是的话其在地图上的本地坐标
    /// </returns>
    public (bool flag, Vector3 pos) worldPos2AnchorLocalPos(Vector3 pos)
    {
        Vector3 rtnPos = graphAnchor.transform.InverseTransformPoint(pos);
        bool rtnFlag = this.checkPosInGraph(rtnPos);

        if (rtnFlag)
        {
            rtnPos.y = 0;
        }

        return (flag: rtnFlag, pos: rtnPos);
    }

    /// <summary>
    /// 世界坐标转地图的格子坐标(格子内取格子中心坐标)
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public (bool flag, Vector3 pos) worldPos2AnchorLocalCenterPos(Vector3 pos)
    {
        var rtn = this.worldPos2AnchorLocalPos(pos);
        rtn.pos = new Vector3(Mathf.Floor(rtn.pos.x) + 0.5f, rtn.pos.y, Mathf.Floor(rtn.pos.z) + 0.5f);

        return rtn; 
    }

    /// <summary>
    /// 世界坐标转地图格子的下标
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public (bool flag, Vector3 pos) worldPos2GraphIdx(Vector3 pos)
    {
        var rtn = this.worldPos2AnchorLocalPos(pos);
        rtn.pos = new Vector3(Mathf.FloorToInt(rtn.pos.x), rtn.pos.y, Mathf.FloorToInt(rtn.pos.z));

        return rtn;
    }

    /// <summary>
    /// 检查坐标是否在地图内
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public bool checkPosInGraph(Vector3 pos)
    {
        if (pos.x <= GameConfig.RangeXYZ.minX || pos.x >= GameConfig.RangeXYZ.maxX
            || pos.z <= GameConfig.RangeXYZ.minZ || pos.z >= GameConfig.RangeXYZ.maxZ
            || pos.y <= -0.15 || pos.y >= 0.15)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// 获取地图上该点的权值(1为障碍物,0为平地)
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    public int getVal(int x, int z)
    {
        return this.checkRange(x, z) ? this.graph[x][z] : -1;
    }

    /// <summary>
    /// 设置地图上某点的值
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <param name="val"></param>
    public void setVal(int x, int z, int val)
    {
        if(this.checkRange(x, z))
        {
            this.graph[x][z] = val;
        }
    }



    /// <summary>
    /// 判断该点是否在地图范围内
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    private bool checkRange(int x, int z)
    {
        if (x < 0 || x > this.graph.Length-1 || z < 0 || z > this.graph[0].Length-1) return false;
        return true;
    }

    //TODO debug用的打印,可删
    public void prin()
    {
        string str = "";
        for(int i = this.graph[0].Length - 1; i >= 0; i--)
        {
            for(int j = 0; j <= this.graph.Length - 1; j++)
            {
                str += this.graph[j][i];
            }
            str += "\n";
        }
        Debug.LogError(str);
    }
}
