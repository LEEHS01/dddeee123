using DG.Tweening;
using Onthesys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ObsMonitoring : MonoBehaviour
{
    ModelProvider modelProvider => UiManager.Instance.modelProvider;

    Vector3 defaultPos;
    Button btnSetting;

    List<ObsMonitoringItem> items = new();

    int obsId;

    private void Start()
    {
        UiManager.Instance.Register(UiEventType.NavigateHome, OnNavigateHome);
        UiManager.Instance.Register(UiEventType.NavigateArea, OnNavigateArea);
        UiManager.Instance.Register(UiEventType.NavigateObs, OnNavigateObs);

        UiManager.Instance.Register(UiEventType.ChangeSensorList, OnChangeSensorList);
        UiManager.Instance.Register(UiEventType.ChangeTrendLine, OnChangeTrendLine);
        UiManager.Instance.Register(UiEventType.ChangeSensorStatus, OnChangeSensorStatus);

        btnSetting = transform.Find("Btn_Setting").GetComponent<Button>();
        btnSetting.onClick.AddListener(OnClickSetting);

        defaultPos = transform.position;

        ObsMonitoringItem board1 = transform.Find("List_A").GetComponentInChildren<ObsMonitoringItem>();
        items.Add(board1);
        List<ObsMonitoringItem> board2 = transform.Find("List_B").Find("Scroll").GetComponentsInChildren<ObsMonitoringItem>().ToList();
        items.AddRange(board2);
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


    private void OnChangeSensorStatus(object obj)
    {
        throw new NotImplementedException();
    }
    private void OnChangeTrendLine(object obj)
    {
        for (int i = 0; i < items.Count; i++)
        {
            ObsMonitoringItem item = items[i];

            item.UpdateTrendLine();
        }
    }
    private void OnChangeSensorList(object obj)
    {
        List<ToxinData> toxins = modelProvider.GetToxins();

        for (int i = 0; i < items.Count; i++)
        {
            ToxinData toxin = toxins[i];
            ObsMonitoringItem item = items[i];

            item.SetToxinData(obsId, toxin);
        }   
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