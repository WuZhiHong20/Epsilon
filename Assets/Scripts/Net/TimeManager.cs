using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    int MeetTime;
    public Epsilon epsilon;

    // Start is called before the first frame update
    void Start()
    {
        //2023-06-13 19-28-25
        string dateTime = System.DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");
        MeetTime = int.Parse(dateTime.Substring(11, 2));
        Logger.Log("本次启动时间 ： " + dateTime);
        epsilon.Greating(MeetTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
