using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using sgg;

public class Tray : MonoBehaviour
{
    public List<TrayItem> trayItems;
    private List<string> finalItems = new List<string>();

    public int currentItemIndexForAnimation = 0;
    public int finalItemIndex = 0;

    public bool IsAnimating => animCounter > 0;

    int animCounter = 0;
    static private float[] xPositions = { -450f, -300f, -150f, 0f, 150f, 300f, 450f };
    const float moveMatchSpeed = .09f;
    const float moveItemSpeed = .09f;
    const float delay = .25f;

    Sequence itemsSequence;

    void Awake()
    {
        foreach (var _ in trayItems)
        {
            finalItems.Add("");
        }
        itemsSequence = DOTween.Sequence();
    }

    void OnMouseDown()
    {
    }

    private (bool result, int index) HasMatch(List<TrayItem> _trayItems, int currentItem)
    {
        int count = 1;
        string activeType = "";
        for (var i = 0; i <= currentItem; i++)
        {
            if (_trayItems[i].fruitType == activeType)
            {
                count += 1;
                if (count >= 3)
                {
                    return (true, i);
                }
            }
            else
            {
                count = 1;
                activeType = _trayItems[i].fruitType;
            }
        }
        return (false, 0);
    }
    
    public bool CalculateCurrentMatch(string type)
    {
        finalItems[finalItemIndex] = type;

        int newItemPos = finalItemIndex;
        for (var i = finalItemIndex - 1; i >= 0; i--)
        {
            if (finalItems[i] == finalItems[finalItemIndex])
            {
                if (i < finalItemIndex - 1)
                {
                    newItemPos = i + 1;
                    finalItems.Move(finalItemIndex, newItemPos);
                }
                break;
            }
        }

        var hasMatch = HasMatch(trayItems, finalItemIndex);
        if (hasMatch.result)
        {
            for (int i = hasMatch.index + 1; i <= finalItemIndex; i++)
            {
                finalItems.Move(i, i - 3);
            }
            finalItemIndex -= 3;
        }
        finalItemIndex += 1;

        return hasMatch.result;
    }
    

    public bool QueueAddItem(Fruit fruit)
    {
        StartCoroutine(CoQueueAddItem(fruit));
        return CalculateCurrentMatch(fruit.type);
    }

    private IEnumerator CoQueueAddItem(Fruit fruit)
    {
        fruit.EnableOutline(true);
        animCounter += 1;
        fruit.clickTime = Time.time;
        while (itemsSequence.active)
        {
            yield return null;
        }
        itemsSequence = DOTween.Sequence();
        AddItem(fruit).AppendCallback(() => animCounter -= 1);
    }

    public Sequence AddItem(Fruit fruit)
    {
        trayItems[currentItemIndexForAnimation].SetFruit(fruit.texture, fruit.type);
        fruit.transform.SetZ(GameManager.Instance.FrontCanvas.transform.position.z - 3f);

        int newItemPos = currentItemIndexForAnimation;
        for (var i = currentItemIndexForAnimation - 1; i >= 0; i--)
        {
            if (trayItems[i].fruitType == trayItems[currentItemIndexForAnimation].fruitType)
            {
                if (i < currentItemIndexForAnimation - 1)
                {
                    newItemPos = i + 1;
                    trayItems.Move(currentItemIndexForAnimation, newItemPos);
                }
                break;
            }
        }

        var oldCurrentItem = currentItemIndexForAnimation;
        List<TrayItem> items = new List<TrayItem>(trayItems);
        var hasMatch = HasMatch(trayItems, currentItemIndexForAnimation);
        if (hasMatch.result)
        {
            for (int i = hasMatch.index + 1; i <= currentItemIndexForAnimation; i++)
            {
                trayItems.Move(i, i - 3);
            }
            currentItemIndexForAnimation -= 3;
        }

        itemsSequence.AppendInterval(0f);
        if (newItemPos < oldCurrentItem)
        {
            // move items together
            for (var i = newItemPos + 1; i <= oldCurrentItem; i++)
            {
                itemsSequence.Join(items[i].transform.DOLocalMoveXAtSpeed(xPositions[i], moveItemSpeed));
            }
        }
        else
        {
            var waitDuration = Time.time - fruit.clickTime;
            if (waitDuration < delay)
            {
                itemsSequence.AppendInterval(delay- waitDuration);
            }
        }

        items[newItemPos].transform.SetLocalX(xPositions[newItemPos]);
        itemsSequence.Append(fruit.transform.DOMove(items[newItemPos].transform.position + new Vector3(0, 0, -3), 0.3f));
        itemsSequence.Join(fruit.transform.DOScale(0.2f, 0.3f));
        itemsSequence.AppendCallback(() => {
            fruit.Remove();
            items[newItemPos].gameObject.SetActive(true);
        });

        if (hasMatch.result)
        {
            HandleMatchSequence(items, itemsSequence, hasMatch.index, oldCurrentItem);
        }
        
        currentItemIndexForAnimation += 1;

        return itemsSequence;
    }

    private void HandleMatchSequence(List<TrayItem> items, Sequence itemsSequence, int index, int oldCurrentItem)
    {
        //itemsSequence.AppendInterval(delay);
        //move match items to one position  (center)
        itemsSequence.Append(items[index-1].transform.DOLocalMoveXAtSpeed(xPositions[index-2], moveMatchSpeed));
        itemsSequence.Join(items[index].transform.DOLocalMoveXAtSpeed(xPositions[index-2], moveMatchSpeed));
        itemsSequence.AppendCallback(() => {
            items[index].gameObject.SetActive(false);
            items[index-1].gameObject.SetActive(false);
            items[index-2].gameObject.SetActive(false);
        });
        for (int i = index + 1; i <= oldCurrentItem; i++)
        {
            itemsSequence.Join(items[i].transform.DOLocalMoveXAtSpeed(xPositions[i-3], moveItemSpeed));
        }
        itemsSequence.AppendCallback(() =>
        {
            // move everything to positions for the next animation
            for (int i = index + 1; i <= oldCurrentItem; i++)
            {
                items.Move(i, i - 3);
            }
            ResetPositions(items);
        });        
    }

    private void ResetPositions(List<TrayItem> items)
    {
        for (var i = 0; i < items.Count; i++)
        {
            items[i].transform.SetLocalX(xPositions[i]);
        }
    }


    public bool IsFull()
    {
        return finalItemIndex >= finalItems.Count;
    }

    public void Clear()
    {
        itemsSequence.Kill();
        foreach (var item in trayItems)
        {
            item.ResetItem();
        }
        currentItemIndexForAnimation = 0;

        for (var idx = 0; idx < finalItems.Count; idx++)
        {
            finalItems[idx] = "";
        }
        finalItemIndex = 0;
    }

}
