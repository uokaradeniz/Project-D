using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class EnemyController : MonoBehaviour
{
    public enum EnemyType
    {
        Wall,
        Capsule,
        Walker
    }

    public EnemyType enemyType;
    [Min(0)]public int health;
    private NavMeshAgent navMesh;
    private GameObject player;
    public float atkRange;
    private Animator animator;

    public int EnemyHealth
    {
        get
        {
            return health;
        }

        set
        {
            health = Mathf.Max(0, value);
        }
    }

    private void Start()
    {
        if (enemyType == EnemyType.Walker)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            navMesh = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (enemyType == EnemyType.Wall)
        {
            if (health <= 0)
            {
                health = 0;
                Instantiate(Resources.Load("WallBroken"), transform.position, transform.rotation);
                Destroy(gameObject);
            }
        }
        else if (enemyType == EnemyType.Capsule)
        {
            if (health <= 0)
            {
                health = 0;
                Instantiate(Resources.Load("CapsuleBroken"), transform.position, transform.rotation);
                Instantiate(Resources.Load("BloodEffect"), transform.position, transform.rotation);
                Destroy(gameObject);
            }
        }
        else if (enemyType == EnemyType.Walker)
        {
            if (health <= 0)
            {
                health = 0;
                Instantiate(Resources.Load("CapsuleBroken"), transform.position, transform.rotation);
                Instantiate(Resources.Load("BloodEffect"), transform.position, transform.rotation);
                Destroy(gameObject);
            }
            else
            {
                var targetPos = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
                transform.LookAt(targetPos);
                navMesh.SetDestination(player.transform.position);
                if (Vector3.Distance(transform.position, player.transform.position) < atkRange)
                    animator.SetBool("IsAttacking", true);
                else
                    animator.SetBool("IsAttacking", false);
            }
        }
    }

    public void HitPlayer()
    {
        player.GetComponent<PlayerCombatController>().Health -= Random.Range(5, 15);
    }
}