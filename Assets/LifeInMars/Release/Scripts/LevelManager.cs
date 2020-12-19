using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LevelManager : MonoBehaviour
{

    public static LevelManager Instance;

    protected void Awake()
    {
        AvoidDuplication();
    }
    public abstract void StartLevel();
    public abstract void FinishLevel(bool success);


    private void AvoidDuplication()
    {
        if (!Instance || Instance.GetType() != this.GetType())
            Instance = this;
        else
            Destroy(gameObject);
    }
}
