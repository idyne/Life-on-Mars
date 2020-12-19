﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screw : MonoBehaviour
{
    private Animator anim;
    private bool isUsed = false;
    private static CableFixLevel levelManager = null;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        if (!levelManager)
            levelManager = (CableFixLevel)LevelManager.Instance;
    }
    private void OnMouseDown()
    {
        if (!isUsed && GameManager.Instance.State == GameManager.GameState.STARTED)
        {
            isUsed = true;
            anim.SetTrigger("Turn");
            levelManager.ScrewCount--;
        }
    }

}
