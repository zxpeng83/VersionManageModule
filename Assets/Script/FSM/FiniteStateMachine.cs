using System.Collections.Generic;
using UnityEngine;

public class FiniteStateMachine
{
    private readonly List<IFsmNode> _nodes = new List<IFsmNode>();
    private IFsmNode _curNode;
    private IFsmNode _preNode;


    /// <summary>
    /// 当前运行的节点名称
    /// </summary>
    public string CurrentNodeName
    {
        get 
        { 
            return _curNode != null ? _curNode.Name : string.Empty;
        }
    }

    /// <summary>
    /// 之前运行的节点名称
    /// </summary>
    public string PreviousNodeName
    {
        get { return _preNode != null ? _preNode.Name : string.Empty; }
    }


    /// <summary>
    /// 启动状态机
    /// </summary>
    /// <param name="entryNode">入口节点</param>
    public void Run(string entryNode)
    {
        _curNode = GetNode(entryNode);
        _preNode = GetNode(entryNode);

        if (_curNode != null)
            _curNode.OnEnter();
        else
            Debug.LogError("状态机启动失败");
    }

    /// <summary>
    /// 加入一个节点
    /// </summary>
    public void AddNode(IFsmNode node)
    {
        if (_nodes.Contains(node) == false)
        {
            _nodes.Add(node);
        }
    }

    /// <summary>
    /// 转换节点
    /// </summary>
    public void Transition(string nodeName)
    {
        IFsmNode node = GetNode(nodeName);
        if (node == null)
        {
            return;
        }

        _preNode = _curNode;
        _curNode.OnExit();
        _curNode = node;
        _curNode.OnEnter();
    }

    private bool IsContains(string nodeName)
    {
        for (int i = 0; i < _nodes.Count; i++)
        {
            if (_nodes[i].Name == nodeName)
                return true;
        }
        return false;
    }

    private IFsmNode GetNode(string nodeName)
    {
        for (int i = 0; i < _nodes.Count; i++)
        {
            if (_nodes[i].Name == nodeName)
                return _nodes[i];
        }
        return null;
    }
}
