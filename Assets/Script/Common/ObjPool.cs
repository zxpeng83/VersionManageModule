using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// 对象池
/// </summary>
public class ObjPool
{
    private static ObjPool ins;
    public static ObjPool instance
    {
        get
        {
            if(ins == null)
            {
                ins = new ObjPool();
            }

            return ins;
        }
    }

    private Dictionary<string, List<GameObject>> pool;
    private Dictionary<string, GameObject> prefabs;
    private Vector3 hidePos = new Vector3(0, -1000, 0);

    public ObjPool()
    {
        this.pool = new Dictionary<string, List<GameObject>>();
        this.prefabs = new Dictionary<string, GameObject>();
    }

    public GameObject getObj(string prefabName, Action<GameObject> callback = null)
    {
        GameObject rtnObj = null;
        if (prefabs.ContainsKey(prefabName) && pool.ContainsKey(prefabName) && pool[prefabName].Count>0)
        {
            rtnObj = pool[prefabName][0];
            pool[prefabName].RemoveAt(0);
        }
        else
        {
            if(!prefabs.ContainsKey(prefabName))
            {
                GameObject loadPrefab = Resources.Load<GameObject>("Prefab/" +  prefabName);
                prefabs.Add(prefabName, loadPrefab);
                pool.Add(prefabName, new List<GameObject>());
            }
            if (prefabs[prefabName] == null)
            {
                Debug.LogError("生成预制体出错：ObjPool.getObj()");
                return null;
            }

            rtnObj = Object.Instantiate(prefabs[prefabName]);
            rtnObj.transform.position = this.hidePos;
        }

        rtnObj.name = prefabName;
        rtnObj.SetActive(true);

        if(callback != null)
        {
            callback(rtnObj);
        }

        return rtnObj;
    }

    public void backObj(GameObject obj, Action<GameObject> callback = null)
    {
        if(obj == null) return;

        if(obj.name.IndexOf("-") != -1)
        {
            string[] tem = obj.name.Split('-');
            obj.name = tem[0];
        }
        if(obj.name.IndexOf("_") != -1)
        {
            string[] tem = obj.name.Split("_");
            obj.name = tem[0];
        }

        if (!pool.ContainsKey(obj.name))
        {
            Object.Destroy(obj);
        }
        else
        {
            obj.transform.parent = null;
            obj.SetActive(false);
            pool[obj.name].Add(obj);

            obj.transform.position = this.hidePos;

            if( callback != null )
            {
                callback(obj);
            }
        }
    }
}
