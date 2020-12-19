using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CableSystem : MonoBehaviour
{
    public Color[] Colors = null;
    private Socket[] startSockets = null, endSockets = null;
    private Cable[] cables = null;

    private bool locked = true;
    public Jack HeldJack = null;
    private int remainingPlugs = -1;
    private CableFixLevel levelManager = null;

    public int RemainingPlugs
    {
        get
        {
            return remainingPlugs;
        }
        set
        {
            remainingPlugs = value;
            if (remainingPlugs == 0)
            {
                locked = true;
                levelManager.FinishLevel(true);
            }
        }
    }

    public bool Locked
    {
        get
        {
            return locked;
        }
    }


    private void Awake()
    {
        levelManager = (CableFixLevel)LevelManager.Instance;
        GameObject[] startSockets = GameObject.FindGameObjectsWithTag("Start Socket");
        this.startSockets = new Socket[startSockets.Length];
        for (int i = 0; i < startSockets.Length; i++)
        {
            this.startSockets[i] = startSockets[i].GetComponent<Socket>();
        }
        GameObject[] endSockets = GameObject.FindGameObjectsWithTag("End Socket");
        this.endSockets = new Socket[endSockets.Length];
        for (int i = 0; i < endSockets.Length; i++)
        {
            this.endSockets[i] = endSockets[i].GetComponent<Socket>();
        }
        cables = FindObjectsOfType<Cable>();
        RemainingPlugs = cables.Length * 2;
        List<int> takenColorIDs = new List<int>();
        for (int i = 0; i < cables.Length; ++i)
        {
            int id = Random.Range(0, Colors.Length);
            while (takenColorIDs.Contains(id))
            {
                id = Random.Range(0, Colors.Length);
            }
            Cable cable = cables[i];
            cable.ColorID = id;
            takenColorIDs.Add(id);
        }
        List<int> givenColorIDs = new List<int>();
        for (int i = 0; i < takenColorIDs.Count; i++)
        {
            int index = Random.Range(0, this.startSockets.Length);
            while (this.startSockets[index].ColorID >= 0)
            {
                index = Random.Range(0, this.startSockets.Length);
            }
            Socket startSocket = this.startSockets[index];
            index = Random.Range(0, this.endSockets.Length);
            while (this.endSockets[index].ColorID >= 0)
            {
                index = Random.Range(0, this.endSockets.Length);
            }
            Socket endSocket = this.endSockets[index];
            int id = takenColorIDs[Random.Range(0, takenColorIDs.Count)];
            while (givenColorIDs.Contains(id))
            {
                id = takenColorIDs[Random.Range(0, takenColorIDs.Count)];
            }
            givenColorIDs.Add(id);
            startSocket.ColorID = id;
            endSocket.ColorID = id;

        }
        for (int i = 0; i < this.startSockets.Length - givenColorIDs.Count; ++i)
        {
            int index = Random.Range(0, this.startSockets.Length);
            while (this.startSockets[index].ColorID >= 0)
            {
                index = Random.Range(0, this.startSockets.Length);
            }
            Socket startSocket = this.startSockets[index];

            int id = Random.Range(0, Colors.Length);
            while (givenColorIDs.Contains(id))
            {
                id = Random.Range(0, Colors.Length);
            }
            startSocket.ColorID = id;
        }

        for (int i = 0; i < this.endSockets.Length - givenColorIDs.Count; ++i)
        {
            int index = Random.Range(0, this.endSockets.Length);
            while (this.endSockets[index].ColorID >= 0)
            {
                index = Random.Range(0, this.endSockets.Length);
            }
            Socket endSocket = this.endSockets[index];

            int id = Random.Range(0, Colors.Length);
            while (givenColorIDs.Contains(id))
            {
                id = Random.Range(0, Colors.Length);
            }
            endSocket.ColorID = id;
        }

        List<int> takenStartSocketIndexes = new List<int>();
        List<int> takenEndSocketIndexes = new List<int>();
        //place cables
        for (int i = 0; i < cables.Length; ++i)
        {
            // place cables' start jacks

            int index = Random.Range(0, this.startSockets.Length);
            while (cables[i].ColorID == this.startSockets[index].ColorID || takenStartSocketIndexes.Contains(index))
            {
                index = Random.Range(0, this.startSockets.Length);
            }
            cables[i].StartJack.Socket = this.startSockets[index];
            takenStartSocketIndexes.Add(index);

            // place cables' end jacks

            index = Random.Range(0, this.endSockets.Length);
            while (cables[i].ColorID == this.endSockets[index].ColorID || takenEndSocketIndexes.Contains(index))
            {
                index = Random.Range(0, this.endSockets.Length);
            }
            cables[i].EndJack.Socket = this.endSockets[index];
            takenEndSocketIndexes.Add(index);
        }


    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            cables[0].EndJack.PlugOut(false);
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            cables[0].EndJack.PlugIn(endSockets[0], false);
        }
    }

    public void Lock(float time)
    {
        StartCoroutine(LockCoroutine(time));
    }

    private IEnumerator LockCoroutine(float time)
    {
        locked = true;
        yield return new WaitForSeconds(time);
        locked = false;
    }

    public void Unlock()
    {
        locked = false;
    }

    public enum Side { START, END };

}
