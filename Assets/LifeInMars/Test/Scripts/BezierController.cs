using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierController : MonoBehaviour
{
    public Transform[] nodes;
    Camera cam;
    LTBezierPath bezierPath;
    void Awake()
    {
        cam = Camera.main;
        bezierPath = new LTBezierPath();
        for (int i = 0; i < nodes.Length; i++)
        {
            bezierPath.place(nodes[i], i);
        }
    }

    private void Start()
    {
        bezierPath.gizmoDraw();
    }

    private void Update()
    {
        //bezierPath.gizmoDraw();
    }


}
