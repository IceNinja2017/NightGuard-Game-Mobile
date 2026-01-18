using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraSystem : MonoBehaviour, IPowerUser
{
    [SerializeField] private GameObject mainCamera;

    [SerializeField] private GameObject vent;
    [SerializeField] private GameObject[] cameras;
    [SerializeField] private GameObject CameraUI;

    [SerializeField] private Text CurrentCameraText;
    [SerializeField] private int currentCamera;
    [SerializeField] private bool isCameraOpen;

    private bool isCamLightOn;
    public bool disabled = false; //for camera ui
    private Dictionary<GameObject, float> cameraBaseRanges = new Dictionary<GameObject, float>();

    AudioSource[] camsfx;
    private float ventVolume;
    private Image camStaticImage;
    private int activeStaticCount = 0;


    // Start is called before the first frame update
    void Start()
    {
        camStaticImage = CameraUI.transform.Find("CamStatic").GetComponent<Image>();
        ventVolume = vent.GetComponent<AudioSource>().volume;
        camsfx = this.GetComponents<AudioSource>();

        CameraUI.SetActive(false);
        CurrentCameraText.text = cameras[currentCamera].name + "_Cam";
        foreach (var cam in cameras)
        {
            var light = cam.GetComponentInChildren<Light>();
            cameraBaseRanges[cam] = light.range;

            light.enabled = false;

            cam.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //forceClose
        if (Input.GetKeyDown(KeyCode.Space) && isCameraOpen)
        {
            isCameraOpen = false;
        }

        ShowCamera();

        foreach (var cam in cameras)
        {
            if (cam.activeInHierarchy == false && cam.GetComponentInChildren<Light>().enabled != false)
            {
                cam.GetComponentInChildren<Light>().enabled = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.F) && cameras[currentCamera].activeInHierarchy == true)
        {
            if (!getCamLightOn()) camsfx[1].pitch = 1f;
            else camsfx[1].pitch = 0.9f;
            camsfx[1].Play();
            toggleCamLights(cameras[currentCamera]);
        }
    }

    public void toggleCamera()
    {
        if (!disabled) isCameraOpen = !isCameraOpen;
    }

    private void ShowCamera()
    {
        if (isCameraOpen)
        {
            vent.GetComponent<AudioSource>().volume = ventVolume / 2f;
            CameraUI.SetActive(true);
            cameras[currentCamera].SetActive(true);
            mainCamera.SetActive(false);
        }
        else
        {
            vent.GetComponent<AudioSource>().volume = ventVolume;
            CameraUI.SetActive(false);
            cameras[currentCamera].SetActive(false);
            mainCamera.SetActive(true);
        }
    }

    public void goToCamera(int prograssion)
    {
        CurrentCameraText.text = cameras[prograssion].name + "_Cam";
        cameras[currentCamera].SetActive(false);
        cameras[prograssion].SetActive(true);

        currentCamera = prograssion;
    }

    public void toggleCamLights(GameObject camera)
    {
        Light camLight = camera.GetComponentInChildren<Light>();

        if (camLight.enabled == true)
        {
            camLight.enabled = false;
            isCamLightOn = false;
        }
        else
        {
            camLight.enabled = true;
            isCamLightOn = true;
        }
    }

    public bool getCamLightOn()
    {
        return isCamLightOn;
    }

    public bool getIsCameraOpen()
    {
        return isCameraOpen;
    }

    //for interface
    public bool IsUsingPower()
    {
        return getIsCameraOpen();
    }

    public void setDisabled() //disabled the cameras
    {
        if (isCameraOpen) toggleCamera();
        disabled = true;

    }

    public string getCurrentActiveCam()
    {
        return cameras[currentCamera].name;
    }

    private IEnumerator PlayStaticEffect(float duration = 1f)
    {
        var cam = cameras[currentCamera];
        var light = cam.GetComponentInChildren<Light>();
        var baseRange = cameraBaseRanges[cam];

        activeStaticCount++;
        camStaticImage.color = new Color(1f, 1f, 1f, 1f);

        // Only dim the light once
        if (activeStaticCount == 1)
        {
            light.range = baseRange - 30f;
        }

        yield return new WaitForSeconds(duration);

        activeStaticCount--;

        // Only reset if this is the last active dim
        if (activeStaticCount == 0)
        {
            camStaticImage.color = new Color(1f, 1f, 1f, 0.25f);
            light.range = baseRange;
        }
    }

    public void animatronicMoveStatic()
    {
        StartCoroutine(PlayStaticEffect());
    }
}
