using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Spawner : MonoBehaviour
{
    private Collider spawnArea;

    public List<Item> fruitPrefabs = new List<Item>();
    public List<Item> bombPrefabs = new List<Item>();

    [Range(0f, 1f)] public float bombChance = 0.05f;

    public float startY = -15f;

    public float minSpawnDelay = 0.25f;
    public float maxSpawnDelay = 1f;

    private float maxLifetime = 5f;
    private int numLayers = 100;
    private float minX = -5f;
    private float maxX = 5f;
    private int index = 0;

//    private LevelData LevelData;

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

    public void NewGame(LevelData levelData)
    {
        minSpawnDelay = levelData.minSpawnDelay;
        maxSpawnDelay = levelData.maxSpawnDelay;
        foreach (var fruitData in levelData.fruit)
        {
            if (fruitData)
            {
                fruitPrefabs.Add(fruitData.fruit);
            }
        }
        foreach (var bombData in levelData.bombs)
        {
            if (bombData)
            {
                bombPrefabs.Add(bombData.bomb);
            }
        }
        enabled = true;
    }

    public void Clear()
    {
        fruitPrefabs.Clear();
        bombPrefabs.Clear();
        enabled = false;
    }

    private IEnumerator Spawn()
    {
        yield return new WaitForSeconds(2f);

        
        while (enabled)
        {
            var prefab = fruitPrefabs[Random.Range(0, fruitPrefabs.Count)];
            if (Random.value < bombChance && bombPrefabs.Count > 0) 
            {
                prefab = bombPrefabs[Random.Range(0, bombPrefabs.Count)];
            }

            Vector3 position = new Vector3
            {
                x = Random.Range(spawnArea.bounds.min.x, spawnArea.bounds.max.x),
                y = Random.Range(spawnArea.bounds.min.y, spawnArea.bounds.max.y),
                z = (index) * -3f
            };

            index = (index + 1) % numLayers;

            var f = Mathf.InverseLerp(minX, maxX, position.x);
            var minAngle = Mathf.Lerp(-7, 0, f);
            var maxAngle = Mathf.Lerp(0, 7, f);

            Quaternion rotation = Quaternion.Euler(0f, 0f, Random.Range(minAngle, maxAngle));

            var item = Instantiate(prefab, position, rotation);
            item.OnSelect += GameManager.Instance.OnFruitSelect;

            Destroy(item.gameObject, maxLifetime);

            float force = Random.Range(item.minForce, item.maxForce);
            var body = item.GetComponent<Rigidbody>();
            body.AddForce(item.transform.up * force, ForceMode.Impulse);

            var xr = Random.Range(item.minXRotation * 1000, item.maxXRotation * 1000) / 1000f;
            var yr = Random.Range(item.minYRotation * 1000, item.maxYRotation * 1000) / 1000f;
            var zr = Random.Range(item.minZRotation * 1000, item.maxZRotation * 1000) / 1000f;
            body.AddRelativeTorque(new Vector3(xr, yr, zr), ForceMode.Impulse);

            yield return new WaitForSeconds(Random.Range(minSpawnDelay, maxSpawnDelay));
        }
    }

    public Texture2D GetTextureByType(string type)
    {
        foreach(var fruit in fruitPrefabs)
        {
            if (fruit.type == type)
            {
                return (fruit as Fruit).texture;
            }
        }
        return null;
    }

}
