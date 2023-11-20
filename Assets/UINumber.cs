using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class UINumber : MonoBehaviour
{
    public void OnValueChanged(float value)
    {
        GetComponent<TextMeshProUGUI>().text = value.ToString();
    }
}
