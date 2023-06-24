using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeSpeaker : MonoBehaviour
{
    private Button btm;
    private Text text;
    private int speaker_id;
    private Text btm_changeSpeaker_text;
    private string name;

    private BasePanel Speakers;

    private void Start()
    {
        btm = GetComponent<Button>();
        btm.onClick.AddListener(ChooseSpeaker);
        text = gameObject.GetComponentInChildren<Text>();
        name = gameObject.transform.name;
        speaker_id = int.Parse(name.Split(' ')[1]);

        name = text.text;

        GameObject btm_ChangeSpeaker = GameObject.Find("btm_ChangeSpeaker");
        btm_changeSpeaker_text = btm_ChangeSpeaker.GetComponentInChildren<Text>();

        Speakers = GameObject.Find("Speakers").GetComponent<BasePanel>();
    }

    public void ChooseSpeaker()
    {
        //Debug.Log(text.text + " " + speaker_id.ToString());
        SendInfo.Speaker = speaker_id;
        btm_changeSpeaker_text.text = "当前说话人:\n" + name;
        Speakers.ClosePanel();
    }
}
