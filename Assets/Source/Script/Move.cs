using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    public float degreesPerSecond = 90.0f;
    public float speedPerSecond = 1.0f;
    public bool movingUp = true;
    public bool start = true;
    void Start()
    {

    }

    void Update()
    {
        if (movingUp)
        {
            transform.Translate(0, speedPerSecond * Time.deltaTime, 0);
            if (transform.position.y >= 5)
            {
                movingUp = false;
            }
        }
        if (!movingUp)
        {
            transform.Translate(0, -speedPerSecond * Time.deltaTime, 0, 0);
            if (transform.position.y <= 0)
            {
                movingUp = true;
            }
        }
    }
}
