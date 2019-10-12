using UnityEngine;
using System.Collections;

public class CoopLogic : MonoBehaviour {

    public string color = "";
    private Animator anim;
	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
	}

    public void Wobble()
    {
        GetComponent<Animator>().SetTrigger("wobble");
    }
}
