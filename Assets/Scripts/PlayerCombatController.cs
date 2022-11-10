using System;
using System.Collections;
using System.Collections.Generic;
using Mono.Cecil;
using UnityEngine;
using UnityEngine.InputSystem;

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

    private PlayerInputActions playerInputActions;

    public int Health
    {
        get => health;

        set => health = Mathf.Max(0, value);
    }

    // Start is called before the first frame update
    private void Start()
    {
        gameController = GameObject.Find("Global Volume").GetComponent<GameController>();
        muzzle = transform.Find("PlayerBody/Weapon/Muzzle");
        fireTimer = fireCooldown - 0.005f;
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Health > 0)
        {
            if (playerInputActions.Player.FireBullet.triggered)
            {
                FireBullet();
            }
            else if (playerInputActions.Player.FireBullet.inProgress)
            {
                fireTimer += Time.deltaTime;
                if (fireTimer >= fireCooldown)
                {
                    FireBullet();
                    fireTimer = 0;
                }
            }

            if (playerInputActions.Player.FireExplosive.triggered)
                FireExplosive();
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
}