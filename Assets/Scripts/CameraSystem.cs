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

    // CHANGED: Track individual light states per camera to avoid layout desyncs
    private bool[] cameraLightStates;

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

        cameraLightStates = new bool[cameras.Length]; // Initialize state array matching size

        CameraUI.SetActive(false);
        CurrentCameraText.text = cameras[currentCamera].name + "_Cam";

        for (int i = 0; i < cameras.Length; i++)
        {
            var cam = cameras[i];
            var light = cam.GetComponentInChildren<Light>();
            cameraBaseRanges[cam] = light.range;

            light.enabled = false;
            cameraLightStates[i] = false; // Ensure everything resets clean

            cam.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // forceClose
        if (Input.GetKeyDown(KeyCode.Space) && isCameraOpen)
        {
            isCameraOpen = false;
        }

        ShowCamera();

        // Safe check cleanup loop synchronization
        for (int i = 0; i < cameras.Length; i++)
        {
            if (cameras[i].activeInHierarchy == false)
            {
                Light camLight = cameras[i].GetComponentInChildren<Light>();
                if (camLight.enabled != false)
                {
                    camLight.enabled = false;
                }

                // Force state tracking off for any disabled camera container safely
                cameraLightStates[i] = false;
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
        // Turn off the light component on the current camera before deactivating it
        Light currentLight = cameras[currentCamera].GetComponentInChildren<Light>();
        if (currentLight != null)
        {
            currentLight.enabled = false;
        }
        cameraLightStates[currentCamera] = false; // Reset state tracking flag for old room node

        // Swap the active view tracking references
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
            cameraLightStates[currentCamera] = false;
        }
        else
        {
            camLight.enabled = true;
            cameraLightStates[currentCamera] = true;
        }
    }

    public bool getCamLightOn()
    {
        // Safe protection check against out-of-bounds arrays during execution ticks
        if (currentCamera >= 0 && currentCamera < cameraLightStates.Length)
        {
            return cameraLightStates[currentCamera];
        }
        return false;
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