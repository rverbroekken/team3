using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class Pause : MonoBehaviour
{
    public CanvasGroup m_Background;
    public Button m_RestartButton;
    public Button m_ContinueButton;
    public Button m_QuitButton;
    public Button m_SettingsButton;
    public TextMeshProUGUI m_Description;

    public void Show(LevelData levelData)
    {
        m_Description.SetText(levelData.description);

        m_Background.alpha = 0f;
        gameObject.SetActive(true);

        Sequence s = DOTween.Sequence(this);
        s.Append(m_Background.DOFade(1, 0.15f));
    }

    public void OnSettings()
    {
        GameManager.Instance.OpenSettings();
    }

    public void OnQuit()
    {
        AudioManager.Instance.Play("button01");
        Hide();
        GameManager.Instance.ToMainScreen(true);
    }

    public void OnRestart()
    {
        AudioManager.Instance.Play("button01");
        Hide();
        GameManager.Instance.NewGame();
    }

    public void OnContinue()
    {
        AudioManager.Instance.Play("button01");
        GameManager.Instance.Pause(false);
    }

    public void Hide()
    {
        DOTween.Kill(this);
        gameObject.SetActive(false);
    }
}


