using DG.Tweening;
using Onthesys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;


/*internal class ToxinList3 : MonoBehaviour
{
    ModelProvider modelProvider => UiManager.Instance.modelProvider;
    private LogData log;
    private List<ArcBar> bars;
    public RectTransform scrollContainer;
    public TMP_Text txtName;

    private void Start()
    {
        UiManager.Instance.Register(UiEventType.SelectCurrentSensor, OnSelectLog);
        UiManager.Instance.Register(UiEventType.ChangeSensorList, OnLoadSetting);
        UiManager.Instance.Register(UiEventType.ChangeSensorStatus, OnIntervalToxinValue);
        UiManager.Instance.Register(UiEventType.SelectAlarmSensor, OnSelectObs);

        this.StartValue();
    }



    public void StartValue()
    {
        for (int i = 0; i < this.bars.Count; i++)
        {
            this.bars[i].gameObject.SetActive(true);
            this.bars[i].txtName.text = "";
            this.bars[i].txtCurrent.text = "";
            this.bars[i].txtProgress.text = "0";
            this.bars[i].txtTotal.text = "";
            this.bars[i].SetArc(0);
        }
        this.txtName.text = "";
    }


    public void OnSelectLog(object data)
    {
        if (data is LogData logData)
        {
            log = logData;
        }
    }
    public void OnLoadSetting(object data)
    {
        if (data is List<ToxinData> toxinDatas)
        {
            Debug.Log("[OnLoadSetting] ToxinData received.");
            float scrollHeight = 0;

            for (int i = 0; i < this.bars.Count; i++)
            {
                this.bars[i].id = i;
                this.bars[i].gameObject.SetActive(toxinDatas[i].on);
                if (toxinDatas[i].on)
                {
                    this.bars[i].txtName.text = toxinDatas[i].hnsName;
                    this.bars[i].txtCurrent.text = ((int)toxinDatas[i].GetLastValue()).ToString();
                    this.bars[i].txtTotal.text = "/" + toxinDatas[i].warning.ToString();
                    this.bars[i].txtProgress.text = toxinDatas[i].GetLastValue().ToString();
                    this.bars[i].txtMax.text = Math.Round(toxinDatas[i].warning, 2).ToString();
                    this.bars[i].txtMin.text = "0";
                    this.bars[i].data = toxinDatas[i];
                    this.bars[i].SetArc(toxinDatas[i].GetLastValuePercent());
                    scrollHeight += this.bars[i].GetComponent<RectTransform>().rect.height;
                }
            }
            this.scrollContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(1230, (scrollHeight / 7) + 76);
        }
    }
    public void OnIntervalToxinValue(object data)
    {
        if (data is List<ToxinData> toxinDatas)
        {
            Debug.Log("[OnIntervalToxinValue] Interval Toxin Data received.");
            for (int i = 0; i < this.bars.Count; i++)
            {
                this.bars[i].txtCurrent.text = toxinDatas[i].GetLastValue().ToString();
                this.bars[i].txtProgress.text = toxinDatas[i].GetLastValue().ToString();
                this.bars[i].SetArc(toxinDatas[i].GetLastValuePercent());
            }
        }
    }
    public void OnSelectObs(object data)
    {
        if (data is int obsId)
        {
            Debug.Log($"[OnSelectObs] Observation ID: {obsId}");
        }
    }
}*/

internal class ToxinList3 : MonoBehaviour
{
    ModelProvider modelProvider => UiManager.Instance.modelProvider;

    private LogData log;
    public List<ArcBar> bars;
    public RectTransform scrollContainer;
    public TMP_Text txtName;

    private int obsId = 0; // 추가: obsId 저장용

    private void Start()
    {
        UiManager.Instance.Register(UiEventType.SelectAlarmSensor, OnSelectObs);
        UiManager.Instance.Register(UiEventType.ChangeSensorList, OnLoadSetting);
        UiManager.Instance.Register(UiEventType.ChangeSensorStatus, OnIntervalToxinValue);
        UiManager.Instance.Register(UiEventType.SelectAlarmSensor, OnSelectObs);

        this.StartValue();
    }

    public void StartValue()
    {
        foreach (var bar in this.bars)
        {
            bar.gameObject.SetActive(true);
            bar.txtName.text = "";
            bar.txtCurrent.text = "";
            bar.txtProgress.text = "0";
            bar.txtTotal.text = "";
            bar.SetArc(0);
        }
        this.txtName.text = "";
    }

    public void OnSelectLog(object data)
    {
        if (data is LogData logData)
        {
            log = logData;
        }
    }

    public void OnLoadSetting(object data)
    {
        if (data is List<ToxinData> toxins)
        {
            float scrollHeight = 0;

            for (int i = 0; i < this.bars.Count; i++)
            {
                this.bars[i].id = i;
                this.bars[i].gameObject.SetActive(toxins[i].on);

                if (toxins[i].on)
                {
                    this.bars[i].txtName.text = toxins[i].hnsName;
                    this.bars[i].txtCurrent.text = ((int)toxins[i].GetLastValue()).ToString();
                    this.bars[i].txtTotal.text = "/" + toxins[i].warning.ToString();
                    this.bars[i].txtProgress.text = toxins[i].GetLastValue().ToString();
                    this.bars[i].txtMax.text = Math.Round(toxins[i].warning, 2).ToString();
                    this.bars[i].txtMin.text = "0";
                    this.bars[i].data = toxins[i];
                    this.bars[i].SetArc(toxins[i].GetLastValuePercent());
                    scrollHeight += this.bars[i].GetComponent<RectTransform>().rect.height;
                }
            }

            this.scrollContainer.DOSizeDelta(new Vector2(1230, (scrollHeight / 7) + 76), 0);

            string areaName = modelProvider.GetArea(obsId).areaName;
            string obsName = modelProvider.GetObs(obsId).obsName;
            this.txtName.text = $"{areaName} - {obsName} 실시간 상태";

            UiManager.Instance.Invoke(UiEventType.SelectCurrentSensor, 0); // 초기 센서 선택
        }
    }

    private void OnIntervalToxinValue(object data)
    {
        if (data is List<ToxinData> toxinDatas)
        {
            var toxins = toxinDatas;

            float scrollHeight = 0;
            for (int i = 0; i < this.bars.Count; i++)
            {
                this.bars[i].gameObject.SetActive(toxins[i].on);
                if (toxins[i].on)
                {
                    this.bars[i].txtCurrent.text = toxins[i].GetLastValue().ToString();
                    this.bars[i].txtTotal.text = "/" + toxins[i].warning.ToString();
                    this.bars[i].txtProgress.text = toxins[i].GetLastValue().ToString();
                    this.bars[i].txtMax.text = Math.Round(toxins[i].warning, 2).ToString();
                    this.bars[i].txtMin.text = "0";
                    this.bars[i].data = toxins[i];
                    this.bars[i].SetArc(toxins[i].GetLastValuePercent());
                    scrollHeight += this.bars[i].GetComponent<RectTransform>().rect.height;
                }
            }
            this.scrollContainer.GetComponent<RectTransform>().DOSizeDelta(new Vector2(1230, (scrollHeight / 7) + 76), 0);

            UiManager.Instance.Invoke(UiEventType.ChangeTrendLine);
        }
    }

    public void OnSelectObs(object data)
    {
        if (data is LogData logData)
        {
            this.log = logData;
            this.obsId = logData.obsId;

            // 💡 이미 저장된 센서 정보 사용
            var toxins = modelProvider.GetToxinsInLog();
            UiManager.Instance.Invoke(UiEventType.ChangeSensorList, toxins);
        }
    }
}
