using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Socket : MonoBehaviour
{
    [SerializeField] private Renderer rend = null;
    public CableSystem.Side Side = CableSystem.Side.START;
    private bool occupied = false;
    private static CableFixLevel levelManager = null;
    private int colorID = -1;

    public int ColorID
    {
        get
        {
            return colorID;
        }
        set
        {
            colorID = value;
            rend.materials[0].color = levelManager.CableSystem.Colors[colorID];
        }
    }

    public bool Occupied
    {
        get
        {
            return occupied;
        }
        set
        {
            occupied = value;
            GetComponent<Collider>().enabled = !occupied;
        }
    }

    private void Awake()
    {
        if (!levelManager)
            levelManager = (CableFixLevel)LevelManager.Instance;
    }

    private void OnMouseDown()
    {
        if (GameManager.Instance.State == GameManager.GameState.STARTED)
        {
            Jack heldJack = levelManager.CableSystem.HeldJack;
            if (heldJack)
            {
                heldJack.PlugIn(this, false);
            }
        }
    }
}
