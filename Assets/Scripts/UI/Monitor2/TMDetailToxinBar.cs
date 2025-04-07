using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Onthesys;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using System;
using Unity.VisualScripting.Antlr3.Runtime;
using Unity.VisualScripting;
using UnityEngine.Events;

public class TMDetailToxinBar : MonoBehaviour
{
    ToxinData toxinData;
    private DateTime aladt;
    public TMP_Text txtName;
    public UILineRenderer2 line;
    public List<TMP_Text> hours;
    public List<TMP_Text> verticals;
    public GameObject btnDetail;

    void Start() 
    {
        //DataManager.Instance.OnSelectLog.AddListener(this.OnSelectLog);
        //DataManager.Instance.OnSelectToxin.AddListener(this.OnSelectToxin);
        //DataManager.Instance.OnUpdateChart.AddListener(this.OnUpdateCurrentValue);
        SetMins();
        this.txtName.text = "";
        this.btnDetail.SetActive(true);
    }

    public void OnSelectLog(LogData data = null)
    {
        this.aladt = data.time;
    }

    public void OnUpdateLog(LogData data = null)
    {
        //this.OnUpdateToxin(data.toxin);    
    }

    public void OnSelectToxin(ToxinData toxinData)
    {
        //Debug.Log("OnUpdateToxin");
        this.toxinData = toxinData;
        this.txtName.text = toxinData.hnsName;
        this.btnDetail.SetActive(true);

        var datas = ConvertToChartData(toxinData);
        this.line.UpdateControlPoints(datas);
        this.SetMins();
    }

    public void OnUpdateChart()
    {
        //Debug.Log("OnUpdateCurrentValue");
        var datas = ConvertToChartData(toxinData);
        this.line.UpdateControlPoints(datas);
        this.SetMins();
    }

    private List<float> ConvertToChartData(ToxinData toxin)
    {
        if (toxin == null) throw new Exception("TMDetailToxinBar.toxinData == null!!! 그래프를 그릴 수 없습니다!");
        
        var max = toxin.values.Max();
        var lchart = new List<float>();
        if (max > toxin.warning)
        {
            lchart = toxin.values.Select(t => t / max).ToList();
        }
        else
        {
            max = toxin.warning;
            lchart = toxin.values.Select(t => t / toxin.warning).ToList();
        }
        SetVertical(max);
        return lchart;
    }

    void SetHours() {
        int hour = DateTime.Now.Hour;
        for(int i = 0; i < this.hours.Count; i++) {
            this.hours[i].text = hour.ToString() + ":00";
            hour -= 3;
            if(hour <= 0) {
                hour = 24 - (3 - hour - 1);
            }
        }
    }
    void SetMins()
    {
        DateTime endDt = DateTime.Now;
        DateTime startDt = endDt.AddHours(-4);
        var turm = (endDt - startDt).TotalMinutes / this.hours.Count;

        for (int i = 0; i < this.hours.Count; i++)
        {
            var t = endDt.AddMinutes(-(turm * i));
            this.hours[i].text = t.ToString("HH:mm:ss");
        }
    }

    void SetVertical(float max)
    {
        var verticalMax = ((max + 1) / (verticals.Count - 1));

        for (int i = 0; i < this.verticals.Count; i++)
        {
            this.verticals[i].text = Math.Round((verticalMax * i), 2).ToString();
        }
    }

    public void OnDetailSelect()
    {
        DataManager.Instance.RequestOnSelectToxin(this.aladt, this.toxinData.hnsid);
    }
}
