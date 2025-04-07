using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TitleArea : MonoBehaviour
{
    TMP_Text lblTitleArea;
    Image imgTitleArea;
    private void Start()
    {
        UiManager.Instance.Register(UiEventType.NavigateHome, OnNavigateHome);
        UiManager.Instance.Register(UiEventType.NavigateArea, OnNavigateArea);
        UiManager.Instance.Register(UiEventType.NavigateObs, OnNavigateObs);

        lblTitleArea = GetComponentInChildren<TMP_Text>();
        imgTitleArea = GetComponent<Image>();

        //imgTitleArea.color = new(1f, 1f, 1f, 0f);
        //lblTitleArea.color = new(1f, 1f, 1f, 0f);
        gameObject.SetActive(false);
    }

    private void OnNavigateArea(object obj)
    {
        //this.gameObject.SetActive(false);
        //SetAnimation(1f, new Vector3(960, 540) + new Vector3(0, 324), 0.5f);

        transform.position = new Vector3(960, 540) + new Vector3(0, 324);
        gameObject.SetActive(true);
    }
    private void OnNavigateHome(object obj)
    {
        //this.gameObject.SetActive(true);
        //SetAnimation(0f, new Vector3(960,  540), 0.5f);

        //transform.position = new Vector3(960, 540);
        gameObject.SetActive(false);
    }
    private void OnNavigateObs(object obj)
    {
        //this.gameObject.SetActive(false);
        //SetAnimation(1f, new Vector3(960, 540) + new Vector3(0, 425), 0.5f);

        transform.position = new Vector3(960, 540) + new Vector3(0, 425);
        gameObject.SetActive(true);
    }


    void SetAnimation(float alpha, Vector3 toPos, float duration)
    {
        float fromAlpha = lblTitleArea.color.a;
        Vector3 fromPos = GetComponent<RectTransform>().position;
        Vector3 fromScale = GetComponent<RectTransform>().localScale;

        DOTween.To(() => fromAlpha, x => fromAlpha = x, alpha, duration / 2f).OnUpdate(() => {
            lblTitleArea.color = new(1f, 1f, 1f, fromAlpha);
            imgTitleArea.color = new(1f, 1f, 1f, fromAlpha / 2f);
        });

        DOTween.To(() => fromPos, x => fromPos = x, toPos, duration).OnUpdate(() => {
            GetComponent<RectTransform>().position = fromPos;
        });

    }
}

