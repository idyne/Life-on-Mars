using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mineral : MonoBehaviour
{
    public Color glowColor;
    [SerializeField] private Renderer rend;
    private Material glowMaterial;
    [SerializeField] private int index = -1;
    public bool isCollected = false;

    private void Awake()
    {
        glowMaterial = rend.materials[1];
        glowMaterial.color = glowColor;
        glowMaterial.SetColor("_EmissionColor", glowColor);

    }

    private void Update()
    {
        transform.Rotate(new Vector3(0, Time.deltaTime * 20, 0));
    }

    public int Index
    {
        get
        {
            return index;
        }
    }
}
