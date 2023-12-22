using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using sgg;
using TMPro;
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
    [SerializeField] private TextMeshProUGUI piggyBankLevelStatus;
    [SerializeField] private TextMeshProUGUI freezeTimer;
    [SerializeField] private Button freezeButton;
    [SerializeField] private TextMeshProUGUI health;


    [Header("Dialogs")]
    [SerializeField] private PlayResult playResultDialog;
    [SerializeField] private Pause pauseDialog;
    [SerializeField] private Lobby lobby;
    [SerializeField] private Settings settings;
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

        spawner.enabled = false;

        ToMainScreen(true);
    }

    public void OnFruitSelect(Fruit fruit)
    {
        if (tray.QueueAddItem(fruit))
        {
            // match of 3 made
            if (!fruit.isGoalItem)
            {
                dummyScore += 1;
                piggyBankLevelStatus.SetText(dummyScore.ToString());
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

        if (spawner.IsActive)
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

    public void FreezeBooster()
    {
        StartCoroutine(CoFreezeBooster());
    }

    private IEnumerator CoFreezeBooster()
    {
        freezeTimer.gameObject.SetActive(true);
        spawner.freeze = true;
        Item[] items = FindObjectsOfType<Item>();
        foreach (Item item in items)
        {
            item.Disable(false);
            item.FruitCollider.enabled = true;
        }

        for (int i = 0; i <= 3; i++)
        {
            freezeTimer.SetText((3 - i).ToString());
            yield return new WaitForSecondsRealtime(1f);
        }

        spawner.freeze = false;
        items = FindObjectsOfType<Item>();
        foreach (Item item in items)
        {
            item?.Enable();
        }
        freezeTimer.gameObject.SetActive(false);
        freezeButton.interactable = false;
    }


    public void ResetSaveGame()
    {
        PlayerData.Reset();
        playerData = PlayerData.Load();
        levelIdx = 0;
        ToMainScreen(true);
    }

    public void Pause(bool pause)
    {
        spawner.paused = pause;
        Item[] items = FindObjectsOfType<Item>();
        if (pause)
        {
            foreach (Item item in items)
            {
                item.Disable(false);
            }

            pauseDialog.Show(activeLevelData);
        }
        else
        {
            foreach (Item item in items)
            {
                item.Enable();
            }
            pauseDialog.Hide();
        }
    }

    public void ToMainScreen(bool instant)
    {
        ClearScene();
        pauseDialog.Hide();
        playResultDialog.Hide();
        lobby.Show(levels[Mathf.Min(levelIdx, levels.Length - 1)], playerData, instant);
    }

    public void OpenSettings()
    {
        settings.Show();
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

        health.SetText(playerData.currentHealth.ToString());
        spawner.NewGame(activeLevelData);
        missionsWidget.NewGame(activeLevelData);
        freezeButton.interactable = true;
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

        spawner.enabled = false;
        spawner.Clear();
        missionsWidget.Clear();
        freezeTimer.gameObject.SetActive(false);
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
        playerData.LevelLost();
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

        yield return new WaitForSecondsRealtime(1f);

        ToMainScreen(false);
    }

}
