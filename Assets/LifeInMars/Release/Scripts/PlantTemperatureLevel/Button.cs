using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    [SerializeField] private Type type = Type.NUMBER;
    [SerializeField] private int number = -1;
    private static Animator anim = null;

    private void Awake()
    {
        if (!anim)
            anim = transform.parent.GetComponent<Animator>();
    }
    private void OnMouseDown()
    {
        if (GameManager.Instance.State == GameManager.GameState.STARTED)
        {
            PlantTemperatureLevel levelManager = (PlantTemperatureLevel)LevelManager.Instance;

            if (type == Type.NUMBER)
            {
                if (levelManager.PressNumberButton(number))
                    Animate();
            }
            else if (type == Type.ENTER)
            {
                if (levelManager.PressEnterButton())
                    Animate();
            }
        }
    }
    private void Animate()
    {
        if (anim)
            anim.SetTrigger(number == -1 ? "Enter" : number.ToString());
    }
    private enum Type { NUMBER, ENTER }
}
