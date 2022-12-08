using System;
using System.Collections;
using System.Collections.Generic;
using Mono.Cecil;
using TMPro;
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
    private Transform spawnPoint1;
    private Transform spawnPoint2;
    private Transform spawnPoint3;
    private Transform spawnPoint4;
    private List<Transform> spList = new List<Transform>();

    public float Score
    {
        get => score;
        set => score = value;
    }

    // Start is called before the first frame update
    private void Start()
    {
        spawnPoint1 = Camera.main.transform.Find("SpawnPoint1");
        spawnPoint2 = Camera.main.transform.Find("SpawnPoint2");
        spawnPoint3 = Camera.main.transform.Find("SpawnPoint3");
        spawnPoint4 = Camera.main.transform.Find("SpawnPoint4");
        spList.Add(spawnPoint1);
        spList.Add(spawnPoint2);
        spList.Add(spawnPoint3);
        spList.Add(spawnPoint4);
        
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
            //var spawnPos = Camera.main.ViewportToWorldPoint(new Vector3(1, yPos, 1));
            foreach (var sp in spList)
                Instantiate(Resources.Load("Walker"), sp.position, Quaternion.identity);    
            
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