using UnityEngine;
using System.Collections;

public class Bomb : MonoBehaviour
{
    private Outline outline;
    private Collider bombCollider;

    private void Awake()
    {
        bombCollider = GetComponent<Collider>();
        outline = GetComponent<Outline>();
        outline.enabled = false;
    }

    void OnMouseDown()
    {
        StartCoroutine(Explode());
    }

    private IEnumerator Explode()
    {
        outline.enabled = true;
        bombCollider.enabled = false;
        yield return new WaitForSecondsRealtime(0.5f);
        GameManager.Instance.Explode();
    }
}


