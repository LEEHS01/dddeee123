using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ObsMonitoring : MonoBehaviour
{
    Vector3 defaultPos;
    Button btnSetting;

    private void Start()
    {
        UiManager.Instance.Register(UiEventType.NavigateHome, OnNavigateHome);
        UiManager.Instance.Register(UiEventType.NavigateArea, OnNavigateArea);
        UiManager.Instance.Register(UiEventType.NavigateObs, OnNavigateObs);

        btnSetting = transform.Find("Btn_Setting").GetComponent<Button>();
        btnSetting.onClick.AddListener(OnClickSetting);

        defaultPos = transform.position;
    }
    private void OnNavigateArea(object obj)
    {
        SetAnimation(defaultPos, 1f);
    }
    private void OnNavigateHome(object obj)
    {
        SetAnimation(defaultPos, 1f);
    }
    private void OnNavigateObs(object obj)
    {
        SetAnimation(defaultPos + new Vector3(-575f, 0f), 1f);
    }
    private void OnClickSetting()
    {
        UiManager.Instance.Invoke(UiEventType.PopupSetting, null);
    }

    void SetAnimation(Vector3 toPos, float duration)
    {
        Vector3 fromPos = GetComponent<RectTransform>().position;
        DOTween.To(() => fromPos, x => fromPos = x, toPos, duration).OnUpdate(() => {
            GetComponent<RectTransform>().position = fromPos;
        });

    }
}