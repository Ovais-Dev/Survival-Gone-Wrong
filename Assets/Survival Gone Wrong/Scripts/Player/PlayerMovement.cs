using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 5f;

    [SerializeField] private CursorObj cursor;
    [SerializeField] private float offset = 1f;
    private Vector2 moveInput;

    private Rigidbody2D rb;
    Vector2 lookDir = Vector2.zero;
    Vector2 lastMousePos = Vector2.zero;
    private InputAction moveAction;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        rb = GetComponent<Rigidbody2D>();
        lastMousePos = cursor.GetMousePosition();
    }

    // Update is called once per frame
    void Update()
    {
        moveInput = moveAction.ReadValue<Vector2>();
        
        LookAtCursor();
    }
    private void FixedUpdate()
    {
        moveInput = transform.TransformDirection(moveInput);
        rb.linearVelocity = moveInput * moveSpeed;

    }
    void LookAtCursor()
    {
        if (!cursor) return;
        if (cursor.GetMouseDelta().sqrMagnitude > 0.1f)
        {
            lookDir = cursor.GetMousePosition() - lastMousePos;
            //if (lastMousePos != lookDir) lastMousePos = lookDir;
            lookDir.Normalize();
            float angle = MathF.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
            //float roat
            transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.Euler(0f, 0f, angle),rotationSpeed*Time.deltaTime);
        }
        else
        {
            if(Vector2.Distance(cursor.GetMousePosition(), lastMousePos) > offset)
            lastMousePos = cursor.GetMousePosition() - lookDir * offset;
        }
    }
}
