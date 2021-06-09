using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Kinect = Windows.Kinect;
using Proyecto26;
using TMPro;
using System;
using System.Globalization;
//positive grad goes left, negative grad goes right
//above 0.6 goes to left
//between -0.6 and 0.6 stay in middle lane
//below -0.6 right lane
public class PlayerController : MonoBehaviour
{
    //Kinect connection
    public double length;
    public double gradient;
    double stretch;
    public double sensitivity;
    double NegStretch;
    // private static BodySourceView gradient = new BodySourceView();
    // public double grad = gradient.grad();

    //Game main part
    private float runspeed;

    public Transform start;
    public Transform left;
    public Transform right;
    public Transform middle;
    public float speed;

    public static int count;

    public TMPro.TMP_Text countText;
    public TMPro.TMP_Text winText;
    public TMPro.TMP_Text Timer;

    public TMPro.TMP_Text Score;
    public GameObject back;
    public GameObject Countdownpage;
    public GameObject Timerarea;
    public TMPro.TMP_Text Countdown;

    public static int realsecond;
    private int second;
    private int minute;
    private float time;
    private int timer1;
    private int timer2;


    //music
    public AudioClip collect;
    private AudioSource music;
    public Slider slider;

    //database
    private int cubeNum;
    User user;
    //public static string Playeremail;
    public TMP_InputField PlayerName;
    public static string playername;

    public static string time_now;
   

    // Start is called before the first frame update
    void Start()
    {
        count = 0;
        second = 0;
        realsecond = 0;
        minute = 0;
        cubeNum = 0;
        SetCountText();
        winText.text = "";
        Score.text = "";
        Countdown.text = " ";
        Timer.text = "00:00";
        back.SetActive(false);
        runspeed = 0.0f;
        Timerarea.SetActive(false);
        

        music = this.GetComponent<AudioSource>();
        music.clip = collect;

        time_now = System.DateTime.UtcNow.ToLocalTime().ToString("dd-MM-yyyy   HH:mm");
    }

    //when input"a" the ball will move to the Left lanes, input"s" will move to the middle lanes, input "d"will move to right lanes
    //When we use kinect can just change the input, relate it to 3 movement
    void Update()
    {
        music.volume = (slider.value) * 0.1f;
        //count down
        if (realsecond == 0)
            Countdown.text = "3";
        if (realsecond == 1)
            Countdown.text = "2";
        if (realsecond == 2)
            Countdown.text = "1";
        if (realsecond == 3)
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


        gradient = BodySourceView.Gradient;
        length = BodySourceView.Length;
        stretch = Menu_Start.Stretch;
        sensitivity = Menu_Start.Sensitivity;
        NegStretch = 0 - stretch;

        //Playeremail = AuthManager.Emailtext;
        //Debug.Log(stretch);
        //Debug.Log(sensitivity);
        //Debug.Log(cubeNum);

        if (second==45)
        {
            back.SetActive(true);
            winText.text = "Time's UP!";
            timer1 = minute;
            timer2 = second;
            Timer.gameObject.SetActive(false);
            Score.text = "\n\nYour Score is: " + count + "\n\nCONGRATULATIONS!";
            runspeed = 0;
           

        }
       
        
        
    }



    //Apply force to make the ball contineously moving, speed can be changed by the public vatriable speed
    void FixedUpdate()
    {


        transform.position += Vector3.forward * runspeed;
        //Debug.Log(gradient);

        if (gradient > 0.2)
        {
            transform.position = Vector3.MoveTowards(start.position, left.position, speed * Time.deltaTime);
            //Debug.Log("left");
        }
        if (gradient < stretch && gradient > NegStretch )
        {
            transform.position = Vector3.MoveTowards(start.position, middle.position, speed * Time.deltaTime);
            //Debug.Log("middle");
        }
        if (gradient < NegStretch )
        {
            transform.position = Vector3.MoveTowards(start.position, right.position, speed * Time.deltaTime);
            //Debug.Log("right");

            //apply force to keep the ball moving
        }
    }

    


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Easy Pick"))
        {
            other.transform.position += Vector3.forward * 400;
            music.Play();
            count = count + 1;
            cubeNum = cubeNum + 1;
            //Debug.Log("collect");
        }
        
        
            if (other.gameObject.CompareTag("Pick up")&& length > sensitivity)
            {
                other.transform.position += Vector3.forward * 400;
                music.Play();
                count = count + 1;
                cubeNum = cubeNum + 1;
            }
            if (other.gameObject.CompareTag("Double")&& length > sensitivity)
            {
                other.transform.position += Vector3.forward * 400;
                music.Play();
                count = count + 2;
                cubeNum = cubeNum + 1;
            }
            
       // }
        SetCountText();
        
    }

    //the functing used to change the count number shown on the screen
    void SetCountText()
    {
        countText.text = "Count: " + count.ToString();

    }

    private void PostToDatabase()
    {
        //Debug.Log("email1: "+Playeremail);
        User user = new User();
        //Debug.Log("email2: " + Playeremail);
        RestClient.Post("https://physio-game-default-rtdb.firebaseio.com/" + playername  + ".json" , user);
    }

    public void OnSubmit()
    {
        playername = PlayerName.text;
        PostToDatabase();
    }


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
