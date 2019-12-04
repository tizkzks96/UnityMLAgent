using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class SnakeCtrl : Agent
{
    private static float saveReward = 0;

    public float speed = 0.5f;

    public GameObject tailObject;

    public Vector2 previousPosition;

    public GameObject foodObject;

    private Vector2 direction = Vector2.right;
    private Vector3 currentDirection = Vector3.right;

    private static int size = 15;

    private GameObject[] tails = new GameObject[size];
    private Vector2[] tailsPrePosition = new Vector2[size];
    private Vector2[] tailsCurrentPosition = new Vector2[size];

    private int tailSize = 0;

    private Coroutine moveCoroutine;

    private bool isDead = false;
    private bool isGoal = false;


    public IEnumerator Move()
    {
        yield return null;

        previousPosition = transform.localPosition;

        transform.localPosition += currentDirection;

        //add tail();

        if(tails.Length > 0)
            MoveTail();

        moveCoroutine = StartCoroutine(Move());

    }

    public void Moves(Vector2 direction)
    {
        previousPosition = transform.localPosition;

        transform.localPosition += new Vector3(direction.x, direction.y, 0);

        //add tail();

        if (tails.Length > 0)
            MoveTail();
    }

    public void MoveTail()
    {
        for(int i = 0; i< tailSize; i++)
        {
            tailsPrePosition[i] = tails[i].transform.localPosition;
            if(i == 0)
            {
                tails[i].transform.localPosition = previousPosition;
                tailsCurrentPosition[i] = previousPosition;
            }
            else if(i > 0)
            {
                tails[i].transform.localPosition = tailsPrePosition[i-1];
                tailsCurrentPosition[i] = tailsPrePosition[i - 1];
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
        //tails[tailSize].SetActive(true);

        tailsPrePosition[tailSize] = tails[tailSize].transform.localPosition;

        if (tailSize == 0)
        {
            tails[tailSize].transform.localPosition = previousPosition;
        }
        else
        {
            tails[tailSize].transform.localPosition = tailsPrePosition[tailSize - 1];
        }


        tailSize++;
    }

    public Vector3 CheckNearTailPosition(int index)
    {
        if(index > 1)
        {
             return tails[index - 1].transform.localPosition - tails[index - 2].transform.localPosition;
        }
        else if(index == 1)
        {
            //print("tailSize = 0  :  " + (tails[tailSize - 1].transform.position - transform.position));
            return tails[index - 1].transform.localPosition - transform.localPosition;
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
        //if(isDead)
        //    StopCoroutine(moveCoroutine);
    }

    public void ChangeFoodPosition()
    {
        //foodObject.transform.localPosition = new Vector3(Random.Range(-25, 26), Random.Range(-14, 16));
        foodObject.transform.localPosition = new Vector3(Random.Range(-8, 7), Random.Range(-4, 4));
        //foodObject.transform.localPosition = new Vector3(Random.Range(-15, 20), Random.Range(-10, 13));

    }

    public void Reset()
    {

        transform.localPosition = Vector3.zero;

        ChangeFoodPosition();

        tailSize = 0;

        //tails = new GameObject[100];

        isDead = false;
        isGoal = false;
    }

    public void InitTails()
    {
        if(tails[tails.Length - 1] == null)
        {
            for (int i = 0; i<tails.Length; i++)
            {
                tails[i] = Instantiate(tailObject, transform.parent);
                tails[i].transform.localPosition = new Vector3(100,100, 10);
                tailsCurrentPosition[i] = new Vector2(0, 0);
                //tails[i].SetActive(false);
            }
        }
        else
        {
            for (int i = 0; i < tails.Length; i++)
            {
                tails[i].transform.localPosition = new Vector3(100, 100, 10);
                tailsCurrentPosition[i] = new Vector2(0, 0);
                //tails[i].SetActive(false);
            }
        }
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
        InitTails();
        print("Done");
        StateDie();
        Reset();
        //moveCoroutine = StartCoroutine(Move());
    }
    Vector2 temp;
    public override void CollectObservations()
    {
        Vector2 distanceToTarget = foodObject.transform.position - transform.position;

        // Target and Agent positions
        AddVectorObs(distanceToTarget);
        AddVectorObs(foodObject.transform.localPosition);
        AddVectorObs(this.transform.localPosition);

        // Agent velocity
        //for (int i = 0; i < tailSize; i++)
        //    AddVectorObs(tails[i].transform.localPosition);

        foreach (Vector2 tailPosition in tailsCurrentPosition)
        {
                AddVectorObs(tailPosition);
        }
    }
    float range = 0.4f;

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        AddReward(-0.001f);

        Vector2 controlSignal = Vector2.zero;
        //print(vectorAction[1]);

        if (Mathf.Abs(vectorAction[0])> range && Mathf.Abs(vectorAction[1]) > range)
        {
            if (Mathf.Abs(vectorAction[0]) > Mathf.Abs(vectorAction[1]))
            {
                if (vectorAction[0] > range)
                {
                    currentDirection = Vector2.right;
                    //print("1");
                }
                else if (vectorAction[0] < -range)
                {
                    currentDirection = -Vector2.right;
                    //print("2");
                }
            }
            else
            {
                if (vectorAction[1] > range)
                {
                    currentDirection = Vector2.up;
                    //print("3");
                }
                else if (vectorAction[1] < -range)
                {
                    currentDirection = -Vector2.up;
                    //print("4");
                }
            }
        }
        else
        {
            //print("5");
        }
        // Actions, size = 2


        //vectorAction[0] = vectorAction[0] > 0 ? 1 : -1;
        //vectorAction[1] = vectorAction[1] > 0 ? 1 : -1;

        //controlSignal = vectorAction[0] > vectorAction[1] ? vectorAction[0] * Vector2.right : vectorAction[1] * Vector2.up;

        //controlSignal = new Vector2(vectorAction[0], vectorAction[1]);

        //new WaitForSeconds(1.1f);
        //GetDiretion(controlSignal);
        //currentDirection = controlSignal;
        Moves(currentDirection);
        // Reached target
        if (isGoal)
        {
            isGoal = false;
            
            AddReward(1f);
            if (saveReward < tailSize)
            {
                saveReward = tailSize;
                print("AA : " + saveReward + " , tailSize : " + tailSize);

            }
            //Done();
        }

        // Fell off platform
        if (isDead)
        {
            //AddReward(-1f);
            Done();
        }

        //if(foodObject.transform.localPosition.x == transform.localPosition.x)
        //{
        //    AddReward(0.01f);
        //}

        //if (foodObject.transform.localPosition.y == transform.localPosition.y)
        //{
        //    AddReward(0.01f);
        //}

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
