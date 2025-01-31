using GameConfig;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarNode : IFsmNode
{
    private string _name = nameof(AStarNode);
    public string Name => _name;

    public void OnEnter()
    {
        GraphMgr.Instance.removeTarOrBarObj(GraphObjType.Target);
        GraphMgr.Instance.removeTarOrBarObj(GraphObjType.Barrier);
        CharMgr.charList[0].reset2AStar(new Vector3(1, 1));
        //throw new System.NotImplementedException();
    }

    public void OnExit()
    {
        GraphMgr.Instance.removeTarOrBarObj(GraphObjType.Target);
        GraphMgr.Instance.removeTarOrBarObj(GraphObjType.Barrier);
        CharMgr.charList[0].reset2AStar(new Vector3(1, 1));
        //throw new System.NotImplementedException();
    }
}
