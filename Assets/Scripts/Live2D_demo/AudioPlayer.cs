using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    public TMP_InputField inputField;

    public Action controMouth;

    public MouthControl mouthControl;

    public btm_ContinueAsk continueAsk;

    public Queue<AudioClip> audioQuene = new Queue<AudioClip>();
    public Queue<string> answers = new Queue<string>();

    private AudioSource audioSource;

    public Enums.Speaker speaker; 
    // Start is called before the first frame update
    void Start()
    {
        audioCount = 0;
        hasPlayCount = 0;
        controMouth += mouthControl.StartMoveMouth;
        audioSource = GetComponent<AudioSource>();
        speaker = Enums.Speaker.ME;
        inputField.text = "Œ“:";
    }

    public void RecieveAudio(AudioClip audioClip,string answerStr)
    {
        audioQuene.Enqueue(audioClip);
        answers.Enqueue(answerStr);

        if(inputField.interactable == true)
        {
            inputField.interactable = false;
        }

        if (audioSource.isPlaying == false)
        {
            StartCoroutine(PlayAudio());
        }
    }

    private int audioCount;
    private int hasPlayCount;

    public void AddAudioCount()
    {
        ++audioCount;
    }

    public bool CheckIsPlaying()
    {
        return audioSource.isPlaying;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator ShowAnswer(string answer,float time)
    {
        answer = "÷˙ ÷:" + answer;
        float charsEverySec = answer.Length / time;
        float curTime = Time.deltaTime;
        //int lastPos = 0;
        //while (curTime < time)
        //{
        //    int curLen = (int)(curTime * charsEverySec);
        //    if (curLen > answer.Length) break;
        //    int deltaStr = curLen - lastPos;
        //    inputField.text += answer.Substring(lastPos, deltaStr);
        //    curTime += Time.deltaTime;
        //    lastPos = curLen;
        //    yield return null;
        //}
        //inputField.text += (answer.Substring(lastPos) + "\n");


        while (curTime < time)
        {
            int curLen = (int)(curTime * charsEverySec);
            inputField.text = answer.Substring(0, curLen);
            curTime += Time.deltaTime;
            yield return null;
        }
        inputField.text = answer;

        SaveToLocal.AnswerSaveToLocal(answer);

        ++hasPlayCount;
        if(hasPlayCount == audioCount)
        {
            SaveToLocal.EndAnswer();
            continueAsk.gameObject.SetActive(true);
            hasPlayCount = 0;
            audioCount = 0;
        }
        
    }

    private IEnumerator PlayAudio()
    {
        
        while (audioQuene.Count > 0)
        {
            audioSource.clip = audioQuene.Dequeue();
            
            audioSource.Play();
            controMouth.Invoke();
            //Debug.Log(audioQuene.Count);
            StartCoroutine(ShowAnswer(answers.Dequeue(), audioSource.clip.length));
            yield return new WaitForSeconds(audioSource.clip.length);

        }
    }
}
