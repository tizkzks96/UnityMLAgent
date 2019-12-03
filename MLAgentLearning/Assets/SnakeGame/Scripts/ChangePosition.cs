using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangePosition : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("block"))
        {
            ChangeFoodPosition();
        }
    }

    public void ChangeFoodPosition()
    {
        transform.position = new Vector3(Random.Range(-25, 26), Random.Range(-14, 16));
    }
}
