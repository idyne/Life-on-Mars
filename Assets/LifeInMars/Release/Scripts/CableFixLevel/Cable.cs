using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cable : MonoBehaviour
{
    [SerializeField] private Jack startJack = null, endJack = null;
    [SerializeField] private Renderer cableRend = null;
    private int colorID = -1;
    private static CableFixLevel levelManager = null;

    private void Awake()
    {
        if (!levelManager)
            levelManager = (CableFixLevel)LevelManager.Instance;
    }
    public int ColorID
    {
        get
        {
            return colorID;
        }
        set
        {
            colorID = value;
            cableRend.material.color = levelManager.CableSystem.Colors[colorID];
            StartJack.ColorID = colorID;
            EndJack.ColorID = colorID;
        }
    }

    public Jack StartJack
    {
        get
        {
            return startJack;
        }
    }

    public Jack EndJack
    {
        get
        {
            return endJack;
        }
    }


}
