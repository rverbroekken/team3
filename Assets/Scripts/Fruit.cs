using UnityEngine;

public class Fruit : Item
{
    [HideInInspector] public Texture2D texture;
    [HideInInspector] public bool isGoalItem;

    protected override void OnItemSelect()
    {
        juiceEffect.Play();
        fruitCollider.enabled = false;        
        fruitRigidbody.Sleep();
        OnSelect?.Invoke(this);
        //whole.SetActive(false);
        //Destroy(gameObject, 2f);
    }

    public void Remove()
    {
        Destroy(gameObject);
    }

}
