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
                    bool success = GraphMgr.Instance.putCube(anchorLocalCenterPos.pos, GraphObjType.Red);
                    if (success)
                    {
                        ///TODO ���¿ɳ־û�2ά�߶���
                    }
                }
                else if (middleScrollDown) //����Green����
                {
                    bool success = GraphMgr.Instance.putCube(anchorLocalCenterPos.pos, GraphObjType.Green);
                    if (success)
                    {
                        ///TODO ���¿ɳ־û�2ά�߶���
                    }
                }
                else if (middleScrollDown) //����Blue����
                {
                    bool success = GraphMgr.Instance.putCube(anchorLocalCenterPos.pos, GraphObjType.Blue);
                    if (success)
                    {
                        ///TODO ���¿ɳ־û�2ά�߶���
                    }
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

    //private void mouseInputRayCast(RaycastHit hitInfo, bool isHitSomething, bool leftMouseDown)
    //{
    //    if (isHitSomething)
    //    {
    //        Debug.DrawLine(Camera.main.transform.position, hitInfo.point);
    //    }

    //    if (isHitSomething) //������
    //    {
    //        ///�Ƿ���Ѿ����õ��ϰ���
    //        bool isHitBarrier = hitInfo.collider.gameObject.name.IndexOf("Barrier") != -1;
    //        ///�Ƿ���Ѿ����õ�Ŀ��
    //        bool isHitTarget = hitInfo.collider.gameObject.name.IndexOf("Target") != -1;

    //        var anchorLocalCenterPos = GraphMgr.Instance.worldPos2AnchorLocalCenterPos(hitInfo.point);
    //        var graphIdx = GraphMgr.Instance.worldPos2GraphIdx(hitInfo.point);
    //        var charGraphIdx = CharMgr.charList[0].getGraphIdx();
    //        ///�Ƿ�򵽽�ɫ
    //        bool isHitChar = (graphIdx.flag && MathTool.getEuclideanDisV3(graphIdx.pos, charGraphIdx.pos) < 0.01);

    //        if (isHitBarrier || isHitChar || !anchorLocalCenterPos.flag) //��֮ǰ���õ��ϰ��� �� ���ڽ�ɫ���ڸ����� �� ���ڵ�ͼ֮��
    //        {
    //            GraphMgr.Instance.clearAllFakeObj();
    //            return;
    //        }
    //        else  //���λ�úϷ�,�ɸ�������
    //        {
    //            if (leftMouseDown) //�����ϰ���
    //            {
    //                GraphMgr.Instance.putTarOrBarObj(anchorLocalCenterPos.pos, GraphObjType.Barrier);
    //            }
    //            else //����Ԥ���������
    //            {
    //                GraphMgr.Instance.putFakeObj(anchorLocalCenterPos.pos, GraphObjType.Fake);
    //            }
    //        }
    //    }
    //    else //û������
    //    {
    //        GraphMgr.Instance.clearAllFakeObj();
    //    }
    //}

    //private void mouseInputFlowField(RaycastHit hitInfo, bool isHitSomething, bool leftMouseDown, bool rightMouseDown, bool middleScrollDown)
    //{
    //    if (isHitSomething)
    //    {
    //        Debug.DrawLine(Camera.main.transform.position, hitInfo.point);
    //    }

    //    if (isHitSomething) //������
    //    {
    //        ///�Ƿ���Ѿ����õ��ϰ���
    //        bool isHitBarrier = hitInfo.collider.gameObject.name.IndexOf("Barrier") != -1;
    //        ///�Ƿ���Ѿ����õ�Ŀ��
    //        bool isHitTarget = hitInfo.collider.gameObject.name.IndexOf("Target") != -1;

    //        var anchorLocalCenterPos = GraphMgr.Instance.worldPos2AnchorLocalCenterPos(hitInfo.point);
    //        var graphIdx = GraphMgr.Instance.worldPos2GraphIdx(hitInfo.point);

    //        bool isHitChar = false;
    //        CharMgr.charList.ForEach((_charMgr) =>
    //        {
    //            if(graphIdx.flag && MathTool.getEuclideanDisV3(graphIdx.pos, _charMgr.getGraphIdx().pos) < 0.01)
    //            {
    //                isHitChar = true;
    //            }
    //        });

    //        if (isHitBarrier || isHitChar || !anchorLocalCenterPos.flag) //��֮ǰ���õ��ϰ��� �� ���ڽ�ɫ���ڸ����� �� ���ڵ�ͼ֮��
    //        {
    //            //Debug.LogErrorFormat("�����Ч isHitbarrier��{0}  ishitchar��{1}  flag��{2}", isHitBarrier, isHitChar, anchorLocalCenterPos.flag);
    //            GraphMgr.Instance.clearAllFakeObj();
    //            return;
    //        }
    //        else  //���λ�úϷ�,�ɸ�������
    //        {
    //            //Debug.LogErrorFormat("�����Ч  left:{0}  right:{1}  middle:{2}", leftMouseDown, rightMouseDown, middleScrollDown);
    //            if (leftMouseDown) //�����ϰ���
    //            {
    //                GraphMgr.Instance.putTarOrBarObj(anchorLocalCenterPos.pos, GraphObjType.Barrier);
    //                FlowField.instance.startNavigation();
    //            }
    //            else if (rightMouseDown) //����Ŀ��
    //            {
    //                GraphMgr.Instance.removeTarOrBarObj(GraphObjType.Target);
    //                GraphMgr.Instance.putTarOrBarObj(anchorLocalCenterPos.pos, GraphObjType.Target);
    //                FlowField.instance.startNavigation();
    //                FlowField.instance.prin();
    //            }
    //            else if (middleScrollDown) //���ý�ɫ
    //            {
    //                Debug.LogError(graphIdx.pos);
    //                GameObject charac = ObjPool.instance.getObj(GraphObjType.Char.ToString(), (go) =>
    //                {
    //                    go.transform.parent = GraphMgr.Instance.graphAnchor.transform;
    //                    var _mgr = go.GetComponent<CharMgr>();
    //                    CharMgr.charList.Add(_mgr);
    //                    _mgr.reset2FlowField(new Vector2(graphIdx.pos.x, graphIdx.pos.z));
    //                });
    //            }
    //            else //����Ԥ���������
    //            {
    //                GraphMgr.Instance.putFakeObj(anchorLocalCenterPos.pos, GraphObjType.Fake);
    //            }
    //        }
    //    }
    //    else //û������
    //    {
    //        GraphMgr.Instance.clearAllFakeObj();
    //    }
    //}

    //private void mouseInputAStar(RaycastHit hitInfo, bool isHitSomething, bool leftMouseDown, bool rightMouseDown)
    //{
    //    if (isHitSomething)
    //    {
    //        Debug.DrawLine(Camera.main.transform.position, hitInfo.point);
    //    }

    //    if (isHitSomething) //������
    //    {
    //        ///�Ƿ���Ѿ����õ��ϰ���
    //        bool isHitBarrier = hitInfo.collider.gameObject.name.IndexOf("Barrier") != -1;
    //        ///�Ƿ���Ѿ����õ�Ŀ��
    //        bool isHitTarget = hitInfo.collider.gameObject.name.IndexOf("Target") != -1;

    //        var anchorLocalCenterPos = GraphMgr.Instance.worldPos2AnchorLocalCenterPos(hitInfo.point);
    //        var graphIdx = GraphMgr.Instance.worldPos2GraphIdx(hitInfo.point);
    //        var charGraphIdx = CharMgr.charList[0].getGraphIdx();
    //        ///�Ƿ�򵽽�ɫ
    //        bool isHitChar = (graphIdx.flag && MathTool.getEuclideanDisV3(graphIdx.pos, charGraphIdx.pos) < 0.01);

    //        if (isHitBarrier || isHitChar || !anchorLocalCenterPos.flag) //��֮ǰ���õ��ϰ��� �� ���ڽ�ɫ���ڸ����� �� ���ڵ�ͼ֮��
    //        {
    //            GraphMgr.Instance.clearAllFakeObj();
    //            return;
    //        }
    //        else  //���λ�úϷ�,�ɸ�������
    //        {
    //            if (leftMouseDown) //�����ϰ���
    //            {
    //                GraphMgr.Instance.putTarOrBarObj(anchorLocalCenterPos.pos, GraphObjType.Barrier);
    //                CharMgr.charList[0].freshAStarPath();
    //            }
    //            else if (rightMouseDown) //����Ŀ��
    //            {
    //                GraphMgr.Instance.removeTarOrBarObj(GraphObjType.Target);
    //                GraphMgr.Instance.putTarOrBarObj(anchorLocalCenterPos.pos, GraphObjType.Target);
    //                CharMgr.charList[0].freshAStarPath();
    //            }
    //            else //����Ԥ���������
    //            {
    //                GraphMgr.Instance.putFakeObj(anchorLocalCenterPos.pos, GraphObjType.Fake);
    //            }
    //        }
    //    }
    //    else //û������
    //    {
    //        GraphMgr.Instance.clearAllFakeObj();
    //    }
    //}

    // Update is called once per frame
    void Update()
    {
        
    }
}
