using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Spawner : MonoBehaviour
{
    private Collider spawnArea;

    public GameObject[] fruitPrefabs;
    public GameObject bombPrefab;
    [Range(0f, 1f)] public float bombChance = 0.05f;

    public float startY = -15f;
    public float minSpawnDelay = 0.25f;
    public float maxSpawnDelay = 1f;

    public float minX = -5f;
    public float maxX = 5f;

    public float minForce = 18f;
    public float maxForce = 22f;

    public float minXRotation = -1f;
    public float minYRotation = -1f;
    public float minZRotation = -1f;
    public float maxXRotation = 1f;
    public float maxYRotation = 1f;
    public float maxZRotation = 1f;

    public float maxLifetime = 5f;

    private int index = 0;

    private void Awake()
    {
        spawnArea = GetComponent<BoxCollider>();
    }

    private void OnEnable()
    {
        StartCoroutine(Spawn());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator Spawn()
    {
        yield return new WaitForSeconds(2f);

        while (enabled)
        {
            GameObject prefab = fruitPrefabs[Random.Range(0, fruitPrefabs.Length)];
            if (Random.value < bombChance) 
            {
                prefab = bombPrefab;
            }

            Vector3 position = new Vector3
            {
                x = Random.Range(spawnArea.bounds.min.x, spawnArea.bounds.max.x),
                y = Random.Range(spawnArea.bounds.min.y, spawnArea.bounds.max.y),
                z = index * -4f
            };

            index = (index + 1) % 100;

            var f = Mathf.InverseLerp(minX, maxX, position.x);
            var minAngle = Mathf.Lerp(-7, 0, f);
            var maxAngle = Mathf.Lerp(0, 7, f);

            Quaternion rotation = Quaternion.Euler(0f, 0f, Random.Range(minAngle, maxAngle));

            GameObject fruit = Instantiate(prefab, position, rotation);
            Destroy(fruit, maxLifetime);

            float force = Random.Range(minForce, maxForce);
            var body = fruit.GetComponent<Rigidbody>();
            body.AddForce(fruit.transform.up * force, ForceMode.Impulse);

            var xr = Random.Range(minXRotation * 1000, maxXRotation * 1000) / 1000f;
            var yr = Random.Range(minYRotation * 1000, maxYRotation * 1000) / 1000f;
            var zr = Random.Range(minZRotation * 1000, maxZRotation * 1000) / 1000f;
            body.AddRelativeTorque(new Vector3(xr, yr, zr), ForceMode.Impulse);

            yield return new WaitForSeconds(Random.Range(minSpawnDelay, maxSpawnDelay));
        }
    }

}
