using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using MLAgents.CommunicatorObjects;
using UnityEngine.UI;

public class RollerAgent : Agent
{
    Rigidbody rBody;
    public float speed = 10;
    public Text scoreText;


    int score = 0;

    private bool isGoal = false;
    void Start()
    {
        rBody = GetComponent<Rigidbody>();
    }

    public Transform planeRotation;
    public Transform Target;
    public override void AgentReset()
    {
        if (this.transform.localPosition.y < 0)
        {
            // If the Agent fell, zero its momentum
            this.rBody.angularVelocity = Vector3.zero;
            this.rBody.velocity = Vector3.zero;
            this.transform.localPosition = new Vector3(0, 0.5f, 0);
        }

        // Move the target to a new spot
        Target.localPosition = new Vector3(Random.value * 8 - 4,
                                      0.5f,
                                      Random.value * 8 - 4);

        
    }

    public override void CollectObservations()
    {
        // Target and Agent positions
        AddVectorObs(Target.localPosition);
        AddVectorObs(this.transform.localPosition);

        // Agent velocity
        AddVectorObs(rBody.velocity.x);
        AddVectorObs(rBody.velocity.z);

        AddVectorObs(planeRotation.rotation.x);
        AddVectorObs(planeRotation.rotation.z);
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        // Actions, size = 2
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = vectorAction[0];
        controlSignal.z = vectorAction[1];
        rBody.AddForce(controlSignal * speed);

        // Rewards
        float distanceToTarget = Vector3.Distance(this.transform.localPosition,
                                                  Target.localPosition);

        // Reached target
        if (distanceToTarget < 1.42f)
        {
            
            SetReward(1.0f);

            Done();

            score++;
            scoreText.text = "score : " + score;
        }

        // Fell off platform
        if (this.transform.localPosition.y < -0.5f)
        {
            //AddReward(-1.0f);
            Done();
        }

    }
    public float moveSpeed = 8f;
    public Joystick joystick;

    public override float[] Heuristic()
    {
        var action = new float[2];

        if (joystick == null)
            return action;
        Vector2 moveVector = (Vector2.right * joystick.Horizontal + Vector2.up * joystick.Vertical);

        

        
        if (moveVector != Vector2.zero)
        {
            action[0] = moveVector.x;
            action[1] = moveVector.y;
            //transform.rotation = Quaternion.LookRotation(Vector3.forward, moveVector);
            //transform.Translate(moveVector * moveSpeed * Time.deltaTime, Space.World);
        }

        //action[0] = Input.GetAxis("Horizontal");
        //action[1] = Input.GetAxis("Vertical");
        return action;
    }
}
