using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Onthesys;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using System;

public class TMBarList : MonoBehaviour
{
    public TMP_Text title;
    public List<BarProgress> bars;
    public List<TMP_Text> verticals;

    private int areaidx = 0;
    private string mapName;

    private const float MinFillAmount = 0.01f; // 최소 fillAmount 값

    // Start is called before the first frame update
    void Start()
    {
        //DataManager.Instance.OnSelectArea.AddListener(this.OnLoadAlarm);
        ////DataManager.Instance.OnAlarmUpdate.AddListener(OnUpdateAlarm);
        //DataManager.Instance.OnChangeSummary.AddListener(this.OnUpdate);
    }

    public void OnLoadAlarm(int idx)
    {
        this.areaidx = idx + 1;
        mapName = DataManager.Instance.GetAreaNameById(areaidx);
        this.title.text = $"{mapName} 지역 알람 발생";


        //StartCoroutine(DataManager.Instance.LoadAlarmSummary(areaidx));
    }

    public void OnUpdateAlarm()
    {
        //var ala = DataManager.Instance.alarmLogs.Count(t => t.areaName.Equals(mapName));
        //if(ala > 0)
        //{
        //    StartCoroutine(DataManager.Instance.LoadAlarmSummary(areaidx));
        //}
    }

    public void OnChangeSummary(List<AlarmSummaryModel> summaryModels)
    {
        var max = summaryModels.Count < 1f ? 0f : summaryModels.Max(t => (float)t.cnt);
        var dic = summaryModels.GroupBy(t => t.obsidx).ToDictionary(t => t.Key, t => t.ToList());

        this.SetVertical(max);

        int i = 0;
        foreach (var item in dic)
        {
            bars[i].obsName.text = DataManager.Instance.GetObsNameById(item.Key);
            var montly = item.Value.ToDictionary(t => t.month, t => t.cnt);
            bars[i].SetMaxVlaue(max);
            bars[i].SetBarValue(montly);
            i++;
        }
    }

    void SetVertical(float max)
    {
        var verticalMax = max >= this.verticals.Count ? ((max + 1) / (verticals.Count - 1)) : 1;

        for (int i = 0; i < this.verticals.Count; i++)
        {
            this.verticals[i].text = Math.Round((verticalMax * i), 2).ToString();
        }
    }

    
}
