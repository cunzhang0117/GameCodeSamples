using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UiCtrl : MonoBehaviour
{

    public float maxHealth = 100000;
    public float currentHealth;
    public GameObject healthBarObj;     //抓取血量
    public GameObject bagObj;   //抓取背包
    public HealthBar healthBar;
    public Text knifeNum;
    public GameObject knifeU;

    private GameObject knifePic;
    public int knife = 0;

    public AudioSource hurt, take, put;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        knifePic = GameObject.Find("Knife");
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHealth < 1)
        {
            Restart();
        }
        Bag();
        knifeNum.text = knife.ToString();
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == "Enemy")        //碰到敵人扣血
        {
            healthBarObj.SetActive(true);
            //Debug.Log("Hit");
            GetHurt(0.1f);
        }else healthBarObj.SetActive(false);
    }


    void GetHurt(float damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);
    }

    private void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void OnTriggerEnter2D(Collider2D other)
    { 
        if(other.gameObject.name == "Knife")
        {
            knife++;
            other.gameObject.SetActive(false);
            take.Play();
        }

        if (other.gameObject.tag == "Enemy")
            hurt.Play();
    }


    void Bag()      //物品欄顯示、放置和補充
    {
        if (knife > 0)
        {
            bagObj.SetActive(true);
            
            //space shift無法同時?
            if (Input.GetKeyDown(KeyCode.Space) || (Input.GetKeyDown(KeyCode.Space) && Input.GetKey(KeyCode.LeftShift)))
            {
                Instantiate(knifeU, transform.position, transform.rotation);
                knife--;
                put.Play();
            }
        }
        else if(knife == 0)
        {
            bagObj.SetActive(false);
            knifePic.SetActive(true);
        }
    }


}
