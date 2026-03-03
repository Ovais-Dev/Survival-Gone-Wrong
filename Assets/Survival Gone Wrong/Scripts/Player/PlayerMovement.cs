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
        lastMousePos = cursor.GetMouseWorldPosition();
    }

    // Update is called once per frame
    void Update()
    {
        moveInput = moveAction.ReadValue<Vector2>();
        moveInput = transform.TransformDirection(moveInput);
        LookAtCursor();
        
    }
    private void FixedUpdate()
    {
        
        rb.linearVelocity = moveInput * moveSpeed;
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
}
