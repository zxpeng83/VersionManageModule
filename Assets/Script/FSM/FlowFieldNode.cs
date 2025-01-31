using GameConfig;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowFieldNode : IFsmNode
{
    private string _name = nameof(FlowFieldNode);
    public string Name => this._name;

    public void OnEnter()
    {
        GraphMgr.Instance.removeTarOrBarObj(GraphObjType.Target);
        GraphMgr.Instance.removeTarOrBarObj(GraphObjType.Barrier);
        for(int i= CharMgr.charList.Count-1; i >= 0; i--)
        {
            if (i == 0)
            {
                CharMgr.charList[i].reset2FlowField(new Vector2(1, 1));
                break;
            }

            var _mgr = CharMgr.charList[i];
            var go = _mgr.gameObject;

            CharMgr.charList.RemoveAt(i);

            _mgr.reset2FlowField();
            ObjPool.instance.backObj(go);
        }

        FlowField.instance.startNavigation();
    }

    public void OnExit()
    {
        GraphMgr.Instance.removeTarOrBarObj(GraphObjType.Target);
        GraphMgr.Instance.removeTarOrBarObj(GraphObjType.Barrier);
        for (int i = CharMgr.charList.Count - 1; i >= 0; i--)
        {
            if (i == 0)
            {
                CharMgr.charList[i].reset2FlowField(new Vector2(1, 1));
                break;
            }

            var _mgr = CharMgr.charList[i];
            var go = _mgr.gameObject;

            CharMgr.charList.RemoveAt(i);

            _mgr.reset2FlowField();
            ObjPool.instance.backObj(go);
        }
        //throw new System.NotImplementedException();
    }
}
