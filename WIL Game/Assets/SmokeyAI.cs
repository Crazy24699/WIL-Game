using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SmokeyAI : MonoBehaviour
{
    public Transform Target;   // The object to circle around
    public NavMeshAgent Agent; // Reference to the NavMeshAgent
    public float CircleRadius = 5f; // Radius of the circle
    public float RotationSpeed = 2f; // Speed of rotation

    private float CurrentAngle = 0f; // Current angle on the circle

    void Start()
    {
        if (Agent == null)
        {
            Agent = GetComponent<NavMeshAgent>();
        }
    }

    void Update()
    {
        // Calculate the new position on the circle
        Vector3 TargetPosition = CalculateCirclePosition();

        // Move the agent to the new position
        Agent.SetDestination(TargetPosition);

        // Increment the angle for circular movement
        CurrentAngle += RotationSpeed * Time.deltaTime;
        if (CurrentAngle >= 360f)
        {
            CurrentAngle = 0f;
        }
    }

    // Method to calculate the position on the circle based on the current angle
    private Vector3 CalculateCirclePosition()
    {
        // Calculate the X and Z position on the circle
        float x = Mathf.Cos(CurrentAngle) * CircleRadius;
        float z = Mathf.Sin(CurrentAngle) * CircleRadius;

        // Set the position relative to the target
        Vector3 PositionOffset = new Vector3(x, 0, z);
        return Target.position + PositionOffset;
    }
}
