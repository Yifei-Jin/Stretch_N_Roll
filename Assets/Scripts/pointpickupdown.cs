using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pointpickupdown : MonoBehaviour
{
    private int time;
    private Vector3 goup;

    void Start()
    {
        goup = new Vector3(0.0f, -1f, 0.0f);

    }



    void Update()
    {
        time = PlayerController.realsecond;


        if (time == 33)
            transform.position += goup;
    }

}
