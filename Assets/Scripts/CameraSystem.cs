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

    public bool disabled = false;

    private Dictionary<GameObject, float> cameraBaseRanges = new Dictionary<GameObject, float>();

    AudioSource[] camsfx;
    private float ventVolume;
    private Image camStaticImage;
    private int activeStaticCount = 0;

    void Start()
    {
        camStaticImage = CameraUI.transform.Find("CamStatic").GetComponent<Image>();
        ventVolume = vent.GetComponent<AudioSource>().volume;
        camsfx = GetComponents<AudioSource>();

        CameraUI.SetActive(false);
        CurrentCameraText.text = cameras[currentCamera].name + "_Cam";

        for (int i = 0; i < cameras.Length; i++)
        {
            var cam = cameras[i];
            var light = cam.GetComponentInChildren<Light>();

            cameraBaseRanges[cam] = light.range;

            light.enabled = false;
            cam.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isCameraOpen)
        {
            isCameraOpen = false;
        }

        ShowCamera();

        // safety cleanup (only visual, no state tracking)
        for (int i = 0; i < cameras.Length; i++)
        {
            if (!cameras[i].activeInHierarchy)
            {
                Light camLight = cameras[i].GetComponentInChildren<Light>();
                if (camLight != null)
                    camLight.enabled = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.F) && cameras[currentCamera].activeInHierarchy)
        {
            camsfx[1].pitch = getCamLightOn() ? 0.9f : 1f;
            camsfx[1].Play();

            toggleCamLights(cameras[currentCamera]);
        }
    }

    public void toggleCamera()
    {
        if (!disabled)
            isCameraOpen = !isCameraOpen;
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

    public void goToCamera(int index)
    {
        Light currentLight = cameras[currentCamera].GetComponentInChildren<Light>();
        if (currentLight != null)
            currentLight.enabled = false;

        CurrentCameraText.text = cameras[index].name + "_Cam";

        cameras[currentCamera].SetActive(false);
        cameras[index].SetActive(true);

        currentCamera = index;
    }

    public void toggleCamLights(GameObject camera)
    {
        Light camLight = camera.GetComponentInChildren<Light>();
        if (camLight == null) return;

        camLight.enabled = !camLight.enabled;
    }

    // ✅ FIX: real truth source = actual Light, not cached bool array
    public bool getCamLightOn()
    {
        if (currentCamera < 0 || currentCamera >= cameras.Length)
            return false;

        Light camLight = cameras[currentCamera].GetComponentInChildren<Light>();
        return camLight != null && camLight.enabled;
    }

    public bool getIsCameraOpen()
    {
        return isCameraOpen;
    }

    public bool IsUsingPower()
    {
        return getIsCameraOpen();
    }

    public void setDisabled()
    {
        if (isCameraOpen)
            toggleCamera();

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

        if (light != null && activeStaticCount == 1)
        {
            light.range = baseRange - 30f;
        }

        yield return new WaitForSeconds(duration);

        activeStaticCount--;

        if (light != null && activeStaticCount == 0)
        {
            camStaticImage.color = new Color(1f, 1f, 0.25f, 0.25f);
            light.range = baseRange;
        }
    }

    public void animatronicMoveStatic()
    {
        StartCoroutine(PlayStaticEffect());
    }
}