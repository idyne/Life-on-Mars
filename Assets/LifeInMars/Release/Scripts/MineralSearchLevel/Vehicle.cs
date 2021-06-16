using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MonoBehaviour
{
    private Rigidbody rb;
    private Vector2 mouseAnchor = Vector2.zero;
    private Vector3 previousDirection = Vector3.zero;
    private Vector3 direction = Vector3.zero;
    private static MineralSearchLevel levelManager;
    private float wheelAngle = 0;

    public int health = 100;

    [SerializeField] private float acceleration = 1, maxSpeed = 10;
    [SerializeField] private Transform frontLeftWheel = null;
    [SerializeField] private Transform frontRightWheel = null;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        levelManager = (MineralSearchLevel)LevelManager.Instance;
    }

    private void Update()
    {
        if (GameManager.Instance.State == GameManager.GameState.STARTED)
        {
            CheckInput();
            wheelAngle = Mathf.MoveTowards(wheelAngle, Vector3.SignedAngle(previousDirection, direction, Vector3.up), Time.deltaTime * 5);
            frontLeftWheel.localRotation = Quaternion.Euler(0, Mathf.Clamp(-wheelAngle * 25, -60, 60), 0);
            frontRightWheel.localRotation = Quaternion.Euler(0, Mathf.Clamp(-wheelAngle * 25, -60, 60), 0);
        }

    }

    private void CheckInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mouseAnchor = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0) && mouseAnchor != Vector2.zero)
        {
            previousDirection = direction;
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
        Quaternion rot = transform.rotation;
        if (direction.magnitude > 0)
        {
            transform.LookAt(transform.position + direction);
        }
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
