using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetTime : MonoBehaviour
{
    private int hour;
    private int minute;
    private int second;
    private int millisecond;

    //已经花费的时间
    float timeSpeed = 0.0f;

    //显示时间区域的文本
    Text text_timeSpeed;

    // Start is called before the first frame update
    void Start()
    {
        text_timeSpeed = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        timeSpeed += Time.deltaTime;
        //hour = (int)timeSpeed / 3600;
        minute = ((int)timeSpeed - hour * 3600) / 60;
        second = (int)timeSpeed - hour * 3600 - minute * 60;
        //text_timeSpeed.text = string.Format("{0:D2}:{1:D2}",  minute, second);
        millisecond = (int)((timeSpeed - (int)timeSpeed) * 1000);
        text_timeSpeed.text = string.Format("{0:D2}:{1:D2}.{2:D2}",  minute, second, millisecond);

    }
}
