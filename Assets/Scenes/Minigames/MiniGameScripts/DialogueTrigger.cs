using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private DialogueEventType dialogueEvent;
    [SerializeField] private float delay = 0f;

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;

        if (other.CompareTag("Player"))
        {
            hasTriggered = true;
            PlayerDialogueManager.Instance.PlayDialogue(dialogueEvent, delay);
        }
    }
}