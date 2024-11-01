using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.InteropServices;


public class Hand : MonoBehaviour
{
    [DllImport("Usc")]
    public static extern byte callStackSize();

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(callStackSize());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
