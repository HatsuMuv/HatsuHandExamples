using Pololu.Usc;

using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
using Pololu.UsbWrapper;

public class HatsuHandControlAndGUISystem : MonoBehaviour
{

    //[SerializeField]
    //private Transform[] LeapLeftFingerObjects = new Transform[5];

    //[SerializeField]
    //private Transform[] LeapRightFingerObjects = new Transform[5];

    //public int rightGoal;
    //public int leftGoal;

    //[SerializeField]
    //private Transform[] HatsuHandRightFingerObjects = new Transform[5];

    //[SerializeField]
    //private Transform[] HatsuHandLeftFingerObjects = new Transform[5];

    [SerializeField]
    private PololuConnecter pololuConnecter;

    [SerializeField]
    private bool sendBindingToHatsuHand;

    [SerializeField]
    private List<string> PololuDeviceSerialNumberList = new List<string>();

    [Header("LeapMotionHand")]
    [SerializeField] private HandRig LeapHandRight = new HandRig();
    [SerializeField] private HandRig LeapHandLeft = new HandRig();
    [SerializeField] private HandRig LeapHandRightRoot = new HandRig();
    [SerializeField] private HandRig LeapHandLeftRoot = new HandRig();



    [Header("HatsuHand")]
    [SerializeField] private HandRig HatsuHandRight = new HandRig();
    [SerializeField] private HandRig HatsuHandLeft = new HandRig();

  
    public HatsuHandFingersBendValues RightHatsuHandFingersBendValues = new HatsuHandFingersBendValues();
    public HatsuHandFingersBendValues LeftHatsuHandFingersBendValues = new HatsuHandFingersBendValues();


    private GameObject[,] RightFingerJoints = new GameObject[5, 3];
    private GameObject[,] LeftFingerJoints = new GameObject[5, 3];

    private Transform[,] InitialRightFingerTransform = new Transform[5,3];
    private Transform[,] InitialLeftFingerTransform = new Transform[5,3];
    private Vector3[,] RightFingerJointsRotationAxis = new Vector3[5, 3];
    private Vector3[,] LeftFingerJointsRotationAxis = new Vector3[5, 3];

    private int currentPololuDeviceIndex = 0;

    private const float THUMB_BINDING_OFFSET = 60.0f;

    private float FINGER_MIN_DEGREE = -360.0f;
    private float FINGER_MAX_DEGREE = 360.0f;

    private float FINGER_MIN_DEGREE_FORBINDING = -20.0f;
    private float FINGER_MAX_DEGREE_FORBINDING = 60.0f;

    private float FINGER_MIN_MOTOR_VALUE = 4000.0f;
    private float FINGER_MAX_MOTOR_VALUE = 8000.0f;


    void Start()
    {
        InitializeHatsuHandCG();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            sendBindingToHatsuHand = !sendBindingToHatsuHand;
        }

        if(Input.GetKeyDown(KeyCode.F1))
        {
            currentPololuDeviceIndex++;
            if(currentPololuDeviceIndex >= PololuDeviceSerialNumberList.Count)
            {
                currentPololuDeviceIndex = 0;
            }
        }

        UpdateList();

        // Calculate Fingers Bending from Leap Motion 

        // Right Hand
        RightHatsuHandFingersBendValues.Thumb = THUMB_BINDING_OFFSET + Mathf.Clamp(CalculateBendValue(LeapHandRightRoot.Thumb.transform, LeapHandRight.Thumb.transform,RotationAxis.y), FINGER_MIN_DEGREE, FINGER_MAX_DEGREE);
        RightHatsuHandFingersBendValues.Index = Mathf.Clamp(CalculateBendValue(LeapHandRightRoot.Index.transform, LeapHandRight.Index.transform, RotationAxis.x), FINGER_MIN_DEGREE, FINGER_MAX_DEGREE);
        RightHatsuHandFingersBendValues.Middle = Mathf.Clamp(CalculateBendValue(LeapHandRightRoot.Middle.transform, LeapHandRight.Middle.transform, RotationAxis.x), FINGER_MIN_DEGREE, FINGER_MAX_DEGREE);
        RightHatsuHandFingersBendValues.Ring = Mathf.Clamp(CalculateBendValue(LeapHandRightRoot.Ring.transform, LeapHandRight.Ring.transform, RotationAxis.x), FINGER_MIN_DEGREE, FINGER_MAX_DEGREE);
        RightHatsuHandFingersBendValues.Pinky = Mathf.Clamp(CalculateBendValue(LeapHandRightRoot.Pinky.transform, LeapHandRight.Pinky.transform, RotationAxis.x), FINGER_MIN_DEGREE, FINGER_MAX_DEGREE);


        // Left Hand
        LeftHatsuHandFingersBendValues.Thumb = THUMB_BINDING_OFFSET - Mathf.Clamp(CalculateBendValue(LeapHandLeftRoot.Thumb.transform, LeapHandLeft.Thumb.transform, RotationAxis.y), FINGER_MIN_DEGREE, FINGER_MAX_DEGREE);
        LeftHatsuHandFingersBendValues.Index = Mathf.Clamp(CalculateBendValue(LeapHandLeftRoot.Index.transform, LeapHandLeft.Index.transform, RotationAxis.x), FINGER_MIN_DEGREE, FINGER_MAX_DEGREE);
        LeftHatsuHandFingersBendValues.Middle = Mathf.Clamp(CalculateBendValue(LeapHandLeftRoot.Middle.transform, LeapHandLeft.Middle.transform, RotationAxis.x), FINGER_MIN_DEGREE, FINGER_MAX_DEGREE);
        LeftHatsuHandFingersBendValues.Ring = Mathf.Clamp(CalculateBendValue(LeapHandLeftRoot.Ring.transform, LeapHandLeft.Ring.transform, RotationAxis.x), FINGER_MIN_DEGREE, FINGER_MAX_DEGREE);
        LeftHatsuHandFingersBendValues.Pinky = Mathf.Clamp(CalculateBendValue(LeapHandLeftRoot.Pinky.transform, LeapHandLeft.Pinky.transform, RotationAxis.x), FINGER_MIN_DEGREE, FINGER_MAX_DEGREE);


        // Send Bending to HatsuHand
        if (sendBindingToHatsuHand && PololuDeviceSerialNumberList.Count != 0)
        {
            bool successSetTarget = true;
            pololuConnecter.TrySetTarget((Byte)0, (UInt16)Remap(RightHatsuHandFingersBendValues.Index, FINGER_MIN_DEGREE_FORBINDING, FINGER_MAX_DEGREE_FORBINDING, FINGER_MIN_MOTOR_VALUE, FINGER_MAX_MOTOR_VALUE),
                PololuDeviceSerialNumberList[currentPololuDeviceIndex]);
            pololuConnecter.TrySetTarget((Byte)1, (UInt16)Remap(RightHatsuHandFingersBendValues.Middle, FINGER_MIN_DEGREE_FORBINDING, FINGER_MAX_DEGREE_FORBINDING, FINGER_MIN_MOTOR_VALUE, FINGER_MAX_MOTOR_VALUE),
                PololuDeviceSerialNumberList[currentPololuDeviceIndex]);
            pololuConnecter.TrySetTarget((Byte)2, (UInt16)Remap(RightHatsuHandFingersBendValues.Ring, FINGER_MIN_DEGREE_FORBINDING, FINGER_MAX_DEGREE_FORBINDING, FINGER_MIN_MOTOR_VALUE, FINGER_MAX_MOTOR_VALUE),
                PololuDeviceSerialNumberList[currentPololuDeviceIndex]);
            pololuConnecter.TrySetTarget((Byte)3, (UInt16)Remap(RightHatsuHandFingersBendValues.Pinky, FINGER_MIN_DEGREE_FORBINDING, FINGER_MAX_DEGREE_FORBINDING, FINGER_MIN_MOTOR_VALUE, FINGER_MAX_MOTOR_VALUE),
                PololuDeviceSerialNumberList[currentPololuDeviceIndex]);
            pololuConnecter.TrySetTarget((Byte)4, (UInt16)Remap(RightHatsuHandFingersBendValues.Thumb, FINGER_MIN_DEGREE_FORBINDING, FINGER_MAX_DEGREE_FORBINDING, FINGER_MIN_MOTOR_VALUE, FINGER_MAX_MOTOR_VALUE),
                PololuDeviceSerialNumberList[currentPololuDeviceIndex]);
            if(!successSetTarget) Debug.Log("Failed to set target");

        }

        //SyncHand();

    }

    private float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    private float CalculateBendValue(Transform palm, Transform fingerRoot, RotationAxis axis)
    {
        Quaternion rotationDifference = Quaternion.Inverse(palm.rotation) * fingerRoot.rotation;

        Vector3 euler = rotationDifference.eulerAngles;

        switch (axis)
        {
            case RotationAxis.x:
                return NormalizeAngle(euler.x);
            case RotationAxis.y:
                return NormalizeAngle(euler.y);
            case RotationAxis.z:
                return NormalizeAngle(euler.z);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }


    private float NormalizeAngle(float angle)
    {
        if (angle > 180.0f)
        {
            angle -= 360.0f;
        }
        return angle;
    }


    private void InitializeHatsuHandCG() // CG and Robot
    {

        // Get child objects of HatsuHand each finger which are joints of finger
        for (int i = 0; i < 5; i++)
        {
            Transform fingerTransform = HatsuHandRight.GetFingerGameObject((Finger)i).transform;
            //Debug.Log($"{fingerTransform.name} child count: {fingerTransform.childCount}");
            RightFingerJoints[i, 0] = fingerTransform.gameObject; // Root joint
            RightFingerJoints[i, 1] = fingerTransform.GetChild(0).gameObject; // First joint
            RightFingerJoints[i, 2] = fingerTransform.GetChild(0).GetChild(0).gameObject; // Second joint (grandchild)
            Debug.Log(RightFingerJoints[i, 0].name + " " + RightFingerJoints[i, 1].name + " " + RightFingerJoints[i, 2].name);
            
            fingerTransform = HatsuHandLeft.GetFingerGameObject((Finger)i).transform;

            
            LeftFingerJoints[i, 0] = fingerTransform.gameObject; // Root joint
            LeftFingerJoints[i, 1] = fingerTransform.GetChild(0).gameObject; // First joint
            LeftFingerJoints[i, 2] = fingerTransform.GetChild(0).GetChild(0).gameObject; // Second joint (grandchild)

            Debug.Log(LeftFingerJoints[i, 0].name + " " + LeftFingerJoints[i, 1].name + " " + LeftFingerJoints[i, 2].name);

        }

        // Get copy Initial position and rotation of each finger's joint
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                //InitalRightFingerTransform[i, j].position = RightFingerJoints[i, j].transform.position;
                //InitalRightFingerTransform[i, j].rotation = RightFingerJoints[i, j].transform.rotation;
                //InitalLeftFingerTransform[i, j].position = LeftFingerJoints[i, j].transform.position;
                //InitalLeftFingerTransform[i, j].rotation = LeftFingerJoints[i, j].transform.rotation;
                InitialRightFingerTransform[i, j] = RightFingerJoints[i, j].transform;
                InitialLeftFingerTransform[i, j] = LeftFingerJoints[i, j].transform;

                if (i == 0)
                {
                    RightFingerJointsRotationAxis[i, j] = InitialRightFingerTransform[i, j].forward;
                    LeftFingerJointsRotationAxis[i, j] = InitialLeftFingerTransform[i, j].forward;
                }
                else
                {
                    RightFingerJointsRotationAxis[i, j] = -InitialRightFingerTransform[i, j].right;
                    LeftFingerJointsRotationAxis[i, j] = -InitialLeftFingerTransform[i, j].right;
                }
            }
        }

    }

    private void UpdateList()
    {
        List<DeviceListItem> connectedDevices = Usc.getConnectedDevices();

        if(connectedDevices.Count != PololuDeviceSerialNumberList.Count)
        {
            currentPololuDeviceIndex = 0;
            PololuDeviceSerialNumberList = new List<string>();
            Debug.Log("Device List Updated");
            foreach(DeviceListItem dli in connectedDevices)
            {
                PololuDeviceSerialNumberList.Add(dli.serialNumber);
            }
        }
    }

    void OnGUI()
    {
        GUIStyle greenStyle = new GUIStyle();
        GUIStyle redStyle = new GUIStyle();
        greenStyle.fontSize = 60; 
        greenStyle.normal.textColor = Color.green;
        redStyle.fontSize = 60; 
        redStyle.normal.textColor = Color.red; 



        (string,GUIStyle) PololuMessage = ("Pololu: Online",greenStyle);
        if(PololuDeviceSerialNumberList.Count == 0 || pololuConnecter.ConnectResult(PololuDeviceSerialNumberList[currentPololuDeviceIndex]) == false)
        {
            PololuMessage = ("Pololu: Offline",redStyle);
        }

        string CurrentDeviceString = "";
        if (PololuDeviceSerialNumberList.Count == 0) CurrentDeviceString = "None";
        (string, GUIStyle) CurrentPololuDevice = ("Current Pololu Device: " + CurrentDeviceString, greenStyle);
        if(PololuDeviceSerialNumberList.Count == 0)
        {
            CurrentPololuDevice = ("Current Pololu Device: None", redStyle);
        }

        (string, GUIStyle) SendingData = ("Send Data: True" , greenStyle);
        if(sendBindingToHatsuHand == false)
        {
            SendingData = ("Send Data: False", redStyle);
        }


        (string, GUIStyle) PololuDeviceList = ("Pololu Device List: \n", greenStyle);
        if (PololuDeviceSerialNumberList.Count == 0)
        {
            PololuDeviceList = ("Pololu Device List: None", redStyle);
        }
        else
        {
            foreach (string s in PololuDeviceSerialNumberList)
            {
                PololuDeviceList.Item1 += s + "\n";
            }
        }

        GUI.Label(new Rect(10, 10, 300, 50), PololuMessage.Item1, PololuMessage.Item2);
        GUI.Label(new Rect(10, 80, 300, 50), SendingData.Item1, SendingData.Item2);
        GUI.Label(new Rect(10, 150, 300, 50), CurrentPololuDevice.Item1, CurrentPololuDevice.Item2);
        GUI.Label(new Rect(10, 220, 300, 50), PololuDeviceList.Item1, PololuDeviceList.Item2);

        GUI.Label(new Rect(10, 900, 300, 50), "F1: Change current Pololu Device", greenStyle);
        GUI.Label(new Rect(10, 960, 300, 50), "Space: Send Data True/False", greenStyle);
    }


    [System.Serializable]
    public class HandRig
    {
        public GameObject Thumb;
        public GameObject Index;
        public GameObject Middle;
        public GameObject Ring;
        public GameObject Pinky;
        public GameObject GetFingerGameObject(Finger finger)
        {
            switch (finger)
            {
                case Finger.Thumb: return Thumb;
                case Finger.Index: return Index;
                case Finger.Middle: return Middle;
                case Finger.Ring: return Ring;
                case Finger.Pinky: return Pinky;
                default: throw new ArgumentOutOfRangeException();
            }
        }
    }

    [System.Serializable]
    public class HatsuHandFingersBendValues
    {
        public float Thumb;
        public float Index;
        public float Middle;
        public float Ring;
        public float Pinky;

        public float this[int i]
        {
            get
            {
                switch (i)
                {
                    case 0: return Thumb;
                    case 1: return Index;
                    case 2: return Middle;
                    case 3: return Ring;
                    case 4: return Pinky;
                    default:
                        throw new IndexOutOfRangeException("Index must be between 0 and 4.");
                }
            }
            set
            {
                switch (i)
                {
                    case 0: Thumb = value; break;
                    case 1: Index = value; break;
                    case 2: Middle = value; break;
                    case 3: Ring = value; break;
                    case 4: Pinky = value; break;
                    default:
                        throw new IndexOutOfRangeException("Index must be between 0 and 4.");
                }
            }
        }
    }

    enum RotationAxis
    {
        x,
        y,
        z
    }
    public enum Finger
    {
        Thumb,
        Index,
        Middle,
        Ring,
        Pinky
    }

}
