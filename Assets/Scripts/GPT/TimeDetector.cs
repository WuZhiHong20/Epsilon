using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeDetector : MonoBehaviour
{
    private static TimeDetector timeDetector;

    public static TimeDetector GetInstance()
    {
        if(timeDetector == null)
        {
            GameObject gObject = new GameObject("TimeDetector");
            gObject.AddComponent<TimeDetector>();
            Instantiate(gObject);
            DontDestroyOnLoad(gObject);
        }
        return timeDetector;
    }

    //����ʱ�䣬û��˵����ʱ��,minute
    Int16 freeTime;

    public btm_ContinueAsk continueAsk;

    public AudioPlayer audioPlayer;

    private Action<AudioClip,string> sendAudio;

    private string date;

    //ÿ����֪ͨһ������������+1�� ���� ϵͳʱ�䵽�����ˣ�Ȼ������
    float minuteTrace;
    // Start is called before the first frame update

    private void Awake()
    {
        if(timeDetector == null)
        {
            timeDetector = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        date = System.DateTime.Now.ToString().Substring(0, 10);
        date = date.Replace("/", "_");
        date = date.Replace(" ", "");

        freeSecInAssState = 0f;
        freeTime = 0;
        minuteTrace = 0;
        sendAudio += audioPlayer.RecieveAudio;
    }

    public string GetDateTime()
    {
        return date;
    }

    void UpdateFreeTime()
    {
        ++freeTime;
        if(freeTime == 2)
        {
            freeTime = 0;
        }
    }

    void Refresh()
    {
        UpdateFreeTime();
    }

    private float freeSecInAssState;

    // Update is called once per frame
    void Update()
    {
        CheckAudioPlayerSpeaker();
    }

    private void CheckIsSpeaking()
    {
        if (audioPlayer.CheckIsPlaying() == false)
        {
            freeSecInAssState += Time.deltaTime;
        }
        else
        {
            freeSecInAssState = 0;
        }
        if(freeSecInAssState > 5f)
        {
            continueAsk.gameObject.SetActive(true);
        }
    }

    private void CheckAudioPlayerSpeaker()
    {
        if(audioPlayer.speaker == Enums.Speaker.ME)
        {
            minuteTrace += Time.deltaTime;
            if (minuteTrace > 60f)
            {
                minuteTrace = 0;
                Refresh();
            }
        }
        //else
        //{
        //    CheckIsSpeaking();
        //}
    }
}
