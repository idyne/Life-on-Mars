using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterStick : MonoBehaviour
{
    [SerializeField] private GameObject[] waterEffects = null;

    public void OpenWater(int index)
    {
        waterEffects[index].SetActive(true);
    }
}
