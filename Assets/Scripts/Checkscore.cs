using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Proyecto26;
using TMPro;

public class Checkscore : MonoBehaviour
{
    public TMP_InputField Check_name;
    public TMPro.TMP_Text Check_score;
    User user;

    

    public void OnGetScore()
    {
        RetrieveFromDatabase();
       
    }
    public void UpdateScore()
    {
        Check_score.text = "Score: " + user.userScore;
    }

    public void RetrieveFromDatabase()
    {
        RestClient.Get<User>("https://physio-game-default-rtdb.firebaseio.com/" + Check_name.text + ".json").Then(response =>
        {
            user = response;
            UpdateScore();
        });
    }

}
