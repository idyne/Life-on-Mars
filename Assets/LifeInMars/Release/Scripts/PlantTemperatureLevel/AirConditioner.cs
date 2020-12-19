using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirConditioner : MonoBehaviour
{
    private bool speedUp = false;
    private Animator anim = null;
    private float speed = 10;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (speedUp)
        {
            anim.SetFloat("Speed", Mathf.MoveTowards(anim.GetFloat("Speed"), speed, Time.deltaTime * speed));
        }
    }

    public void SpeedUp()
    {
        speedUp = true;
    }
}
