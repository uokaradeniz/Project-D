using System;
using System.Collections;
using System.Collections.Generic;
using Mono.Cecil;
using UnityEngine;

public class ShrapnelMechanic : MonoBehaviour
{
    public Material transparentMat;
    public float damagingFactor;

    private Rigidbody rb;

    private Renderer pieceMat;
    private float lifeTimer;
    public float lifeTime;

    public float fadeSpeed;

    // Start is called before the first frame update
    private void Start()
    {
        pieceMat = GetComponent<Renderer>();
        rb = GetComponent<Rigidbody>();
        pieceMat.material = transparentMat;
    }

    private void Update()
    {
        lifeTimer += Time.deltaTime;
        if (lifeTimer >= lifeTime)
        {
            var newColor = pieceMat.material.color;
            newColor.a -= Time.deltaTime * fadeSpeed;
            pieceMat.material.color = newColor;
            Destroy(gameObject, 1);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Enemy"))
        {
            var totalDamage = rb.velocity.magnitude * damagingFactor;
            collision.collider.GetComponent<EnemyController>().health -= Mathf.CeilToInt(totalDamage);
        }
    }
}