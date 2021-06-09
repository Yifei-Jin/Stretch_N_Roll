//This the is main script that added to the player in the Stretch N Roll game. 
//it contains many functions, the most important functions are: score collection and count, ball movement, ball control using kinect input, real time database 

using System.Collections;
using System.Collections.Generic;
using System;
using System.Globalization;
using UnityEngine.UI;
using UnityEngine;
using Kinect = Windows.Kinect;
using Proyecto26;
using TMPro;



public class PlayerController : MonoBehaviour
{
    //Variables...............................................................


    //Game main part
    private float runspeed;

    public Transform start;
    public Transform left;
    public Transform right;
    public Transform middle;
    public float speed;
    public static int count;//score

    //UI needed in the game
    public TMPro.TMP_Text countText;
    public TMPro.TMP_Text winText;
    public TMPro.TMP_Text Timer;
    public TMPro.TMP_Text Score;
    public GameObject back; //the page occure when the game finish
    public GameObject Countdownpage;
    public GameObject Timerarea;
    public TMPro.TMP_Text Countdown;

    //variables for Timer
    public static int realsecond;
    private int second;
    private int minute;
    private float time;


    //Music
    public AudioClip collect;
    private AudioSource music;
    public Slider slider;

    //Kinect connection
    public double length;
    public double gradient;
    double stretch;
    public double sensitivity;
    double NegStretch;

    //database
    User user;
    private int cubeNum;
    public TMP_InputField PlayerName;
    public static string playername;
    public static string time_now;

    //Functions...............................................................

    // Start is called before the first frame update...........................
    void Start()
    {
        count = 0;
        realsecond = 0;
        second = 0;
        minute = 0;
        cubeNum = 0;

        SetCountText();
        winText.text = "";
        Score.text = "";
        Countdown.text = " ";
        Timer.text = "00:00";
        back.SetActive(false);
        runspeed = 0.0f;    //initially the ball is not moving 
        Timerarea.SetActive(false);

        //music setting
        music = this.GetComponent<AudioSource>();
        music.clip = collect;

        //the date and time, which is upload to the database for the physiotherapy to see the student's progress
        time_now = System.DateTime.UtcNow.ToLocalTime().ToString("dd-MM-yyyy   HH:mm");
    }


    //repeating update..........................................................
    void Update()
    {
        //music setting, the slider provided in the pause menu for control volume of music
        music.volume = (slider.value) * 0.1f;

        //count down page before the game start
        if (realsecond == 0)
            Countdown.text = "3";
        if (realsecond == 1)
            Countdown.text = "2";
        if (realsecond == 2)
            Countdown.text = "1";
        if (realsecond == 3)    //start the game when count 1
        {
            Countdownpage.SetActive(false);
            Timerarea.SetActive(true);
            runspeed = 0.15f;
        }

        //The timer
        time += Time.deltaTime;
        if (time >= 1)
        {
            realsecond++;
            second = realsecond - 3;
            time = time - 1;
        }
        if (second == 60)
        {
            minute++;
            second = 0;
        }
        Timer.text = "Time: " + minute.ToString() + ":" + second.ToString();

        //Kinect connect to the game, using global variables.
        gradient = BodySourceView.Gradient; //The gradient of the band from the BodySourceView.cs 
        length = BodySourceView.Length;     //The length of the band (length between hands) from the BodySourceView.cs 
                                            //the difficulty selected at the "game difficulty" page. 
        stretch = Menu_Start.Stretch;
        sensitivity = Menu_Start.Sensitivity;
        NegStretch = 0 - stretch;


        if (second == 40) //stop the game when time equals 40 second
        {
            back.SetActive(true);
            winText.text = "Time's UP!";
            Timer.gameObject.SetActive(false);
            Score.text = "\n\nYour Score is: " + count + "\n\nCONGRATULATIONS!";
            runspeed = 0;
        }
    }


    //Apply force to make the ball contineously moving, speed can be changed by the public vatriable speed........
    //positive grad goes left, negative grad goes right
    void FixedUpdate()
    {
        transform.position += Vector3.forward * runspeed;
        if (gradient > stretch)//left
        {
            transform.position = Vector3.MoveTowards(start.position, left.position, speed * Time.deltaTime);
        }
        if (gradient < stretch && gradient > NegStretch)//middle
        {
            transform.position = Vector3.MoveTowards(start.position, middle.position, speed * Time.deltaTime);
        }
        if (gradient < NegStretch)//right
        {
            transform.position = Vector3.MoveTowards(start.position, right.position, speed * Time.deltaTime);
        }
        //apply force to keep the ball moving
    }


    //collect the cubes when the collision happen................................
    private void OnTriggerEnter(Collider other)
    {
        //the blue cubes in the game, they will be collected when the ball hit the cube, no other requirement
        if (other.gameObject.CompareTag("Easy Pick"))
        {
            other.transform.position += Vector3.forward * 400;
            music.Play();   //the sound "ding" when the cube is successfully collected
            count = count + 1;
            cubeNum = cubeNum + 1;
        }
        //the yellow cubes in the game, only can be collected when the band is stretched to exceed certain length
        if (other.gameObject.CompareTag("Pick up") && length > sensitivity)
        {
            other.transform.position += Vector3.forward * 400;
            music.Play();
            count = count + 1;
            cubeNum = cubeNum + 1;
        }
        //the green cubes in the game, only can be collected when the band is stretched to exceed certain length
        if (other.gameObject.CompareTag("Double") && length > sensitivity)
        {
            other.transform.position += Vector3.forward * 400;
            music.Play();
            count = count + 2;
            cubeNum = cubeNum + 1;
        }        
        SetCountText();
    }


    //the functing used to change the count number shown on the screen............
    void SetCountText()
    {
        countText.text = "Count: " + count.ToString();
    }


    //Post the data: user name/score/date to the databse...........................
    private void PostToDatabase()
    {       
        User user = new User(); //User is the class defined in another script User.cs     
        RestClient.Post("https://physio-game-default-rtdb.firebaseio.com/" + playername + ".json", user);
    }


    //the submit button functions...................................................
    public void OnSubmit()
    {
        playername = PlayerName.text;   //get the name from the input field, and put it into playername
        PostToDatabase();               //submit to database
    }


    //the function used to get the date and time.....................................
    public static void date()
    {
        DateTime localDate = DateTime.Now;
        DateTime utcDate = DateTime.UtcNow;
        String[] cultureNames = { "en-US", "en-GB", "fr-FR",
                                "de-DE", "ru-RU" };
        foreach (var cultureName in cultureNames)
        {
            var culture = new CultureInfo(cultureName);
            Console.WriteLine("{0}:", culture.NativeName);
            Console.WriteLine("   Local date and time: {0}, {1:G}",
                              localDate.ToString(culture), localDate.Kind);
            Console.WriteLine("   UTC date and time: {0}, {1:G}\n",
                              utcDate.ToString(culture), utcDate.Kind);
        }
    }
}
