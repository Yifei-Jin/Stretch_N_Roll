using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Encourage : MonoBehaviour
{
    public TMPro.TMP_Text encouragewords;
    private int time;
    
    void Start()
    {
        encouragewords.text = " ";
    }
   
    

    // Update is called once per frame
    void Update()
    {
        time = PlayerController.realsecond;
        if (time == 10)
            encouragewords.text = " Good! Keep going!";
        if(time==12)
            encouragewords.text = " ";
        if (time==31)
            encouragewords.text = "Green cubes for 2 points!";
        if (time == 28)
            encouragewords.text = " ";
        if (time == 34)
            encouragewords.text = " You Can Do It!";
        if (time == 36)
            encouragewords.text = " ";
        if (time == 43)
            encouragewords.text = " Well Done!!!";
        if (time == 45)
            encouragewords.text = " ";

    }
}
