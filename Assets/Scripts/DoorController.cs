using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField] public Material[] materials;
    [SerializeField] private Door door;

    Renderer rend;
    Material[] mats;

    void Start()
    {
        rend = GetComponent<Renderer>();
        mats = rend.sharedMaterials;
        mats[1] = materials[1];
    }
    private void OnMouseDown()
    {
        if (!door.disabled)
        {
            door.ToggleDoor();
            if (door.getIsOpen() == true)
            {
                mats[1] = materials[0];
            }
            else
            {
                mats[1] = materials[1];
            }
            rend.sharedMaterials = mats;
        }
    }
}