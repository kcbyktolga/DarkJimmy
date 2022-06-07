using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Target : MonoBehaviour
{
    [Range(0, 20)]
    [SerializeField]
    private float speed;

    Rigidbody2D rb;

    private void Start()
    {
        rb = gameObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    void FixedUpdate()
    {
        rb.MovePosition(transform.position + speed * Time.fixedDeltaTime * Vector3.right);
    }
}
