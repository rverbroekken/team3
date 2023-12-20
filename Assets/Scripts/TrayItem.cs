using UnityEngine;
using UnityEngine.UI;

public class TrayItem : MonoBehaviour
{
    [HideInInspector] public string fruitType;
    private RawImage fruitImage;

    void Awake()
    {
        fruitImage = GetComponent<RawImage>();
    }

    public void SetFruit(Texture2D texture, string type)
    {
        if (texture)
        {
            if (!fruitImage)
            {
                fruitImage = GetComponent<RawImage>();
            }
            fruitImage.texture = texture;
            fruitType = type;
        }
    }

    public void ResetItem()
    {
        fruitType = null;
        gameObject.SetActive(false);
        if (fruitImage)
        {
            fruitImage.texture = null;
        }
    }

    void OnMouseDown()
    {
    }

}
