using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour {

    private int score = 0;
    private int scoreMultiplier = 1;
    public Text scoreText;

    private bool paused;
    private int level = 0;
    public Text levelText;

    public MenuScript menu;

    private List<GameObject> coops;
    private List<GameObject> chickens;
    private List<GameObject> chicks;
    private int capturedChickens = 0; 
    
    private float chickenSpawnTimer = 0;
    public float chickenSpawnInterval = 10;


    private void Start()
    {
        this.paused = false;

        this.menu = GameObject.FindWithTag("menu").GetComponent<MenuScript>();
        this.menu.newMenu("restart", "main_menu", "credits", "exit");

        this.coops = new List<GameObject>();
        this.chickens = new List<GameObject>();
        this.chicks = new List<GameObject>();

        this.SpawnNewCoop();
        this.chickenSpawnTimer = this.chickenSpawnInterval - 1;
    }

    private void OnEnable()
    {
        ChickenSortEvents.onPause += PauseGame;
        ChickenSortEvents.onUnpause += UnpauseGame;
        ChickenSortEvents.onChickenCaptured += ChickenCaptured;
        ChickenSortEvents.onLevelChange += LevelChange;
        ChickenSortEvents.onChickGrowup += ChickGrowsUp;
        ChickenSortEvents.onEggCollect += EggCollected;
        ChickenSortEvents.onHatchEgg += HatchEgg;
        ChickenSortEvents.onMultiplierReset += ResetMultiplier;
    }
    private void OnDisable()
    {
        ChickenSortEvents.onPause -= PauseGame;
        ChickenSortEvents.onUnpause -= UnpauseGame;
        ChickenSortEvents.onChickenCaptured -= ChickenCaptured;
        ChickenSortEvents.onLevelChange -= LevelChange;
        ChickenSortEvents.onChickGrowup -= ChickGrowsUp;
        ChickenSortEvents.onHatchEgg -= HatchEgg;
        ChickenSortEvents.onEggCollect -= EggCollected;
        ChickenSortEvents.onMultiplierReset -= ResetMultiplier;
    }

    /** GAME LOOP **/
    private void Update()
    {
        /** get menu **/
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!this.paused) ChickenSortEvents.PauseGame();
            else ChickenSortEvents.UnpauseGame();
        }

        /** chicken spawn checker **/
        if (!this.paused)
        {
            this.chickenSpawnTimer += Time.deltaTime;
            if (this.chickens.Count < Mathf.Min((Mathf.Pow(this.level, 2) + 1), 10) && this.chickenSpawnTimer > chickenSpawnInterval)
            {
                this.chickenSpawnInterval += 0.02f;
                this.chickenSpawnTimer = 0;
                SpawnChicken();
            }
        }
    }

    private void ChickGrowsUp(GameObject chick)
    {
        this.chicks.RemoveAt(this.chicks.IndexOf(chick));
        this.chickens.Add(Spawner.SpawnChicken(chick.transform.position));
        GameObject.Destroy(chick);
    }

    private void SpawnChicken()
    {
        this.chickens.Add(Spawner.SpawnChicken());
    }

    private void SpawnNewCoop()
    {
        this.coops.Add(Spawner.SpawnCoop(this.level));
    }

    private void ChickenCaptured(GameObject chicken)
    {
        this.capturedChickens++;
        this.chickens.RemoveAt(this.chickens.IndexOf(chicken));
        GameObject message = Instantiate(Resources.Load<GameObject>("Prefabs/ScoreScroller"));
        string[] details =
        {
            "Captured! +1",
            "Chain! x" + this.scoreMultiplier
        };
        message.GetComponent<WorldUI>().SetText(chicken.transform.position, chicken.GetComponent<SpriteRenderer>().color, this.scoreMultiplier.ToString(), details);
        GameObject.Destroy(chicken);
        this.IncreaseScore(this.scoreMultiplier);
        this.scoreMultiplier++;
        this.chickenSpawnInterval -= 0.1f;
        this.GetComponent<SoundManager>().PlayAudio("capture", false, 0.3f);
    }

    private void EggCollected(GameObject egg)
    {
        this.GetComponent<SoundManager>().PlayAudio("pickup");
        GameObject message = Instantiate(Resources.Load<GameObject>("Prefabs/ScoreScroller"));
        string[] details = { };
        message.GetComponent<WorldUI>().SetText(egg.transform.position, Color.white, "5", details);
        this.IncreaseScore(5, false);
        GameObject.Destroy(egg);
    }

    private void HatchEgg(GameObject egg)
    {
        this.chicks.Add(Spawner.SpawnChick(egg.transform.position));
        GameObject.Destroy(egg);
    }

    private void LevelChange(int newLevel)
    {
        this.level = newLevel;
        this.levelText.text = "Level: " + (this.level + 1);
        this.SpawnCoopsAccordingToLevel();
    }

    private void SpawnCoopsAccordingToLevel()
    {
        if (this.coops.Count < 5)
        {
            for (int i = this.coops.Count; i <= this.level; i++)
            {
                this.SpawnNewCoop();
            }
        }
    }
    
    #region utils 
    private void PauseGame()
    {
        this.paused = true;
    }

    private void UnpauseGame()
    {
        this.paused = false;
    }

    /** Increases Score, updates score text and checks wether a new level is reached **/
    public void IncreaseScore(int by, bool checkLevelUp = true)
    {
        this.score += by;
        this.scoreText.text = "Score: " + score;
        if (checkLevelUp)
        {
            if (this.score > Mathf.Pow(this.level * 2 + 1, 2))
            {
                ChickenSortEvents.LevelChange(this.level + 1);
            }
        }
    }
    
    private void ResetMultiplier()
    {
        this.scoreMultiplier = 1;
    }

    private void SoundsReady()
    {
        this.GetComponent<SoundManager>().PlayAudio("monkey-music", true, 0.2f);
    }
    
    
    #endregion
}
