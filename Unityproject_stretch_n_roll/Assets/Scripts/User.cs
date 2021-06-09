using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[Serializable]
public class User
{
    public string userName;
    public int userScore;
    public string Date_time;
    private double stretch;
    private double sensitivity;
    public string Stretch_Difficulty;
    public string Sensitivity;

    public User()
    {
        userName = PlayerController.playername;
        userScore = PlayerController.count;
        Date_time = PlayerController.time_now;
        stretch = Menu_Start.Stretch;
        sensitivity = Menu_Start.Sensitivity;
        if (stretch == 0.2)
            Stretch_Difficulty = "Easy";
        if (stretch == 0.5)
            Stretch_Difficulty = "Medium";
        if (stretch == 0.6)
            Stretch_Difficulty = "Hard";

        if (sensitivity == 5.5)
            Sensitivity = "Easy";
        if (sensitivity == 6.5)
            Sensitivity = "Medium";
        if (sensitivity == 7.5)
            Sensitivity = "Hard";
        Debug.Log("userclass: "+userName);
        Debug.Log("userclass: "+userScore);
    }
}