using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// CustomPointer class defines the custom software pointer
/// </summary>
public class CustomPointer : MonoBehaviour
{
    Image pointer;

    /// <summary>
    /// At start, fetches the cursor Image
    /// </summary>
    private void Start()
    {
        pointer = GetComponent<Image>();
    }

    /// <summary>
    /// At Update, move the software cursor to the CustomInputModule cursor position
    /// </summary>
    private void Update()
    {
        pointer.transform.position = CustomInputModule.CursorPos;
    }
}
