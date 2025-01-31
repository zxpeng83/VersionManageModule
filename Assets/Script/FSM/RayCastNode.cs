using GameConfig;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCastNode : IFsmNode
{
    private string _name = nameof(RayCastNode);
    public string Name => this._name;

    public void OnEnter()
    {
        GraphMgr.Instance.removeTarOrBarObj(GraphObjType.Target);
        GraphMgr.Instance.removeTarOrBarObj(GraphObjType.Barrier);
        CharMgr.charList[0].reset2RayCast(new Vector2(1, 1));
        RayCast.instance.reset();
        GraphMgr.Instance.putPet();
        //throw new System.NotImplementedException();
    }

    public void OnExit()
    {
        GraphMgr.Instance.removeTarOrBarObj(GraphObjType.Target);
        GraphMgr.Instance.removeTarOrBarObj(GraphObjType.Barrier);
        CharMgr.charList[0].reset2RayCast(new Vector2(1, 1));
        RayCast.instance.reset();
        GraphMgr.Instance.removePet();
        //throw new System.NotImplementedException();
    }
}
