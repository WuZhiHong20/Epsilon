using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TestUWR : MonoBehaviour
{
    AudioSource audioSource;

    string audioPath = "File:///D:/ChatWife/EpsilonAudio/EpsilonAudioLibrary/2023_2_26/����һ����Ȥ������/0����һ����Ȥ������.wav";
    // Start is called before the first frame update
    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        StartCoroutine(GetAudio(audioPath));
    }

    IEnumerator GetAudio(string path)
    {
        UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.WAV);
        Debug.Log("Geting");
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(www.error);
        }
        else
        {
            Debug.Log("Success!");
            AudioClip audioClip = DownloadHandlerAudioClip.GetContent(www);
            // ������Ƶ

            audioSource.clip = audioClip;
            audioSource.Play();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
