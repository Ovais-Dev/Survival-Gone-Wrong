using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Speed Control Variables")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintSpeedMultiplier = 2f;
    [SerializeField] private float crouchSpeedMultiplier = 0.3f;
    [SerializeField] private float rotationSpeed = 5f;

    [Header("Walking Hearing Variables")]
    public float baseSound = 1f;

    float stepTimer;

    public float walkStepInterval = 0.45f;
    public float sprintStepInterval = 0.28f;
    public float crouchStepInterval = 0.7f;

    [SerializeField] private CursorObj cursor;
    [SerializeField] private float offset = 1f;
    private Vector2 moveInput;

    private Rigidbody2D rb;
    private InputAction moveAction;
    private InputAction sprintAction;
    private InputAction crouchAction;

    PlayerAnimation playerAnim;
    bool isSprinting = false;
    bool isCrouching = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerAnim = GetComponent<PlayerAnimation>();
        moveAction = InputSystem.actions.FindAction("Move");
        sprintAction = InputSystem.actions.FindAction("Sprint");
        crouchAction = InputSystem.actions.FindAction("Crouch");

        playerAnim.SetState(PlayerAnimation.AnimState.Idle);
    }

    // Update is called once per frame
    void Update()
    {
        moveInput = moveAction.ReadValue<Vector2>();
        LookAtCursor();

        AnimationHandler();

        HandleFootsteps();
        //moveInput = transform.TransformDirection(moveInput);

    }
    void AnimationHandler()
    {
        if (moveInput.sqrMagnitude > 0.01f)
        {
            isCrouching = crouchAction.IsPressed();
            if (isCrouching)
            {
                playerAnim.SetState(PlayerAnimation.AnimState.Crouch);
                return;
            }

            isSprinting = sprintAction.IsPressed();
            if (isSprinting)
            {
                playerAnim.SetState(PlayerAnimation.AnimState.Run);
                return;
            }
            playerAnim.SetState(PlayerAnimation.AnimState.Move);
        }
        else
        {
            playerAnim.SetState(PlayerAnimation.AnimState.Idle);
        }
    }

    private void FixedUpdate()
    {
        float multiplier = isCrouching ? crouchSpeedMultiplier : (isSprinting ? sprintSpeedMultiplier : 1f);
        rb.linearVelocity = moveInput * moveSpeed * multiplier;
    }

    void LookAtCursor()
    {
        if (!cursor) return;
        //if (cursor.GetMouseDelta().sqrMagnitude > 0.1f)
        //{
            //lookDir = cursor.GetMouseWorldPosition() - lastMousePos;
            ////if (lastMousePos != lookDir) lastMousePos = lookDir;
            //lookDir.Normalize();
            Vector2 lookDir = cursor.GetMouseWorldPosition() - (Vector2)transform.position;
            lookDir.Normalize();
            float angle = MathF.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
            //float roat
            transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.Euler(0f, 0f, angle),rotationSpeed*Time.fixedDeltaTime);
        //}
        //else
        //{
        //    if(Vector2.Distance(cursor.GetMouseWorldPosition(), lastMousePos) > offset)
        //    lastMousePos = cursor.GetMouseWorldPosition() - lookDir * offset;
        //}
    }
    void HandleFootsteps()
    {
        if (moveInput.sqrMagnitude < 0.01f)
        {
            stepTimer = 0f;
            return;
        }

        float stepInterval = walkStepInterval;

        if (isSprinting)
            stepInterval = sprintStepInterval;
        else if (isCrouching)
            stepInterval = crouchStepInterval;

        stepTimer += Time.deltaTime;

        if (stepTimer >= stepInterval)
        {
            stepTimer = 0f;
            MakeFootstepSound();
        }
    }
    void MakeFootstepSound()
    {
        float baseSound = this.baseSound;

        if (isSprinting)
            baseSound *= 2.5f;

        if (isCrouching)
            baseSound *= 0.3f;

        SoundManager.EmitSound(transform.position, baseSound, 10f);
    }
}
