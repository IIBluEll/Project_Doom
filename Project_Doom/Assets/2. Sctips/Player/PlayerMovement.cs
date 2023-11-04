using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class PlayerMovement : MonoBehaviour
{
   [SerializeField] private Transform orientation;

    [Header(" MoveMent ")] [SerializeField]
    private float moveSpeed = 6f;

    [SerializeField] private float movementMulti = 10f;
    [SerializeField] private float airMulti = 0.4f;

    [Space(10f), Header(" Sprinting ")]
    [SerializeField] private float walkSpeed = 4f;
    [SerializeField] private float sprintSpeed = 6f;
    [SerializeField] private float acceleration = 10f;
    
    [Space(10f), Header(" Keybinds ")] 
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;

    [Space(10f), Header(" Jumping Power ")] 
    [SerializeField] private float jumpForce = 5f;

    [Space(10f), Header(" Drag ")] 
    [SerializeField] private float groundDrag = 6f;

    [SerializeField] private float airDrag = 2f;

    private float playerHeight = 2f;

    private float horizontalMove;
    private float verticalMove;

    [Space(10f), Header(" Ground Detection")] 
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private Transform groundCheck;
    public bool isGrounded;
    private float groundDistance = 0.4f;

    private Vector3 moveDirection;
    private Vector3 slopeMoveDirection;

    public Rigidbody rb;
    

    private RaycastHit slopeHit;

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight / 2 + 0.5f))
        {
            return slopeHit.normal != Vector3.up;
        }

        return false;
    }

    private void Awake()
    {
        rb.freezeRotation = true;
    }

    private void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        PlayerInput();
        ControlDrag();
        ControlSpeed();
        
        if (Input.GetKeyDown(jumpKey) && isGrounded)
        {
            Jump();
        }
        
        slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void Jump()
    {
        if (isGrounded)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void PlayerInput()
    {
        horizontalMove = Input.GetAxis("Horizontal");
        verticalMove = Input.GetAxis("Vertical");

        moveDirection = orientation.forward * verticalMove + orientation.right * horizontalMove;
    }

    private void ControlSpeed()
    {
        if (Input.GetKey(sprintKey) && isGrounded)
        {
            moveSpeed = Mathf.Lerp(moveSpeed, sprintSpeed, acceleration * Time.deltaTime);
        }
        else if (horizontalMove == 0 && verticalMove == 0)
        {
            moveSpeed = Mathf.Lerp(moveSpeed, 0, acceleration * Time.deltaTime);
        }
        else
        {
            moveSpeed = Mathf.Lerp(moveSpeed, walkSpeed, acceleration * Time.deltaTime);
        }
    }
    
    private void ControlDrag()
    {
        if (isGrounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = airDrag;
        }
    }

    private void MovePlayer()
    {
        if (isGrounded && !OnSlope())
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * movementMulti, ForceMode.Acceleration);
        }
        else if (isGrounded && OnSlope())
        {
            rb.AddForce(slopeMoveDirection.normalized * moveSpeed * movementMulti, ForceMode.Acceleration);
        }
        else if (!isGrounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * movementMulti * airMulti, ForceMode.Acceleration);
        }
    }

}
