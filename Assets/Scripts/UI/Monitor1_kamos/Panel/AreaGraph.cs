using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using static UnityEditorInternal.ReorderableList;

public class AreaGraph : MonoBehaviour 
{
    Vector3 defaultPos;
    private void Start()
    {
        UiManager.Instance.Register(UiEventType.NavigateHome, OnNavigateHome);
        UiManager.Instance.Register(UiEventType.NavigateArea, OnNavigateArea);
        UiManager.Instance.Register(UiEventType.NavigateObs, OnNavigateObs);

        //imgAreaType = transform.Find("icon").GetComponent<Image>();
        //lblTitle = transform.Find("Text (TMP)").GetComponent<TMP_Text>();

        //foreach (Transform item in transform.Find("List_Panel"))
        //    items.Add(item);

        defaultPos = transform.position;
    }
    private void OnNavigateArea(object obj)
    {
        //this.gameObject.SetActive(false);
        SetAnimation(defaultPos + new Vector3(0f, +400f), 1f);
    }
    private void OnNavigateHome(object obj)
    {
        //this.gameObject.SetActive(true);
        SetAnimation(defaultPos, 1f);
    }
    private void OnNavigateObs(object obj)
    {
        //this.gameObject.SetActive(false);
        SetAnimation(defaultPos, 1f);
    }


    void SetAnimation(Vector3 toPos, float duration)
    {
        Vector3 fromPos = GetComponent<RectTransform>().position;
        DOTween.To(() => fromPos, x => fromPos = x, toPos, duration).OnUpdate(() => {
            GetComponent<RectTransform>().position = fromPos;
        });

    }
}