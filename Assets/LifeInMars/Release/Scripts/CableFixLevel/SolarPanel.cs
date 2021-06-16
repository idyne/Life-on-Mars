using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarPanel : MonoBehaviour
{
    [SerializeField] protected Transform rod = null, panel = null;
    [SerializeField] GameObject brokenEffect = null;

    private void Start()
    {
        panel.Rotate(Vector3.up * Random.Range(0, 120f));
    }
    public void Fix()
    {
        float angle = 67.94f;
        panel.LeanRotate(new Vector3(0, angle, 0), 1);
        brokenEffect.SetActive(false);
    }
}
