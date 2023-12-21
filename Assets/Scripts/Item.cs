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

    /*
        [Header("Rotation Settings")]
        public float minXRotation = -1f;
        public float minYRotation = -1f;
        public float minZRotation = -1f;
        public float maxXRotation = 1f;
        public float maxYRotation = 1f;
        public float maxZRotation = 1f;

        [Header("Force Settings")]
        public float minForce = 16f;
        public float maxForce = 21f;
    */

    [HideInInspector] public ItemData itemData;
    [HideInInspector] public Vector3 originalScale;

    protected Rigidbody fruitRigidbody;
    protected Collider fruitCollider;
    protected ParticleSystem juiceEffect;
    protected Outline outline;

    public Action<Fruit> OnSelect;
    public Action OnOutOfScreen;
    [HideInInspector] public float YValueForEvent;

    void Awake()
    {
        fruitRigidbody = GetComponent<Rigidbody>();
        fruitCollider = GetComponentInChildren<Collider>();
        outline = GetComponent<Outline>();
        juiceEffect = GetComponentInChildren<ParticleSystem>();
        originalScale = transform.localScale;
        outline.enabled = false;
        if (itemCamera)
        {
            itemCamera.enabled = false;
            itemCamera.gameObject.SetActive(false);
        }
        if (type == "")
        {
            type = name;
        }

        Disable(true);
        OnAwake();
    }

    protected virtual void OnAwake() {}

    void OnMouseDown()
    {
        OnItemSelect();
    }

    private void Update()
    {
        if (fruitCollider.enabled)
        {
            if (transform.position.y < YValueForEvent)
            {
                OnOutOfScreen?.Invoke();
            }
        }
    }

    public void Disable(bool deactivate = false)
    {
        fruitRigidbody.Sleep();

        fruitCollider.enabled = false;
        if (deactivate)
        {
            gameObject.SetActive(false);
        }
    }
    public void Enable()
    {
        fruitRigidbody.WakeUp();
        fruitCollider.enabled = true;
        gameObject.SetActive(true);
        fruitRigidbody.velocity = Vector3.zero;
        fruitRigidbody.angularVelocity = Vector3.zero;
    }

    public void EnableOutline(bool enable)
    {
        outline.enabled = enable;
    }

    protected virtual void OnItemSelect() { }

}
