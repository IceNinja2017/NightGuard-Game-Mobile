using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Rename to CameraController later
public class CameraLook : MonoBehaviour, IPowerUser
{
    [SerializeField] private GameObject flashlight;
    [SerializeField] private float cameraSensitivity = 75;
    [SerializeField] private float minYaw; //90.262
    [SerializeField] private float maxYaw; //174.964
    [SerializeField, Range(0f, 0.5f)] private float edgeThresholdPercent = 0.1f;
    private float screenCenterX;
    private float edgeThresholdPixels;
    private float camLookDistance; //rename to camYawAngle later

    private bool isFlashlightOn;
    public bool disabled = false; // for the flashlight
    private AudioSource[] audios;

    // Start is called before the first frame update
    void Start()
    {
        camLookDistance = transform.localEulerAngles.y;
        screenCenterX = Screen.width / 2f;
        edgeThresholdPixels = Screen.width * edgeThresholdPercent;
        audios = flashlight.GetComponents<AudioSource>();
        flashlight.GetComponent<Light>().enabled = false;
        isFlashlightOn = false;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.mousePosition.x;
        float deltaX = mouseX - screenCenterX;

        if (Mathf.Abs(deltaX) > edgeThresholdPixels)
        {
            float direction = Mathf.Sign(deltaX);
            camLookDistance += direction * cameraSensitivity * Time.deltaTime;
            //Debug.Log($"Edge threshold: {edgeThresholdPixels} pixels");
        }
        camLookDistance = Mathf.Clamp(camLookDistance, minYaw, maxYaw);
        transform.localRotation = Quaternion.Euler(0f, camLookDistance, 0f);

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (!disabled)
            {
                //Debug.Log("Flashlight!!!!");
                ToggleFlashlight();
                if (getFlashlightState()) audios[0].pitch = 1f;
                else audios[0].pitch = 0.9f;
                audios[0].Play();
            }
            else
            {
                audios[1].Play();
            }
        }
    }

    public void ToggleFlashlight()
    {
        if (flashlight.GetComponent<Light>().enabled == true)
        {
            flashlight.GetComponent<Light>().enabled = false;
            isFlashlightOn = false;
            
        }
        else
        {
            flashlight.GetComponent<Light>().enabled = true;
            isFlashlightOn = true;
        }

    }

    public bool getFlashlightState()
    {
        return isFlashlightOn;
    }

    public int getDoorSide() //if -1 it's left, 0 is middle, 1 is right
    {
        /*
        Use this in the future for more precise calculation of door side based on yaw angle:
        float t = Mathf.InverseLerp(minYaw, maxYaw, yaw);

        if (t < 0.33f) return -1;
        if (t > 0.66f) return 1;
        return 0;
        */

        if (this.transform.rotation.eulerAngles.y <= 110f)
        {
            return -1;
        }
        else if (this.transform.rotation.eulerAngles.y >= 169f)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

    //for interface
    public bool IsUsingPower()
    {
        return getFlashlightState();
    }

    public void setDisabled() //disabled the flashlights
    {
        if (isFlashlightOn) ToggleFlashlight();
        disabled = true;
        
    }
}