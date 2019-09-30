using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour
{
    public float Radius { get; private set; }

    private Suspension m_Suspension;

    // Start is called before the first frame update
    void Awake()
    {
        Radius = CalculateRadius();
        m_Suspension = GetComponentInParent<Suspension>();
    }

    public void Execute()
    {
        var p = transform.localPosition;
        transform.localPosition = new Vector3(p.x, -m_Suspension.CurrentSpringLength - Radius, p.z);
    }

    public float CalculateRadius()
    {
        var verts = GetComponent<MeshFilter>().mesh.vertices;
        var maxY = 0.0f;
        foreach (var v in verts)
        {
            if (v.y > maxY)
                maxY = v.y;
        }
        return maxY;
    }
}
