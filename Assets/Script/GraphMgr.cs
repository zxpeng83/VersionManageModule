using GameConfig;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��ͼ������
/// </summary>
public class GraphMgr: MonoBehaviour
{
    public static GraphMgr Instance;

    /// <summary>
    /// ��ͼ���󵽴�����ϰ���ͼ 1Ϊ�ϰ��� 0Ϊƽ��
    /// </summary>
    private int[][] graph;
    /// <summary>
    /// ��ͼ����0��,ͬʱҲ���ϰ�������
    /// </summary>
    public GameObject graphAnchor;
    /// <summary>
    /// ��ͼ��Ҫ��������ʱ��Ԥ������
    /// </summary>
    private GameObject fakeObj = null;
    /// <summary>
    /// ��ɫ�ڵ�ͼ�ϵ�����
    /// </summary>
    private List<Vector2> charGraphIdx = new List<Vector2>();

    /// <summary>
    /// ��ǰĿ���
    /// </summary>
    private Vector2Int curTarget = Vector2Int.zero;

    /// <summary>
    /// ����
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
    /// ��ͼ��ɨ���ͼ
    /// </summary>
    public void buildAndScanGraph()
    {
        // -1 �ǵ�ͼx�������, +2 ��x�����ߵı߽�, ����д�Ա����
        this.graph = new int[GameConfig.RangeXYZ.maxX - 1 + 2][];
        for(int i = 0; i < this.graph.Length; i++)
        {
            this.graph[i] = new int[GameConfig.RangeXYZ.maxZ - 1 + 2];
        }

        //��ʼ��Χǽ
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
    /// ɨ�貢ˢ�µ�ͼ
    /// </summary>
    public void scanGraph()
    {
        GameObject[] objs = ObjTool.instance.getChildWithL1(this.graphAnchor);

        ///������ǽ����0
        for(int i = 1; i <= this.graph.Length - 2; i++)
        {
            for(int j = 1; j <= this.graph[i].Length - 2; j++)
            {
                this.graph[i][j] = (int)GraphObjType.None;
            }
        }
        this.charGraphIdx.Clear();

        //ɨ�趯̬����
        foreach (var item in objs)
        {
            Debug.LogError(item.name);
            string[] str = item.name.Split("-");
            if (str.Length < 3) continue;

            int xx = int.Parse(str[1]);
            int zz = int.Parse(str[2]);
            string type = str[0];

            if (!Enum.TryParse(type, out GraphObjType eType)) continue;

            // ��Ҫ��¼����ͼ������:  Ŀ��                          �ϰ���                  ��ɫ      (Ԥ�����岻��¼)
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
    /// ��ȡ��ǰĿ��ڵ�
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
    /// �ڵ�ͼ�Ϸ���Ԥ������
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
    /// �������Ԥ������
    /// </summary>
    public void clearAllFakeObj()
    {
        Debug.Log("�������Ԥ������");
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
    /// �ڵ�ͼ�Ϸ���Ŀ����ϰ�������
    /// </summary>
    /// <param name="pos">��ͼ�ĸ�������</param>
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
            Debug.LogError("�����������");
        }
    }

    /// <summary>
    /// �Ƴ��Ѿ����õ�Ŀ����ϰ�������
    /// </summary>
    /// <param name="type">Ҫ���������,ȫ�����</param>
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
    /// �ڽ�ɫ�������ó���
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
    /// �Ƴ�����
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
    /// ����CharMgrˢ�½�ɫ�ڶ�ά�����ϵı��
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
    /// ����ͼ�ı�������ת��Ϊ��������
    /// </summary>
    /// <param name="localPos"></param>
    /// <returns></returns>
    public Vector3 localPos2WorldPos(Vector3 localPos)
    {
        Vector3 rtnPos = graphAnchor.transform.TransformPoint(localPos);
        return rtnPos;
    }

    /// <summary>
    /// ����������ת��Ϊ��ͼ�ϵı�������(��this.graphAnchor�ı�������)
    /// </summary>
    /// <param name="pos">��������</param>
    /// <returns>
    /// flag:�Ƿ��ڵ�ͼ��Χ��
    /// pos:�ǵĻ����ڵ�ͼ�ϵı�������
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
    /// ��������ת��ͼ�ĸ�������(������ȡ������������)
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
    /// ��������ת��ͼ���ӵ��±�
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
    /// ��������Ƿ��ڵ�ͼ��
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
    /// ��ȡ��ͼ�ϸõ��Ȩֵ(1Ϊ�ϰ���,0Ϊƽ��)
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    public int getVal(int x, int z)
    {
        return this.checkRange(x, z) ? this.graph[x][z] : -1;
    }

    /// <summary>
    /// ���õ�ͼ��ĳ���ֵ
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
    /// �жϸõ��Ƿ��ڵ�ͼ��Χ��
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    private bool checkRange(int x, int z)
    {
        if (x < 0 || x > this.graph.Length-1 || z < 0 || z > this.graph[0].Length-1) return false;
        return true;
    }

    //TODO debug�õĴ�ӡ,��ɾ
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
