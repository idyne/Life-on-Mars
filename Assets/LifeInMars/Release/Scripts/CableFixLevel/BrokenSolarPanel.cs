using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenSolarPanel : MonoBehaviour
{
    [SerializeField] protected Transform rod = null, panel = null;
    [SerializeField] GameObject brokenEffect = null;
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void Fix()
    {
        anim.enabled = false;
        float angle = -20.06f;
        panel.LeanRotate(new Vector3(0, angle, 0), 1);
        brokenEffect.SetActive(false);
    }
}
