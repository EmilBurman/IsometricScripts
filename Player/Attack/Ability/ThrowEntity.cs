using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ThrowEntity : MonoBehaviour
{
    public GameObject entity;
    private Rigidbody instatiatedEntity;
    private Transform instantiatedAtPosition;
    public Transform target;

    private float heighOfArc;
    public float gravity = -40;

    public bool debugPath;

    void Start()
    {
        instantiatedAtPosition = GetComponent<Rigidbody>().transform;
        entity.GetComponent<Rigidbody>().useGravity = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Launch();
        }

        if (debugPath)
        {
            DrawPath();
        }
    }

    void Launch()
    {
        Physics.gravity = Vector3.up * gravity;
        instatiatedEntity = Instantiate(entity, instantiatedAtPosition.position, instantiatedAtPosition.rotation).GetComponent<Rigidbody>();
        instatiatedEntity.useGravity = true;
        instatiatedEntity.velocity = CalculateLaunchData().initialVelocity;
    }

    LaunchData CalculateLaunchData()
    {
        float displacementY = target.position.y - instatiatedEntity.position.y;
        heighOfArc = CalculateHeightOfArc(instantiatedAtPosition.position, target.position);
        Vector3 displacementXZ = new Vector3(target.position.x - instatiatedEntity.position.x, 0, target.position.z - instatiatedEntity.position.z);
        float time = Mathf.Sqrt(-2 * heighOfArc / gravity) + Mathf.Sqrt(2 * (displacementY - heighOfArc) / gravity);
        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * heighOfArc);
        Vector3 velocityXZ = displacementXZ / time;

        return new LaunchData(velocityXZ + velocityY * -Mathf.Sign(gravity), time);
    }

    void DrawPath()
    {
        LaunchData launchData = CalculateLaunchData();
        Vector3 previousDrawPoint = instatiatedEntity.position;

        int resolution = 30;
        for (int i = 1; i <= resolution; i++)
        {
            float simulationTime = i / (float)resolution * launchData.timeToTarget;
            Vector3 displacement = launchData.initialVelocity * simulationTime + Vector3.up * gravity * simulationTime * simulationTime / 2f;
            Vector3 drawPoint = instatiatedEntity.position + displacement;
            Debug.DrawLine(previousDrawPoint, drawPoint, Color.green);
            previousDrawPoint = drawPoint;
        }
    }

    float CalculateHeightOfArc(Vector3 startPos, Vector3 endPos)
    {
        return Mathf.Abs(Vector3.Distance(startPos, endPos))/4;
    }

    struct LaunchData
    {
        public readonly Vector3 initialVelocity;
        public readonly float timeToTarget;

        public LaunchData(Vector3 initialVelocity, float timeToTarget)
        {
            this.initialVelocity = initialVelocity;
            this.timeToTarget = timeToTarget;
        }

    }
}
