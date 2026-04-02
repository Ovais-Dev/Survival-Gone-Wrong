using UnityEngine;

public class TopDownCamera : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0, 10, 0);

    [Header("Smoothness Settings")]
    [Range(0, 1)]
    [SerializeField] private float positionSmoothSpeed = 0.125f;
    [Range(0, 1)]
    [SerializeField] private float rotationSmoothSpeed = 0.05f;

    private void LateUpdate()
    {
        if (target == null) return;

        HandlePosition();
        RotateTowardsMouse();
    }

    private void HandlePosition()
    {
        // Calculate the desired position based on the target's current orientation
        Vector3 desiredPosition = target.position + (target.rotation * offset);

        // Smoothly move from current position to desired position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, positionSmoothSpeed);
        transform.position = smoothedPosition;
    }

    void RotateTowardsMouse()
    {
        Vector3 mousePos = CursorObj.Instance.GetMouseWorldPosition();

        // 3. Check where the ray intersects the plane
        
            Vector3 direction = (mousePos - target.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        // We only want to rotate on the Y axis for top-down
        // Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.z));
        Quaternion lookRotation = Quaternion.AngleAxis(angle,Vector3.forward);
            transform.rotation = lookRotation;
        
    }
}