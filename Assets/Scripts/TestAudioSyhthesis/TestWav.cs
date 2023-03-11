using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WWUtils.Audio
{
    public class TestWav : MonoBehaviour
    {
        AudioSource audioSource;
        // Start is called before the first frame update
        void Start()
        {
            WAV wav = new WAV(@"D:\ChatWife\EpsilonAudio\EpsilonAudioLibrary\2023_2_23\0_计算机组成原理中，外.wav");
            Debug.Log(wav);
            AudioClip audioClip = AudioClip.Create("testSound", wav.SampleCount, 1, wav.Frequency, false);
            audioClip.SetData(wav.LeftChannel, 0);
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = audioClip;
            audioSource.Play();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}