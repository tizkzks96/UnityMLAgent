using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
public class BallAgent : Agent
{
    private Rigidbody ballRigidbody;

    public Transform pivotTransform;

    public Transform target;

    public float moveForce = 10f;

    private bool targetEaten = false;

    private bool dead = false;

    private void Awake()
    {
        ballRigidbody = GetComponent<Rigidbody>();
    }

    void ResetTarget()
    {
        targetEaten = false;
        Vector3 randomPos = new Vector3(Random.Range(-5f, 5f), 0.5f, Random.Range(-5f, 5f));
        target.position = randomPos + pivotTransform.position;
    }

    public override void AgentReset()
    {
        Vector3 randomPos = new Vector3(Random.Range(-5f, 5f), 0.5f, Random.Range(-5f, 5f));
        target.position = randomPos + pivotTransform.position;

        dead = false;
        ballRigidbody.velocity = Vector3.zero;

        ResetTarget();
    }

    public override void CollectObservations()
    {
        Vector3 distanceToTarget = target.position - transform.position;
        
        AddVectorObs(Mathf.Clamp(distanceToTarget.x / 5f, -1f, 1f));
        AddVectorObs(Mathf.Clamp(distanceToTarget.z / 5f, -1f, 1f));

        Vector3 relativePos = transform.position - pivotTransform.position;

        
        AddVectorObs(Mathf.Clamp(relativePos.x / 5f, -1f, 1f));
        AddVectorObs(Mathf.Clamp(relativePos.z / 5f, -1f, 1f));

        AddVectorObs(Mathf.Clamp(ballRigidbody.velocity.x / 10f, -1f, 1f));
        AddVectorObs(Mathf.Clamp(ballRigidbody.velocity.z / 10f, -1f, 1f));
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        AddReward(-0.001f);

        float horizontalInoput = vectorAction[0];
        float verticalInput = vectorAction[1];

        ballRigidbody.AddForce(horizontalInoput * moveForce, 0f, verticalInput * moveForce);

        if (targetEaten)
        {
            AddReward(1.0f);
            ResetTarget();
        }
        else if (dead)
        {
            AddReward(-1.0f);
            Done();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("dead"))
        {
            dead = true;
        }
        else if (other.CompareTag("goal"))
        {
            targetEaten = true;
        }
    }
}
