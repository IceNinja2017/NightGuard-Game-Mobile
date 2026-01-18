using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Play honk sound on mouse click (for the lols)
public class NoseHonk : MonoBehaviour
{
    [SerializeField] private float honkCooldown = 0.5f;
    float honkCooldownTimer = 0f;

    private void Update()
    {
        if (honkCooldownTimer > 0)
        {
            honkCooldownTimer -= Time.deltaTime;
        }
    }
    private void OnMouseDown()
    {
        if (honkCooldownTimer <= 0)
        {
            this.GetComponent<AudioSource>().Play();
            honkCooldownTimer = honkCooldown;
        }

    }
}
