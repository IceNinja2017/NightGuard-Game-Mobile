using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FreeroamJupscareManager : MonoBehaviour
{
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject catObject;
    [SerializeField] private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        if (NightData.Instance.additionalPower > 0)
        {
            NightData.Instance.additionalPower = 0;
        }

        gameOverUI.SetActive(false);


        catObject.SetActive(true);
        animator.Play("Jumpscare");
        StartCoroutine(WaitForAnimation(animator, "Jumpscare"));

    }

    private IEnumerator WaitForAnimation(Animator animator, string animName)
    {
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(animName))
            yield return null;

        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
            yield return null;

        AnimatText();
    }

    private void AnimatText()
    {
        gameOverUI.SetActive(true);
        gameOverUI.GetComponentInChildren<TMP_Text>().DOFade(0, 3f).From();
        StartCoroutine(ProceedToOffice(1f));
    }

    private IEnumerator ProceedToOffice(float duration)
    {
        yield return new WaitForSeconds(duration);
        SceneManager.LoadScene("NightIntroScene");
    }
}
