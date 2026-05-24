using UnityEngine;

public class CollectibleCube : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.CollectCube();

            Destroy(gameObject);
        }
    }
}