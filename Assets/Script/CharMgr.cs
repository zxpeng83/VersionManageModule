using GameConfig;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class CharMgr : MonoBehaviour
{
    public static List<CharMgr> charList = new List<CharMgr>();

    private CharacterController myController;
    private Animator myAnimator;
    /// <summary>
    /// AStar自动寻路的路径
    /// </summary>
    private List<Vector2Int> path = new List<Vector2Int>();

    /// <summary>
    /// 是否是主控角色(只有一个主控角色能被操控)
    /// </summary>
    public bool isMaster = false;

    /// <summary>
    /// FlowField寻路过程中的下一个点
    /// </summary>
    private Vector3 nexFlowFieldIdxV3 = Vector3.zero;

    private void Awake()
    {
        if(CharMgr.charList == null || CharMgr.charList.Count == 0)
        {
            CharMgr.charList.Add(this);
            CharMgr.charList[0].isMaster = true;
        }

        myAnimator = GetComponent<Animator>();
        myController = GetComponent<CharacterController>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        //this.freshPreData();
        //this.myReset();
    }

    public Vector3 getnexFlowFieldIdxV3()
    {
        return this.nexFlowFieldIdxV3;
    }

    /// <summary>
    /// 获取角色在地图上的本地坐标(相当于graphAhchor的本地坐标)
    /// </summary>
    /// <returns></returns>
    public (bool flag, Vector3 pos) getAnchorLocalPos()
    {
        return GraphMgr.Instance.worldPos2AnchorLocalPos(gameObject.transform.position);
    }

    /// <summary>
    /// 获取角色在地图上的格子坐标(格子内取格子中心坐标)
    /// </summary>
    /// <returns></returns>
    public (bool flag, Vector3 pos) getAnchorLocalCenterPos()
    {
        return GraphMgr.Instance.worldPos2AnchorLocalCenterPos(gameObject.transform.position);
    }

    /// <summary>
    /// 获取角色在地图格子的下标
    /// </summary>
    /// <returns></returns>
    public (bool flag, Vector3 pos) getGraphIdx()
    {
        return GraphMgr.Instance.worldPos2GraphIdx(gameObject.transform.position);
    }

    /// <summary>
    /// 刷新寻路路径
    /// </summary>
    public void freshAStarPath()
    {
        this.path = AStar.instance.startNavigation();
    }

    /// <summary>
    /// 针对AStar的角色重置，重置之前记得先清楚地图上的障碍物和目标
    /// </summary>
    /// <param name="graphIdx">地图格子的下标</param>
    public void reset2AStar(Vector2 graphIdx = default(Vector2))
    {
        if(graphIdx != default(Vector2))
        {
            transform.localPosition = new Vector3(graphIdx.x+0.5f, 0, graphIdx.y + 0.5f);
        }

        this.gameObject.name = "Char" + "-" + this.getGraphIdx().pos.x + "-" + this.getGraphIdx().pos.z;

        GraphMgr.Instance.refreshChar();

        this.freshAStarPath();
    }

    /// <summary>
    /// 针对FlowField的重置
    /// </summary>
    public void reset2FlowField(Vector2 graphIdx = default(Vector2))
    {
        if (graphIdx != default(Vector2))
        {
            transform.localPosition = new Vector3(graphIdx.x + 0.5f, 0, graphIdx.y + 0.5f);
        }

        this.gameObject.name = "Char" + "-" + this.getGraphIdx().pos.x + "-" + this.getGraphIdx().pos.z;

        GraphMgr.Instance.refreshChar();

        if (this.path != null)
        {
            this.path.Clear();
        }

        this.nexFlowFieldIdxV3 = Vector3.zero;
    }

    /// <summary>
    /// 针对RayCast的重置
    /// </summary>
    /// <param name="graphIdx"></param>
    public void reset2RayCast(Vector2 graphIdx = default(Vector2))
    {
        if (graphIdx != default(Vector2))
        {
            transform.localPosition = new Vector3(graphIdx.x + 0.5f, 0, graphIdx.y + 0.5f);
        }

        this.gameObject.name = "Char" + "-" + this.getGraphIdx().pos.x + "-" + this.getGraphIdx().pos.z;

        GraphMgr.Instance.refreshChar();
    }

    void Update()
    {
        GameControllMgr.instance.charUpdate(this);
    }

    /// <summary>
    /// 通过键盘输入手动操控玩家移动(RayCast模式)
    /// </summary>
    public void moveByKeyboard()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 dir = new Vector3(h, 0, v).normalized;

        if (Mathf.Abs(dir.x) < 1e-3 && Mathf.Abs(dir.z) < 1e-3)
        {
            myAnimator.SetBool("isRun", false);
            return;
        }
        else
        {
            myAnimator.SetBool("isRun", true);
        }

        transform.rotation = Quaternion.LookRotation(dir);
        myController.SimpleMove(dir * 2);

        GraphMgr.Instance.refreshChar();

        RayCast.instance.addIdxV3(this.getGraphIdx().pos);

        this.gameObject.name = "Char" + "-" + this.getGraphIdx().pos.x + "-" + this.getGraphIdx().pos.z;
    }

    /// <summary>
    /// A*自动寻路
    /// </summary>
    public void moveByAStar()
    {
        while(this.path != null && this.path.Count > 0) 
        {
            Vector2 nexPointV2 = this.path.Last();
            nexPointV2.x += 0.5f;
            nexPointV2.y += 0.5f;
            var curPointV2 = new Vector2(this.getAnchorLocalPos().pos.x, this.getAnchorLocalPos().pos.z);
            if (MathTool.getEuclideanDisV2(nexPointV2, curPointV2) < 0.1)
            {
                this.path.RemoveAt(path.Count - 1);
                continue;
            }

            Vector2 dirV2 = (nexPointV2 - curPointV2).normalized;
            Vector3 dirV3 = new Vector3(dirV2.x, 0, dirV2.y);
            myAnimator.SetBool("isRun", true);
            transform.rotation = Quaternion.LookRotation(dirV3);
            myController.SimpleMove(dirV3 * 2);

            //this.freshPreData();
            GraphMgr.Instance.refreshChar();

            this.gameObject.name = "Char" + "-" + this.getGraphIdx().pos.x + "-" + this.getGraphIdx().pos.z;

            break;
        }

        if(this.path == null || this.path.Count <= 0)
        {
            this.rotation2Target();
            myAnimator.SetBool("isRun", false);
        }
    }

    /// <summary>
    /// FlowField自动寻路
    /// </summary>
    public void moveByFlowField()
    {
        if (!this.getGraphIdx().flag)
        {
            //Debug.LogError("flag为false，return");
            myAnimator.SetBool("isRun", false);
            return;
        }

        int[][] flowFieldGraph = FlowField.instance.getGraph();
        int[][] graph = GraphMgr.Instance.getGraph();

        if(flowFieldGraph == null || graph == null)
        {
            //Debug.LogError("地图为空，return");
            myAnimator.SetBool("isRun", false);
            return;
        }

        Vector3 nexFlowFieldLocalPos = new Vector3(this.nexFlowFieldIdxV3.x + 0.5f, 0, this.nexFlowFieldIdxV3.z + 0.5f);
        ///已经到达this.nexFlowFieldIdxV3或还没初始化，则更新最新的this.nexFlowFieldIdxV3
        if(this.nexFlowFieldIdxV3 == Vector3.zero || MathTool.getEuclideanDisV3(this.getAnchorLocalPos().pos, nexFlowFieldLocalPos) < 0.01)
        {
            Vector3 curIdx = this.getGraphIdx().pos;
            int flowFieldVal = flowFieldGraph[Mathf.RoundToInt(curIdx.x)][Mathf.RoundToInt(curIdx.z)];
            ///寻路正常结束，到达目标外面一层
            if (flowFieldVal < 0)
            {
                //Debug.LogError("flowfield地图当前位置非法，return");
                myAnimator.SetBool("isRun", false);
                this.rotation2Target();
                return;
            }
            int nexIdxX = Mathf.RoundToInt(curIdx.x) + MoveDirec.dx[flowFieldVal];
            int nexIdxZ = Mathf.RoundToInt(curIdx.z) + MoveDirec.dy[flowFieldVal];

            Vector3 nexIdxV3 = new Vector3(nexIdxX, 0, nexIdxZ);
            if (!this.checkNexFlowFieldIdx(nexIdxV3))
            {
                //Debug.LogError("下一个位置为其他角色的nexFlowFieldIdx点，return");
                myAnimator.SetBool("isRun", false);
                return;
            }
            if (graph[nexIdxX][nexIdxZ] == (int)GraphObjType.Char)
            {
                //Debug.LogError("下一个位置有其他角色,return");
                myAnimator.SetBool("isRun", false);
                return;
            }

            this.nexFlowFieldIdxV3 = nexIdxV3;
        }

        nexFlowFieldLocalPos = new Vector3(this.nexFlowFieldIdxV3.x + 0.5f, 0, this.nexFlowFieldIdxV3.z + 0.5f);

        Vector3 nexDir = (nexFlowFieldLocalPos - this.getAnchorLocalPos().pos).normalized;
        myAnimator.SetBool("isRun", true);
        transform.rotation = Quaternion.LookRotation(nexDir);
        //myController.SimpleMove(nexDir * 2);
        transform.Translate(Vector3.forward * 2 * Time.deltaTime, Space.Self);

        this.gameObject.name = "Char" + "-" + this.getGraphIdx().pos.x + "-" + this.getGraphIdx().pos.z;

        GraphMgr.Instance.refreshChar();

    }

    /// <summary>
    /// 判断下一个FlowField寻路点是否已经是其他角色的下一个寻路点
    /// </summary>
    /// <param name="idxV3"></param>
    /// <returns></returns>
    private bool checkNexFlowFieldIdx(Vector3 idxV3)
    {
        bool flag = true;
        CharMgr.charList.ForEach((_charMgr) =>
        {
            if (MathTool.getEuclideanDisV3(_charMgr.getnexFlowFieldIdxV3(), idxV3) < 0.01)
            {
                flag = false;
            }
        });

        return flag;
    }

    /// <summary>
    /// 使角色转向目标
    /// </summary>
    private void rotation2Target()
    {
        Vector3? target = GraphMgr.Instance.getCurTarget<Vector3>();
        if (target == null) return;

        Vector3 dir = (Vector3)target - this.getAnchorLocalPos().pos;
        transform.rotation = Quaternion.LookRotation(dir);
    }
}
