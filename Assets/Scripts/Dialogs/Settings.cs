using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class Settings : MonoBehaviour
{
    public CanvasGroup m_Background;
    public Button m_DeleteButton;
    public Button m_CloseButton;

    public Slider m_MusicVolume;
    public Slider m_FXVolume;

    public void Show()
    {
        m_Background.alpha = 0f;
        gameObject.SetActive(true);

        Sequence s = DOTween.Sequence(this);
        s.Append(m_Background.DOFade(1, 0.15f));
    }

    public void OnDelete()
    {
        AudioManager.Instance.Play("button01");
        Hide();
        GameManager.Instance.ResetSaveGame();
    }

    public void OnClose()
    {
        AudioManager.Instance.Play("button01");
        Hide();
    }

    public void Hide()
    {
        DOTween.Kill(this);
        gameObject.SetActive(false);
    }
}


