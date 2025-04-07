using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class MapNation : MonoBehaviour
{
    List<MapNationMarker> nationMarkers;
    Image imgBackground;

    private void Start()
    {
        UiManager.Instance.Register(UiEventType.NavigateHome, OnNavigateHome);
        UiManager.Instance.Register(UiEventType.NavigateArea, OnNavigateArea);
        UiManager.Instance.Register(UiEventType.NavigateObs, OnNavigateObs);

        nationMarkers = transform.Find("MarkerList").GetComponentsInChildren<MapNationMarker>(true).ToList();
        imgBackground = transform.Find("MapNationBackground").GetComponent<Image>();

        //DEBUG
        for (int i = 0; i < nationMarkers.Count; i++)
        {
            MapNationMarker nationMarker = nationMarkers[i];
            nationMarker.SetAreaData(i + 1, nationMarker.areaName, nationMarker.areaType, nationMarker.status);
        }
    }
    private void OnNavigateArea(object obj)
    {
        //this.gameObject.SetActive(false);
        SetAnimation(0f, new Vector3(960, 540) + new Vector3(700, 300), 0.60f, 1f);
    }
    private void OnNavigateHome(object obj)
    {
        //this.gameObject.SetActive(true);
        SetAnimation(2 / 5f, new Vector3(960,540), 1f, 1f);
    }
    private void OnNavigateObs(object obj)
    {
        //this.gameObject.SetActive(false);
        SetAnimation(0f, new Vector3(960, 540) + new Vector3(700, 300), 0.60f, 1f);
    }


    void SetAnimation(float alpha, Vector3 toPos, float toScale, float duration) 
    {
        Color fromColor = imgBackground.color;
        Vector3 fromPos = GetComponent<RectTransform>().position;
        Vector3 fromScale = GetComponent<RectTransform>().localScale;

        DOTween.ToAlpha(() => fromColor, x => fromColor = x, alpha, duration/2f).OnUpdate(() => {
            imgBackground.color = fromColor;
        });

        DOTween.To(() => fromPos, x => fromPos = x, toPos, duration).OnUpdate(() => {
            GetComponent<RectTransform>().position = fromPos;
        });

        DOTween.To(() => fromScale, x => fromScale = x, Vector3.one * toScale, duration).OnUpdate(() => {
            GetComponent<RectTransform>().localScale = fromScale;
        });
    }
}

