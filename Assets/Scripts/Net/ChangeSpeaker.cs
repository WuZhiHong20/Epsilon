using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeSpeaker : MonoBehaviour
{
    private Button btm;
    private Text text;
    private int speaker_id;

    private void Start()
    {
        btm = GetComponent<Button>();
        btm.onClick.AddListener(ChooseSpeaker);
        text = gameObject.GetComponentInChildren<Text>();
        string name = gameObject.transform.name;
        speaker_id = int.Parse(name.Split(' ')[1]);
    }

    public void ChooseSpeaker()
    {
        //Debug.Log(text.text + " " + speaker_id.ToString());
        SendInfo.Speaker = speaker_id;
    }
}
