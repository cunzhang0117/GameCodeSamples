using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

public class handEvent : MonoBehaviour
{
    protected AudioSource audioSource;
    public AudioClip pushC, blood, breathing;
    public GameObject rHand, shadow, mask, target, allObj;
    public Image rHandPic, shadowPic;
    public Sprite[] changeRH;
    public Sprite[] changeSP;
    public float a, b, c, d;        //ab for shadow cd for wound
    private float speed;
    public bool hold;
    public GameObject[] wound;
    public GameObject currentWound;
    public Vector2 woundPos;
    public VolumeProfile profile, profile02;
    public Volume volume, volume02;
    private bool canMove;

    public int timer = 0;  //計時
    float f = 0;
    private float alpha;
    public Image blackImage;
    private ChromaticAberration ca;
    private FilmGrain fg;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FadeOut());  //淡出
        //AgentInput.instance.isAni = true;
        speed = 0.3f;
        //抓volume?
        //volume.profile.TryGet(out ca);
        //volume.profile = profile;
        audioSource = GetComponent<AudioSource>();
    }

    private void FixedUpdate()
    {
        if (!hold && canMove)
            rHand.transform.Translate(Input.GetAxisRaw("Horizontal") * speed, Input.GetAxisRaw("Vertical") * speed, 0);
        if (hold && Input.GetKeyDown(KeyCode.RightArrow))       //播音樂
        {
            if (audioSource.clip != blood)
            {
                audioSource.clip = blood;
            }audioSource.Play();
        }
        else if (hold && Input.GetKey(KeyCode.RightArrow))  //抓住且右移
        {
            rHand.transform.Translate(Input.GetAxisRaw("Horizontal") * speed, 0, 0);
            timer++;
        }
        else if (hold && Input.GetAxisRaw("Horizontal") == 0 && audioSource.clip == blood)
        {
            audioSource.Pause();
        }
        if (hold)
        {
            if (woundPos == Vector2.zero) woundPos = currentWound.transform.GetChild(0).transform.position;
            currentWound.transform.GetChild(0).transform.position = new Vector2(woundPos.x - Input.GetAxisRaw("Horizontal") * speed, woundPos.y - Input.GetAxisRaw("Vertical") * speed);
            currentWound.transform.GetChild(0).transform.localPosition = new Vector3(currentWound.transform.GetChild(0).transform.localPosition.x, currentWound.transform.GetChild(0).transform.localPosition.y, 0);
            currentWound.transform.localPosition = new Vector2(rHand.transform.localPosition.x + c, rHand.transform.localPosition.y + d);
        }
        shadow.transform.localPosition = new Vector3(rHand.transform.localPosition.x + a, rHand.transform.localPosition.y + b, 0);
    }
    // Update is called once per frame
    void Update()
    {
        blackImage.color = new Color(0, 0, 0, alpha);
        if (timer > 500 && canMove)
        {
            canMove = false;
            hold = false;
            StartCoroutine(CloseFadeOut());
        }
        volume.profile.TryGet(out ca);
        volume.profile.TryGet(out fg);


        //Volume//
        if (canMove)
        {
            if (Input.GetKey(KeyCode.C))
            {
                f += 0.01f;
                float j = Mathf.PingPong(f, 0.5f);
                ca.intensity.value = j;

                DOTween.To(() => fg.intensity.value, x => fg.intensity.value = x, 1, 3f);
                DOTween.To(() => volume02.weight, x => volume02.weight = x, 1, 3f);
            }
            else
            {
                DOTween.To(() => ca.intensity.value, x => ca.intensity.value = x, 0, 1f);
                DOTween.To(() => fg.intensity.value, x => fg.intensity.value = x, 0, 1f);
                DOTween.To(() => volume02.weight, x => volume02.weight = x, 0, 1f);
            }

            //
            if (Input.GetKeyDown(KeyCode.C))
            {
                //ca.intensity.value = 0.5f;
                int i = Random.Range(0, 4);
                currentWound = Instantiate(wound[i], target.transform.position, Quaternion.identity, mask.transform);
                StartCoroutine(HandHold());
                PlayClip(pushC);
                if (Input.GetKey(KeyCode.C))
                {
                    hold = true;
                    speed = 0.1f;
                }
            }
            if (Input.GetKeyUp(KeyCode.C))
            {
                hold = false;
                StartCoroutine(HandOpen());
                speed = 0.3f;
                currentWound = null;
                woundPos = Vector3.zero;
                PlayClip(breathing);
            }
        }
    }

    IEnumerator HandHold()
    {
            rHandPic.sprite = changeRH[1];
            shadowPic.sprite = changeSP[1];
            yield return new WaitForSeconds(0.1f);
            rHandPic.sprite = changeRH[2];
            shadowPic.sprite = changeSP[2];
    }

    IEnumerator HandOpen()
    {
        rHandPic.sprite = changeRH[1];
        shadowPic.sprite = changeSP[1];
        yield return new WaitForSeconds(0.1f);
        rHandPic.sprite = changeRH[0];
        shadowPic.sprite = changeSP[0];
    }


    IEnumerator FadeOut()       //開啟
    {
        AgentInput.instance.isAni = true;   //記得關掉？
        alpha = 0;
        while (alpha < 1)
        {
            alpha += Time.deltaTime;
            blackImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
        allObj.SetActive(true);
        volume02.profile = profile02;
        volume.profile = profile;
        canMove = true;
        StartCoroutine(FadeIn());
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
    }

    IEnumerator CloseFadeOut()      //關掉
    {
        alpha = 0;
        while (alpha < 1)
        {
            alpha += Time.deltaTime;
            blackImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        allObj.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        GameObject.Find("AT-05").SendMessage("EndAT_05");
        //AgentInput.instance.isAni = false;
        //this.gameObject.SetActive(false);
        yield return null;
    }

    protected void PlayClip(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
    }
}