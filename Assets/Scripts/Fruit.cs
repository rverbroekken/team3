using UnityEngine;

public class Fruit : Item
{
    [HideInInspector] public Texture2D texture;

    protected override void OnItemSelect()
    {
        juiceEffect.Play();
        fruitCollider.enabled = false;
        whole.SetActive(false);
        OnSelect?.Invoke(this);
    }

}
