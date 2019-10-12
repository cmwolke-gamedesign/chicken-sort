using UnityEngine;
using System.Collections;

public class Chick : MonoBehaviour {


    public bool b_walking;
    public bool b_idling;

    private bool paused; 

    Vector2 original_position, target_position, last_position;

    private Animator anim;

    private Vector2 movement;

    private int idle_direction_counter, idle_switch_counter, idle_switch_counter_max;


    public float growthTimer;
    public float growUpTime = 15;

    // Use this for initialization
    void Start () {

        b_idling = true;
        anim = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        ChickenSortEvents.onPause += Pause;
        ChickenSortEvents.onUnpause += Unpause;
    }

    private void OnDisable()
    {
        ChickenSortEvents.onPause -= Pause;
        ChickenSortEvents.onUnpause -= Unpause;
    }

    // Update is called once per frame
    void Update () {

        if (!paused)
        {
            if (b_idling && idle_switch_counter++ > idle_switch_counter_max)
            {
                 StartWalking();
            }

            if (b_walking)
            {
                transform.Translate(movement * Time.deltaTime * 0.5f);
                if (idle_direction_counter++ == 20)
                {
                    movement = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
                    idle_direction_counter = 0;
                }
                if (idle_switch_counter++ > idle_switch_counter_max)
                {
                    StopWalking();
                }

            }

            FlipSpriteOnMoveDirection();

            growthTimer += Time.deltaTime;
            if (growthTimer > growUpTime)
            {
                ChickenSortEvents.ChickGrowsUp(this.gameObject);
                this.paused = true;
            }

        }
    }

    private void FlipSpriteOnMoveDirection()
    {
        GetComponent<SpriteRenderer>().flipX = (movement.x < 0) ? true : false;
    }
    
    private void StartWalking()
    {
        if (!paused)
        {
            b_walking = true;
            b_idling = false;
            anim.SetBool("walking", true);
            idle_switch_counter_max = Random.Range(40, 90);
            idle_switch_counter = 0;
        }
    }

    private void StopWalking()
    {
        if (!paused)
        {
            b_walking = false;
            b_idling = true;
            anim.SetBool("walking", false);
            idle_switch_counter_max = Random.Range(40, 90);
            idle_switch_counter = 0;
        }
    }
    
    private void Pause()
    {
        this.paused = true;
    }
    private void Unpause()
    {
        this.paused = false;
    }

    private void SoundsReady()
    {
        this.GetComponent<SoundManager>().PlayAudio("clucking", true, 0.1f, 2f);
    }
}
