using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

public class MouseMgr : MonoBehaviour
{
    public static MouseMgr instance;

    private void Awake()
    {
        MouseMgr.instance = this;
    }

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        bool isHitSomthing = Physics.Raycast(ray, out hitInfo, Mathf.Infinity, ~(1<<2));
        bool leftMouseDown = Input.GetMouseButtonDown(0);
        bool rightMouseDown = Input.GetMouseButtonDown(1);
        bool middleScrollDown = Input.GetMouseButtonDown(2);

        GameControllMgr.instance.mouseInput(hitInfo, isHitSomthing, leftMouseDown, rightMouseDown, middleScrollDown);
    }
}
