using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using System.Linq;
//using UnityEngine.Experimental.Rendering;

/*
[CreateAssetMenu(menuName = "Levels/LevelData")]
public class LevelData : ScriptableObject
{
    [NestedScriptableObjectField]
    public NestedScriptableObject field;
    [NestedScriptableObjectList]
    public List<NestedScriptableObject> list = new List<NestedScriptableObject>();
}
*/

public abstract class NestedScriptableObject : ScriptableObject { }
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private Camera mainCamera;
    [SerializeField] private Spawner spawner;

    [SerializeField] private Text scoreText;
    [SerializeField] private Image fadeImage;

    [SerializeField] private RenderTexture renderTextureDescriptor;

    [SerializeField] private Tray tray;
    [SerializeField] private MissionsWidget missionsWidget;

    [Header("Levels")]
    [SerializeField] private LevelData[] levels;

    private float prev_aspect;
    private AudioSource audioData;    
    
    private void Awake()
    {
        if (Instance != null) {
            DestroyImmediate(gameObject);
        } else {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        audioData = GetComponent<AudioSource>();
    }

    public void OnFruitSelect(Fruit fruit)
    {
        if (tray.QueueAddItem(fruit))
        {
            // match of 3 made

        }

        if (tray.IsFull())
        {
            Explode();
        }
        else
        {
            if (missionsWidget.ItemToTray(fruit.type))
            {
                // all missions done

                // wait for anim to be done before level end
                Explode();
                return;
            }
        }
    }

    private void Start()
    {
        NewGame();
    }

    private void Update()
    {
        var aspect = Screen.width / (float)Screen.height;
        if (aspect != prev_aspect)
        {
            prev_aspect = aspect;

            var f = Mathf.InverseLerp(9f / 16f, 9 / 20f, aspect);
            var size = Mathf.Lerp(10, 12.5f, f);
            mainCamera.orthographicSize = size;
            spawner.transform.position = new Vector3(spawner.transform.position.x, spawner.startY + (10f - size), spawner.transform.position.z);
        }
    }

    private void NewGame()
    {
        Time.timeScale = 1f;

        ClearScene();
        CreateRenderTextures();

        spawner.enabled = true;

        audioData.Play(0);

        missionsWidget.AddMission("apple", 5, spawner.GetTextureByType("apple"));
//        missionsWidget.AddMission("Bananna", 3, spawner.GetTextureByType("Bananna"));
    }

    void CreateRenderTextures()
    {
        foreach (var fruit in spawner.fruitPrefabs)
        {
            RenderTexture t = new RenderTexture(renderTextureDescriptor);
            var item = Instantiate(fruit, new Vector3(40f, -40f, 0f), Quaternion.identity);
            var rigidbody = item.GetComponent<Rigidbody>();
            rigidbody.useGravity = false;
            rigidbody.mass = 0;
            item.itemCamera.gameObject.SetActive(true);
            item.itemCamera.targetTexture = t;
            item.itemCamera.Render();
            (fruit as Fruit).texture = t.toTexture2D();
            DestroyImmediate(item.gameObject);
        }
    }

    private void ClearScene()
    {
        Fruit[] fruits = FindObjectsOfType<Fruit>();

        foreach (Fruit fruit in fruits) {
            Destroy(fruit.gameObject);
        }

        Bomb[] bombs = FindObjectsOfType<Bomb>();

        foreach (Bomb bomb in bombs) {
            Destroy(bomb.gameObject);
        }

        missionsWidget.Clear();
        tray.Clear();
    }

    public void Explode()
    {
        Item[] items = FindObjectsOfType<Item>();
        foreach (Item item in items)
        {
            item.Disable();
        }

        spawner.enabled = false;
        StartCoroutine(ExplodeSequence());

    }

    private IEnumerator ExplodeSequence()
    {
        float elapsed = 0f;
        float duration = 0.5f;

        // Fade to white
        while (elapsed < duration)
        {
            float t = Mathf.Clamp01(elapsed / duration);
            fadeImage.color = Color.Lerp(Color.clear, Color.white, t);

            Time.timeScale = 1f - t;
            elapsed += Time.unscaledDeltaTime;

            yield return null;
        }

        yield return new WaitForSecondsRealtime(1f);

        NewGame();

        elapsed = 0f;

        // Fade back in
        while (elapsed < duration)
        {
            float t = Mathf.Clamp01(elapsed / duration);
            fadeImage.color = Color.Lerp(Color.white, Color.clear, t);

            elapsed += Time.unscaledDeltaTime;

            yield return null;
        }
    }

}
