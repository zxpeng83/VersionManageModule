using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VersionManageNode : IFsmNode
{
    private string _name = nameof(VersionManageNode);
    public string Name => this._name;

    public void OnEnter()
    {
        GameControllMgr.instance.reset();
        //throw new System.NotImplementedException();
    }

    public void OnExit()
    {
        GameControllMgr.instance.reset();
        //throw new System.NotImplementedException();
    }
}
