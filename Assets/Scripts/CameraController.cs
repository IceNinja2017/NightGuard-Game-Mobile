using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CameraSystem camSystem;
    [SerializeField] private Material[] materials;

    Renderer rend;
    Material[] mats;

    void Start()
    {
        rend = GetComponent<Renderer>();
        mats = rend.sharedMaterials;
        mats[0] = materials[1];
    }
    private void OnMouseDown()
    {
        if (!camSystem.disabled)
        {
            this.GetComponent<AudioSource>().Play();
            camSystem.toggleCamera();
        }
    }

    public void ChangetoRed()
    {
            mats[0] = materials[1];
            rend.sharedMaterials = mats;
    }
}
