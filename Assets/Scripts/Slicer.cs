using UnityEngine;
using System;

public class Slicer : MonoBehaviour
{
    public Action doSlice;
 
    private void OnTriggerEnter(Collider other)
    {
        if (GameManager.Instance.doSlice && other.CompareTag("Player"))
        {
            doSlice?.Invoke();
        }
    }
}
