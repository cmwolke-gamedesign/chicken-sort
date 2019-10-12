using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Chicken : MonoBehaviour {

    // movement 
    public enum MovementStates : int { Idle = 1, Walk = 2, BeingMoved = 3, Angry = 4, Selected = 5, LayingEgg = 6 }
    public int movementState;
    private int collision_direction_counter, idle_direction_counter, idle_switch_counter, idle_switch_counter_max;
    private float angryTimer = 0;
    private float angryTimerMax = 2f;
    private Vector2 movement;

    // pathing
    Vector2 original_position, target_position, last_position;
    private Transform ghost;
    private Vector2 collision_path; 
    private Queue<Vector3> path;
    private LineRenderer pathRenderer; 
    
    public string color = "";

    private Animator anim;

    private bool paused;

    private float eggTimer;

    // Use this for initialization
    void Start () {
        movementState = (int)MovementStates.Idle;
        ghost = transform.Find("Ghost");
        original_position = transform.position;
        path = new Queue<Vector3>();
        anim = GetComponent<Animator>();
        resetIdleSwitchCounter();
        newEggTimer();
        pathRenderer = this.GetComponentInChildren<LineRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
        if (!paused)
        {
            
            switch(movementState)
            {
                case (int)Chicken.MovementStates.Idle:
                    DecayEggTimer();
                    if (idle_switch_counter++ > idle_switch_counter_max)
                    {
                        StartWalking();
                    }
                    break;
                case (int)Chicken.MovementStates.Walk:
                    DecayEggTimer();
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
                    break;
                case (int)Chicken.MovementStates.BeingMoved:
                    if (path.Count > 0)
                    {
                        movement = path.Peek() - transform.position;
                        transform.Translate(movement.normalized * Time.deltaTime * 1.7f);

                        if (movement.magnitude < 0.2f)
                        {
                            path.Dequeue();
                            this.pathRenderer.SetPositions(path.ToArray());
                        }
                    }
                    else
                    {
                        movementState = (int)MovementStates.Idle;
                        GetComponent<Animator>().SetBool("walking", false);
                    }
                    break;
                case (int)Chicken.MovementStates.Angry:
                    //change direction
                    if (collision_direction_counter++ >= 6)
                    {
                        movement.x += Random.Range(-0.4f, 0.4f);
                        movement.y += Random.Range(-0.4f, 0.4f);
                        movement.Normalize();
                        collision_direction_counter = 0;
                    }

                    transform.Translate(movement * Time.deltaTime * 3);

                    this.angryTimer += Time.deltaTime;
                    if (this.angryTimer > this.angryTimerMax)
                    {
                        this.movementState = (int)MovementStates.Idle;
                        GetComponent<Animator>().SetBool("angry", false);
                    }
                    break;
                case (int)Chicken.MovementStates.LayingEgg:
                    if (anim.GetCurrentAnimatorStateInfo(0).IsName("ChickenIdle"))
                    {
                        LayEgg();
                        movementState = (int)MovementStates.Idle;
                    }
                    break;
            }
            FlipSpriteOnMoveDirection();
        }
        
    }

    private void StartWalking()
    {
        movementState = (int)MovementStates.Walk;
        anim.SetBool("walking", true);
        resetIdleSwitchCounter();
    }

    private void StopWalking()
    {
        movementState = (int)MovementStates.Idle;
        anim.SetBool("walking", false);
        resetIdleSwitchCounter();
    }

    private void FlipSpriteOnMoveDirection()
    {
        if (movement.x < 0)
        {
            GetComponentInChildren<PolygonCollider2D>().transform.localScale = new Vector3(-1f, 1f, 1);
            GetComponent<SpriteRenderer>().flipX = true;
        }
        else
        {
            GetComponentInChildren<PolygonCollider2D>().transform.localScale = new Vector3(1f, 1f, 1);
            GetComponent<SpriteRenderer>().flipX = false;
        }
    }
    private void resetIdleSwitchCounter()
    {
        idle_switch_counter_max = Random.Range(30, 80);
        idle_switch_counter = 0;
    }


    void OnMouseDrag()
    {
        if (!paused)
        {
            Vector3 tmp = Input.mousePosition;
            tmp.z = transform.position.z - Camera.main.transform.position.z;
            target_position = Camera.main.ScreenToWorldPoint(tmp);
            if (Vector3.Distance(target_position, last_position) > 0.15f)
            {
                this.path.Enqueue(target_position);
                this.pathRenderer.positionCount = this.path.Count;
                this.pathRenderer.SetPositions(this.path.ToArray());
                last_position = target_position;
            }
            ghost.position = target_position;
        }
    }
    
    void OnMouseDown()
    {
        StartCoroutine(LerpPathEndColorOverTime(0f, Color.white));
        if (!paused)
        {
            movementState = (int)MovementStates.Selected;
            ghost.gameObject.SetActive(true);
            StopPathing();
        }
    }

    void OnMouseUp()
    {
        if (!paused)
        {
            ghost.gameObject.SetActive(false);
            StartMoving();
        }
    }

    private void StartMoving()
    {
        original_position = transform.position;
        ghost.localPosition = new Vector3(0, 0, 0);
        movementState = (int)MovementStates.BeingMoved;
        anim.SetBool("walking", true);
    }
    
    void OnCollisionEnter2D(Collision2D coll)
    {
        if (this.movementState == (int)MovementStates.BeingMoved)
        {
            if (coll.gameObject.tag != "noAngry")
            {
                this.ChickenAngry(coll.contacts[0].normal);
            } else
            {
                this.StopPathing();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Coop")
        {
            if (!this.CheckCoopMatchingAndDestroy(other.transform.parent.gameObject))
            {
                this.ChickenAngry();
                ChickenSortEvents.MultiplierReset();
            } 
        }
    }


    // Uses impact point to turn around and get angry
    public void ChickenAngry(Vector3 runDir)
    {
        this.GetComponent<SoundManager>().PlayAudio("angry");
        StopPathing();
        GetComponent<Animator>().SetBool("walking", false);
        GetComponent<Animator>().SetBool("angry", true);
        this.movementState = (int)MovementStates.Angry;
        this.angryTimer = 0;
        original_position = transform.position;
        movement = runDir;
        movement.x += Random.Range(-0.4f, 0.4f);
        movement.y += Random.Range(-0.4f, 0.4f);
        collision_direction_counter = 0;
    }

    // turns chicken around and get angry
    public void ChickenAngry()
    {
        this.GetComponent<SoundManager>().PlayAudio("angry");
        this.angryTimer = 0;
        this.collision_direction_counter = 0;
        StopPathing();
        original_position = transform.position;
        movement = -movement;
        movement.x += Random.Range(-0.4f, 0.4f);
        movement.y += Random.Range(-0.4f, 0.4f);
        this.movementState = (int)MovementStates.Angry;
        GetComponent<Animator>().SetBool("walking", false);
        GetComponent<Animator>().SetBool("angry", true);
    }
    
    private bool CheckCoopMatchingAndDestroy(GameObject coop)
    {
        CoopLogic cl = coop.GetComponent<CoopLogic>();
        if (this.DoesColorFit(cl))
        {
            cl.Wobble();
            ChickenSortEvents.CaptureChicken(this.gameObject);
            return true;
        }
        return false;
    }

    private bool DoesColorFit(CoopLogic cl)
    {
        if (cl.color == this.color) return true;
        return false;
    }

    /** EGG STUFF **/
    private void newEggTimer()
    {
        this.eggTimer = 15f;
    }
    private void DecayEggTimer()
    {
        eggTimer -= Time.deltaTime;
        if (eggTimer <= 0)
        {
            anim.SetTrigger("layEgg");
            movementState = (int)MovementStates.LayingEgg;
        }
    }
    private void LayEgg()
    {
        Vector3 eggPos = transform.position - new Vector3(-1, 0, 0); 

        //in case egg is outside of fence:
        while (eggPos.x > Spawner.spawnArea.x2)
        {
            eggPos.x--;
        }

        Instantiate(Resources.Load("Prefabs/Egg"), eggPos, Quaternion.identity);
        this.newEggTimer();
    }

    /** EVENTS **/
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

    private void Pause()
    {
        this.paused = true;
    }
    private void Unpause()
    {
        this.paused = false;
    }

    /** Line renderer and pathing **/ 
    private void renewLineRenderer()
    {
        this.pathRenderer.positionCount = this.path.Count;
        this.pathRenderer.SetPositions(this.path.ToArray());
    }

    private IEnumerator LerpPathEndColorOverTime(float time, Color target)
    {
        Color originalColor = this.pathRenderer.endColor;
        float timer = 0f;
        if (target == originalColor) timer = time;
        if (time == timer)
        {
            this.pathRenderer.endColor = target;
            yield break;
        }
        while (timer < time)
        {
            timer += Time.deltaTime;
            this.pathRenderer.endColor = Color.Lerp(originalColor, target, timer / time);
            yield return null; 
        }
    }

    public void StopPathing()
    {
        this.path.Clear();
        this.renewLineRenderer();
    }

    /** SOUNDS **/
    private void SoundsReady()
    {
        this.GetComponent<SoundManager>().PlayAudio("warning", false, 0.2f);
        this.GetComponent<SoundManager>().PlayAudio("clucking", true, 0.2f, Random.Range(0.6f, 1.4f));
    }
}
