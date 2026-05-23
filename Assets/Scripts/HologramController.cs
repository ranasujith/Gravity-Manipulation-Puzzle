using UnityEngine;

public class HologramController : MonoBehaviour
{
    public Animator animator;

    public float moveDistance = 1.5f;
    public float moveSpeed = 2f;
    public float fadeTime = 1f;

    private Vector3 startPos;
    private Vector3 targetPos;

    private float timer;

    private void OnEnable()
    {
        startPos = transform.position;

        targetPos =
            transform.position +
            transform.forward * moveDistance;

        timer = 0f;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        transform.position = Vector3.Lerp(
            startPos,
            targetPos,
            timer * moveSpeed
        );

        animator.SetBool("IsRunning", true);

        if (timer >= fadeTime)
        {
            gameObject.SetActive(false);
        }
    }
}