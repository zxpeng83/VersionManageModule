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
    /// AStar�Զ�Ѱ·��·��
    /// </summary>
    private List<Vector2Int> path = new List<Vector2Int>();

    /// <summary>
    /// �Ƿ������ؽ�ɫ(ֻ��һ�����ؽ�ɫ�ܱ��ٿ�)
    /// </summary>
    public bool isMaster = false;

    /// <summary>
    /// FlowFieldѰ·�����е���һ����
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
    /// ��ȡ��ɫ�ڵ�ͼ�ϵı�������(�൱��graphAhchor�ı�������)
    /// </summary>
    /// <returns></returns>
    public (bool flag, Vector3 pos) getAnchorLocalPos()
    {
        return GraphMgr.Instance.worldPos2AnchorLocalPos(gameObject.transform.position);
    }

    /// <summary>
    /// ��ȡ��ɫ�ڵ�ͼ�ϵĸ�������(������ȡ������������)
    /// </summary>
    /// <returns></returns>
    public (bool flag, Vector3 pos) getAnchorLocalCenterPos()
    {
        return GraphMgr.Instance.worldPos2AnchorLocalCenterPos(gameObject.transform.position);
    }

    /// <summary>
    /// ��ȡ��ɫ�ڵ�ͼ���ӵ��±�
    /// </summary>
    /// <returns></returns>
    public (bool flag, Vector3 pos) getGraphIdx()
    {
        return GraphMgr.Instance.worldPos2GraphIdx(gameObject.transform.position);
    }

    /// <summary>
    /// ˢ��Ѱ··��
    /// </summary>
    public void freshAStarPath()
    {
        this.path = AStar.instance.startNavigation();
    }

    /// <summary>
    /// ���AStar�Ľ�ɫ���ã�����֮ǰ�ǵ��������ͼ�ϵ��ϰ����Ŀ��
    /// </summary>
    /// <param name="graphIdx">��ͼ���ӵ��±�</param>
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
    /// ���FlowField������
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
    /// ���RayCast������
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
    /// ͨ�����������ֶ��ٿ�����ƶ�(RayCastģʽ)
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
    /// A*�Զ�Ѱ·
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
    /// FlowField�Զ�Ѱ·
    /// </summary>
    public void moveByFlowField()
    {
        if (!this.getGraphIdx().flag)
        {
            //Debug.LogError("flagΪfalse��return");
            myAnimator.SetBool("isRun", false);
            return;
        }

        int[][] flowFieldGraph = FlowField.instance.getGraph();
        int[][] graph = GraphMgr.Instance.getGraph();

        if(flowFieldGraph == null || graph == null)
        {
            //Debug.LogError("��ͼΪ�գ�return");
            myAnimator.SetBool("isRun", false);
            return;
        }

        Vector3 nexFlowFieldLocalPos = new Vector3(this.nexFlowFieldIdxV3.x + 0.5f, 0, this.nexFlowFieldIdxV3.z + 0.5f);
        ///�Ѿ�����this.nexFlowFieldIdxV3��û��ʼ������������µ�this.nexFlowFieldIdxV3
        if(this.nexFlowFieldIdxV3 == Vector3.zero || MathTool.getEuclideanDisV3(this.getAnchorLocalPos().pos, nexFlowFieldLocalPos) < 0.01)
        {
            Vector3 curIdx = this.getGraphIdx().pos;
            int flowFieldVal = flowFieldGraph[Mathf.RoundToInt(curIdx.x)][Mathf.RoundToInt(curIdx.z)];
            ///Ѱ·��������������Ŀ������һ��
            if (flowFieldVal < 0)
            {
                //Debug.LogError("flowfield��ͼ��ǰλ�÷Ƿ���return");
                myAnimator.SetBool("isRun", false);
                this.rotation2Target();
                return;
            }
            int nexIdxX = Mathf.RoundToInt(curIdx.x) + MoveDirec.dx[flowFieldVal];
            int nexIdxZ = Mathf.RoundToInt(curIdx.z) + MoveDirec.dy[flowFieldVal];

            Vector3 nexIdxV3 = new Vector3(nexIdxX, 0, nexIdxZ);
            if (!this.checkNexFlowFieldIdx(nexIdxV3))
            {
                //Debug.LogError("��һ��λ��Ϊ������ɫ��nexFlowFieldIdx�㣬return");
                myAnimator.SetBool("isRun", false);
                return;
            }
            if (graph[nexIdxX][nexIdxZ] == (int)GraphObjType.Char)
            {
                //Debug.LogError("��һ��λ����������ɫ,return");
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
    /// �ж���һ��FlowFieldѰ·���Ƿ��Ѿ���������ɫ����һ��Ѱ·��
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
    /// ʹ��ɫת��Ŀ��
    /// </summary>
    private void rotation2Target()
    {
        Vector3? target = GraphMgr.Instance.getCurTarget<Vector3>();
        if (target == null) return;

        Vector3 dir = (Vector3)target - this.getAnchorLocalPos().pos;
        transform.rotation = Quaternion.LookRotation(dir);
    }
}
