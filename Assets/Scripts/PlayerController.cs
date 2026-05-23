using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("MOVEMENT")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    public float jumpForce = 7f;

    [Header("GRAVITY")]
    public float gravityForce = 9.81f;

    [Header("GROUND CHECK")]
    public Transform groundCheck;
    public float groundDistance = 0.3f;
    public LayerMask groundLayer;

    [Header("HOLOGRAM")]
    public Transform hologram;

    [Header("REFERENCES")]
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
        rb.constraints = RigidbodyConstraints.FreezeRotation;

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

    private void HandleInput()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
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

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }

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

    private void HandleGravityChange()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            SetGravity(Vector3.up);
            UpdateHologram(Vector3.up);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            SetGravity(Vector3.down);
            UpdateHologram(Vector3.down);
        }
    }

    private void SetGravity(Vector3 direction)
    {
        gravityDirection = direction.normalized;

        Physics.gravity = gravityDirection * gravityForce;

        Quaternion targetRotation =
            Quaternion.FromToRotation(
                transform.up,
                -gravityDirection
            ) * transform.rotation;

        transform.rotation = targetRotation;
    }

    private void CheckGround()
    {
        isGrounded = Physics.CheckSphere(
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
            rb.linearVelocity.y < -0.1f;

        animator.SetBool("IsRunning", isRunning);

        animator.SetBool("IsGrounded", isGrounded);

        animator.SetBool("IsFalling", isFalling);
    }

    private void CheckFallDeath()
    {
        if (!isGrounded)
        {
            airTime += Time.fixedDeltaTime;

            if (airTime >= 3f)
            {
                Debug.Log("Game Over");

                //gameObject.SetActive(false);
            }
        }
        else
        {
            airTime = 0f;
        }
    }

    private void UpdateHologram(Vector3 gravityDir)
    {
        RaycastHit hit;

        Vector3 rayOrigin =
            transform.position;

        if (Physics.Raycast(
            rayOrigin,
            gravityDir,
            out hit,
            10f))
        {
            hologram.gameObject.SetActive(true);

            hologram.position =
                hit.point + (-gravityDir * 1f);

            hologram.rotation =
                Quaternion.LookRotation(
                    transform.forward,
                    -gravityDir
                );

            Animator holoAnimator =
                hologram.GetComponent<Animator>();

            holoAnimator.SetBool(
                "IsRunning",
                animator.GetBool("IsRunning")
            );

            holoAnimator.SetBool(
                "IsGrounded",
                true
            );

            holoAnimator.SetBool(
                "IsFalling",
                false
            );
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