using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class Dialog01 : MonoBehaviour
{
    //public GameObject point;
    //private GameObject target;

    public static bool canRay;     //現階段是否可以調查?

    public Material mat;
    public Material normalMat;
    public GameObject currentObj;
    public GameObject interObj;

    public TextAsset textFile02;    //測試
    public bool ok;

    public bool canInput;

    public TMP_Text textLabel;  //文字物件
    public GameObject talkUI;   //文字框
    //public GameObject dialogUI;
    static public Animator anim;

    static public bool canCheck, canCheckDoor;   //是否有碰撞到調查物
    
    private bool playerMove;    //玩家是否移動中

    [Header("文本文件")]
    static public TextAsset textFile;  //描述內容
    public int index;

    List<string> textList = new List<string>();

    public bool textFinished;

    Vector2 dir;
    // Start is called before the first frame update
    void Start()
    {
        canRay = true;
        textFinished = true;
        anim = talkUI.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (canInput)
        {
            if (Input.GetKeyDown(KeyCode.C) && !AgentInput.instance.bagIsOpen)
            {
                //interObj.GetComponent<water>().SendMessage
                if (!interObj.GetComponent<water>().opening)
                    interObj.SendMessage("OpenObj");
                else if (interObj.GetComponent<water>().opening && !interObj.GetComponent<water>().needClose) interObj.GetComponent<water>().SendMessage("CloseObj");
            }
            if (Input.GetKeyDown(KeyCode.X) && !AgentInput.instance.bagIsOpen)
            {
                if (interObj.GetComponent<water>().opening && interObj.GetComponent<water>().needClose) interObj.GetComponent<water>().SendMessage("CloseObjWithX");
            }
        }
        /*if (target != null)
        {
            point.transform.position = new Vector2(target.transform.position.x, target.transform.position.y);
        }*/
        if (talkUI.activeInHierarchy)       //如果文字框顯示中
        {
            if (Input.GetKeyDown(KeyCode.C) && index == textList.Count && textFinished || playerMove)
            {
                anim.SetBool("close", true);
                anim.SetBool("show", false);
                return;
            }
            if (Input.GetKeyDown(KeyCode.C) && textFinished)
            {
                StartCoroutine(SetTextUI());
            }
            if (AgentInput.instance.bagIsOpen)
            {
                talkUI.SetActive(false);
                CleanText();
            }
        }
        if (textFile == null)
        {
            //point.SetActive(false);
            if (currentObj != null)
            {
                currentObj.GetComponent<Renderer>().material = normalMat;
                currentObj = null;
            }
        }

        if (NPCTalk.nearNPC)
        {
            canCheck = false;
            canCheckDoor = false;
        }
        if (canCheck || canCheckDoor) ShowDialog();
    }

    public void CleanText()
    {
        textLabel.text = "";
        index = 0;
    }
    public void PhysicsCheck(Vector2 movementInput)
    {
        if (movementInput != Vector2.zero)
        {
            dir = movementInput;
            playerMove = true;
        }
        else playerMove = false;
        int layerMask = ~(1 << 6);
        if (canRay) { 
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, 1.0f, layerMask);
            Debug.DrawRay(transform.position, dir, Color.red, 1.0f);
            if (hit.collider)
            {
                //Debug.Log("Hit something: " + hit.collider.name);
                if (hit.collider.tag == "Investigable")
                {
                    hit.transform.gameObject.SendMessage("RandomChangeInfo");
                    if (currentObj == null)     //如果當前物件為空
                    {
                        currentObj = hit.transform.gameObject;      //設當前物件為hit物件
                        currentObj.GetComponent<Renderer>().material = mat;     //改變當前物件材質
                    }
                    else if (currentObj != hit.transform.gameObject)    //如果當前不等於hit物件
                    {
                        currentObj.GetComponent<Renderer>().material = normalMat;
                        currentObj = null;
                    }

                    //point.SetActive(true);
                    //if(hit.transform.childCount > 0)
                        //target = hit.transform.GetChild(0).gameObject;
                    textFile = hit.collider.GetComponent<StuffInfo>().info;

                    canCheck = true;

                    Debug.Log("Hit something: " + hit.collider.name);
                }

                if (hit.collider.tag == "interactive")  //可互動物件
                {
                    if (interObj == null)     //如果當前物件為空
                    {
                        interObj = hit.transform.gameObject;      //設當前物件為hit物件
                        interObj.GetComponent<Renderer>().material = mat;     //改變當前物件材質
                        canInput = true;
                    }
                    Debug.Log("Hit something: " + hit.collider.name);
                }
                if (interObj != null && interObj != hit.transform.gameObject)    //如果當前不等於hit物件
                {
                    interObj.GetComponent<Renderer>().material = normalMat;
                    interObj = null;
                    canInput = false;
                }
                if (currentObj != null && currentObj != hit.transform.gameObject)
                {
                    currentObj.GetComponent<Renderer>().material = normalMat;
                    currentObj = null;
                    textFile = null;
                }
            }
            else
            {
                if(!canCheckDoor)
                    textFile = null;
                canCheck = false;

                if (interObj != null)
                {
                    interObj.GetComponent<Renderer>().material = normalMat;
                    interObj = null;
                    canInput = false;
                }
            }
        } 
    }

    void ShowDialog()
    {
        if (Input.GetKeyDown(KeyCode.C) && !talkUI.activeInHierarchy && !AgentInput.instance.bagIsOpen && textFile != null)
        {
            talkUI.SetActive(true);
            anim.SetBool("show", true);
            anim.SetBool("close", false);
            GetTextFromFile(textFile);

            StartCoroutine(SetTextUI());
        }
    }
    void GetTextFromFile(TextAsset file)
    {
        textList.Clear();
        index = 0;

        var lineData = file.text.Split('\n');
        foreach (var line in lineData)
        {
            textList.Add(line);
        }
    }
    IEnumerator SetTextUI()
    {
        textFinished = false;
        textLabel.text = "";
        for (int i = 0; i < textList[index].Length; i++)
        {
            textLabel.text += textList[index][i];

            yield return new WaitForSeconds(0.05f);
        }
        index++;
        textFinished = true;
    }
}