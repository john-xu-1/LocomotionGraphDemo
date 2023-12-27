using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class POI : MonoBehaviour
{
    public Text label;
    public Image panel;

    public void Setup(string text, Color color)
    {
        label.text = text;
        panel.color = color;
    }
}
