using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("TARGET")]
    public Transform target;

    [Header("SETTINGS")]
    public float distance = 6f;

    public float height = 2f;

    public float mouseSensitivity = 120f;

    public float smoothSpeed = 8f;

    public float gravityRotateSpeed = 4f;

    [Header("PITCH LIMIT")]
    public float minPitch = -40f;

    public float maxPitch = 80f;

    private float yaw;
    private float pitch;

    private Quaternion gravityAlignment =
        Quaternion.identity;


    private void Start()
    {
        yaw = transform.eulerAngles.y;
    }


    private void LateUpdate()
    {
        if (target == null)
            return;

        HandleMouseInput();

        FollowTarget();
    }


    private void HandleMouseInput()
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

        pitch = Mathf.Clamp(
            pitch,
            minPitch,
            maxPitch
        );
    }


    private void FollowTarget()
    {
        Vector3 gravityUp =
            -Physics.gravity.normalized;

        Quaternion targetGravityAlignment =
            Quaternion.FromToRotation(
                Vector3.up,
                gravityUp
            );

        gravityAlignment =
            Quaternion.Slerp(
                gravityAlignment,
                targetGravityAlignment,
                gravityRotateSpeed *
                Time.deltaTime
            );

        Quaternion cameraRotation =
            Quaternion.Euler(
                pitch,
                yaw,
                0f
            );

        Quaternion finalRotation =
            gravityAlignment *
            cameraRotation;

        Vector3 offset =
            finalRotation *
            new Vector3(
                0f,
                height,
                -distance
            );

        Vector3 desiredPosition =
            target.position + offset;

        transform.position =
            Vector3.Lerp(
                transform.position,
                desiredPosition,
                smoothSpeed *
                Time.deltaTime
            );

        transform.rotation =
            Quaternion.Slerp(
                transform.rotation,
                finalRotation,
                smoothSpeed *
                Time.deltaTime
            );
    }
}