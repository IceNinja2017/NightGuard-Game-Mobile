using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HandleWin : MonoBehaviour
{
    [SerializeField] private TMP_Text clock;
    [SerializeField] private AudioSource winMusic;
    private bool hasHandledWin = false;
    void Start()
    {
        clock.text = "5:59 AM";
        clock.DOFade(0, 4.5f).From();
        Invoke("FlipToSix", 4.5f);
    }



    void Update()
    {
        if (!winMusic.isPlaying && !hasHandledWin)
        {
            hasHandledWin = true;

            SaveManager.instance.SaveGame(); //SaveGame after completing everyshift

            if (NightData.Instance.getCurrentNight() >= 5)
            {
                Debug.Log("You Win Hurrahh!!!");
                NightData.Instance.isShiftCompleted = true;

                //Transition to PayDay Screen (EndCredits)
                SceneManager.LoadScene("PayDayScene");
            }
            else
            {
                NightData.Instance.nextNight();
                SceneManager.LoadScene("NightIntroScene");
            }
        }
    }


    void FlipToSix()
    {
        clock.text = "6:00 AM";
    }
}
