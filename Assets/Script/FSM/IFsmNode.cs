using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFsmNode
{
    /// <summary>
    /// ½ÚµãÃû³Æ
    /// </summary>
    string Name { get; }

    void OnEnter();
    //void OnUpdate();
    //void OnFixedUpdate();
    void OnExit();
    //void OnHandleMessage(object msg);
}
