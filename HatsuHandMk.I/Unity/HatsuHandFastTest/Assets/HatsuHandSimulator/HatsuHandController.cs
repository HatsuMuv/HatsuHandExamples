using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HatsuHandController : MonoBehaviour
{
    [SerializeField]
    private bool sendToMotor = false;

    [SerializeField]
    private PololuConnecter pololuConnecter;

    #region UI
    private Toggle sendToMotorToggle;

    private Slider[] fingerControlSliders = new Slider[5];

    private Button[] fingerControlButtons = new Button[5];
    #endregion

    #region Fingers
    private string[] fingerJointsObjectName
        = new string[5] {"f_index","f_middle","f_ring" ,"f_pinky","thumb"};

    private GameObject[,] fingerJoints = new GameObject[5,3];

    private Transform[,] fingerJointsTransforms = new Transform[5,3];

    private Quaternion[,] fingerJointsInitialQuaternion = new Quaternion[5,3];

    private Vector3[,] fingerJointsRotationAxis = new Vector3[5, 3];
    #endregion

    #region const
    private const int MIN_LIMIT = 0;

    private const int MAX_LIMIT = 1;

    private const float SIMULATION_FINGER_MOVESPEED = 0.7f;
    #endregion


    private void Start()
    {
        if (pololuConnecter == null) Debug.Log("no HatsuHandController");
        //fingerControlToggle.isOn = fingerControl;
        //fingerControlToggle.onValueChanged.AddListener(OnToggleChange);

        InitializeUI();
        InitializeFingerObjects();


    }

    void Update()
    {
        if (sendToMotor)
        {
            for (int i = 0; i < 5; i++)
            {
                pololuConnecter.MoveFingerWithParam(i, fingerControlSliders[i].value);
            }
        }
    }

    private void OnToggleChange(bool isEnable)
    {
        sendToMotor = isEnable;
    }

    private void Exit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;//ゲームプレイ終了
        #else
            Application.Quit();//ゲームプレイ終了
        #endif

    }

    #region HandControlFunctions

    private void MoveFinger(int fingerNum, float param)
    {
        for (int i = 0; i < 3; i++)
        {
            Debug.Log("MoveFinger" + fingerNum + " " + i + " " + param);
            fingerJointsTransforms[fingerNum, i].rotation
                = Quaternion.AngleAxis(param * 70 * (i + 1), fingerJointsRotationAxis[fingerNum, i]) * fingerJointsInitialQuaternion[fingerNum, i];
        }
    }

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
        sendToMotorToggle = GameObject.Find("SendToMotorToggle").GetComponent<Toggle>();
        sendToMotorToggle.isOn = sendToMotor;
        sendToMotorToggle.onValueChanged.AddListener(OnToggleChange);

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

        fingerControlButtons[4] = GameObject.Find("ExitButton").GetComponent<Button>();
        fingerControlButtons[4].onClick.AddListener(() =>
        {
            Exit();
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
