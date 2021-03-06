//This is the script used to calculate the gradient and length between two hands using the data input from the kinect
//This is part of the original kinect sciprt from the website
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Kinect = Windows.Kinect;

public class BodySourceView : MonoBehaviour
{
    //Variables.............................................................................................................
    public static double Length;    //the global variable that can be access by other script
    public static double Gradient;  //the global variable that can be access by other script
    public Material BoneMaterial;
    public GameObject BodySourceManager;

    private Dictionary<ulong, GameObject> _Bodies = new Dictionary<ulong, GameObject>();
    private BodySourceManager _BodyManager;

    private Dictionary<Kinect.JointType, Kinect.JointType> _BoneMap = new Dictionary<Kinect.JointType, Kinect.JointType>()
    {
        //Here we have removed all the joints exceot those that we needed to be tracked for the game
        //Just the right and left hand components
        { Kinect.JointType.HandTipLeft, Kinect.JointType.HandLeft },
        { Kinect.JointType.ThumbLeft, Kinect.JointType.HandLeft },
        { Kinect.JointType.HandTipRight, Kinect.JointType.HandRight },
        { Kinect.JointType.ThumbRight, Kinect.JointType.HandRight },    //all the joints we needed for the shoulder extension stretch
    };


    //Functions................................................................................................................
    void Update ()
    {
        if (BodySourceManager == null)
        {
            return;
        }

        _BodyManager = BodySourceManager.GetComponent<BodySourceManager>();
        if (_BodyManager == null)
        {
            return;
        }

        Kinect.Body[] data = _BodyManager.GetData();
        if (data == null)
        {
            return;
        }
        //Creating tracking ids and assigning the id to each joint in the body
        List<ulong> trackedIds = new List<ulong>();
        foreach(var body in data)
        {
            if (body == null)
            {
                continue;
              }

            if(body.IsTracked)
            {
                trackedIds.Add (body.TrackingId);
            }
        }

        List<ulong> knownIds = new List<ulong>(_Bodies.Keys);

        // First delete untracked bodies
        foreach(ulong trackingId in knownIds)
        {
            if(!trackedIds.Contains(trackingId))
            {
                Destroy(_Bodies[trackingId]);
                _Bodies.Remove(trackingId);
            }
        }

        foreach(var body in data)
        {
            if (body == null)
            {
                continue;
            }

            if(body.IsTracked)
            {
                if(!_Bodies.ContainsKey(body.TrackingId))
                {
                    _Bodies[body.TrackingId] = CreateBodyObject(body.TrackingId);
                }

                RefreshBodyObject(body, _Bodies[body.TrackingId]);

                Gradient = Get_gradient(body);
                Length = Get_length(body);
            }
        }
    }

    private GameObject CreateBodyObject(ulong id)
    {
        //Creating a temporary body object when the kinect is connected to store the joints
        GameObject body = new GameObject("Body:" + id);
        // Setting which joints we want to be added to this temporary body, from the left shoulder to the right hands
        //The order of the joint list is given with the kinect package and we cannot change it
        for (Kinect.JointType jt = Kinect.JointType.HandLeft; jt <= Kinect.JointType.HandRight; jt=jt+4)
        {
            GameObject jointObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //This came with the kinect package, here we are connecting the joints to each other by lines to resemble a person
            LineRenderer lr = jointObj.AddComponent<LineRenderer>();
            lr.SetVertexCount(2);
            lr.material = BoneMaterial;
            lr.SetWidth(0.05f, 0.05f);

            jointObj.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            jointObj.name = jt.ToString();
            jointObj.transform.parent = body.transform;
        }
        return body;
    }


    private void RefreshBodyObject(Kinect.Body body, GameObject bodyObject)
    {
        for (Kinect.JointType jt = Kinect.JointType.HandLeft; jt <= Kinect.JointType.HandRight; jt=jt+4)
        {
            Kinect.Joint sourceJoint = body.Joints[jt];
            Kinect.Joint? targetJoint = null;

            if(_BoneMap.ContainsKey(jt))
            {
                targetJoint = body.Joints[_BoneMap[jt]];
            }

            Transform jointObj = bodyObject.transform.Find(jt.ToString());
            jointObj.localPosition = GetVector3FromJoint(sourceJoint);

            LineRenderer lr = jointObj.GetComponent<LineRenderer>();
            if(targetJoint.HasValue)
            {
                lr.SetPosition(0, jointObj.localPosition);
                lr.SetPosition(1, GetVector3FromJoint(targetJoint.Value));
                lr.SetColors(GetColorForState (sourceJoint.TrackingState), GetColorForState(targetJoint.Value.TrackingState));
            }
            else
            {
                lr.enabled = false;
            }
        }
    }


    private static Color GetColorForState(Kinect.TrackingState state)
    {
        switch (state)
        {
        case Kinect.TrackingState.Tracked:
            return Color.green;

        case Kinect.TrackingState.Inferred:
            return Color.red;

        default:
            return Color.black;
        }
    }


    /*This is a function that gets the x,y,z position of a joint inserted into its parameters
   and equates each component of the postion into a vector of size 3*/
    private static Vector3 GetVector3FromJoint(Kinect.Joint joint)
    {
        return new Vector3(joint.Position.X * 10, joint.Position.Y * 10, joint.Position.Z * 10);
    }


    //This function  is used to calculate the gradient between hands
    private double Get_gradient(Kinect.Body body)
    {
      Kinect.Joint sjt1 = body.Joints[Kinect.JointType.HandRight];
      Kinect.Joint sjt2 = body.Joints[Kinect.JointType.HandLeft];
        //get the position of hands
      Vector3 righthand = GetVector3FromJoint(sjt1);
      Vector3 lefthand = GetVector3FromJoint(sjt2);
        //create a new vector contain 2D position of lefthand and righthand
      Vector4 positions = new Vector4(lefthand.x, lefthand.y, righthand.x, righthand.y);
        //gradient = (y1-y2)/(x1-x2)
      double grad = (positions.w-positions.y)/(positions.z-positions.x);    
      return grad;  //returning the value of the gradient 
    }

    //the function used to calculate the length between hands
    private double Get_length(Kinect.Body body)
    {
        Kinect.Joint sjt1 = body.Joints[Kinect.JointType.HandRight];
        Kinect.Joint sjt2 = body.Joints[Kinect.JointType.HandLeft];
            //get the position of hands
        Vector3 righthand = GetVector3FromJoint(sjt1);
        Vector3 lefthand = GetVector3FromJoint(sjt2);
        //create a new vector contain 2D position of lefthand and righthand
        Vector4 positions = new Vector4(lefthand.x, lefthand.y, righthand.x, righthand.y);
        //magnitude= square root of ((y2-y1)^2+(x2-x1)^2)
        double length = Mathf.Sqrt((positions.w - positions.y) * (positions.w - positions.y) + (positions.z - positions.x) * (positions.z - positions.x));
        return length;  //returning the value of the length 
    }
}
