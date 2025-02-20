using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float speed;
    public float startWaitTime;
    public Transform[] moveSpots;       //巡邏點
    public Animator anim;
    //判斷撞物件
    public LayerMask background;
    public Collider2D coll;
    
    private Transform playerpos;
    private int randomSpot;
    private float waitTime;

    [SerializeField]    //追逐判斷
    public bool isFollowing;
    public Transform playerCheck;
    public float alertRadius;
    public LayerMask playerLayer;

    public AudioSource die;
    public GameObject check;

    // Start is called before the first frame update
    void Start()
    {
        waitTime = startWaitTime;
        randomSpot = Random.Range(0, moveSpots.Length);     //隨機值
        anim.SetBool("move", true);
        playerpos = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        isFollowing = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!check.activeInHierarchy)
        {
            if (isFollowing == false)
            {
                patrolling();
            }
            else if (isFollowing == true)
            {
                anim.SetBool("move", true);
                speed = 5;
                if (Vector2.Distance(transform.position, playerpos.position) > 2)
                {
                    transform.position = Vector3.MoveTowards(transform.position, playerpos.position, speed * Time.deltaTime);
                }
                if (transform.position.x > playerpos.position.x)
                {
                    transform.localScale = new Vector3(-1, 1, 1);
                }
                else transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }

    void FixedUpdate()
    {
        isFollowing = Physics2D.OverlapCircle(playerCheck.position, alertRadius, playerLayer);
    }

    void patrolling()
    {
        transform.position = Vector3.MoveTowards(transform.position, moveSpots[randomSpot].position, speed * Time.deltaTime);

        if (coll.IsTouchingLayers(background))
        {
            randomSpot = Random.Range(0, moveSpots.Length);
            Flip();
        }

        //檢查物件是否到點(與點的距離及碰觸到場景物件)
        if (Vector2.Distance(transform.position, moveSpots[randomSpot].position) < 0.2f)
        {
            if (waitTime <= 0)
            {
                randomSpot = Random.Range(0, moveSpots.Length);
                waitTime = startWaitTime;
                anim.SetBool("move", true);
                Flip();
            }
            else
            {
                waitTime -= Time.deltaTime;
                anim.SetBool("move", false);
            }
        }
    }

    void Flip()
    {
        if (transform.position.x > moveSpots[randomSpot].position.x)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else transform.localScale = new Vector3(1, 1, 1);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "weapon")
        {
            Destroy(other.gameObject);
            anim.SetTrigger("isHurt");
            die.Play();
        }
    }

    void Death()
    {
        Destroy(this.gameObject);
    }
}
