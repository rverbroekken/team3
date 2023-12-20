using UnityEngine;
using System;

public class Item : MonoBehaviour
{
    public GameObject whole;
    public Camera itemCamera;
    public string type = "";

    [Header("Sound Settings")]
    public AudioClip m_SpawnSound;
    public AudioClip m_TapSound;

    [Header("Rotation Settings")]
    public float minXRotation = -1f;
    public float minYRotation = -1f;
    public float minZRotation = -1f;
    public float maxXRotation = 1f;
    public float maxYRotation = 1f;
    public float maxZRotation = 1f;

    [Header("Force Settings")]
    public float minForce = 18f;
    public float maxForce = 22f;


    [HideInInspector] public Vector3 originalScale;

    protected Rigidbody fruitRigidbody;
    protected Collider fruitCollider;
    protected ParticleSystem juiceEffect;
    protected Outline outline;

    public Action<Fruit> OnSelect;

    void Awake()
    {
        fruitRigidbody = GetComponent<Rigidbody>();
        fruitCollider = GetComponent<Collider>();
        outline = GetComponent<Outline>();
        juiceEffect = GetComponentInChildren<ParticleSystem>();
        originalScale = transform.localScale;
        outline.enabled = false;
        if (itemCamera)
        {
            itemCamera.gameObject.SetActive(false);
        }
        if (type == "")
        {
            type = name;
        }

        OnAwake();
    }

    protected virtual void OnAwake() {}

    void OnMouseDown()
    {
        OnItemSelect();
    }

    public void Disable()
    {
        fruitRigidbody.Sleep();
        fruitCollider.enabled = false;
    }

    public void EnableOutline(bool enable)
    {
        outline.enabled = enable;
    }

    protected virtual void OnItemSelect() { }

}
