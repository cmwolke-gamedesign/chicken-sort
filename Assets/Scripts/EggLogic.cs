using UnityEngine;
using System.Collections;

public class EggLogic : MonoBehaviour {

    private bool b_hatched;
    public float timer;
    public float hatch_time = 10;
	// Use this for initialization
	
	// Update is called once per frame
	void Update () {
        if (!b_hatched)
        {
            timer += Time.deltaTime;
            if (timer > hatch_time)
            {
                b_hatched = true;
                StartCoroutine(EggHatched());
            }
        }
	}

    private IEnumerator EggHatched()
    {
        GetComponent<Animator>().SetTrigger("hatch");
        yield return new WaitForSeconds(Resources.Load<AnimationClip>("anims/EggHatching").length);
        ChickenSortEvents.HatchEgg(this.gameObject);
    }

    void OnMouseUpAsButton()
    {
        this.GetComponent<SoundManager>().PlayAudio("pickup", false, 0.2f);
        ChickenSortEvents.CollectEgg(this.gameObject);
    }
    private void SoundsReady()
    {
        this.GetComponent<SoundManager>().PlayAudio("plop", false, 0.2f);
    }
}
