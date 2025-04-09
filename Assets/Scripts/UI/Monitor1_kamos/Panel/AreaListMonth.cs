using DG.Tweening;
using Onthesys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class AreaListMonth : MonoBehaviour
{
    ModelProvider modelProvider => UiManager.Instance.modelProvider;
    List<AreaListMonthItem> items = new();
    List<Image> imgRingCharts = new();

    private void Start()
    {
        var items = transform.Find("List_Panel").GetComponentsInChildren<AreaListMonthItem>();
        this.items.AddRange(items);
        
        var charts = transform.Find("Doughnut Chart").GetComponentsInChildren<Image>();
        imgRingCharts.AddRange(charts);

        UiManager.Instance.Register(UiEventType.Initiate, OnInitiate);
        UiManager.Instance.Register(UiEventType.ChangeAlarmMonthly, OnInitiate);
    }

    private void OnInitiate(object obj)
    {
        var alarmMonthlyList = modelProvider.GetAlarmMonthly();

        //DB에서 받은 데이터가 없는 경우, 시연용 데이터로 대체
        if (alarmMonthlyList.Count == 0)
            alarmMonthlyList = new() {
                (1,31),(2,24),(3,21),(4,16),(5,7),
            };

        //상위 5개 지역의 알람 총계를 산출
        int sum = alarmMonthlyList.Sum(item => item.count);

        //AreaListMonthItem 업데이트
        for (int i = 0; i < items.Count; i++)
        {
            AreaListMonthItem item = items[i];
            (int, int) alarmYearly = alarmMonthlyList[i];
            AreaData area = modelProvider.GetArea(alarmYearly.Item1);
            float percent = (float)alarmYearly.Item2 / sum;

            item.SetAreaData(area.areaId, area.areaName, alarmYearly.Item2, percent);
        }


        //RingChart 업데이트
        const float fillRatioMin = 0.01f; // 최소 fillAmount 값

        var duration = 1f;
        var rotation = fillRatioMin;

        for (int i = 0; i < items.Count; i++)
        {
            (int, int) alarmYearly = alarmMonthlyList[i];
            float p = (float)alarmYearly.Item2 / sum;

            var setPercent = p < fillRatioMin ? fillRatioMin : p;
            imgRingCharts[i].DOFillAmount(setPercent, duration);
            imgRingCharts[i].transform.DOLocalRotate(new Vector3(0, 0, rotation), duration);
            
            rotation -= (360 * setPercent);
        }



    }
}