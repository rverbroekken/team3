using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private Camera camera;
    [SerializeField] private Spawner[] spawners;

    [SerializeField] private Text scoreText;
    [SerializeField] private Image fadeImage;

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
            foreach (var s in spawners)
            {
                s.transform.position = new Vector3(s.transform.position.x, s.startY + (10f - size), s.transform.position.z);
            }
        }
    }

    private void NewGame()
    {
        Time.timeScale = 1f;

        ClearScene();

        foreach (var s in spawners)
        { 
            s.enabled = true;
        }

        score = 0;
        scoreText.text = score.ToString();
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
        foreach (var s in spawners)
        {
            s.enabled = false;
        }
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
