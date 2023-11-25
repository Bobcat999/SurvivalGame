using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    public float moveSpeed = 5f;

    public Rigidbody2D rb;
    public Animator animator;

    Vector2 movement;

    private PlayerControls playerControls;


    private void Awake()
    {
        Time.timeScale = 1f;
        playerControls = new PlayerControls();
        playerControls.UI.Menu.performed += Menu_performed;
    }

    private void Menu_performed(InputAction.CallbackContext obj)
    {
        if (playerControls.Player.enabled)
        {
            playerControls.Player.Disable();
            Time.timeScale = 0f;
        }
        else
        {
            playerControls.Player.Enable();
            Time.timeScale = 1f;
        }
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    private void Update()
    {
        //Input
        movement = playerControls.Player.Move.ReadValue<Vector2>();

        //normalize the vector
        movement = movement.normalized;

        //only move if were not mining
        if (BuildingSystem.Instance.IsBreaking())
        {
            movement = Vector2.zero;
        }

        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Speed", movement.sqrMagnitude);
    }

    private void FixedUpdate()
    {
        
            //Movement
            rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

}
