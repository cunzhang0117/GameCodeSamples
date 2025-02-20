using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiaryButton : MonoBehaviour
{
    private Image image;
    public bool isSelecting, isPushed; 
    public Sprite normal, selecting, pushing, pushed;
    public bool correct;
    // Start is called before the first frame update
    void Start()
    {
        DiaryCtrl.n = 0;
        DiaryCtrl.j = 0;
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isSelecting && !isPushed) image.sprite = selecting;       //正常選中
        else if (isPushed && isSelecting) image.sprite = pushing;         //選中且被按
        else if (isPushed && !isSelecting) image.sprite = pushed;
        else image.sprite = normal;
    }

    void itemSelected()     //被選中
    {
        isSelecting = (!isSelecting);
    }

    void itemPushed()
    {
        isPushed = (!isPushed);
        if (isPushed)
        {
            DiaryCtrl.n++;
            if (correct) DiaryCtrl.j++;
        }
        else {
            DiaryCtrl.n--;
            if (correct) DiaryCtrl.j--;
        }
    }

    void ResetButton()
    {
        isSelecting = false;
        isPushed = false;
    }
}