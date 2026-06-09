using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class Timer : MonoBehaviour
{
    public static Timer instance;
    public TextMeshProUGUI time;

    private bool start;
    private float timer;
    private int hour, minute;
    private float second;
    void Awake()
    {
        if(instance == null)
        {
            DontDestroyOnLoad(time);
            DontDestroyOnLoad(gameObject);
            instance = this;
            start = false;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timer = 7200.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (start)
        {
            timer -= Time.deltaTime;
            hour = (int)timer / 3600;
            minute = ((int)timer - hour * 3600) / 60;
            second = (timer - hour * 3600) -  minute * 60;
            time.text = hour + ":" + minute.ToString("D2") + ":" + second.ToString("00.00");
        }
    }

    public void OnTimer()
    {
        start = true;
    }

    public void OffTimer()
    {
        start = false;
    }
}
