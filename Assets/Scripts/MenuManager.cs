using UnityEngine;
using System.Collections;

public class MenuManager : MonoBehaviour {


    MenuScript menu;

	// Use this for initialization
	void Start () {

        menu = GameObject.FindGameObjectWithTag("menu").GetComponent<MenuScript>();
        menu.newMenu("endless_game", "explorer_game", "credits", "exit");
        menu.ShowMenuWithoutPause();
        menu.loadingBar = GameObject.Find("LoadingBar");

        if (menu.loadingBar != null)
        {
            menu.loadingBar.SetActive(false);
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
