using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] Transform orientation;
    [SerializeField] LayerMask ground;
    [SerializeField] float moveSpeed = 7;
    [SerializeField] float playerHeight = 2;
    [SerializeField] float groundDrag = 5;
    [SerializeField] float jumpForce = 6.5f;
    [SerializeField] float jumpCooldown = 0.25f;
    [SerializeField] float airMultiplier = 8;
    [SerializeField] MoveCamera moveCamera;
    [SerializeField] float slideSpeed;
    [SerializeField] float slideTime;
    bool readyToJump = true;
    float horizontalInput;
    float verticalInput;
    Vector3 moveDirection;
    Rigidbody rb;
    bool grounded;
    PlayerControls playerControls;
    float tempSpeed;
    bool sliding;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        playerControls = new PlayerControls();
        playerControls.Player.Enable();
    }

    private void Start()
    {
        rb.freezeRotation = true;

        tempSpeed = moveSpeed;
    }

    private void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, ground);

        PlayerInput();
        SpeedControl();
        DragControl();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    void PlayerInput()
    {
        horizontalInput = playerControls.Player.Movement.ReadValue<Vector2>().x;
        verticalInput = playerControls.Player.Movement.ReadValue<Vector2>().y;

        if (playerControls.Player.Jump.ReadValue<float>() > 0 && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        if (playerControls.Player.Slide.ReadValue<float>() > 0 && grounded && !sliding)
        {
            StartCoroutine(Slide());
        }
        else if (playerControls.Player.Slide.ReadValue<float>() <= 0 && grounded)
        {
            sliding = false;
            moveCamera.SetSliding(false);
            moveSpeed = tempSpeed;
        }
    }

    void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10, ForceMode.Force);
        }
        else
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * airMultiplier, ForceMode.Force);
        }
    }

    void SpeedControl()
    {
        Vector3 flatVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (flatVelocity.magnitude > moveSpeed)
        {
            Vector3 limitedVelocity = flatVelocity.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y, limitedVelocity.z);
        }
    }

    void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    void ResetJump()
    {
        readyToJump = true;
    }

    void DragControl()
    {
        if (grounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = 0;
        }
    }

    IEnumerator Slide()
    {
        // change collider to reflect height

        sliding = true;
        moveCamera.SetSliding(true);
        moveSpeed = slideSpeed;

        yield return new WaitForSeconds(slideTime);

        if (grounded)
        {
            moveSpeed = tempSpeed / 2;
        }
    }

    private void OnDestroy()
    {
        playerControls.Player.Disable();
    }
}
