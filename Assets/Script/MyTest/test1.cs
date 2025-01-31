using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test1 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("------------------------------");
            Debug.Log(transform.eulerAngles);
            Debug.Log(transform.forward);
            Vector3 tmp = new Vector3(0.71f, 0, 0.71f).normalized;
            Debug.Log(tmp);

            Vector3 ve = transform.localEulerAngles;
        }
    }
}
