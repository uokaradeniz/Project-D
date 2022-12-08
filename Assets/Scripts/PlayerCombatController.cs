using System;
using System.Collections;
using System.Collections.Generic;
using Mono.Cecil;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class PlayerCombatController : MonoBehaviour
{
    public float maxPrjctlDist;
    private Transform muzzle;
    public float fireCooldown;
    private float fireTimer;
    [Min(0)] public int health;
    private RaycastHit hit;
    public bool playerDied;
    private GameController gameController;
    private Animator animator;
    private int meleeTickCount;
    [SerializeField] private float meleeForce;
    private PlayerInputActions playerInputActions;

    private Transform meleeWeapon;

    public int Health
    {
        get => health;

        set => health = Mathf.Max(0, value);
    }

    public enum WeaponType
    {
        Melee,
        Rifle,
    }

    private WeaponType weaponType;

    // Start is called before the first frame update
    private void Start()
    {
        meleeWeapon = transform.Find("PlayerBody/Melee");
        gameController = GameObject.Find("Global Volume").GetComponent<GameController>();
        muzzle = transform.Find("PlayerBody/Weapon/Muzzle");
        animator = GetComponent<Animator>();
        fireTimer = fireCooldown - 0.005f;
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Health > 0)
        {
            if (playerInputActions.Player.MeleeWeapon.triggered)
            { weaponType = WeaponType.Melee; }
            else if (playerInputActions.Player.RifleWeapon.triggered)
            { weaponType = WeaponType.Rifle; }
            
            switch (weaponType)
            {
                case WeaponType.Melee:
                    muzzle.parent.gameObject.SetActive(false);
                    meleeWeapon.gameObject.SetActive(true);
                    if(playerInputActions.Player.Attack.triggered)
                        meleeTickCount++;

                    switch (meleeTickCount)
                    {
                        case 1:
                            animator.SetBool("MeleeAttack", true);
                            break;
                        case 2:
                            animator.SetTrigger("MeleeSecondSwing");
                            break;
                    }
                    break;
                
                case WeaponType.Rifle:
                    muzzle.parent.gameObject.SetActive(true);
                    meleeWeapon.gameObject.SetActive(false);
                    if (playerInputActions.Player.Attack.triggered)
                    {
                        FireBullet();
                    }
                    else if (playerInputActions.Player.Attack.inProgress)
                    {
                        fireTimer += Time.deltaTime;
                        if (fireTimer >= fireCooldown)
                        {
                            FireBullet();
                            fireTimer = 0;
                        }
                    }

                    if (playerInputActions.Player.SecondaryAttack.triggered)
                        FireExplosive();
                    break;
            }
        }
        else
        {
            if (!playerDied)
            {
                gameController.ShowDeathScreen();
                playerDied = true;
            }
        }
    }

    public void FireBullet()
    {
        var projectile =
            (GameObject)Instantiate(Resources.Load("Projectile"), muzzle.position, muzzle.rotation);
    }

    public void FireExplosive()
    {
        var projectile =
            (GameObject)Instantiate(Resources.Load("ProjectileExplosive"), muzzle.position, muzzle.rotation);
    }

    public void MeleeAttack()
    {
        Collider[] caughtObjs = Physics.OverlapSphere(muzzle.transform.position, 2);
        foreach (var caughtObj in caughtObjs)
        {
            if (caughtObj.CompareTag("Enemy"))
            {
                if(caughtObj.GetComponent<Rigidbody>() != null)
                    caughtObj.GetComponent<Rigidbody>().AddForce((caughtObj.transform.position - transform.position) * meleeForce, ForceMode.Impulse);
                
                caughtObj.GetComponent<EnemyController>().health -= Random.Range(15, 30);
            }
        }
    }
    
    public void StopMeleeTimeWindow()
    {
        animator.SetBool("MeleeAttack", false);
        animator.ResetTrigger("MeleeSecondSwing");
        meleeTickCount = 0;
    }
}