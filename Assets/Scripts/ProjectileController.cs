using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Random = UnityEngine.Random;

public class ProjectileController : MonoBehaviour
{
    public float projectileSpeed;
    private float destroyTimer;

    public float maxLifeTime;
    private ParticleSystem particleDestroyVFX;
    private Rigidbody rb;

    public float explosionForce;
    public float explosionRadii;

    public enum ProjectileType
    {
        Bullet,
        Explosive
    }

    public ProjectileType projectileType;

    private void Start()
    {
        Physics.IgnoreLayerCollision(3, 6);
        Physics.IgnoreLayerCollision(6, 6);
        
        rb = GetComponent<Rigidbody>();
        if (projectileType == ProjectileType.Bullet)
        {
            particleDestroyVFX = Resources.Load("ProjectileDestroyVFX").GetComponent<ParticleSystem>();
            var trajectory = transform.forward * projectileSpeed;
            rb.AddForce(trajectory, ForceMode.Impulse);
        }
        else if (projectileType == ProjectileType.Explosive)
        {
            particleDestroyVFX = Resources.Load("ProjectileExplosiveDestroyVFX").GetComponent<ParticleSystem>();
            var trajectory = transform.forward * projectileSpeed;
            rb.AddForce(trajectory, ForceMode.Impulse);
        }
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (projectileType == ProjectileType.Bullet)
        {
            destroyTimer += Time.fixedDeltaTime;
            if (destroyTimer >= maxLifeTime)
            {
                Instantiate(particleDestroyVFX, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
        }
        else if (projectileType == ProjectileType.Explosive)
        {
            destroyTimer += Time.fixedDeltaTime;
            if (destroyTimer >= maxLifeTime)
            {
                var objects = Physics.OverlapSphere(transform.position, explosionRadii);
                foreach (var obj in objects)
                {
                    if (obj.CompareTag("Enemy"))
                        obj.GetComponent<EnemyController>().health -= Random.Range(40, 60);
                }

                Invoke("ApplyExplosionEffect", .05f);
            }
        }
    }

    void ApplyExplosionEffect()
    {
        var pieces = Physics.OverlapSphere(transform.position, explosionRadii);
        foreach (var piece in pieces)
        {
            if (piece.CompareTag("ObjectPiece"))
                piece.GetComponent<Rigidbody>()
                    .AddExplosionForce(explosionForce, transform.position, explosionRadii, 5);
        }

        Instantiate(particleDestroyVFX, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (gameObject.CompareTag("Projectile"))
        {
            if (!collision.collider.CompareTag("Player") && !collision.collider.CompareTag("Projectile") &&
                projectileType == ProjectileType.Bullet)
            {
                if (collision.collider.CompareTag("Enemy"))
                {
                    collision.collider.GetComponent<EnemyController>().health -= Random.Range(10, 20);
                }
            }
            Instantiate(particleDestroyVFX, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
        else if (gameObject.CompareTag("EnemyProjectile"))
        {
            if (!collision.collider.CompareTag("Enemy") && !collision.collider.CompareTag("Projectile") &&
                projectileType == ProjectileType.Bullet)
            {
                if (collision.collider.CompareTag("Player"))
                {
                    collision.collider.GetComponent<PlayerCombatController>().health -=
                        (int)Random.Range(EnemyController.DAMAGE_MIN, EnemyController.DAMAGE_MAX);
                } 
            }
            Instantiate(particleDestroyVFX, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}