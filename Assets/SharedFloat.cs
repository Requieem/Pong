using UnityEngine;

[CreateAssetMenu(fileName = "SharedFloat", menuName = "Shared/Float")]
public class SharedFloat : ScriptableObject
{
    [field: SerializeField] public float Value { get; private set; } = 1f;
}