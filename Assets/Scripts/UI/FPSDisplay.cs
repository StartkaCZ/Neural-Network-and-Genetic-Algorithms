using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FPSDisplay : MonoBehaviour
{
    float   _deltaTime;
    float   _averageFPS;
    float   _minFPS;

    int     _counter;


    void Start()
    {
        _deltaTime = 0.0f;
        _averageFPS = 0;
        _counter = 0;
        _minFPS = 60;
    }


    void Update()
    {
        _deltaTime += (Time.deltaTime - _deltaTime) * 0.1f;

        if (_counter > 300)
        {
            _counter = 0;
            _averageFPS = 0;
            _minFPS = 60;
        }

        float milliseconds = _deltaTime * 1000.0f;
        float fps = 1.0f / _deltaTime;

        _counter++;
        _averageFPS += fps;

        if (fps < _minFPS)
        {
            _minFPS = fps;
        }

        string text = string.Format("{0:0.0} ms ({1:0.} fps)", milliseconds, fps);
        string text2 = string.Format("average ({0:0.} fps)", (_averageFPS / _counter));
        string text3 = string.Format("min ({0:0.} fps)", _minFPS);

        GetComponent<Text>().text = text + "\n" + text2 + "\n" + text3;
    }
}
