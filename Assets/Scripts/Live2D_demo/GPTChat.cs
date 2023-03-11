using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using AudioSynhthesis;
using TMPro;
using UnityEngine.Networking;

public class GPT
{
    public Action<string> sendOutputData;
    private Process process;
    public GPT()
    {
        try
        {
            process = new Process()
            {
                StartInfo = new ProcessStartInfo("cmd.exe")
                {
                    Arguments = "/k",
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }       
            };
            process.OutputDataReceived += ProcessOutputDataReceived;
            process.ErrorDataReceived += ProcessErrorDataReceived;
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
        }
        catch(Exception e)
        {
            UnityEngine.Debug.Log(e);
        }

    }

    private void ProcessErrorDataReceived(object sender, DataReceivedEventArgs e)
    {
        OutPutData(e.Data);
    }

    private void ProcessOutputDataReceived(object sender, DataReceivedEventArgs e)
    {
        OutPutData(e.Data);
    }

    private void OutPutData(string data)
    {
        sendOutputData?.Invoke(data);
    }

    public void Write(string data)
    {
        try
        {
            process.StandardInput.WriteLine(data);
        }
        catch(Exception e)
        {
            UnityEngine.Debug.Log(e);
        }
    }

    public void Write(byte[] data)
    {
        try
        {
            process.StandardInput.BaseStream.Write(data, 0, data.Length);
            process.StandardInput.BaseStream.WriteByte((byte)'\n');
            process.StandardInput.BaseStream.Flush();
        }
        catch (Exception e)
        {
            UnityEngine.Debug.Log(e);
        }
    }

    public void Quit()
    {
        Write("Exit");
        UnityEngine.Debug.Log("程序已安全退出");
    }
}

public class GPTChat : MonoBehaviour
{
    /// <summary>
    /// cmd 返回一个GBK string，是JSON的地址
    /// </summary>
    /// <param name="info"></param>
    public void GetOutPutInfo(string jsonPath)
    {
        UnityEngine.Debug.Log(jsonPath);
        if (jsonPath != null)
        {
            int len = jsonPath.Length;
            var style = jsonPath.Substring(len - 4);
            UnityEngine.Debug.Log(style);
            if (style == "JSON")
            {
                if (File.Exists(jsonPath))
                {
                    string jsonString = File.ReadAllText(jsonPath);
                    UnityEngine.Debug.Log(jsonString);
                    if (jsonString != null)
                    {
                        AnswerInfo answerInfo = JsonConvert.DeserializeObject<AnswerInfo>(jsonString);
                        UnityEngine.Debug.Log(answerInfo.AudioPath);
                        answerInfo.AudioPath = "file:///" + answerInfo.AudioPath.Replace("\\", "/");
                        readyPath.Enqueue(answerInfo);
                        audioPlayer.AddAudioCount();
                    }
                }
            }
        }
    }

    IEnumerator GetAudio(AnswerInfo answerInfo)
    {
        UnityEngine.Debug.Log("Getting");
        UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(answerInfo.AudioPath, AudioType.WAV);
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            UnityEngine.Debug.LogError(www.error);
        }
        else
        {
            if (DownloadHandlerAudioClip.GetContent(www) != null)
            {
                AudioClip audioClip = DownloadHandlerAudioClip.GetContent(www);
                // 播放音频
                UnityEngine.Debug.Log("success!");

                audioPlayer.RecieveAudio(audioClip,answerInfo.AnswerStr);
                if(audioPlayer.speaker == Enums.Speaker.ME)
                {
                    audioPlayer.speaker = Enums.Speaker.ASSISTANT;
                }
            }
        }
    }



    private string exePath = @"D:\ChatWife\EpsilonChat\AudioSynhthesis.exe";

    private GPT chatGPT;

    public TMP_InputField inputField;

    public AudioPlayer audioPlayer;

    private Queue<AnswerInfo> readyPath = new Queue<AnswerInfo>();

    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        UnityEngine.Debug.Log("miao");
        SaveToLocal.Init();
        chatGPT = new GPT();
        chatGPT.sendOutputData += GetOutPutInfo;
        //SendGBK(exePath);
        chatGPT.Write(exePath);
        animator.SetTrigger("Wink");

    }

    public void SendGBK(string str)
    {
        string input;
        if(str.Substring(0,2) == "我:")
        {
            input = str.Substring(2);
        }
        else
        {
            input = str;
        }
        UnityEngine.Debug.Log(input);

        SaveToLocal.InputSaveToLocal(str);

        byte[] inputCodes = System.Text.Encoding.GetEncoding("GBK").GetBytes(input);
        chatGPT.Write(inputCodes);
    }

    // Update is called once per frame
    void Update()
    {
        CheckReadyPath();
    }

    private void CheckReadyPath()
    {
        if(readyPath.Count > 0)
        {
            StartCoroutine(GetAudio(readyPath.Dequeue()));
        }
    }

    private void OnApplicationQuit()
    {
        chatGPT.Quit();
    }
}
