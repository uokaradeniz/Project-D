using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class EnemyController : MonoBehaviour
{
    public static float DAMAGE_MAX = 10;
    public static float DAMAGE_MIN = 5;
    
    public enum EnemyType
    {
        Wall,
        Capsule,
        Walker,
        Gunner
    }

    public EnemyType enemyType;
    [Min(0)] public int health;
    private NavMeshAgent navMesh;
    private GameObject player;
    public float atkRange;
    private Animator animator;
    private GameController gameController;
    
    public int EnemyHealth
    {
        get { return health; }

        set { health = Mathf.Max(0, value); }
    }

    private void Start()
    {
        gameController = GameObject.Find("Global Volume").GetComponent<GameController>();
        if (enemyType == EnemyType.Walker)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            navMesh = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
        }
        else if (enemyType == EnemyType.Gunner)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            navMesh = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if(transform.position.y <=-3) { Destroy(gameObject); }
        
        if (enemyType == EnemyType.Wall)
        {
            if (health <= 0)
            {
                health = 0;
                Instantiate(Resources.Load("WallBroken"), transform.position, transform.rotation);
                gameController.Score += 5;
                gameController.scoreAdditionText.text = "+5";
                gameController.ScoreAdditionAnim();
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
                gameController.Score += 10;
                gameController.scoreAdditionText.text = "+10";
                gameController.ScoreAdditionAnim();
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
                gameController.Score += 15;
                gameController.scoreAdditionText.text = "+15";
                gameController.ScoreAdditionAnim();
                Destroy(gameObject);
            }
            else
            {
                var targetPos = new Vector3(player.transform.position.x, transform.position.y,
                    player.transform.position.z);
                transform.LookAt(targetPos);
                navMesh.SetDestination(player.transform.position);
                if (Vector3.Distance(transform.position, player.transform.position) < atkRange)
                    animator.SetBool("IsAttacking", true);
                else
                    animator.SetBool("IsAttacking", false);
            }
        }
        else if (enemyType == EnemyType.Gunner)
        {
            if (health <= 0)
            {
                health = 0;
                Instantiate(Resources.Load("CapsuleBroken"), transform.position, transform.rotation);
                Instantiate(Resources.Load("BloodEffect"), transform.position, transform.rotation);
                gameController.Score += 20;
                gameController.scoreAdditionText.text = "+20";
                gameController.ScoreAdditionAnim();
                Destroy(gameObject);
            }
            else
            {
                if (Vector3.Distance(transform.position, player.transform.position) > 3)
                {
                    navMesh.stoppingDistance = 10;
                    navMesh.SetDestination(player.transform.position);
                }
                else
                {
                    navMesh.stoppingDistance = 5;
                    navMesh.SetDestination(Vector3.MoveTowards(transform.position, player.transform.position,
                        -navMesh.speed));
                }

                var targetPos = new Vector3(player.transform.position.x, transform.position.y,
                    player.transform.position.z);
                transform.LookAt(targetPos);
                if (Vector3.Distance(transform.position, player.transform.position) < atkRange)
                    animator.SetBool("IsAttacking", true);
                else
                    animator.SetBool("IsAttacking", false);
            }
        }
    }

    public void HitPlayer()
    {
        player.GetComponent<PlayerCombatController>().Health -= (int)Random.Range(DAMAGE_MIN, DAMAGE_MAX);
    }

    public void FireProjectile()
    {
        Transform muzzle = transform.Find("Cube/Muzzle");
        Instantiate(Resources.Load("ProjectileEnemy"), muzzle.position, muzzle.rotation);
    }
}