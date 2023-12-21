using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using sgg;

[RequireComponent(typeof(Collider))]
public class Spawner : MonoBehaviour
{
    private Collider spawnArea;

    public List<Item> fruitPrefabs = new List<Item>();
    public List<Item> bombPrefabs = new List<Item>();

    public List<Item> dummyItems = new List<Item>();
    public List<Item> missionItems = new List<Item>();


    [Range(0f, 1f)] public float bombChance = 0.05f;

    public float startY = -12f;
    public bool paused = false;

    public float minSpawnDelay = 0.25f;
    public float maxSpawnDelay = 1f;

    private int numLayers = 100;
    private float minX = -8f;
    private float maxX = 8f;
    private int zIndex = 0;

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
        paused = false;
        minSpawnDelay = levelData.minSpawnDelay;
        maxSpawnDelay = levelData.maxSpawnDelay;
        minX = levelData.minSpawnX;
        maxX = levelData.maxSpawnX;
        foreach (var fruitData in levelData.fruit)
        {
            if (fruitData)
            {
                fruitData.fruit.itemData = fruitData;
                fruitData.fruit.isGoalItem = fruitData.isGoalItem;
                fruitPrefabs.Add(fruitData.fruit);

                for (var i = 0; i < fruitData.amount * 3; i++)
                {
                    var item = Instantiate(fruitData.fruit, Vector3.one, Quaternion.identity);
                    item.OnSelect += GameManager.Instance.OnFruitSelect;
                    if (fruitData.isGoalItem)
                    {
                        missionItems.Add(item);
                    }
                    else
                    {
                        dummyItems.Add(item);
                    }
                }
            }
        }
        foreach (var bombData in levelData.bombs)
        {
            if (bombData)
            {
                bombData.bomb.itemData = bombData;
                bombPrefabs.Add(bombData.bomb);

                for (var i = 0; i < bombData.amount; i++)
                {
                    var item = Instantiate(bombData.bomb, Vector3.one, Quaternion.identity);
                    item.OnSelect += GameManager.Instance.OnFruitSelect;
                    dummyItems.Add(item);
                }

            }
        }
        enabled = true;
    }

    public void Clear()
    {
        dummyItems.Clear();
        fruitPrefabs.Clear();
        bombPrefabs.Clear();
        missionItems.Clear();
        enabled = false;
    }

    private IEnumerator Spawn()
    {
        yield return new WaitForSeconds(2f);

        var canvasZ = GameManager.Instance.FrontCanvas.transform.position.z + 3f;
        while (enabled)
        {
            if (paused || (dummyItems.Count == 0 && missionItems.Count == 0))
            {
                yield return null;
                continue;
            }

            Vector3 position = new Vector3
            {
                x = Random.Range(minX, maxX),
                y = spawnArea.bounds.max.y,
                z = (zIndex * -3) - canvasZ
            };
            zIndex = (zIndex + 1) % numLayers;

            var f = Math.InverseLerpUnclamped(spawnArea.bounds.min.x, spawnArea.bounds.max.x, position.x);
            var minAngle = Mathf.LerpUnclamped(-8f, 2f, f);
            var maxAngle = Mathf.LerpUnclamped(-1f, 8f, f);

            Quaternion rotation = Quaternion.Euler(0f, 0f, Random.Range(minAngle, maxAngle));

            var items = dummyItems;
            if (dummyItems.Count == 0 || (Random.Range(0, 2) == 0 && missionItems.Count > 0))
            {
                items = missionItems;
            }
            var itemIdx = Random.Range(0, items.Count);
            var item = items[itemIdx];
            if (!item)
            {
                yield return null;
                continue;
            }
            items.RemoveAt(itemIdx);
            item.transform.position = position;
            item.transform.rotation = rotation;
            item.YValueForEvent = spawnArea.bounds.min.y;
            item.OnOutOfScreen = () => {             
                item.Disable(true);
                items.Add(item); };
            item.Enable();
            item.StartFly();

            float force = Random.Range(item.itemData.minForce, item.itemData.maxForce);
            var body = item.GetComponent<Rigidbody>();
            body.ResetInertiaTensor();
            body.AddForce(item.transform.up * force, ForceMode.Impulse);

            var xr = Random.Range(item.itemData.minXRotation * 1000, item.itemData.maxXRotation * 1000) / 1000f;
            var yr = Random.Range(item.itemData.minYRotation * 1000, item.itemData.maxYRotation * 1000) / 1000f;
            var zr = Random.Range(item.itemData.minZRotation * 1000, item.itemData.maxZRotation * 1000) / 1000f;
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
