using Onthesys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;


internal class ToxinList3 : MonoBehaviour
{

    private LogData log;
    private List<ArcBar> bars = new List<ArcBar>();
    public RectTransform scrollContainer;
    public TMP_Text txtName;

    private void Start()
    {
        InitializeBars();
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

    public void InitializeBars()
    {
        Transform parent = transform.Find("Content");

        if (parent == null)
        {
            Debug.LogError("Content를 찾을수 없음");
            return;
        }
        foreach (Transform child in parent)
        {
            ArcBar arcBar = child.GetComponent<ArcBar>();
            if (arcBar != null)
            {
                bars.Add(arcBar);
            }
            else
            {
                Debug.LogWarning("ArcBar 컴포넌트가 없는 객체");
            }
        }
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
                this.bars[i].gameObject.SetActive(toxinDatas[i].on);
                if (toxinDatas[i].on)
                {
                    this.bars[i].txtName.text = toxinDatas[i].hnsName;
                    this.bars[i].txtCurrent.text = ((int)toxinDatas[i].GetLastValue()).ToString();
                    this.bars[i].txtTotal.text = "/" + toxinDatas[i].warning.ToString();
                    this.bars[i].txtProgress.text = toxinDatas[i].GetLastValue().ToString();
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
}

