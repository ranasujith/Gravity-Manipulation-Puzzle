using UnityEngine;

public class HologramController : MonoBehaviour
{
    public float fadeTime = 1f;
    public Animator animator;
    private float timer;

    private void OnEnable()
    {
        timer = 0f;
        animator.SetBool("IsGrounded", true);
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= fadeTime)
        {
            gameObject.SetActive(false);
        }
    }
}