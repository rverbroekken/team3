using UnityEngine;

public class Fruit : Item
{
    [HideInInspector] public Texture2D texture;
    [HideInInspector] public bool isGoalItem;

    protected override void OnItemSelect()
    {
        base.OnItemSelect();
        juiceEffect.Play();
        fruitCollider.enabled = false;        
        fruitRigidbody.Sleep();
        OnSelect?.Invoke(this);
    }

    public void Remove()
    {
        Destroy(gameObject);
    }

}
