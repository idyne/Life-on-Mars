using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GradientTest : MonoBehaviour
{
    Gradient gradient;
    GradientColorKey[] colorKey;
    public float gradientValue = 0.25f;
    public Renderer rend;

    void Start()
    {
        gradient = new Gradient();

        // Populate the color keys at the relative time 0 and 1 (0 and 100%)
        colorKey = new GradientColorKey[2];
        colorKey[0].color = Color.red;
        colorKey[0].time = 0.0f;
        colorKey[1].color = Color.blue;
        colorKey[1].time = 1.0f;

        gradient.colorKeys = colorKey;

        // What's the color at the relative time 0.25 (25 %) ?
        Debug.Log(gradient.Evaluate(0.25f));
    }

    private void Update()
    {
        Color color = gradient.Evaluate(gradientValue);
        rend.material.color = color;

    }
}
