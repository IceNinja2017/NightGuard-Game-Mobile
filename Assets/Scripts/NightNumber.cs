using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NightNumber : MonoBehaviour
{
    public TextMeshProUGUI text;

    private void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }
    void Update()
    {
        text.text = "Night " + NightData.Instance.getCurrentNight();
    }
}
