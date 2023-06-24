using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TalkManager : MonoBehaviour
{
    private TMP_InputField inputField;
    public GPTChat chater;
    // Start is called before the first frame update
    void Start()
    {
        inputField = GetComponent<TMP_InputField>();
        inputField.onEndEdit.AddListener(chater.SendGBK);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
