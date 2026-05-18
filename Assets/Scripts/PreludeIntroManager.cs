using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PreludeIntroManager : MonoBehaviour
{
    [SerializeField] private TMP_Text nightText;
    [SerializeField] private float delayBeforeStart = 3f;

    void Start()
    {
        int night = NightData.Instance.getCurrentNight();
        nightText.text = $"Prelude: 11PM\nNight {night}";
        StartCoroutine(LoadMainSceneAfterDelay());
    }

    IEnumerator LoadMainSceneAfterDelay()
    {
        nightText.DOFade(0, 1.5f)
            .SetDelay(delayBeforeStart - 1.5f);

        yield return new WaitForSeconds(delayBeforeStart);

        // START ASYNC LOAD (not blocking)
        AsyncOperation op = SceneManager.LoadSceneAsync("FreeRoam");
        op.allowSceneActivation = false;

        // wait until scene is almost fully loaded
        while (op.progress < 0.9f)
            yield return null;

        // let Unity initialize render pipeline one frame
        yield return null;

        // FORCE WARMUP STEP
        Shader.WarmupAllShaders();

        // optional: force camera initialization
        foreach (var cam in FindObjectsOfType<Camera>())
        {
            cam.Render();
        }

        op.allowSceneActivation = true;
    }
}