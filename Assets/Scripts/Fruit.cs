using UnityEngine;

public class Fruit : Item
{
    public int points = 1;
    public RenderTexture texture;

    protected override void OnItemSelect()
    {
        GameManager.Instance.IncreaseScore(points);
        juiceEffect.Play();
        fruitCollider.enabled = false;
        //        outline.enabled = true;
        //        camera.enabled = true;
        //        yield return new WaitForSecondsRealtime(0.1f);
        whole.SetActive(false);

        OnSelect?.Invoke(this);
    }

}
