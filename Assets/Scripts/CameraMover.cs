using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMover : MonoBehaviour
{    
    private Vector3 point;//the coord to the point where the camera looks at
    private bool isOKToMove;
    private Vector3 startPos;


    private void Start()
    {
        isOKToMove = false;
    }


    public Vector3 lookPoint
    {
        set
        {
            point = value;
        }
       
    }

    public Vector3 initPoint
    {
        set
        {
            startPos = value;
        }
    }


    public bool okToMove
    {
        set
        {
            isOKToMove = value;
        }
    }


    public void doReset()
    {
        isOKToMove = false;
        transform.position = startPos;
        transform.LookAt(point);
    }


    void Update()
    {
        if (isOKToMove)
        {
            transform.RotateAround(point, new Vector3(0.0f, 1.0f, 0.0f), 15 * Time.deltaTime);
        }
    }
}
