using DG.Tweening;
using Onthesys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AreaListType : MonoBehaviour
{
    //Main Parameter
    public AreaData.AreaType areaType;
    public Sprite nuclearSprite;
    public Sprite oceanSprite;

    //Core Components
    Image imgAreaType;
    TMP_Text lblTitle;
    List<Transform> items = new();


    Vector3 defaultPos;

    private void OnValidate()
    {
        imgAreaType = transform.Find("icon").GetComponent<Image>();
        lblTitle = transform.Find("Text (TMP)").GetComponent<TMP_Text>();
        imgAreaType.sprite = areaType == AreaData.AreaType.Ocean ? oceanSprite : nuclearSprite;
        lblTitle.text = areaType == AreaData.AreaType.Ocean ? "해양산업시설 방류구 및 주변 해역" : "발전소 방류구 및 주변 해역";
    }

    private void Start()
    {
        UiManager.Instance.Register(UiEventType.NavigateHome, OnNavigateHome);
        UiManager.Instance.Register(UiEventType.NavigateArea, OnNavigateArea);
        UiManager.Instance.Register(UiEventType.NavigateObs, OnNavigateObs);

        imgAreaType = transform.Find("icon").GetComponent<Image>();
        lblTitle = transform.Find("Text (TMP)").GetComponent<TMP_Text>();

        foreach (Transform item in transform.Find("List_Panel"))
            items.Add(item);

        defaultPos = transform.position;
    }
    private void OnNavigateArea(object obj)
    {
        //this.gameObject.SetActive(false);
        SetAnimation(defaultPos + new Vector3(800, 0f), 1f);
    }
    private void OnNavigateHome(object obj)
    {
        //this.gameObject.SetActive(true);
        SetAnimation(defaultPos , 1f);
    }
    private void OnNavigateObs(object obj)
    {
        //this.gameObject.SetActive(false);
        SetAnimation(defaultPos + new Vector3(800, 0f), 1f);
    }


    void SetAnimation(Vector3 toPos, float duration)
    {
        Vector3 fromPos = GetComponent<RectTransform>().position;
        DOTween.To(() => fromPos, x => fromPos = x, toPos, duration).OnUpdate(() => {
            GetComponent<RectTransform>().position = fromPos;
        });

    }

}
