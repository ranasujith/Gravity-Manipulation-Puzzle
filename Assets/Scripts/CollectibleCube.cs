using System.Collections;
using UnityEngine;

public class CollectibleCube : MonoBehaviour
{
    public ParticleSystem burstVFX;
    public AudioClip collectSFX;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.CollectCube();

            StartCoroutine(DestroyObject());
            GetComponent<MeshRenderer>().enabled = false;
            AudioManager.Instance.PlaySFX(collectSFX);
            burstVFX.Play();
        }
    }

    IEnumerator DestroyObject()
    {
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
}