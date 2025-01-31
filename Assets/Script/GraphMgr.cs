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
    /// <summary>
    /// 角色在地图上的坐标
    /// </summary>
    private List<Vector2> charGraphIdx = new List<Vector2>();

    /// <summary>
    /// 当前目标点
    /// </summary>
    private Vector2Int curTarget = Vector2Int.zero;

    /// <summary>
    /// 宠物
    /// </summary>
    private PetMgr pet = null;

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
        this.charGraphIdx.Clear();

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

            // 需要记录进地图的物体:  目标                          障碍物                  角色      (预瞄物体不记录)
            if(eType == GraphObjType.Target || eType == GraphObjType.Barrier || eType == GraphObjType.Char)
            {
                this.graph[xx][zz] = (int)eType;
                if(eType == GraphObjType.Char)
                {
                    this.charGraphIdx.Add(new Vector2(xx, zz));
                }

                if(eType == GraphObjType.Target)
                {
                    this.curTarget = new Vector2Int(xx, zz);
                }
            }
        }
    }

    /// <summary>
    /// 获取当前目标节点
    /// </summary>
    /// <returns></returns>
    public T? getCurTarget<T>() where T : struct
    {
        if(this.curTarget == Vector2Int.zero) return null;

        if (typeof(T) == typeof(Vector2))
        {
            return (T)(object)this.curTarget;
        }
        else if (typeof(T) == typeof(Vector3))
        {
            Vector3 rtn = new Vector3(this.curTarget.x, 0, this.curTarget.y);
            return (T)(object)rtn;
        }
        else if (typeof(T) == typeof(Vector2Int))
        {
            return (T)(object)this.curTarget;
        }
        else if (typeof(T) == typeof(Vector3Int))
        {
            Vector3Int rtn = new Vector3Int(this.curTarget.x, 0, this.curTarget.y);
            return (T)(object)rtn;
        }

        return null;
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
    /// 在地图上放置目标或障碍物物体
    /// </summary>
    /// <param name="pos">地图的格子坐标</param>
    /// <param name="type"></param>
    public void putTarOrBarObj(Vector3 pos, GraphObjType type)
    {
        if (type != GraphObjType.Target && type != GraphObjType.Barrier) return;

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

            if(type == GraphObjType.Target)
            {
                this.curTarget = new Vector2Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.z));
            }
        }
        else
        {
            Debug.LogError("生成物体错误");
        }
    }

    /// <summary>
    /// 移除已经放置的目标或障碍物物体
    /// </summary>
    /// <param name="type">要清除的类型,全部清除</param>
    public void removeTarOrBarObj(GraphObjType type)
    {
        if (type != GraphObjType.Target && type != GraphObjType.Barrier) return;

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

        if(type == GraphObjType.Target)
        {
            this.curTarget = Vector2Int.zero;
        }
    }

    /// <summary>
    /// 在角色附近放置宠物
    /// </summary>
    public void putPet()
    {
        if (this.pet != null) return;
        ObjPool.instance.getObj(GraphObjType.Pet.ToString(), (go) =>
        {
            var _petMgr = go.GetComponent<PetMgr>();
            this.pet = _petMgr;
            go.transform.parent = this.graphAnchor.transform;
            ObjTool.instance.setLayWithChild(go, 2);
            ObjTool.instance.setNameWithChild(go, GraphObjType.Pet.ToString());
        });

        for(int i = 0; i < MoveDirec.dx.Length; i++)
        {
            int xx = Mathf.RoundToInt(CharMgr.charList[0].getGraphIdx().pos.x) + MoveDirec.dx[i];
            int zz = Mathf.RoundToInt(CharMgr.charList[0].getGraphIdx().pos.z) + MoveDirec.dy[i];

            if(this.getVal(xx, zz) == (int)GraphObjType.None)
            {
                this.pet.gameObject.transform.localPosition = new Vector3(xx + 0.5f, 0, zz + 0.5f);
                break;
            }
        }

        this.pet.starFollow();
    }

    /// <summary>
    /// 移除宠物
    /// </summary>
    public void removePet()
    {
        if (this.pet == null) return;

        this.pet.stopFollow();

        ObjPool.instance.backObj(this.pet.gameObject, (go) =>
        {
            ObjTool.instance.setNameWithChild(go, GraphObjType.Pet.ToString());
        });

        this.pet = null;
    }

    /// <summary>
    /// 根据CharMgr刷新角色在二维数组上的标记
    /// </summary>
    public void refreshChar()
    {
        List<CharMgr> chars = CharMgr.charList;
        List<Vector2> curCharGraphIdx = new List<Vector2>();
        chars.ForEach((val) =>
        {
            var tem = val.getGraphIdx();
            if (!tem.flag) return;

            Vector2 graphIdx = new Vector2(tem.pos.x, tem.pos.z);
            curCharGraphIdx.Add(graphIdx);
        });

        this.charGraphIdx.ForEach((val) =>
        {
            this.setVal(Mathf.RoundToInt(val.x), Mathf.RoundToInt(val.y), (int)GraphObjType.None);
        });

        curCharGraphIdx.ForEach((val) =>
        {
            this.setVal(Mathf.RoundToInt(val.x), Mathf.RoundToInt(val.y), (int)GraphObjType.Char);
        });

        this.charGraphIdx = curCharGraphIdx;
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
