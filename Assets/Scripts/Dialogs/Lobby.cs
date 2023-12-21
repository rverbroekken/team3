using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class Lobby : MonoBehaviour
{
    public CanvasGroup m_Background;
    public Button m_StartButton;
    public Button m_SettingsButton;
    public Button m_ShopButton;
    public Button m_ClanButton;
    public TextMeshProUGUI m_Description;
    public TextMeshProUGUI m_LevelId;
    public TextMeshProUGUI m_BankLevelId;
    public TextMeshProUGUI m_BankStatus;

    public void Show(LevelData levelData, PlayerData playerData, bool instant)
    {
        m_LevelId.SetText((playerData.currentLevelIdx+1).ToString());
        m_Description.SetText(levelData.description);
        m_BankLevelId.SetText((playerData.piggybankLevelIdx+1).ToString());
        m_BankStatus.SetText($"{playerData.piggybankLevelState}/{PlayerData.piggybankScoreForNextLevel}");

        if (instant)
        {
            m_Background.alpha = 1f;
            gameObject.SetActive(true);
        }
        else
        {
            m_Background.alpha = 0f;
            gameObject.SetActive(true);

            Sequence s = DOTween.Sequence(this);
            s.Append(m_Background.DOFade(1, 0.15f));
        }
    }

    public void OnSettings()
    {
        GameManager.Instance.OpenSettings();
    }

    public void OnPlay()
    {
        Hide();
        GameManager.Instance.NewGame();
    }

    public void OnShop()
    {
    }

    public void OnClan()
    {
    }

    public void Hide()
    {
        DOTween.Kill(this);
        gameObject.SetActive(false);
    }
}


