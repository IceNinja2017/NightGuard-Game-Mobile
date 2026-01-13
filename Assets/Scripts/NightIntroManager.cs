using DG.Tweening; //From DOTween
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NightIntroManager : MonoBehaviour
{
    [SerializeField] private TMP_Text nightText;
    [SerializeField] private float delayBeforeStart = 3f;

    void Start()
    {
        int night = NightData.Instance.getCurrentNight();
        nightText.text = $"Night {night}";
        StartCoroutine(LoadMainSceneAfterDelay());
    }

    IEnumerator LoadMainSceneAfterDelay()
    {
        nightText.DOFade(0, 1.5f).SetDelay(delayBeforeStart - 1.5f);
        yield return new WaitForSeconds(delayBeforeStart);
        SceneManager.LoadScene("MainScene");
    }
}
