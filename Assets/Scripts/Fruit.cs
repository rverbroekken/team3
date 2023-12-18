using UnityEngine;
using System.Collections;

public class Fruit : MonoBehaviour
{
    public GameObject whole;

    private Rigidbody fruitRigidbody;
    private Collider fruitCollider;
    private ParticleSystem juiceEffect;
    private Outline outline;

    public int points = 1;

    private void Awake()
    {
        fruitRigidbody = GetComponent<Rigidbody>();
        fruitCollider = GetComponent<Collider>();
        outline = GetComponent<Outline>();
        juiceEffect = GetComponentInChildren<ParticleSystem>();
        outline.enabled = false;
    }

    void OnMouseDown()
    {
        StartCoroutine(OnItemSelect());
    }

    private IEnumerator OnItemSelect()
    {
        GameManager.Instance.IncreaseScore(points);
        juiceEffect.Play();
        fruitCollider.enabled = false;
        outline.enabled = true;

        yield return new WaitForSecondsRealtime(0.5f);

        whole.SetActive(false);
    }

}
