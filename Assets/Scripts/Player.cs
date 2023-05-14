using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float groundDrag = 6f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float jumpCooldown = 0.5f;
    [SerializeField] private float airMultiplier = 0.4f;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;

    [Header("Ground Detection")]
    [SerializeField] private float playerHeight = 2f;
    [SerializeField] private LayerMask whatIsGround;

    [Header("Components")]
    [SerializeField] private Transform orientation;
    [SerializeField] private Rigidbody rb;

    private bool isGrounded;
    private bool isReadyToJump = true;
    private float horizontalInput;
    private float verticalInput;

    private void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        CheckGrounded();
        ReadInput();
        MovePlayer();
        LimitSpeed();
    }

    private void CheckGrounded()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, whatIsGround);
        Physics.gravity = new Vector3(0, -40f, 0);
    }

    private void ReadInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(jumpKey) && isReadyToJump && isGrounded)
        {
            isReadyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void MovePlayer()
    {
        Vector3 moveDir = orientation.forward * verticalInput + orientation.right * horizontalInput;
        moveDir.Normalize();
        float multiplier = isGrounded ? 1f : airMultiplier;
        rb.AddForce(moveDir * moveSpeed * multiplier, ForceMode.Force);
    }

    private void LimitSpeed()
    {
        Vector3 horizontalVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        if (horizontalVelocity.magnitude > moveSpeed)
        {
            Vector3 limitedVelocity = horizontalVelocity.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y, limitedVelocity.z);
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        isReadyToJump = true;
    }
}
