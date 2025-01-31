using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathTool
{
    /// <summary>
    /// ���������ŷ����þ���(Vector2)
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static float getEuclideanDisV2(Vector2 a, Vector2 b)
    {
        return (float)Mathf.Sqrt(Mathf.Pow(a.x - b.x, 2) + Mathf.Pow(a.y - b.y, 2));
    }

    /// <summary>
    /// ���������ŷ����þ���(Vector3)
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static float getEuclideanDisV3(Vector3 a, Vector3 b)
    {
        return (float)Mathf.Sqrt(Mathf.Pow(a.x - b.x, 2) + Mathf.Pow(a.y - b.y, 2) + Mathf.Pow(a.z - b.z, 2));
    }
}
