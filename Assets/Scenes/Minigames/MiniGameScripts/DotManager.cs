using UnityEngine;
using UnityEngine.UI;

public class DotManager : MonoBehaviour
{
    public Image[] dots;

    private int currentIndex = 0;

    public void ResetDots()
    {
        currentIndex = 0;

        for (int i = 0; i < dots.Length; i++)
        {
            dots[i].color = Color.white;
        }
    }

    public void SetCorrect()
    {
        if (currentIndex >= dots.Length) return;

        dots[currentIndex].color = Color.green;
        currentIndex++;
    }

    public void SetWrong()
    {
        if (currentIndex >= dots.Length) return;

        dots[currentIndex].color = Color.red;
        currentIndex++;
    }
}