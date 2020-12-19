using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jack : MonoBehaviour
{
    [SerializeField] private Renderer baseRend = null, connectorRend = null;
    public CableSystem.Side Side = CableSystem.Side.START;
    public Socket Socket = null;
    private Cable cable;
    private bool pluggedIn;
    private float time = 0.1f;
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
            baseRend.materials[0].color = levelManager.CableSystem.Colors[colorID];
            connectorRend.materials[1].color = levelManager.CableSystem.Colors[colorID];
        }
    }

    private void Awake()
    {
        cable = GetComponentInParent<Cable>();
        if (!levelManager)
            levelManager = (CableFixLevel)LevelManager.Instance;

    }

    private void Start()
    {
        if (Socket)
        {
            Socket.Occupied = true;
            pluggedIn = true;
            transform.position = Socket.transform.position;
        }
    }

    public void PlugIn(bool ignoreLock)
    {
        PlugIn(Socket, ignoreLock);
    }
    public void PlugIn(Socket socket, bool ignoreLock)
    {
        if ((!levelManager.CableSystem.Locked || ignoreLock) && levelManager.CableSystem.HeldJack != null && socket.Side == Side && !socket.Occupied)
        {
            GetComponent<Collider>().enabled = true;
            levelManager.CableSystem.HeldJack = null;
            if (ColorID == socket.ColorID)
                levelManager.CableSystem.RemainingPlugs--;
            socket.Occupied = true;
            if (!ignoreLock) levelManager.CableSystem.Lock(time);
            Socket = socket;
            pluggedIn = true;
            transform.LeanMove(socket.transform.position, time);
        }
    }

    public void PlugOut(bool ignoreLock)
    {
        if (levelManager.CableSystem.HeldJack && levelManager.CableSystem.HeldJack != this)
            levelManager.CableSystem.HeldJack.PlugIn(true);
        if ((!levelManager.CableSystem.Locked || ignoreLock) && pluggedIn)
        {
            if (ColorID == Socket.ColorID)
                levelManager.CableSystem.RemainingPlugs++;
            GetComponent<Collider>().enabled = false;
            if (!ignoreLock) levelManager.CableSystem.Lock(time);
            levelManager.CableSystem.HeldJack = this;
            pluggedIn = false;
            Socket.Occupied = false;
            Vector3 newPos = transform.position;
            newPos += cable.transform.up * 0.04f;
            newPos.x -= transform.right.x * 0.08f;
            transform.LeanMove(newPos, time);
            Instantiate(levelManager.ElectricEffectPrefab, Socket.transform.position, levelManager.ElectricEffectPrefab.transform.rotation);
        }

    }

    private void OnMouseDown()
    {
        PlugOut(false);
    }
}
