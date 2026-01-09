using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShowSelectoOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] GameObject textMeshPro;
    // Start is called before the first frame update
    public void Start()
    {
        if (textMeshPro)
            textMeshPro.SetActive(false);
    }

    // Update is called once per frame
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (textMeshPro)
            textMeshPro.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (textMeshPro)
            textMeshPro.SetActive(false);
    }
}
