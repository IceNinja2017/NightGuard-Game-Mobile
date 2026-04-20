using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TurnOffPower : MonoBehaviour
{
    private void Update()
    {
        if (NightData.Instance.haspoweroutage)
        {
            gameObject.SetActive(false);
        }
    }
}
