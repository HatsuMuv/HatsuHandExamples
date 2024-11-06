using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class DoSomething : MonoBehaviour
{
    //��قǍ쐬�����N���X
    public SerialHandler serialHandler;
    int i = 0;
    int j = 0;

    public Slider testSlider;
    public Text testText;

  void Start()
    {
        //�M������M�����Ƃ��ɁA���̃��b�Z�[�W�̏������s��
        serialHandler.OnDataReceived += OnDataReceived;
    }

    /*void FixedUpdate() //������0.02�b���ƂɎ��s�����
    {
        i = i + 1;   //i�����Z���Ă�����1�b���Ƃ�"1"�̃V���A�����M�����s
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

    //��M�����M��(message)�ɑ΂��鏈��
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
