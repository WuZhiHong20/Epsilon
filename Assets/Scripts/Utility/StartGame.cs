using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGame : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        UIKeeper.Instance.OpenPanel("btm_Exit");
        UIKeeper.Instance.OpenPanel("btm_Send");
        UIKeeper.Instance.OpenPanel("chatWindow");
        UIKeeper.Instance.OpenPanel("btm_ChangeSpeaker");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
