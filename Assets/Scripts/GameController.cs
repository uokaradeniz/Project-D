using System;
using System.Collections;
using System.Collections.Generic;
using Mono.Cecil;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{
    private Slider healthSlider;
    private PlayerCombatController player;
    private float spawnTimer;
    public float yPos;

    // Start is called before the first frame update
    private void Start()
    {
        healthSlider = GameObject.Find("HealthSlider").GetComponent<Slider>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCombatController>();
    }

    // Update is called once per frame
    private void Update()
    {
        SpawnSystem();
        healthSlider.value = player.Health;
    }

    public void SpawnSystem()
    {
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= 3)
        {
            var spawnPos = Camera.main.ViewportToWorldPoint(new Vector3(1, yPos, 1));
            Instantiate(Resources.Load("Walker"), spawnPos, Quaternion.identity);
            spawnTimer = 0; 
        }
    }

    public void ShowDeathScreen()
    {
        GameObject.Find("DeathScreen").GetComponent<Animation>().Play();
    }
}