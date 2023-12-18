using UnityEngine;
using System.Collections;

public class Fruit : MonoBehaviour
{
    public GameObject whole;
    public Camera camera;

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
//        outline.enabled = true;
//        camera.enabled = true;
//        yield return new WaitForSecondsRealtime(0.1f);
        whole.SetActive(false);
        yield return null;
    }

}
