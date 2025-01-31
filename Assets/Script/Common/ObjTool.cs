using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class ObjTool
{
    private static ObjTool ins = null;

    public static ObjTool instance
    {
        get
        {
            if(ins == null)
            {
                ins = new ObjTool();
            }

            return ins;
        }
    }

    /// <summary>
    /// �������������, ��������������
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="name"></param>
    public void setNameWithChild(GameObject go,  string name)
    {
        go.name = name;

        Transform[] children = go.GetComponentsInChildren<Transform>();

        foreach (var item in children)
        {
            item.gameObject.name = name;
        }
    }

    /// <summary>
    /// ��������Ĳ㼶,��������������
    /// </summary>
    /// <param name="go"></param>
    /// <param name="lay"></param>
    public void setLayWithChild(GameObject go, int lay)
    {
        go.layer = lay;

        Transform[] children = go.GetComponentsInChildren<Transform>();

        foreach (var item in children)
        {
            item.gameObject.layer = lay;
        }
    }

    /// <summary>
    /// ��ȡ���е�һ���������
    /// </summary>
    /// <param name="go"></param>
    /// <returns></returns>
    public GameObject[] getChildWithL1(GameObject go)
    {
        GameObject[] gos = new GameObject[go.transform.childCount];
        
        for(int i = 0; i < go.transform.childCount; i++)
        {
            GameObject child = go.transform.GetChild(i).gameObject;
            gos[i] = child;
        }

        return gos;
    }
}
