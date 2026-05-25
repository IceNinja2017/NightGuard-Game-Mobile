using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float movementSpeed;

    public Transform orientation;

    [Header("Audio")]
    public AudioSource footstep;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        MyInput();
        HandleFootsteps();
    }

    private void FixedUpdate()
    {
        movePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }

    private void movePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        Vector3 velocity = moveDirection.normalized * movementSpeed;

        rb.velocity = new Vector3(
            velocity.x,
            rb.velocity.y,
            velocity.z
        );
    }

    private void HandleFootsteps()
    {
        bool isMoving = horizontalInput != 0 || verticalInput != 0;

        if (isMoving)
        {
            if (!footstep.isPlaying)
            {
                footstep.Play();
            }
        }
        else
        {
            if (footstep.isPlaying)
            {
                footstep.Stop();
            }
        }
    }
}