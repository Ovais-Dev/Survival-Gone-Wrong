using UnityEngine;
using UnityEngine.InputSystem;

public class CursorObj : MonoBehaviour
{
    public static CursorObj Instance;

    Vector2 mousePos; // screen
    Vector2 mouseDelta;

    private void Awake()
    {
        Instance = this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Cursor.visible = false;
       // Cursor.lockState = CursorLockMode.Locked;
       // Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        mousePos = Mouse.current.position.ReadValue();
        mouseDelta = Mouse.current.delta.ReadValue();
        //Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        //if (cursorTransform) cursorTransform.position = worldPos + Vector3.forward * 10f;
        if (Keyboard.current.eKey.IsPressed())
        {
            MakeVisible();
        }
    }
    public Vector2 GetMouseDelta()
    {
        return mouseDelta;
    }
    public Vector2 GetMouseWorldPosition()
    {
        return Camera.main.ScreenToWorldPoint(mousePos);
    }
    void MakeVisible()
    {
        Cursor.visible = !Cursor.visible;
    }
}