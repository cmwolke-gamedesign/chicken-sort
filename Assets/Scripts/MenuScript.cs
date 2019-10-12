using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class MenuScript : MonoBehaviour {
    
    public bool paused;
    private bool gameStarted;
    public GameObject bg_tint;

    private float loadtimer;
    public GameObject loadingBar;

    public Image music;
    public Image sound;
    public Button pause;
    public Button cont;

    private Vector3 downPos; 

    void Awake()
    {

    }
    // Use this for initialization
    void Start()
    {
        sound.sprite = (Settings.SoundEffects) ? Resources.Load<Sprite>("Icons/sound_on") : Resources.Load<Sprite>("Icons/sound_off");
        bg_tint = GameObject.Find("bg_tint");
        if (bg_tint != null)
        {
            bg_tint.SetActive(false);
        }
        
    }

    private void OnEnable()
    {
        ChickenSortEvents.onPause += ToggleMenu;
        ChickenSortEvents.onUnpause += ToggleMenu;
    }

    private void ToggleMenu()
    {
        this.paused = !this.paused;
        GetComponent<Animator>().SetTrigger("Menu");
        if (bg_tint == null) bg_tint = GameObject.Find("bg_tint");
        bg_tint.SetActive(!bg_tint.activeSelf);

        pause.gameObject.SetActive(!pause.gameObject.activeSelf);
        cont.gameObject.SetActive(!cont.gameObject.activeSelf);
    }

    public void ToggleMusic()
    {
        Settings.ToggleMusic();
        music.sprite = (Settings.Music) ? Resources.Load<Sprite>("Icons/music_on") : Resources.Load<Sprite>("Icons/music_off");
    }

    public void ToggleSounds()
    {
        Settings.ToggleSounds();
        sound.sprite = (Settings.SoundEffects) ? Resources.Load<Sprite>("Icons/sound_on") : Resources.Load<Sprite>("Icons/sound_off");
    }

    public void PauseGame()
    {
        if (this.paused) ChickenSortEvents.UnpauseGame();
        else ChickenSortEvents.PauseGame();
    }
    void Update () {
        /*
        //Loading bar updates:
        if (async != null)
        {
            if (!async.isDone)
            {
                if (loadingBar != null)
                {
                    loadtimer += Time.deltaTime;
                    loadingBar.GetComponentInChildren<Slider>().value = async.progress;
                    if (loadtimer > 3)
                    {
                        loadtimer = 0;
                        Instantiate(Resources.Load("Prefabs/MenuChicken"), new Vector3(UnityEngine.Random.Range(-7, 7), UnityEngine.Random.Range(0, -3.8f), 0), Quaternion.identity);
                    }
                }
                if (async.progress == 0.9f && gameStarted == false)
                {
                    gameStarted = true;
                    SwitchToActivatedScene();
                }
            }
        }*/
	}

    public bool newMenu(string item0, string item1, string item2, string item3)
    {
        Button[] buttons = GetComponentsInChildren<Button>();
        

        if (bg_tint == null)
        {
            bg_tint = GameObject.Find("bg_tint");
            if (bg_tint != null)
                bg_tint.SetActive(false);
        }
        int tmp = 0;
        

        if (createNewMenuItem(0, item0)) tmp++;
        if (createNewMenuItem(1, item1)) tmp++;
        if (createNewMenuItem(2, item2)) tmp++;
        if (createNewMenuItem(3, item3)) tmp++;

        if (tmp == 4) return true;
        else return false;
    }

    public void newMenu(int pos, string item)
    {
        createNewMenuItem(pos, item);
    }

    
    private bool createNewMenuItem(int pos, string item)
    {

        //GameObject button = transform.Find("Buttons").Find("Button"+(pos+1)).gameObject;
        Button button = transform.GetComponentsInChildren<Button>()[pos];
        Text text = button.GetComponentInChildren<Text>();
        
        switch (item)
        {
            case "endless_game":
                text.text = "Endless Game";
                button.GetComponent<Button>().onClick.AddListener(() => MenuEndlessGame());
                break;

            case "explorer_game":
                text.text = "Explorer Game";
                button.GetComponent<Button>().onClick.AddListener(() => MenuExplorerGame());
                break;

            case "credits":
                text.text = "Credits";
                button.GetComponent<Button>().onClick.AddListener(() => MenuCredits());
                break;

            case "exit":
                text.text = "Exit";
                button.GetComponent<Button>().onClick.AddListener(() => MenuCloseGame());
                break;

            case "restart":
                text.text = "Restart Game";
                button.GetComponent<Button>().onClick.AddListener(() => MenuRestartGame());
                break;

            case "main_menu":
                text.text = "Main Menu";
                button.GetComponent<Button>().onClick.AddListener(() => MenuMainMenu());
                break;

            case "continue":
                text.text = "Continue";
                button.GetComponent<Button>().onClick.AddListener(() => PauseGame());
                break;

            default:
                return false;

        }
        
        return true;
        
    }

    private void newButtonText(int button, string text)
    {
        transform.Find("Button"+button+1).GetComponentInChildren<Text>().text = text;
    }

    public void MenuCloseGame()
    {
        Application.Quit();
    }

    public void MenuRestartGame()
    {
        ChickenColorScript.dePopulateColors();
        switch (SceneManager.GetActiveScene().name)
        {
            case "endless":
                MenuEndlessGame();
                break;
            case "explorer":
                MenuExplorerGame();
                break;
        }
    }

    public void MenuContinue()
    {
        PauseGame();
    }

    public void MenuEndlessGame()
    {
        GetComponent<Animator>().SetTrigger("Menu");
        ChickenColorScript.dePopulateColors();
        StartLoadingAsyncScene("endless");
    }

    public void MenuExplorerGame()
    {
        GetComponent<Animator>().SetTrigger("Menu");
        ChickenColorScript.dePopulateColors();
        StartLoadingAsyncScene("explorer");
    }

    public void MenuCredits()
    {
        GetComponent<Animator>().SetTrigger("Menu");
        StartLoadingAsyncScene("credits");
    }

    public void MenuMainMenu()
    {
        GetComponent<Animator>().SetTrigger("Menu");
        SceneManager.LoadScene("mainmenu");
    }
    

    public void ShowMenuWithoutPause()
    {
        GetComponent<Animator>().SetTrigger("Menu");
    }
    
    /* SCENE LOADING */ 
    public void StartLoadingAsyncScene(string scenename)
    {
        StartCoroutine("load", scenename);
        scene = scenename;
    }

    AsyncOperation async; string scene;
    private IEnumerator load(string scenename)
    {
        async = SceneManager.LoadSceneAsync(scenename);
        //async.allowSceneActivation = false;
        if (loadingBar != null)
        {
            loadingBar.SetActive(true);
        }
        yield return async;
    }

    public void SwitchToActivatedScene()
    {
        async.allowSceneActivation = true;
    }
}


