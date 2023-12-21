using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class PlayResult : MonoBehaviour
{
    public CanvasGroup m_Background;
    public Button m_ContinueButton;
    public RectTransform m_Star1;
    public RectTransform m_Star2;
    public RectTransform m_Star3;
    public TextMeshProUGUI m_Header;
    public TextMeshProUGUI m_Description;
    public TextMeshProUGUI m_PiggyBankLevel;
    public TextMeshProUGUI m_PiggyBankProgress;
    public Slider m_PiggyBankProgressSlider;

    public void Show(PlayerData playerData, string description)
    {
        m_Header.SetText($"Level {playerData.currentLevelIdx+1} won!!!");
        m_Description.SetText(description);

        m_Star1.gameObject.SetActive(false);
        m_Star2.gameObject.SetActive(false);
        m_Star3.gameObject.SetActive(false);

        m_Background.alpha = 0f;
        gameObject.SetActive(true);

        Sequence s = DOTween.Sequence(this);
        s.Append(m_Background.DOFade(1, 0.15f));
        s.AppendInterval(0.5f);
        s.AppendCallback(() => m_Star1.gameObject.SetActive(true));

        var numStars = playerData.GetLastLevelState().numStars;
        if (numStars > 1)
        {
            s.AppendInterval(0.5f);
            s.AppendCallback(() => m_Star2.gameObject.SetActive(true));
        }
        if (numStars > 2)
        {
            s.AppendInterval(0.5f);
            s.AppendCallback(() => m_Star3.gameObject.SetActive(true));
        }

        m_PiggyBankLevel?.SetText((playerData.piggybankLevelIdx+1).ToString());
        m_PiggyBankProgress?.SetText($"{playerData.piggybankLevelState}/{PlayerData.piggybankScoreForNextLevel}");
    }

    public void Hide()
    {
        DOTween.Kill(this);
        gameObject.SetActive(false);
    }

    public void OnContinue()
    {
        Hide();
        GameManager.Instance.NewGame();
    }
}


