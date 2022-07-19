using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class Target : MonoBehaviour
{
    private Vector3 originalPos;

    Rigidbody2D rb;
    private void Start()
    {
        rb = gameObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        originalPos = transform.position;
        //Move();
    }

    public void Move()
    {
      
        rb.MovePosition(transform.position + 1.6f * Time.fixedDeltaTime * Vector3.right);

    }

    private void FixedUpdate()
    {
        Move();
    }
}
