using System.Collections;
using UnityEngine;

public class PhoneCallManager : MonoBehaviour
{
    [Header("MuteBtn UI")]
    public GameObject muteBtn;

    [Header("Audio Source")]
    public AudioSource audioSource;

    [Header("PhoneCall Audio Clips")]
    public AudioClip[] phoneCallClips;

    private void Start()
    {
        int currentNight = NightData.Instance.getCurrentNight();

        int clipIndex = Mathf.Clamp(currentNight - 1, 0, phoneCallClips.Length - 1);

        audioSource.clip = phoneCallClips[clipIndex];
        muteBtn.SetActive(false);

        StartCoroutine(StartPhoneCall());
    }

    private void Update()
    {
        if (!audioSource.isPlaying)
        {
            muteBtn.SetActive(false);
        }
    }

    private IEnumerator StartPhoneCall()
    {
        yield return new WaitForSeconds(1f);
        muteBtn.SetActive(true);
        audioSource.Play();
    }

    public void HandleMuteButtons()
    {
        audioSource.Stop();
        muteBtn.SetActive(false);
    }
}