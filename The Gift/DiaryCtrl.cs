using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiaryCtrl : MonoBehaviour
{
    protected AudioSource audioSource;
    public AudioClip button01, button02, button03, page;

    public Image blackImage;        //淡入淡出
    public GameObject password;     //書密碼環節
    private float alpha;
    public Image[] button;
    public int i = 0, k=0;          //密碼選擇、頁碼選擇
    public static int n=0, j=0;     //n->按了幾個  j->是否等於3

    public int testnumN, testnumJ;
    private bool canChange;  

    [Header("選擇")]
    public Image selecting, currentSelect, selected01, selected02, selected03;     //選擇中、選中

    [Header("內頁")]
    //public bool opened;
    public Sprite[] diaryContent;
    public GameObject diaryContentObj;
    public Image image;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        /*n = 0; k = 0;
        selecting = button[0];
        StartCoroutine(FadeOut());
        AgentInput.instance.isAni = true;   //記得關掉？*/
        image = diaryContentObj.GetComponent<Image>();

    }

    // Update is called once per frame
    void Update()
    {
        if (alpha <= 0 && Input.GetKeyDown(KeyCode.X) && canChange)
        {
            StartCoroutine(CloseFadeOut());
            audioSource.clip = null;
        }
        if (!ItemManager.instance.diaryOpened && password.activeInHierarchy)
        {
            //測試
            testnumJ = j;
            testnumN = n;
            //測試
            selecting = button[i];
            if (currentSelect == null)
            {
                currentSelect = selecting;
                currentSelect.SendMessage("itemSelected");
            }
            else if (currentSelect != selecting)
            {
                currentSelect.SendMessage("itemSelected");
                currentSelect = null;
            }

            if (i != 0 && Input.GetKeyDown(KeyCode.UpArrow))        //移動按鈕
            {
                i--;
            }
            if (i != 10 && Input.GetKeyDown(KeyCode.DownArrow))
            {
                i++;
            }

            if (Input.GetKeyDown(KeyCode.C) && currentSelect != null)
            {
                if (currentSelect != button[10])
                {
                    PlayClip(button01);
                    currentSelect.SendMessage("itemPushed");
                }
                else if (currentSelect == button[10]) CheckPassword();
            }
        }
        else if (ItemManager.instance.diaryOpened && diaryContentObj.activeInHierarchy && canChange)    //日記本內頁
        {
            //diaryContentObj.SetActive(true);
            //password.SetActive(false);
            image.sprite = diaryContent[k];
            if (k != 0 && Input.GetKeyDown(KeyCode.LeftArrow))
            {
                PlayClip(page);
                k--;
            }
            if (k != 6 && Input.GetKeyDown(KeyCode.RightArrow))
            {
                PlayClip(page);
                k++;
            }
        }
    }

    void CheckPassword()
    {
        if (j != 3 || n !=3 )
        {
            PlayClip(button02);
            j = 0;
            n = 0;  //重算
            for (int p = 0; p < 10; p++)
            {
                button[p].SendMessage("ResetButton");
            }
        }
        if (j == 3 && n == 3)   //密碼正確
        {
            PlayClip(button03);
            ItemManager.instance.diaryOpened = true;
            EventManager.instance.A6 = true;
            EventManager.instance.A7 = true;
            //opened = true;
            diaryContentObj.SetActive(true);
            password.SetActive(false);
        }
    }

    protected void PlayClip(AudioClip clip)
    {
        //audioSource.Stop();
        audioSource.clip = clip;
        audioSource.Play();
    }
    IEnumerator FadeOut()       //開啟日記本
    {
        canChange = false;
        AgentInput.instance.isAni = true;   //記得關掉？
        alpha = 0;
        while (alpha < 1)
        {
            alpha += Time.deltaTime;
            blackImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        //yield return new WaitForSeconds(0.5f);
        if (ItemManager.instance.diaryOpened) diaryContentObj.SetActive(true);
        else {
            password.SetActive(true);
            ResetObj();
        }
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(FadeIn());
        //canChange = true;
        yield return null;
    }

    IEnumerator CloseFadeOut()      //關掉日記本
    {
        canChange = false;
        alpha = 0;
        while (alpha < 1)
        {
            alpha += Time.deltaTime;
            blackImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        //yield return new WaitForSeconds(0.5f);
        diaryContentObj.SetActive(false);
        password.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(FadeIn());
        yield return new WaitForSeconds(1f);
        AgentInput.instance.isAni = false;
        GameObject.Find("SisterRoom_manager").SendMessage("DiaryOpening");
        this.gameObject.SetActive(false);
        yield return null;
    }

    IEnumerator FadeIn()
    {
        alpha = 1;
        while (alpha > 0)
        {
            alpha -= Time.deltaTime;
            blackImage.color = new Color(0, 0, 0, alpha);
            yield return new WaitForSeconds(0);     //每1秒alpha會減少Time.deltaTime
        }
        canChange = true;
    }

    public void ResetObj()      //重置數值
    {
        i = 0; k = 0; n = 0; j = 0;
        selecting = button[0];
        currentSelect = null;
        image = diaryContentObj.GetComponent<Image>();
        image.sprite = diaryContent[0];
        for (int p = 0; p < 10; p++)
        {
            button[p].SendMessage("ResetButton");
        }
    }
    public void ShowDiary() //開啟日記本
    {
        StartCoroutine(FadeOut());
    }
}