using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JupscareManager : MonoBehaviour
{
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject wortoxObject;
    [SerializeField] private GameObject angieObject;
    [SerializeField] private GameObject catObject;

    private Animator currentAnimator;
    // Start is called before the first frame update
    void Start()
    {
        NightData.Instance.additionalPower = 0; //reset additional power when you die
        gameOverUI.SetActive(false);
        Animatronic who = NightData.Instance != null ? NightData.Instance.JumpscaringAnimatronic : Animatronic.Wortox;

        wortoxObject.SetActive(false);
        angieObject.SetActive(false);
        catObject.SetActive(false);

        switch (who)
        {
            case Animatronic.Wortox:
                wortoxObject.SetActive(true);
                currentAnimator = wortoxObject.GetComponent<Animator>();
                currentAnimator.Play("Jumpscare");
                StartCoroutine(WaitForAnimation(currentAnimator, "Jumpscare"));
                break;
            case Animatronic.Angie:
                angieObject.SetActive(true);
                currentAnimator = angieObject.GetComponent<Animator>();
                currentAnimator.Play("Jumpscare");
                StartCoroutine(WaitForAnimation(currentAnimator, "Jumpscare"));
                break;
            case Animatronic.Cat:
                catObject.SetActive(true);
                currentAnimator = catObject.GetComponent<Animator>();
                currentAnimator.Play("Jumpscare");
                StartCoroutine(WaitForAnimation(currentAnimator, "Jumpscare"));
                break;
        }
    }

    private IEnumerator WaitForAnimation(Animator animator, string animName)
    {
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(animName))
            yield return null;

        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
            yield return null;

        AnimatText();
    }

    private void AnimatText() //you died text
    {
        gameOverUI.SetActive(true);
        gameOverUI.GetComponentInChildren<TMP_Text>().DOFade(0, 3f).From();
        StartCoroutine(ReturnToMainMenu(3f));
    }

    private IEnumerator ReturnToMainMenu(float duration)
    {
        yield return new WaitForSeconds(duration);
        SceneManager.LoadScene("MainMenu");
        Debug.Log("Returning to MainMenu...");
    }
}
