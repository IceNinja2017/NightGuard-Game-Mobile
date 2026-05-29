using UnityEngine;

public enum DialogueEventType
{
    NightStart,
    TimerEnd,
    EnterOffice
}

[System.Serializable]
public class NightDialogue
{
    public AudioClip nightStartClip;
    public AudioClip timerEndClip;
    public AudioClip enterOfficeClip;
}