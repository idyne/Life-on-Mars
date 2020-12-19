using FSG.MeshAnimator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{
    [SerializeField] private MeshAnimator meshAnimator = null;
    [SerializeField] private Renderer rend;

    private Color[] badColors;
    private Color[] goodColors;
    private Gradient[] gradients;

    private bool success = false;
    private bool finish = false;
    private float currentGradientTime = 0.5f;


    private void Awake()
    {
        transform.Rotate(new Vector3(0, Random.Range(0, 360), 0));
        meshAnimator.Play(0);
        SetGoodColors();
        SetBadColors();
        SetColorKeys();

        SetDefaultColors();

    }

    private void Update()
    {
        if (finish)
            SetColors(success ? 1 : 0);
    }

    private void SetGoodColors()
    {
        goodColors = new Color[rend.materials.Length];
        for (int i = 0; i < rend.materials.Length; i++)
        {
            goodColors[i] = rend.materials[i].color;
        }
    }

    private void SetBadColors()
    {
        badColors = new Color[rend.materials.Length];
        for (int i = 0; i < rend.materials.Length; i++)
        {
            float H, S, V;
            Color.RGBToHSV(rend.materials[i].color, out H, out S, out V);
            badColors[i] = Color.HSVToRGB(40f / 360f, S, V);
        }
    }

    private void SetColorKeys()
    {
        gradients = new Gradient[rend.materials.Length];
        for (int i = 0; i < rend.materials.Length; i++)
        {
            gradients[i] = new Gradient();
            GradientColorKey[] colorKeys = new GradientColorKey[2];
            colorKeys[0].color = badColors[i];
            colorKeys[0].time = 0.0f;
            colorKeys[1].color = goodColors[i];
            colorKeys[1].time = 1.0f;
            gradients[i].colorKeys = colorKeys;
        }
    }

    private void SetDefaultColors()
    {
        for (int i = 0; i < rend.materials.Length; i++)
        {
            rend.materials[i].color = gradients[i].Evaluate(currentGradientTime);
        }
    }

    private void SetColors(float targetTime)
    {
        for (int i = 0; i < rend.materials.Length; i++)
        {
            currentGradientTime = Mathf.MoveTowards(currentGradientTime, targetTime, Time.deltaTime);
            rend.materials[i].color = gradients[i].Evaluate(currentGradientTime);
        }
    }

    public void Fail()
    {
        meshAnimator.Play(1);
        success = false;
        finish = true;
    }

    public void Success()
    {
        meshAnimator.Play(2);
        success = true;
        finish = true;
    }
}
