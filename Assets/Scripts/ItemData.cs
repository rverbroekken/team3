using UnityEngine;

public class ItemData : ScriptableObject
{
    [Header("Rotation Settings")]
    [Range(-10f, 10f)] public float minXRotation = -1f;
    [Range(-10f, 10f)] public float minYRotation = -1f;
    [Range(-10f, 10f)] public float minZRotation = -1f;
    [Range(-10f, 10f)] public float maxXRotation = 1f;
    [Range(-10f, 10f)] public float maxYRotation = 1f;
    [Range(-10f, 10f)] public float maxZRotation = 1f;

    [Header("Force Settings")]
    [Range(15f, 23f)] public float minForce = 16f;
    [Range(15f, 23f)] public float maxForce = 21f;
}
