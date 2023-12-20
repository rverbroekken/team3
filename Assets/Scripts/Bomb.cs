using UnityEngine;
using System.Collections;

public class Bomb : Item
{
//    private Item item;


    protected override void OnItemSelect()
    {
        StartCoroutine(Explode());
    }

    private IEnumerator Explode()
    {
        outline.enabled = true;
        fruitCollider.enabled = false;
        whole.SetActive(false);

        juiceEffect.Play();
        yield return new WaitForSecondsRealtime(0.5f);
        GameManager.Instance.LevelLost();
    }
}


