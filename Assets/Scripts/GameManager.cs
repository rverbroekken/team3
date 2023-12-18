using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Experimental.Rendering;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private Camera camera;
    [SerializeField] private Spawner spawner;

    [SerializeField] private Text scoreText;
    [SerializeField] private Image fadeImage;

    [SerializeField] private RenderTexture renderTextureDescriptor;

    [SerializeField] private Tray tray;

    private int score;
    public int Score => score;

    private float prev_aspect;
    private void Awake()
    {
        if (Instance != null) {
            DestroyImmediate(gameObject);
        } else {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void OnFruitSelect(Fruit fruit)
    {
        if (tray.IsFull())
        {
            Explode();
        }
        else
        {
            tray.AddItem(fruit);
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
            camera.orthographicSize = size;
            spawner.transform.position = new Vector3(spawner.transform.position.x, spawner.startY + (10f - size), spawner.transform.position.z);
        }
    }

    private void NewGame()
    {
        Time.timeScale = 1f;

        ClearScene();
        CreateRenderTextures();

        spawner.enabled = true;

        score = 0;
        scoreText.text = score.ToString();
    }

    void CreateRenderTextures()
    {
        float x = 0;
        foreach (var fruit in spawner.fruitPrefabs)
        {
            RenderTexture t = new RenderTexture(renderTextureDescriptor);
            var item = Instantiate(fruit, new Vector3(40f + x, -40f, 0f), Quaternion.identity);
            var rigidbody = item.GetComponent<Rigidbody>();
            rigidbody.useGravity = false;
            rigidbody.mass = 0;
            item.camera.targetTexture = t;
            item.camera.Render();
            (fruit as Fruit).texture = t;
            x += 5f;
//            tray.AddTexture(t);
//            Destroy(item.gameObject);
//          break;
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

        tray.Clear();
    }

    public void IncreaseScore(int points)
    {
        score += points;
        scoreText.text = score.ToString();

        float hiscore = PlayerPrefs.GetFloat("hiscore", 0);

        if (score > hiscore)
        {
            hiscore = score;
            PlayerPrefs.SetFloat("hiscore", hiscore);
        }
    }

    public void Explode()
    {
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
