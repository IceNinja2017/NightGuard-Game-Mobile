using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public Camera mainCam;
    public float interactRange = 3f;

    public GameObject InteractionUI;

    void Update()
    {
        InteractionRay();
    }

    private void InteractionRay()
    {
        // Check if MinigameManager exists and if a minigame is currently active
        if (MinigameManager.Instance != null && MinigameManager.Instance.IsMinigameActive)
        {
            // Turn off the UI if it was on, and exit early
            if (InteractionUI.activeSelf) InteractionUI.SetActive(false);
            return;
        }

        Ray ray = mainCam.ViewportPointToRay(Vector3.one / 2f);
        RaycastHit hit;

        bool hitSomething = false;

        if (Physics.Raycast(ray, out hit, interactRange))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            // Check if it has the interface AND make sure the minigame isn't already completed
            if (interactable != null && !interactable.isComplete())
            {
                hitSomething = true;

                if (Input.GetKeyDown(KeyCode.E))
                {
                    interactable.Interact();
                }
            }
        }

        InteractionUI.SetActive(hitSomething);
    }
}