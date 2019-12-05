using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakePlane : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    float rotation = 0.05f;

    // Update is called once per frame
    void Update()
    {
        var actionZ = 2f * Random.Range(-1f,1f);// Mathf.Clamp(M, -1f, 1f);
        var actionX = 2f * Random.Range(-1f, 1f);

        if ((gameObject.transform.rotation.z < rotation && actionZ > 0f) ||
            (gameObject.transform.rotation.z > -rotation && actionZ < 0f))
        {
            gameObject.transform.Rotate(new Vector3(0, 0, 1), actionZ);
        }

        if ((gameObject.transform.rotation.x < rotation && actionX > 0f) ||
            (gameObject.transform.rotation.x > -rotation && actionX < 0f))
        {
            gameObject.transform.Rotate(new Vector3(1, 0, 0), actionX);
        }
    }
}
