using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Pololu.UsbWrapper;
using Pololu.Usc;

public class HatsuHandController : MonoBehaviour
{
    [SerializeField]
    private Toggle sendToMotorToggle;

    [SerializeField]
    private bool sendToMotor = false;

    [SerializeField]
    private Slider[] fingerControlSliders = new Slider[5];

    [SerializeField]
    private Button[] fingerControlButtons = new Button[5];

    [SerializeField]
    private PololuConnecter pololuConnecter;

    private const int MIN_LIMIT = 0;

    private const int MAX_LIMIT = 1;

    private const float SIMULATION_FINGER_MOVESPEED = 0.7f;

    private int currentPololuDeviceIndex = 0;

    [SerializeField]
    private List<string> PololuDeviceSerialNumberList = new List<string>();


    private string[] fingerJointsObjectName
        = new string[5] {"f_index","f_middle","f_ring" ,"f_pinky","thumb"};

    private GameObject[,] fingerJoints = new GameObject[5,3];

    private Transform[,] fingerJointsTransforms = new Transform[5,3];

    private Quaternion[,] fingerJointsInitialQuaternion = new Quaternion[5,3];

    private Vector3[,] fingerJointsRotationAxis = new Vector3[5, 3]; 


    private void Start()
    {
        if (pololuConnecter == null) Debug.Log("no HatsuHandController");
        sendToMotorToggle.isOn = sendToMotor;
        sendToMotorToggle.onValueChanged.AddListener(OnToggleChange);

        InitializeUI();
        InitializeFingerObjects();


    }

    void Update()
    {
        UpdateList();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            sendToMotor = !sendToMotor;
        }

        if (Input.GetKeyDown(KeyCode.F1))
        {
            currentPololuDeviceIndex++;
            if (currentPololuDeviceIndex >= PololuDeviceSerialNumberList.Count)
            {
                currentPololuDeviceIndex = 0;
            }
        }

        if (sendToMotor)
        {
            for (int i = 0; i < 5; i++)
            {
                pololuConnecter.MoveFingerWithParam(i, fingerControlSliders[i].value, PololuDeviceSerialNumberList[currentPololuDeviceIndex]);
            }
        }
    }


    private void UpdateList()
    {
        List<DeviceListItem> connectedDevices = Usc.getConnectedDevices();

        if (connectedDevices.Count != PololuDeviceSerialNumberList.Count)
        {
            currentPololuDeviceIndex = 0;
            PololuDeviceSerialNumberList = new List<string>();
            Debug.Log("Device List Updated");
            foreach (DeviceListItem dli in connectedDevices)
            {
                PololuDeviceSerialNumberList.Add(dli.serialNumber);
            }
        }
    }


    void OnGUI()
    {
        // フォントサイズやスタイルを設定する
        GUIStyle greenStyle = new GUIStyle();
        GUIStyle redStyle = new GUIStyle();
        greenStyle.fontSize = 60; // フォントサイズを設定
        greenStyle.normal.textColor = Color.green; // 文字の色を設定
        redStyle.fontSize = 60; // フォントサイズを設定
        redStyle.normal.textColor = Color.red; // 文字の色を設定
        // 画面に文字を描画する（位置はx=10, y=10に設定）


        (string, GUIStyle) PololuMessage = ("Pololu: Online", greenStyle);
        if (PololuDeviceSerialNumberList.Count == 0 || pololuConnecter.ConnectResult(PololuDeviceSerialNumberList[currentPololuDeviceIndex]) == false)
        {
            PololuMessage = ("Pololu: Offline", redStyle);
        }

        (string, GUIStyle) CurrentPololuDevice = ("Current Pololu Device: " + PololuDeviceSerialNumberList[currentPololuDeviceIndex], greenStyle);
        if (PololuDeviceSerialNumberList.Count == 0)
        {
            CurrentPololuDevice = ("Current Pololu Device: None", redStyle);
        }

        (string, GUIStyle) SendingData = ("Send Data: True", greenStyle);
        if (sendToMotor == false)
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

    private void MoveFinger(int fingerNum, float param)
    {
        for (int i = 0; i < 3; i++)
        {
            Debug.Log("MoveFinger" + fingerNum + " " + i + " " + param);    
            fingerJointsTransforms[fingerNum, i].rotation 
                = Quaternion.AngleAxis(param * 70 * (i + 1), fingerJointsRotationAxis[fingerNum, i]) * fingerJointsInitialQuaternion[fingerNum, i];
        }
        //fingerControlSliders[fingerNum].value = param;
    }

    private void OnToggleChange(bool isEnable)
    {
        sendToMotor = isEnable;
        Debug.Log(isEnable);
    }

    #region HandControlFunctions
    private IEnumerator OpenHand()
    {
        for (int i = 4; i >= 0; i--)
        {
            fingerControlSliders[i].DOValue(MIN_LIMIT, SIMULATION_FINGER_MOVESPEED);
            if (i == 4)
            {
                yield return new WaitForSeconds(0.5f);
            }
        }
    }

    private IEnumerator CloseHand()
    {
        for (int i = 0; i < 5; i++)
        {
            fingerControlSliders[i].DOValue(MAX_LIMIT, SIMULATION_FINGER_MOVESPEED);
            //MoveFinger(i, MAX_LIMIT);
            if (i == 3)
            {
                yield return new WaitForSeconds(0.5f);
            }
        }
    }

    private IEnumerator FingerWave()
    {
        float delay = 0.3f;

        for (int i = 0; i < 3; i++)
        {
            for (int j=0; j<4;j++)
            {
                fingerControlSliders[j].DOValue(MAX_LIMIT, SIMULATION_FINGER_MOVESPEED);
                yield return new WaitForSeconds(delay);
            }

            for (int j = 0; j < 4; j++)
            {
                fingerControlSliders[j].DOValue(MIN_LIMIT, SIMULATION_FINGER_MOVESPEED);
                yield return new WaitForSeconds(delay);
            }

            yield return new WaitForSeconds(delay);
        }
    }

    private IEnumerator CountBinaryNumber()
    {
        List<string> binaryNumberList = new List<string>();
        for (int i = 1; i < 16; i++)
        {
            string binary = Convert.ToString(i, 2).PadLeft(4, '0');
            char[] reversedBinary = binary.ToCharArray();
            System.Array.Reverse(reversedBinary);
            binaryNumberList.Add(new string(reversedBinary));
        }

        foreach (string bin in binaryNumberList)
        {
            for (int i = 0; i < 4; i++)
            {
                //MoveFinger(i, bin[i] == '1' ? MIN_LIMIT : MAX_LIMIT);
                fingerControlSliders[i].DOValue(bin[i] == '1' ? MIN_LIMIT : MAX_LIMIT, SIMULATION_FINGER_MOVESPEED);
            }
            yield return new WaitForSeconds(1.5f);
        }
    }
    #endregion

    #region Initialize
    private void InitializeUI()
    {
        fingerControlSliders[0] = GameObject.Find("IndexSlider").GetComponent<Slider>();
        fingerControlSliders[1] = GameObject.Find("MiddleSlider").GetComponent<Slider>();
        fingerControlSliders[2] = GameObject.Find("RingSlider").GetComponent<Slider>();
        fingerControlSliders[3] = GameObject.Find("PinkySlider").GetComponent<Slider>();
        fingerControlSliders[4] = GameObject.Find("ThumbSlider").GetComponent<Slider>();

        Debug.Log(fingerControlSliders[0].name);

        for(int i=0;i < 5; i++)
        {
            int _i = i;
            fingerControlSliders[_i].onValueChanged.AddListener((float value) =>
            {
                MoveFinger(_i, value);
            });
        }

        fingerControlButtons[0] = GameObject.Find("OpenHandButton").GetComponent<Button>();
        fingerControlButtons[0].onClick.AddListener(() =>
        {
            StartCoroutine(OpenHand());
        });

        fingerControlButtons[1] = GameObject.Find("CloseHandButton").GetComponent<Button>();
        fingerControlButtons[1].onClick.AddListener(() =>
        {
            StartCoroutine(CloseHand());
        });

        fingerControlButtons[2] = GameObject.Find("FingerWaveButton").GetComponent<Button>();
        fingerControlButtons[2].onClick.AddListener(() =>
        {
            StartCoroutine(FingerWave());
        });

        fingerControlButtons[3] = GameObject.Find("CountBinaryNumberButton").GetComponent<Button>();
        fingerControlButtons[3].onClick.AddListener(() =>
        {
            StartCoroutine(CountBinaryNumber());
        });
    }
    private void InitializeFingerObjects()
    {
        for(int i=0;i<5;i++)
        {
            for(int j=0;j<3;j++)
            {
                fingerJoints[i,j] = GameObject.Find(fingerJointsObjectName[i] + ".0" + (j+1) + ".R");
            }
        }

        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                fingerJointsTransforms[i, j] = fingerJoints[i, j].transform;
                fingerJointsInitialQuaternion[i, j] = fingerJointsTransforms[i, j].rotation;
                if(i == 4) fingerJointsRotationAxis[i, j] = fingerJointsTransforms[i, j].forward;
                else fingerJointsRotationAxis[i, j] = -fingerJointsTransforms[i, j].right;
            }
        }
    }
    #endregion
}
