using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SetText : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI textInput;
    [SerializeField]
    string textToSet = "localhost";

    private void Start()
    {
        textInput = GetComponent<TextMeshProUGUI>();
        textInput.text = textToSet;
    }

    private void OnEnable()
    {
        textInput.text = textToSet;
    }
}
