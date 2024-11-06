using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class DoSomething : MonoBehaviour
{
    //先ほど作成したクラス
    public SerialHandler serialHandler;
    int i = 0;
    int j = 0;

    public Slider testSlider;
    public Text testText;

  void Start()
    {
        //信号を受信したときに、そのメッセージの処理を行う
        serialHandler.OnDataReceived += OnDataReceived;
    }

    /*void FixedUpdate() //ここは0.02秒ごとに実行される
    {
        i = i + 1;   //iを加算していって1秒ごとに"1"のシリアル送信を実行
        if (i > 49) //
        {
            if (j == 0)
            {
                serialHandler.Write("1");
                j = 1;
            }
            else if(j==1)
            {
                serialHandler.Write("2");
                j = 2;
            }
            else
            {
                serialHandler.Write("3");
                j = 0;
            }
            i = 0;
        }
    }*/

    public void SendSerialTest()
    {
        StartCoroutine(SendSerial());
    }

    IEnumerator SendSerial()
    {
        serialHandler.Write("1");
        serialHandler.Write(testSlider.value.ToString());
        serialHandler.Write("\n");
        yield return new WaitForSeconds(0.5f);
        serialHandler.Write("255");
        yield break;
    }

    private void OnApplicationQuit()
    {
        serialHandler.Write("0");
        serialHandler.Close();
        Debug.Log("QUIT");
    }

    //受信した信号(message)に対する処理
    void OnDataReceived(string message)
    {
        var data = message.Split(
                new string[] { "\t" }, System.StringSplitOptions.None);
        if (data.Length < 2) return;

        try
        {
            
        }
        catch (System.Exception e)
        {
            Debug.LogWarning(e.Message);
        }
    }

    public void ChangeText()
    {
        testText.text = testSlider.value.ToString();
    }
}
