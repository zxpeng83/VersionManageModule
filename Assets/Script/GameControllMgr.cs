using GameConfig;
using UnityEngine;
using UnityEngine.UIElements;

public class GameControllMgr : MonoBehaviour
{
    public static GameControllMgr instance;

    private FiniteStateMachine fsm = new FiniteStateMachine();

    private void Awake()
    {
        instance = this;

        this.fsm.AddNode(new VersionManageNode());
    }

    // Start is called before the first frame update
    void Start()
    {
        this.fsm.Run(nameof(VersionManageNode));
    }

    public void changeMode(string newMode)
    {
        this.fsm.Transition(newMode);
    }

    public void mouseInput(RaycastHit hitInfo, bool isHitSomething, bool leftMouseDown, bool rightMouseDown, bool middleScrollDown)
    {
        switch (this.fsm.CurrentNodeName)
        {
            case nameof(VersionManageNode):
                this.mouseInputVersionManage(hitInfo,  isHitSomething, leftMouseDown, rightMouseDown, middleScrollDown);
                break;
            default:
                break;
        }
    }

    private void mouseInputVersionManage(RaycastHit hitInfo, bool isHitSomething, bool leftMouseDown, bool rightMouseDown, bool middleScrollDown)
    {
        if (isHitSomething)
        {
            Debug.DrawLine(Camera.main.transform.position, hitInfo.point);
        }

        if (isHitSomething) //打到物体
        {
            string hitObjName = hitInfo.collider.gameObject.name;
            //是否打到之前放置的物体
            bool isHitObj = hitObjName.IndexOf(GraphObjType.Blue.ToString()) != -1
                || hitObjName.IndexOf(GraphObjType.Green.ToString()) != -1
                || hitObjName.IndexOf(GraphObjType.Red.ToString()) != -1;

            var anchorLocalCenterPos = GraphMgr.Instance.worldPos2AnchorLocalCenterPos(hitInfo.point);
            var graphIdx = GraphMgr.Instance.worldPos2GraphIdx(hitInfo.point);

            if (isHitObj || !anchorLocalCenterPos.flag) //打到之前放置的障碍物 或 打在角色所在格子里 或 打在地图之外
            {
                GraphMgr.Instance.clearAllFakeObj();
                return;
            }
            else  //打的位置合法,可跟随或放置
            {
                if (leftMouseDown) //放置Red方块
                {
                    this.putCube(anchorLocalCenterPos.pos, graphIdx.pos, GraphObjType.Red);
                }
                else if (middleScrollDown) //放置Green方块
                {
                    this.putCube(anchorLocalCenterPos.pos, graphIdx.pos, GraphObjType.Green);
                }
                else if (rightMouseDown) //放置Blue方块
                {
                    this.putCube(anchorLocalCenterPos.pos, graphIdx.pos, GraphObjType.Blue);
                }
                else //放置预瞄物体跟随
                {
                    GraphMgr.Instance.putFakeObj(anchorLocalCenterPos.pos, GraphObjType.Fake);
                }
            }
        }
        else //没打到物体
        {
            GraphMgr.Instance.clearAllFakeObj();
        }
    }

    /// <summary>
    /// 放置方块，地图以及线段树的更新
    /// </summary>
    /// <param name="anchorLocalCenterPos"></param>
    /// <param name="graphIdx"></param>
    /// <param name="type"></param>
    public void putCube(Vector3 anchorLocalCenterPos, Vector3 graphIdx, GraphObjType type)
    {
        bool success = GraphMgr.Instance.putCube(anchorLocalCenterPos, type);
        if (success)
        {
            PersistentSegmentTree2D.instance.putCube(Mathf.RoundToInt(graphIdx.x), Mathf.RoundToInt(graphIdx.z), type);
            HudUI.instance.freshUI();
        }
    }

    /// <summary>
    /// 转跳到某版本
    /// </summary>
    /// <param name="version"></param>
    public void jump2Version(int version)
    {
        PersistentSegmentTree2D.instance.jump2Version(version);
        HudUI.instance.freshUI();
    }

    /// <summary>
    /// 重置
    /// </summary>
    public void reset()
    {
        PersistentSegmentTree2D.instance.reset();
        HudUI.instance.freshUI();
    }

    /// <summary>
    /// 刷新界面
    /// </summary>
    public void freshUI()
    {
        HudUI.instance.freshUI();
    }

    void Update()
    {
        
    }
}
