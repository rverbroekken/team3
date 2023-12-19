using UnityEngine;

public class Fruit : Item
{
    public int points = 1;
    [HideInInspector] public Texture2D texture;

    protected override void OnItemSelect()
    {
        juiceEffect.Play();
        fruitCollider.enabled = false;
        whole.SetActive(false);
        OnSelect?.Invoke(this);
    }

}
