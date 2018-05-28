using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ThrowArch : MonoBehaviour
{

    #region Variables
    public float velocity;
    public float angle;
    public int resolution = 10;

    float gravity;
    float radiantAngle;
    LineRenderer lr;
    #endregion

    #region Initalize
    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        gravity = Mathf.Abs(Physics.gravity.y);
    }

    private void Start()
    {
        RendererArc();
    }
    #endregion
    // Update is called once per frame
    void Update()
    {

    }

    void RendererArc()
    {
        lr.SetVertexCount(resolution + 1);
        lr.SetPositions(CalculateArray());
    }
    Vector3[] CalculateArray()
    {
        Vector3[] arcArray = new Vector3[resolution + 1];
        radiantAngle = Mathf.Deg2Rad * angle;
        float maxDistance = velocity * velocity * Mathf.Sin(2 * radiantAngle) / gravity;

        for (int i=0; i<=resolution; i++)
        {
            float t = (float)i / (float)resolution;
            arcArray[i] = CalculateArcPoint(t, maxDistance);
        }
        return arcArray;
    }
    Vector3 CalculateArcPoint (float t, float maxdistance)
    {
        return new Vector3(0,0,0);
    }
}
