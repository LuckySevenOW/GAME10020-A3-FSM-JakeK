using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

public class Character : MonoBehaviour
{
    /// <summary>
    ///  blah balsdhlaks da
    ///  s;lfnadk;
    ///  fnakd;nf
    ///  adknfa
    ///  dk;nfa
    ///  d;nf
    /// </summary>


    public float moveSpeed = 5.0f;
    public float gravity = -9.81f;
    public float throwForce = 1.0f;

    public InputActionReference moveInput;

    CharacterController controller;
    PlayerInput playerInput;
    Vector3 velocity;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
    }

    public void Update()
    {
        PlayerMotion();
    }
    void PlayerMotion()
    {
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        Vector2 moveDirection = playerInput.currentActionMap["Move"].ReadValue<Vector2>();
        Vector3 move = Vector3.right * moveDirection.x + Vector3.forward * moveDirection.y;
        Vector3 moveVelocity = move * moveSpeed;

        velocity.y += gravity * Time.deltaTime;

        moveVelocity.y = velocity.y;

        controller.Move(moveVelocity * Time.deltaTime);


        Vector3 horizontalVelocity = new Vector3(moveVelocity.x, 0f, moveVelocity.z);
        if (horizontalVelocity.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(horizontalVelocity);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                15f * Time.deltaTime
            );
        }
    }
}
