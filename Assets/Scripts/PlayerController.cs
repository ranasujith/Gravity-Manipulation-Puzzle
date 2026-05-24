using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("MOVEMENT")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 6f;
    public float jumpForce = 7f;

    [Header("GRAVITY")]
    public float gravityForce = 9.81f;
    public float gravityTransitionForce = 8f;
    public float gravityCooldown = 0.3f;

    [Header("GROUND CHECK")]
    public Transform groundCheck;
    public float groundDistance = 0.3f;
    public LayerMask groundLayer;

    [Header("HOLOGRAM")]
    public Transform hologram;
    public Transform hologramPosRight;
    public Transform hologramPosLeft;   
    public Transform hologramPosUp;
    public Transform hologramPosDown;

    [Header("REFERENCES")]
    public Animator animator;

    private Rigidbody rb;

    private Vector3 gravityDirection = Vector3.down;

    private Vector3 pendingGravityDirection;

    private bool isGrounded;

    private float horizontal;
    private float vertical;

    private float airTime;

    private bool canChangeGravity = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.constraints =
            RigidbodyConstraints.FreezeRotation;

        rb.interpolation =
            RigidbodyInterpolation.Interpolate;

        Physics.gravity =
            gravityDirection * gravityForce;
    }

    private void Update()
    {
        HandleInput();

        HandleJump();

        HandleGravityPreview();

        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        CheckGround();

        MovePlayer();

        CheckFallDeath();
    }

    private void HandleInput()
    {
        float h = 0f;
        float v = 0f;

        if (Input.GetKey(KeyCode.A)) h -= 1f;
        if (Input.GetKey(KeyCode.D)) h += 1f;
        if (Input.GetKey(KeyCode.S)) v -= 1f;
        if (Input.GetKey(KeyCode.W)) v += 1f;

        horizontal = h;
        vertical = v;
    }

    private void MovePlayer()
    {
        Vector3 forward =
            Vector3.ProjectOnPlane(
                Camera.main.transform.forward,
                gravityDirection
            ).normalized;

        Vector3 right =
            Vector3.ProjectOnPlane(
                Camera.main.transform.right,
                gravityDirection
            ).normalized;

        Vector3 moveDirection =
            forward * vertical +
            right * horizontal;

        moveDirection.Normalize();

        Vector3 gravityVelocity =
            Vector3.Project(
                rb.linearVelocity,
                gravityDirection
            );

        rb.linearVelocity =
            moveDirection * moveSpeed +
            gravityVelocity;

        if (moveDirection.magnitude > 0.1f)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(
                    moveDirection,
                    -gravityDirection
                );

            transform.rotation =
                Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime
                );
        }
    }

    private void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space)
            && isGrounded)
        {
            rb.AddForce(
                -gravityDirection * jumpForce,
                ForceMode.Impulse
            );

            animator.SetTrigger("Jump");
        }
    }

    private void HandleGravityPreview()
    {
        if (!canChangeGravity)
            return;

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            PreviewGravity(Vector3.up);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            PreviewGravity(Vector3.down);
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            PreviewGravity(Vector3.left);
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            PreviewGravity(Vector3.right);
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (pendingGravityDirection != Vector3.zero)
            {
                ChangeGravity(
                    pendingGravityDirection
                );
            }
        }
    }

    private void PreviewGravity(
        Vector3 direction)
    {
        pendingGravityDirection =
            direction;

        UpdateHologram(direction);
    }

    private void ChangeGravity(
        Vector3 direction)
    {
        canChangeGravity = false;

        SetGravity(direction);

        Invoke(
            nameof(ResetGravityCooldown),
            gravityCooldown
        );
    }

    private void ResetGravityCooldown()
    {
        canChangeGravity = true;
    }

    private void SetGravity(
        Vector3 direction)
    {
        gravityDirection =
            direction.normalized;

        Physics.gravity =
            gravityDirection * gravityForce;

        rb.linearVelocity = Vector3.zero;

        rb.AddForce(
            -transform.up *
            gravityTransitionForce,
            ForceMode.Impulse
        );

        rb.AddForce(
            gravityDirection *
            gravityForce,
            ForceMode.Impulse
        );

        Quaternion targetRotation =
            Quaternion.FromToRotation(
                transform.up,
                -gravityDirection
            ) * transform.rotation;

        StopAllCoroutines();

        StartCoroutine(
            SmoothRotate(targetRotation)
        );

        isGrounded = false;
    }

    private IEnumerator SmoothRotate(
        Quaternion targetRotation)
    {
        Quaternion startRotation =
            transform.rotation;

        float time = 0f;

        while (time < 1f)
        {
            time +=
                Time.deltaTime *
                rotationSpeed;

            transform.rotation =
                Quaternion.Slerp(
                    startRotation,
                    targetRotation,
                    time
                );

            yield return null;
        }
    }

    private void CheckGround()
    {
        isGrounded =
            Physics.CheckSphere(
                groundCheck.position,
                groundDistance,
                groundLayer
            );
    }

    private void UpdateAnimator()
    {
        bool isRunning =
            Mathf.Abs(horizontal) > 0.1f ||
            Mathf.Abs(vertical) > 0.1f;

        bool isFalling =
            !isGrounded &&
            Vector3.Dot(
                rb.linearVelocity,
                gravityDirection
            ) > 0.1f;

        animator.SetBool(
            "IsRunning",
            isRunning
        );

        animator.SetBool(
            "IsGrounded",
            isGrounded
        );

        animator.SetBool(
            "IsFalling",
            isFalling
        );
    }

    private void CheckFallDeath()
    {
        if (!isGrounded)
        {
            airTime +=
                Time.fixedDeltaTime;

            if (airTime >= 10f)
            {
                Debug.Log("GAME OVER");

                GameManager.Instance.GameOver();

                gameObject.SetActive(false);
            }
        }
        else
        {
            airTime = 0f;
        }
    }

    private void UpdateHologram(
    Vector3 gravityDir)
    {
        if (hologram == null)
            return;

        hologram.gameObject.SetActive(true);

        Transform targetPoint = null;

        if (gravityDir == Vector3.right)
        {
            targetPoint = hologramPosRight;
        }
        else if (gravityDir == Vector3.left)
        {
            targetPoint = hologramPosLeft;
        }
        else if (gravityDir == Vector3.up)
        {
            targetPoint = hologramPosUp;
        }
        else if (gravityDir == Vector3.down)
        {
            targetPoint = hologramPosDown;
        }

        if (targetPoint == null)
            return;

        hologram.position =
            targetPoint.position;

        hologram.rotation =
            targetPoint.rotation;
    }

    private void OnTriggerEnter(
        Collider other)
    {
        if (other.gameObject.CompareTag("Obstacle"))
        {
            GameManager.Instance.GameOver();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
            return;

        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            groundCheck.position,
            groundDistance
        );
    }
}