using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using System.Linq;
//using UnityEngine.Experimental.Rendering;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;

public static class ExtensionMethods
{
    public static Texture2D toTexture2D(this RenderTexture rTex)
    {
        Texture2D tex = new Texture2D(rTex.width, rTex.height, TextureFormat.ARGB32, false);

        var old_rt = RenderTexture.active;
        RenderTexture.active = rTex;

        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        tex.Apply();

        RenderTexture.active = old_rt;
        return tex;
    }


    public static void Move<T>(this List<T> list, int oldIndex, int newIndex)
    {
        T item = list[oldIndex];
        list.RemoveAt(oldIndex);
        list.Insert(newIndex, item);
    }

    public static Vector3 SetX(this Vector3 pos, float x)
    {
        return new Vector3(x, pos.y, pos.z);
    }

    public static void SetLocalX(this Transform t, float x)
    {
        t.localPosition = t.localPosition.SetX(x);
    }

    public static TweenerCore<Vector3, Vector3, VectorOptions> DOLocalMoveXAtSpeed(this Transform t, float destX, float speedPer100)
    {
        float d = Mathf.Abs(t.localPosition.x - destX);
        var duration = (d / 100) * speedPer100;
        return t.DOLocalMoveX(destX, duration);
    }
    
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private Camera camera;
    [SerializeField] private Spawner spawner;

    [SerializeField] private Text scoreText;
    [SerializeField] private Image fadeImage;

    [SerializeField] private RenderTexture renderTextureDescriptor;

    [SerializeField] private Tray tray;
    [SerializeField] private MissionsWidget missionsWidget;

    private float prev_aspect;
    private AudioSource audioData;

    private int score;
    public int Score => score;

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
            IncreaseScore(fruit.points);
            if (missionsWidget.MatchMade(fruit.type))
            {
                // all missions done
                Explode();
                return;
            }
        }

        if (tray.IsFull())
        {
            Explode();
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

        audioData.Play(0);

        missionsWidget.AddMission("Apple", 5, spawner.GetTextureByType("Apple"));
        missionsWidget.AddMission("Bananna", 3, spawner.GetTextureByType("Bananna"));
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
