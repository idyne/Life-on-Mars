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
    private Material[] originalMaterials;

    private bool success = false;
    private bool finish = false;
    private float currentGradientTime = 0.5f;


    private void Awake()
    {
        transform.Rotate(new Vector3(0, Random.Range(0, 360), 0));
        originalMaterials = (Material[])rend.materials.Clone();
        SetGoodColors();
        SetBadColors(40);
        SetColorKeys();

        SetDefaultColors(currentGradientTime);

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

    private void SetBadColors(float hue)
    {
        SetBadColors(hue, 1);
    }

    private void SetBadColors(float hue, float saturationMultiplier)
    {
        badColors = new Color[originalMaterials.Length];
        for (int i = 0; i < originalMaterials.Length; i++)
        {
            badColors[i] = originalMaterials[i].color;
            badColors[i] = ColorManager.SetHue(badColors[i], hue);
            badColors[i] = ColorManager.SetSaturation(badColors[i], ColorManager.GetSaturation(badColors[i]) * saturationMultiplier);
        }
    }

    private void SetColorKeys()
    {
        gradients = new Gradient[rend.materials.Length];
        for (int i = 0; i < rend.materials.Length; i++)
        {
            gradients[i] = ColorManager.CreateGradient(badColors[i], goodColors[i]);
        }
    }

    private void SetDefaultColors(float time)
    {
        for (int i = 0; i < rend.materials.Length; i++)
        {
            rend.materials[i].color = gradients[i].Evaluate(time);
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

    public void Fail(int animationIndex)
    {
        meshAnimator.Play(animationIndex);
        success = false;
        finish = true;
    }

    public void Success(int animationIndex)
    {

        meshAnimator.Play(animationIndex);
        if (animationIndex + 1 < meshAnimator.animations.Length)
            meshAnimator.PlayQueued(meshAnimator.animations[animationIndex + 1].animationName);
        success = true;
        finish = true;
    }

    public void Freeze()
    {
        SetBadColors(180, 1 / 2f);
        SetGoodColors();
        SetColorKeys();
        SetDefaultColors(1);
    }
}
