using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Layers : MonoBehaviour
{
    private Vector3 fVec;

    public Vector3 forward
    {
        get
        {
            return fVec;
        }
        set
        {
            fVec = value;
        }
    }

    public Vector3 paint
    {
        get
        {
            return fVec * .01f;
        }
    }

    public Vector3 outline
    {
        get
        {
            return fVec * .02f;
        }
    }

    public Vector3 cursor
    {
        get
        {
            return fVec * .04f;
        }
    }

    public Vector3 ui
    {
        get
        {
            return fVec * .03f;
        }
    }
}
