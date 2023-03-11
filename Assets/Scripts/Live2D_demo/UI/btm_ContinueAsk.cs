using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class btm_ContinueAsk : MonoBehaviour
{
    private Button button;

    public AudioPlayer audioPlayer;
    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(ChangeSpeaker);
    }

    private void ChangeSpeaker()
    {
        audioPlayer.speaker = Enums.Speaker.ME;
        audioPlayer.inputField.text = "Œ“:";
        audioPlayer.inputField.interactable = true;
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
