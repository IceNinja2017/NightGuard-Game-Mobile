using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    [SerializeField] private Image iconImage;

    public Sprite hiddenIconSprite;
    [HideInInspector] public Sprite iconSprite;

    public bool isSelected;
    public CardController controller;

    public void OnCardClicked()
    {
        controller.SetSelected(this);
    }

    public void Show()
    {
        iconImage.sprite = iconSprite;
        isSelected = true;
    }

    public void Hide()
    {
        iconImage.sprite = hiddenIconSprite;
        iconImage.color = Color.white; // Resets card tint back to white when flipped down
        isSelected = false;
    }

    // Helper method to cleanly change the card's color tint
    public void SetColor(Color newColor)
    {
        if (iconImage != null)
        {
            iconImage.color = newColor;
        }
    }
}