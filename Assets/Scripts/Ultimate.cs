using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ultimate : MonoBehaviour
{
    // Start is called before the first frame update
    float _startTime;
    void Start()
    {
        _startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - _startTime > GameController.instance.dashTime)
        {
            Destroy(gameObject);
        }
    }


}
