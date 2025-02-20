using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Fungus;

public class CorridorMG : MonoBehaviour
{
    [Header("燈")]
    public GameObject dayLight;
    public GameObject nightLight;
    public GameObject volume;
    public VolumeProfile dayOne;

    [Header("出入")]
    public GameObject bathroom, bathroomExit;
    public GameObject MBroom, MBroomExit;
    public GameObject sisRoom, sisRoomExit;
    public GameObject Brotherroom, stair;
    public GameObject atticRoom, atticExit;
    public GameObject studyRoom;

    [Header("妹妹")]
    public GameObject sister, sister_04;
    public Flowchart sis_flowchart, sis_flowchart_04;
    public static bool sis_talked, sis_AT_04; //和妹妹第一次談話過
    public GameObject bag;
    public Animator sisAT_01, sisAT_04;

    [Header("紙條")]
    public GameObject A4;
    public Flowchart Scratch;

    [Header("調查物")]
    public GameObject[] investigate;
    public int objNum;

    [Header("AT-05")]
    public static bool openDialog;      //判斷是否開啟
    public GameObject AT_05;
    public GameObject doorFor5;         //判斷變場景的門
    public GameObject handEvent;

    // Start is called before the first frame update
    void Start()
    {
        if (EventManager.instance.AT_01)
        {
            sister.SetActive(true);
            if (!sis_talked)
            {
                AgentInput.instance.isAni = true;
                sisAT_01.SetTrigger("run");
            }
            else if (sis_talked) sister.transform.position = new Vector3(-4.06f, 0.75f, 0f); 
        }
        if (ItemManager.instance.isDay)     //如果睡過了
        {
            //EventManager.instance.bagCanOpen = true;
            nightLight.SetActive(false);
            dayLight.SetActive(true);
            volume.GetComponent<Volume>().profile = dayOne;
            //sister.SetActive(true);
            MBroom.SetActive(false);
            MBroomExit.SetActive(true);
            sisRoom.SetActive(false);
            sisRoomExit.SetActive(true);
            atticRoom.SetActive(false);
            atticExit.SetActive(true);

            for (int i = 0; i < investigate.Length; i++)
            {
                investigate[i].tag = "Investigable";
            }
        }
        if (ItemManager.instance.pajamas)
        {
            bathroomExit.SetActive(true);
            Destroy(bathroom);
        }else if(ItemManager.instance.pajamas) bathroomExit.SetActive(false);

        if (EventManager.instance.A4)
        {
            A4.SetActive(true);
            investigate[1].tag = "UnInves";
        }
        if (EventManager.instance.AT_04)       //妹妹的強制對話
        {
            //sisAT_04.SetBool("bear0", true);
            if (!sis_AT_04)
                AgentInput.instance.isAni = true;
            sisRoomExit.SetActive(false);
            sister_04.SetActive(true);
            if (!sis_AT_04) Invoke("CallAT_04", 1f);
            //MBroomExit.SetActive(false);
        }
        else sisAT_04.SetBool("bear0", false) ;
        if (EventManager.instance.AT_05)
        {
            doorFor5.SetActive(true);
        }
        //else if (!EventManager.instance.AT_05) EventManager.instance.AT_05ChangeSC = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (EventManager.instance.AT_04)
            sisAT_04.SetBool("bear0", true);
        if (EventManager.instance.AT_05) MBroomExit.SetActive(false);
        if (openDialog)
        {
            AT_05.SetActive(true);
        }
        sis_flowchart.SetBooleanVariable("sis_talked", sis_talked);
        sis_flowchart_04.SetBooleanVariable("sis_talked", sis_AT_04);
        Scratch.SetBooleanVariable("getcoin", ItemManager.instance.coin);
        Scratch.SetBooleanVariable("paintcheck", ItemManager.instance.paintCheck);

        if (EventManager.instance.A4)
        {
            investigate[1].tag = "UnInves";
        }

        if (AgentInput.instance.bagIsOpen)
        {
            bag = GameObject.Find("BagSystem/Bag");
        }
    }

    void SisterTalked()
    {
        GameObject.Find("EventManger").SendMessage("BagOpen");
    }

    void SisterTalKed_04()
    {
        GameObject.Find("EventManger").SendMessage("AT_04talked");
        EventManager.instance.AT_05 = true;
        doorFor5.SetActive(true);
        AgentInput.instance.isAni = false;
        //EventManager.instance.AT_04 = false;
        //sis_AT_04 = true;
    }

    void CallAT_04()
    {
        //AgentInput.instance.isAni = true;
        MBroomExit.SetActive(false);
        sis_flowchart_04.SendFungusMessage("start");
        CloseBag();
    }

    void CloseBag()
    {
        if (bag != null)
        {
            bag.SetActive(false);
            AgentInput.instance.bagIsOpen = false;
        }
    }

    void CloseDoor()        //forAT-05
    {
        bathroomExit.SetActive(false);
        MBroomExit.SetActive(false);
        sisRoomExit.SetActive(false);
        Brotherroom.SetActive(false);
        stair.SetActive(false);
        atticExit.SetActive(false);
        studyRoom.SetActive(false);
    }

    void OpenDoor()
    {
        bathroomExit.SetActive(true);
        MBroomExit.SetActive(true);
        sisRoomExit.SetActive(true);
        Brotherroom.SetActive(true);
        stair.SetActive(true);
        studyRoom.SetActive(true);
    }
}