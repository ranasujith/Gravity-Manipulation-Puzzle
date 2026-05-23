using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Settings")]
    public Vector3 offset = new Vector3(0f, 3f, -6f);

    public float mouseSensitivity = 120f;

    public float smoothSpeed = 10f;

    [Header("Gravity Reference")]
    public PlayerController playerController;

    private float yaw;
    private float pitch;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        Cursor.visible = false;
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        RotateCamera();

        FollowTarget();
    }

    // =========================================
    // CAMERA ROTATION
    // =========================================
    private void RotateCamera()
    {
        float mouseX =
            Input.GetAxis("Mouse X") *
            mouseSensitivity *
            Time.deltaTime;

        float mouseY =
            Input.GetAxis("Mouse Y") *
            mouseSensitivity *
            Time.deltaTime;

        yaw += mouseX;

        pitch -= mouseY;

        pitch = Mathf.Clamp(pitch, -80f, 80f);
    }

    // =========================================
    // CAMERA FOLLOW
    // =========================================
    private void FollowTarget()
    {
        // Current gravity up direction
        Vector3 gravityUp =
            -Physics.gravity.normalized;

        // Camera rotation
        Quaternion rotation =
            Quaternion.LookRotation(
                target.forward,
                gravityUp
            );

        rotation *= Quaternion.Euler(pitch, yaw, 0f);

        // Desired position
        Vector3 desiredPosition =
            target.position +
            rotation * offset;

        // Smooth follow
        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed * Time.deltaTime
        );

        // Look at player
        transform.rotation = Quaternion.LookRotation(
            target.position - transform.position,
            gravityUp
        );
    }
}