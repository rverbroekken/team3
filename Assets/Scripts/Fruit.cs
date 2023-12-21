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
//        whole.SetActive(false);
        OnSelect?.Invoke(this);
        //Destroy(gameObject, 2f);
    }

    public void Remove()
    {
        Destroy(gameObject);
    }

}
