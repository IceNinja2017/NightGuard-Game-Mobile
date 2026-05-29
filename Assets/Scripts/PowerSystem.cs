using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public interface IPowerUser
{
    bool IsUsingPower();
    void setDisabled();
}

public class PowerSystem : MonoBehaviour
{
    [SerializeField] private float power = 100;
    [SerializeField] private float powerDrain; //drains 1% every 6 seconds
    [SerializeField] private int usagelevel = 1;
    [SerializeField] private TMP_Text batteryPercent;
    [SerializeField] private GameObject[] UsageSpite;
    [SerializeField] private GameObject player; //flashlight
    [SerializeField] private GameObject cameraSystem;
    [SerializeField] private GameObject LeftDoor;
    [SerializeField] private GameObject RightDoor;
    [SerializeField] private AudioSource poweroutage;
    [SerializeField] private AudioSource vent;
    [SerializeField] private AudioSource breaker;
    [SerializeField] private Light officeLight;
    [SerializeField] private CameraController Cambutton;
    private List<GameObject> activeTools = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        NightData.Instance.haspoweroutage = false;
        powerDrain = NightData.Instance.getPowerdrainOnCurrentNight();
        for (int i = 0; i < UsageSpite.Length; i++)
        {
            UsageSpite[i].SetActive(false);
        }

        power += NightData.Instance.additionalPower; //add any additional power from minigames in prologue
    }

    // Update is called once per frame
    void Update()
    {
        if (!NightData.Instance.haspoweroutage)
        {
            if (power <= 1)
            {
                TriggerPowerOutage();
            }

            DrainPower();
            UpdateUsageLevel();
            UpdateUsageSprite();
            TrackActiveTools();
            batteryPercent.text = string.Format($"{Math.Floor(power)}%");

        }
    }

    public void ToggleObject(GameObject obj)
    {
        if (!obj.TryGetComponent<IPowerUser>(out var user)) return;

        if (user.IsUsingPower())
        {
            if (!activeTools.Contains(obj))
            {
                activeTools.Add(obj);
            }
        }
        else
        {
            if (activeTools.Contains(obj))
            {
                activeTools.Remove(obj);
            }
        }
    }

    public void DisabledToggle(GameObject obj)
    {
        if (!obj.TryGetComponent<IPowerUser>(out var user)) return;

        user.setDisabled();
    }

    public void TriggerPowerOutage()
    {
            NightData.Instance.haspoweroutage = true;
            powerDrain = 0;
            usagelevel = 0;
            poweroutage.Play();

            DisabledToggle(player);
            DisabledToggle(cameraSystem);
            DisabledToggle(LeftDoor);
            DisabledToggle(RightDoor);

            officeLight.enabled = false;
            vent.Stop();
            breaker.Stop();
            Cambutton.ChangetoRed();
    }
    public void DrainPower()
    {
        float finalpowerDrain = powerDrain * usagelevel;
        power -= finalpowerDrain * Time.deltaTime;
    }

    public void UpdateUsageLevel()
    {
        usagelevel = activeTools.Count + 1;
    }

    public void UpdateUsageSprite()
    {
            for (int i = 0; i < UsageSpite.Length; i++)
            {
                UsageSpite[i].SetActive(i < usagelevel);
            }
    }
    public void TrackActiveTools()
    {
        ToggleObject(player);
        ToggleObject(cameraSystem);
        ToggleObject(LeftDoor);
        ToggleObject(RightDoor);
    }
}
