using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine;
using System;

public class ShiftTimer : MonoBehaviour
{

    [SerializeField] private float timer;
    [SerializeField] private float shiftEndTime = 6f;
    [SerializeField] private string digitalClock;
    [SerializeField] private float timeMultiplier = 1;
    [SerializeField] private TextMeshProUGUI clockUI;
    int currentHour = 0;

    int hours;
    // Start is called before the first frame update
    void Start()
    {
        digitalClock = "";
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime * timeMultiplier; //starts a timer in seconds
        hours = Mathf.FloorToInt(timer / 60); //converts minutes since 1 hour = 1 minute

        if (hours == 0)
        {
            hours = 12;
        }

        digitalClock = string.Format("{0:00} AM", hours);

        clockUI.text = digitalClock;

        if (hours >= shiftEndTime && hours != 12)
        {
            SceneManager.LoadScene("WinScreen");
        }
    }

    public int getCurrentHour()
    {
        return hours;
    }
    public bool IsnewHour()
    {
        if (currentHour != hours)
        {
            currentHour = hours;
            return true;
        }
        else
        {
            return false;
        }
    }
}
