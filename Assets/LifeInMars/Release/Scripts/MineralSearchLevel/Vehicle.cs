using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MonoBehaviour
{
    private Rigidbody rb;
    private Vector2 mouseAnchor = Vector2.zero;
    private Vector3 direction = Vector3.zero;
    private static MineralSearchLevel levelManager;

    public int health = 100;

    [SerializeField] private float acceleration = 1, maxSpeed = 10;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        levelManager = (MineralSearchLevel) LevelManager.Instance;
    }

    private void Update()
    {
        if (GameManager.Instance.State == GameManager.GameState.STARTED)
            CheckInput();
    }

    private void CheckInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mouseAnchor = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0) && mouseAnchor != Vector2.zero)
        {
            direction = new Vector3(
                Input.mousePosition.x - mouseAnchor.x,
                0,
                Input.mousePosition.y - mouseAnchor.y);
            direction.Normalize();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            direction = Vector3.zero;
            mouseAnchor = Vector2.zero;
        }
        if (direction.magnitude > 0)
            transform.LookAt(transform.position + direction);
        rb.AddForce(direction * acceleration * Time.deltaTime * 500);
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
    }

    /*private void OnCollisionEnter(Collision collision)
    {
        float collisionForce = collision.impulse.magnitude / Time.fixedDeltaTime;
        print("Collision Force: " + collisionForce);
        if (collisionForce > 450.0F)
        {
            health = Mathf.Clamp(health - 10, 0, 100);
        }


    }*/

    private void OnTriggerEnter(Collider other)
    {
        Mineral mineral = other.GetComponent<Mineral>();
        if (mineral)
        {
            //Destroy(mineral.gameObject);
            StartCoroutine(levelManager.CollectMineral(mineral));
        }
    }


}
