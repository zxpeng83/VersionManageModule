using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetMgr : MonoBehaviour
{
    private bool isFollow = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void starFollow()
    {
        this.isFollow = true;
    }

    public void stopFollow()
    {
        this.isFollow = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(!this.isFollow)
        {
            return;
        }

        LinkedList<Vector3> path = RayCast.instance.getPath();

        if (path == null || path.Count == 0)
        {
            return;
        }

        Vector3 charCurGraphIdx = path.First.Value;
        Vector3 charCurWorldPos = GraphMgr.Instance.localPos2WorldPos(charCurGraphIdx);
        if(MathTool.getEuclideanDisV3(charCurWorldPos, transform.position) <= 1)
        {
            return;
        }

        foreach(Vector3 charGraphIdx in path)
        {
            Vector3 charLocalCenterPos = new Vector3(charGraphIdx.x+0.5f, 0, charGraphIdx.z+0.5f);
            Vector3 charWorldPos = GraphMgr.Instance.localPos2WorldPos(charLocalCenterPos);

            Vector3 rayStar = new Vector3(transform.position.x, 0.5f, transform.position.z);
            Vector3 rayEnd = new Vector3(charWorldPos.x, 0.5f, charWorldPos.z);

            if (!Physics.Linecast(rayStar, rayEnd))
            {
                Debug.DrawLine(rayStar, rayEnd, Color.green);
                //可达
                Vector3 dir = (charWorldPos - transform.position).normalized;
                transform.Translate(dir*1.2f*Time.deltaTime, Space.World);

                break;
            }
            else
            {
                Debug.DrawLine(rayStar, rayEnd, Color.red);
                //不可达
            }
        }
    }
}
