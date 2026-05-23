using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    public float jumpForce = 7f;

    [Header("Gravity")]
    public float gravityForce = 9.81f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.3f;
    public LayerMask groundLayer;

    [Header("References")]
    public Animator animator;

    private Rigidbody rb;

    private Vector3 gravityDirection = Vector3.down;

    private bool isGrounded;

    private float horizontal;
    private float vertical;

    private float airTime;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Freeze unwanted physics rotation
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        // Set default gravity
        Physics.gravity = gravityDirection * gravityForce;
    }

    private void Update()
    {
        HandleInput();

        HandleJump();

        HandleGravityChange();

        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        CheckGround();

        MovePlayer();

        CheckFallDeath();
    }

    // =========================================
    // INPUT
    // =========================================
    private void HandleInput()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
    }

    // =========================================
    // MOVEMENT
    // =========================================
    private void MovePlayer()
    {
        // Camera forward relative to gravity
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

        // Movement direction
        Vector3 moveDirection =
            forward * vertical +
            right * horizontal;

        moveDirection.Normalize();

        // Preserve gravity velocity
        Vector3 gravityVelocity =
            Vector3.Project(
                rb.linearVelocity,
                gravityDirection
            );

        // Apply movement
        rb.linearVelocity =
            moveDirection * moveSpeed +
            gravityVelocity;

        // Rotate player toward movement direction
        if (moveDirection.magnitude > 0.1f)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(
                    moveDirection,
                    -gravityDirection
                );

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    // =========================================
    // JUMP
    // =========================================
    private void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(
                -gravityDirection * jumpForce,
                ForceMode.Impulse
            );

            animator.SetTrigger("Jump");
        }
    }

    // =========================================
    // GRAVITY MANIPULATION
    // =========================================
    private void HandleGravityChange()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            SetGravity(Vector3.up);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            SetGravity(Vector3.down);
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            SetGravity(Vector3.left);
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            SetGravity(Vector3.right);
        }
    }

    private void SetGravity(Vector3 direction)
    {
        gravityDirection = direction.normalized;

        Physics.gravity = gravityDirection * gravityForce;

        // Rotate player to align with new gravity
        Quaternion targetRotation =
            Quaternion.FromToRotation(
                transform.up,
                -gravityDirection
            ) * transform.rotation;

        transform.rotation = targetRotation;
    }

    // =========================================
    // GROUND CHECK
    // =========================================
    private void CheckGround()
    {
        isGrounded = Physics.CheckSphere(
            groundCheck.position,
            groundDistance,
            groundLayer
        );
    }

    // =========================================
    // ANIMATOR
    // =========================================
    private void UpdateAnimator()
    {
        bool isRunning =
            Mathf.Abs(horizontal) > 0.1f ||
            Mathf.Abs(vertical) > 0.1f;

        bool isFalling =
            !isGrounded &&
            rb.linearVelocity.y < -0.1f;

        animator.SetBool("IsRunning", isRunning);

        animator.SetBool("IsGrounded", isGrounded);

        animator.SetBool("IsFalling", isFalling);
    }

    // =========================================
    // FALL DEATH
    // =========================================
    private void CheckFallDeath()
    {
        if (!isGrounded)
        {
            airTime += Time.fixedDeltaTime;

            // Player stayed in air too long
            if (airTime >= 3f)
            {
                Debug.Log("Game Over");

                gameObject.SetActive(false);
            }
        }
        else
        {
            airTime = 0f;
        }
    }

    // =========================================
    // DEBUG GIZMOS
    // =========================================
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