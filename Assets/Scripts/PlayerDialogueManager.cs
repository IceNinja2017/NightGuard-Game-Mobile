using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDialogueManager : MonoBehaviour
{
    public static PlayerDialogueManager Instance;

    public AudioSource audioSource;
    public NightDialogue[] nights;

    private HashSet<DialogueEventType> playedEvents = new();

    void Awake()
    {
        Instance = this;
    }

    private int CurrentNight()
    {
        return NightData.Instance.getCurrentNight();
    }

    public void ResetNight()
    {
        playedEvents.Clear();
    }

    public void PlayDialogue(DialogueEventType type, float delay = 0f)
    {
        StartCoroutine(PlayDialogueRoutine(type, delay));
    }

    private IEnumerator PlayDialogueRoutine(DialogueEventType type, float delay)
    {
        if (playedEvents.Contains(type))
            yield break;

        AudioClip clip = GetClip(type);

        if (clip == null)
            yield break;

        if (delay > 0f)
            yield return new WaitForSeconds(delay);

        audioSource.PlayOneShot(clip);
        playedEvents.Add(type);
    }

    private AudioClip GetClip(DialogueEventType type)
    {
        int nightIndex = CurrentNight()-1;

        if (nightIndex < 0 || nightIndex >= nights.Length)
            return null;

        NightDialogue night = nights[nightIndex];

        switch (type)
        {
            case DialogueEventType.NightStart:
                return night.nightStartClip;

            case DialogueEventType.TimerEnd:
                if (nightIndex == 0 || nightIndex == 1)
                    return null;

                return night.timerEndClip;

            case DialogueEventType.EnterOffice:
                return night.enterOfficeClip;
        }

        return null;
    }
}