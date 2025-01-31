using System.Collections.Generic;
using UnityEngine;

public class FiniteStateMachine
{
    private readonly List<IFsmNode> _nodes = new List<IFsmNode>();
    private IFsmNode _curNode;
    private IFsmNode _preNode;


    /// <summary>
    /// ��ǰ���еĽڵ�����
    /// </summary>
    public string CurrentNodeName
    {
        get 
        { 
            return _curNode != null ? _curNode.Name : string.Empty;
        }
    }

    /// <summary>
    /// ֮ǰ���еĽڵ�����
    /// </summary>
    public string PreviousNodeName
    {
        get { return _preNode != null ? _preNode.Name : string.Empty; }
    }


    /// <summary>
    /// ����״̬��
    /// </summary>
    /// <param name="entryNode">��ڽڵ�</param>
    public void Run(string entryNode)
    {
        _curNode = GetNode(entryNode);
        _preNode = GetNode(entryNode);

        if (_curNode != null)
            _curNode.OnEnter();
        else
            Debug.LogError("״̬������ʧ��");
    }

    /// <summary>
    /// ����һ���ڵ�
    /// </summary>
    public void AddNode(IFsmNode node)
    {
        if (_nodes.Contains(node) == false)
        {
            _nodes.Add(node);
        }
    }

    /// <summary>
    /// ת���ڵ�
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
