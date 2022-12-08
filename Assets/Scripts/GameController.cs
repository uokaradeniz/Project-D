using System;
using System.Collections;
using System.Collections.Generic;
using Mono.Cecil;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    //UI
    private Slider healthSlider;
    private TextMeshProUGUI scoreText;
    [HideInInspector]public TextMeshProUGUI scoreAdditionText;
    private float score;

    //SPAWN
    private PlayerCombatController player;
    private float spawnTimer;
    public float yPos;
    public bool spawnEnemies;
    [SerializeField]private float spawnDelay;
    
    private List<GameObject> spList = new List<GameObject>();

    public float Score
    {
        get => score;
        set => score = value;
    }

    // Start is called before the first frame update
    private void Start()
    {
        foreach (var sp in GameObject.FindGameObjectsWithTag("SpawnPoint"))
            spList.Add(sp);
        
        scoreText = GameObject.Find("ScoreText").GetComponent<TextMeshProUGUI>();
        scoreAdditionText = GameObject.Find("ScoreAdditionText").GetComponent<TextMeshProUGUI>();

        healthSlider = GameObject.Find("HealthSlider").GetComponent<Slider>();
        
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCombatController>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (spawnEnemies)
            SpawnSystem();

        healthSlider.value = player.Health;
        scoreText.text = "Score: " + score.ToString();
    }

    public void SpawnSystem()
    {
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnDelay)
        {
            foreach (var sp in spList)
            {
                Instantiate(Resources.Load("Walker"), sp.transform.position, Quaternion.identity);
                Instantiate(Resources.Load("SpawnVFX"), sp.transform.position + new Vector3(0,1,0), Quaternion.identity);
            }

            spawnTimer = 0;
        }
    }

    public void ShowDeathScreen()
    {
        GameObject.Find("DeathScreen").GetComponent<Animation>().Play();
    }

    public void ScoreAdditionAnim()
    {
        Animator animator = scoreAdditionText.GetComponent<Animator>();
        animator.SetTrigger("PlayScoreAdditionAnim");
    }
}