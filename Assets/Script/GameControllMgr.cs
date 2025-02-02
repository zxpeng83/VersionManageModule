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

        if (isHitSomething) //������
        {
            string hitObjName = hitInfo.collider.gameObject.name;
            //�Ƿ��֮ǰ���õ�����
            bool isHitObj = hitObjName.IndexOf(GraphObjType.Blue.ToString()) != -1
                || hitObjName.IndexOf(GraphObjType.Green.ToString()) != -1
                || hitObjName.IndexOf(GraphObjType.Red.ToString()) != -1;

            var anchorLocalCenterPos = GraphMgr.Instance.worldPos2AnchorLocalCenterPos(hitInfo.point);
            var graphIdx = GraphMgr.Instance.worldPos2GraphIdx(hitInfo.point);

            if (isHitObj || !anchorLocalCenterPos.flag) //��֮ǰ���õ��ϰ��� �� ���ڽ�ɫ���ڸ����� �� ���ڵ�ͼ֮��
            {
                GraphMgr.Instance.clearAllFakeObj();
                return;
            }
            else  //���λ�úϷ�,�ɸ�������
            {
                if (leftMouseDown) //����Red����
                {
                    this.putCube(anchorLocalCenterPos.pos, graphIdx.pos, GraphObjType.Red);
                }
                else if (middleScrollDown) //����Green����
                {
                    this.putCube(anchorLocalCenterPos.pos, graphIdx.pos, GraphObjType.Green);
                }
                else if (rightMouseDown) //����Blue����
                {
                    this.putCube(anchorLocalCenterPos.pos, graphIdx.pos, GraphObjType.Blue);
                }
                else //����Ԥ���������
                {
                    GraphMgr.Instance.putFakeObj(anchorLocalCenterPos.pos, GraphObjType.Fake);
                }
            }
        }
        else //û������
        {
            GraphMgr.Instance.clearAllFakeObj();
        }
    }

    /// <summary>
    /// ���÷��飬��ͼ�Լ��߶����ĸ���
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
    /// ת����ĳ�汾
    /// </summary>
    /// <param name="version"></param>
    public void jump2Version(int version)
    {
        PersistentSegmentTree2D.instance.jump2Version(version);
        HudUI.instance.freshUI();
    }

    /// <summary>
    /// ����
    /// </summary>
    public void reset()
    {
        PersistentSegmentTree2D.instance.reset();
        HudUI.instance.freshUI();
    }

    /// <summary>
    /// ˢ�½���
    /// </summary>
    public void freshUI()
    {
        HudUI.instance.freshUI();
    }

    void Update()
    {
        
    }
}
