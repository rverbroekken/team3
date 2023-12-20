using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MissionWidget : MonoBehaviour
{
    public RawImage fuitTypeImage;
    public TextMeshProUGUI amountLabel;

    private string fruitType;
    private int amount = 0;

    void Awake()
    {
    }

    public void Init(string fruitType, int amount, Texture2D fuitTypeTexture)
    {
        this.fruitType = fruitType;
        this.amount = amount;
        if (fuitTypeTexture)
        {
            fuitTypeImage.texture = fuitTypeTexture;
        }
        UpdateValues();
    }

    public bool ItemToTray(string type)
    {
        if (type == fruitType)
        {
            this.amount -= 1;
            UpdateValues();
        }
        return this.amount <= 0;
    }

    public void ItemFromTray(string type)
    {
        if (type == fruitType)
        {
            this.amount += 1;
            UpdateValues();
        }
    }
    
    private void UpdateValues()
    {
        amountLabel.text = amount.ToString();
    }

}
