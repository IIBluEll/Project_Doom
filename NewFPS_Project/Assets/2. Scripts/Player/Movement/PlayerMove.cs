using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private Transform orientation;

    public bool isPlayerDie = false;
    
    // 플레이어 이동 속도 변수 선언
    [Header("Movement")]    
    public float moveSpeed = 4f;
    public float movementMultiplier = 10f;
    public float airMultiplier = 0.4f;

    // 플레이어 걷기/뛰기 속도 변수 선언
    [Space(10), Header("Sprinting")]    
    public float walkSpeed = 4f;
    public float sprintSpeed = 6f;
    public float acceleration = 10f;
    
    // 키보드 입력 변수 선언
    [Space(10), Header("Keybinds")]     
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;

    // 플레이어 점프 힘 변수 선언
    [Space(10), Header("Jumping Power")]    
    public float jumpForce = 5f;

    // 플레이어 마찰력 변수 선언
    [Space(10), Header("Drag")]         
    public float groundDrag = 6f;
    public float airDrag = 2f;

    private const float PlayerHeight = 2f; 
    private const float SlopeCheckDistance = 0.5f; 

    private float horizontalMove;
    private float verticalMove;

    // 지상 충돌 체크
    [Space(10), Header("Ground Detection")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private Transform groundCheck;
    public bool isGrounded;
    private float groundDistance = 0.4f;
    
    private Vector3 moveDirection;
    private Vector3 slopeMoveDirection;

    public Rigidbody rb;

    private RaycastHit slopeHit;
    private bool isOnSlope; 

    private void Awake()
    {
        rb.freezeRotation = true;
    }

    private void Start()
    {
        isOnSlope = false;
    }

    private void Update()
    {
        if (isPlayerDie)
        {
            return;
        }
        
        // 지면 감지
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        
        isOnSlope = OnSlope(); 

        PlayerInput();  // 입력 처리
        ControlDrag();  // 마찰력 제어
        ControlSpeed(); // 속도 제어

        // 점프 처리
        if (Input.GetKeyDown(jumpKey) && isGrounded)
        {
            Jump();
        }

        // 경사면 수직 방향 계산
        slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    
    // 플레이어가 경사면 위에 있는지 검사
    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, PlayerHeight / 2 + SlopeCheckDistance))
        {
            return slopeHit.normal != Vector3.up;
        }
        return false;
    }

    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void PlayerInput()
    {
        horizontalMove = Input.GetAxis("Horizontal");
        verticalMove = Input.GetAxis("Vertical");
        moveDirection = orientation.forward * verticalMove + orientation.right * horizontalMove;
    }

    // 속도 제어
    private void ControlSpeed()
    {
        float targetSpeed = walkSpeed;

        if (Input.GetKey(sprintKey) && isGrounded)
        {
            targetSpeed = sprintSpeed;
        }
        else if (horizontalMove == 0 && verticalMove == 0)
        {
            targetSpeed = 0;
        }

        moveSpeed = Mathf.Lerp(moveSpeed, targetSpeed, acceleration * Time.deltaTime);
    }
    
    // 마찰력 제어
    private void ControlDrag()
    {
        rb.drag = isGrounded ? groundDrag : airDrag;
    }

    // 플레이어 이동 처리
    private void MovePlayer()
    {
        Vector3 forceDirection = isOnSlope ? slopeMoveDirection.normalized : moveDirection.normalized;
        float multiplier = isGrounded ? movementMultiplier : movementMultiplier * airMultiplier;

        rb.AddForce(forceDirection * moveSpeed * multiplier, ForceMode.Acceleration);
    }
}
