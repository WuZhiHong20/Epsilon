using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestWAV : MonoBehaviour
{
    public AudioSource audio;

    private void Start()
    {
        WAV wav = new WAV(@"D:\ChatWife\_chatAssistant\Audios\2023_5_29\0\你好，我是牧濑红莉牺.wav");
        Debug.Log(wav);
        AudioClip audioClip = AudioClip.Create("testSound", wav.SampleCount, 1, wav.Frequency, false);
        audioClip.SetData(wav.LeftChannel, 0);
        audio.clip = audioClip;
        audio.Play();
    }
}
