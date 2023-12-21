using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using sgg;
//using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private Camera mainCamera;
    [SerializeField] private Spawner spawner;
    [SerializeField] private Canvas backCanvas;
    [SerializeField] private Canvas frontCanvas;

    [SerializeField] private RenderTexture renderTextureDescriptor;

    [Header("Items")]
    [SerializeField] private Text timerText;
    [SerializeField] private Tray tray;
    [SerializeField] private MissionsWidget missionsWidget;

    [Header("Dialogs")]
    [SerializeField] private PlayResult playResultDialog;
    [SerializeField] private Image fadeImage;

    [Header("Levels")]
    [SerializeField] private LevelData[] levels;
    [SerializeField] private int levelIdx = 0; // only for cheating

    private LevelData activeLevelData = null;
    private PlayerData playerData;
    private float prev_aspect;
    private AudioSource audioData;    
    private float levelDuration;

    public Camera MainCamera => mainCamera;
    public Canvas BackCanvas => backCanvas;
    public Canvas FrontCanvas => frontCanvas;

    private int dummyScore = 0;

    private void Awake()
    {
        if (Instance != null) {
            DestroyImmediate(gameObject);
        } else {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        audioData = GetComponent<AudioSource>();
        playResultDialog.gameObject.SetActive(false);
        playerData = PlayerData.Load();
        if (levelIdx == 0)
        {
            levelIdx = playerData.currentLevelIdx;
        }
    }

    public void OnFruitSelect(Fruit fruit)
    {
        if (tray.QueueAddItem(fruit))
        {
            // match of 3 made
            if (!fruit.isGoalItem)
            {
                dummyScore += 1;
            }
        }

        if (tray.IsFull())
        {
            LevelLost();
        }
        else
        {
            if (missionsWidget.ItemToTray(fruit.type))
            {
                // all missions done

                // wait for anim to be done before level end
                levelIdx = Mathf.Min(levels.Length, levelIdx + 1);
                LevelWon();
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

        if (spawner.enabled)
        {
            levelDuration = Mathf.Max(0, levelDuration - Time.deltaTime);
            TimeSpan t = TimeSpan.FromSeconds(levelDuration);
            timerText.text = t.ToString(@"mm\:ss");
            if (levelDuration == 0)
            {
                LevelLost();
            }
        }
    }

    public void NewGame()
    {
        Time.timeScale = 1f;

        ClearScene();

        levelIdx = Mathf.Min(levelIdx, levels.Length - 1);
        activeLevelData = levels[levelIdx];
        levelDuration = activeLevelData.levelTimeInSeconds;

        CreateRenderTextures();

        audioData.Play(0);

        spawner.NewGame(activeLevelData);
        missionsWidget.NewGame(activeLevelData);
        Update();
    }

    void CreateRenderTextures()
    {
        foreach (var fruitData in activeLevelData.fruit)
        {
            if (fruitData)
            {
                RenderTexture t = new RenderTexture(renderTextureDescriptor);
                var item = Instantiate(fruitData.fruit, new Vector3(40f, -20f, 0f), Quaternion.identity);
                item.Enable();
                var rigidbody = item.GetComponent<Rigidbody>();
                rigidbody.Sleep();
                item.itemCamera.gameObject.SetActive(true);
                item.itemCamera.enabled = true;
                item.itemCamera.targetTexture = t;
                item.itemCamera.Render();
                (fruitData.fruit as Fruit).texture = t.toTexture2D();
                DestroyImmediate(item.gameObject);
            }
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

        spawner.Clear();
        missionsWidget.Clear();
        tray.Clear();

        dummyScore = 0;
    }

    private void DisableLevel()
    {
        Item[] items = FindObjectsOfType<Item>();
        foreach (Item item in items)
        {
            item.Disable();
        }
        spawner.enabled = false;
    }

    public void LevelLost()
    {
        DisableLevel();
        StartCoroutine(ExplodeSequence());
    }

    private void LevelWon()
    {
        DisableLevel();

        int numStars = ((int)levelDuration / (activeLevelData.levelTimeInSeconds / 3)) + 1;
        playerData.LevelWon(levelIdx - 1, numStars, dummyScore);

        StartCoroutine(LevelWonSequence());
    }

    private IEnumerator LevelWonSequence()
    {
        while (tray.IsAnimating)
        {
            yield return null;
        }

        yield return new WaitForSecondsRealtime(0.3f);

        playResultDialog.Show(playerData, activeLevelData.description);

        activeLevelData = null;
    }

    private IEnumerator ExplodeSequence()
    {
        while (tray.IsAnimating)
        {
            yield return null;
        }

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

        activeLevelData = null;

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
