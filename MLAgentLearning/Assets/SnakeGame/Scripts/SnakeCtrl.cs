using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class SnakeCtrl : Agent
{
    public float speed = 0.5f;

    public GameObject tailObject;

    public Vector2 previousPosition;

    public GameObject foodObject;

    private Vector3 direction = Vector3.right;
    private Vector3 currentDirection = Vector3.right;

    private GameObject[] tails = new GameObject[100];
    private Vector3[] tailsPrePosition = new Vector3[100];

    private int tailSize = 0;

    private Coroutine moveCoroutine;

    private bool isDead = false;
    private bool isGoal = false;


    public IEnumerator Move()
    {
        yield return new WaitForSeconds(speed);

        previousPosition = transform.position;

        transform.position += currentDirection;

        //add tail();

        if(tails.Length > 0)
            MoveTail();

        moveCoroutine = StartCoroutine(Move());
    }

    public void MoveTail()
    {
        for(int i = 0; i< tailSize; i++)
        {
            tailsPrePosition[i] = tails[i].transform.position;
            print("asd  :  " + (transform.position + CheckNearTailPosition(i)));
            if(i == 0)
            {
                tails[i].transform.position = previousPosition;
            }
            else if(i > 0)
            {
                tails[i].transform.position = tailsPrePosition[i-1];
            }

        }
    }

    public Vector3 GetDiretion(Vector2 controlSignal)
    {
        //direction.y = Input.GetAxisRaw("Vertical");
        //direction.x = Input.GetAxisRaw("Horizontal");

        direction = controlSignal;

        if(direction.y + direction.x != 0)
        {
            currentDirection = direction;
            return direction;
        }

        return currentDirection;
    }

    public void RunMove()
    {

    }

    public void AddTail()
    {
        tails[tailSize] = Instantiate(tailObject);
        tailsPrePosition[tailSize] = tails[tailSize].transform.position;

        if (tailSize == 0)
        {
            tails[tailSize].transform.position = previousPosition;
        }
        else
        {
            tails[tailSize].transform.position = tailsPrePosition[tailSize - 1];
        }


        tailSize++;
    }

    public Vector3 CheckNearTailPosition(int index)
    {
        if(index > 1)
        {
             return tails[index - 1].transform.position - tails[index - 2].transform.position;
        }
        else if(index == 1)
        {
            //print("tailSize = 0  :  " + (tails[tailSize - 1].transform.position - transform.position));
            return tails[index - 1].transform.position - transform.position;
        }

        return -currentDirection;
    }




    public void GetFood(Collider2D collision)
    {
        if (collision.CompareTag("food"))
        {
            isGoal = true;
            StateGetFood();
            ChangeFoodPosition();
        }
    }

    public void CheckDie(Collider2D collision)
    {
        if (collision.CompareTag("wall") || collision.CompareTag("block"))
        {
            isDead = true;
        }
    }

    public void StateGetFood()
    {
        AddTail();
    }

    public void StateDie()
    {
        if(isDead)
            StopCoroutine(moveCoroutine);
    }

    public void ChangeFoodPosition()
    {
        foodObject.transform.position = new Vector3(Random.Range(-25, 26), Random.Range(-14, 16));
    }

    public void Reset()
    {
        transform.position = Vector3.zero;

        ChangeFoodPosition();

        tailSize = 0;

        tails = new GameObject[100];

        isDead = false;
        isGoal = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CheckDie(collision);
        GetFood(collision);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //GetDiretion();
    }

    public override void AgentReset()
    {
        StateDie();
        Reset();
        moveCoroutine = StartCoroutine(Move());
    }

    public override void CollectObservations()
    {
        // Target and Agent positions
        AddVectorObs(foodObject.transform.localPosition);
        AddVectorObs(this.transform.localPosition);

        // Agent velocity
        for(int i = 0; i < tailSize; i++)
            AddVectorObs(tails[i].transform.localPosition);
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        // Actions, size = 2
        Vector2 controlSignal = Vector2.zero;
        controlSignal.x = vectorAction[0];
        controlSignal.y = vectorAction[1];
        GetDiretion(controlSignal);

        // Reached target
        if (isGoal)
        {

            SetReward(1.0f);
            Done();
        }

        // Fell off platform
        if (isDead)
        {
            Done();
        }

    }

    public override float[] Heuristic()
    {
        var action = new float[2];
        action[0] = Input.GetAxisRaw("Horizontal");
        action[1] = Input.GetAxisRaw("Vertical");

        GetDiretion(new Vector2(action[0], action[1]));

        return action;
    }
}
