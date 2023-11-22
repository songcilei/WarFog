using System;
using System.Collections;
using System.Collections.Generic;
using CodeMonkey.Utils;
using Unity.VisualScripting;
using UnityEngine;

public class Testing : MonoBehaviour
{
    public Grid grid;
    public Transform targetTrans;
    void Start()
    {
        grid = new Grid(10, 10,10, new Vector3(0,0,0));
    }


    private void Update()
    {

    }


    
    
}
