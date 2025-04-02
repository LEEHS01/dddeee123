using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Onthesys;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Unity.VisualScripting;
using System;

public class TMToxinList3 : MonoBehaviour
{
    private LogData log;
    public List<TmArcBar> bars;
    public RectTransform scrollContainer;
    public TMP_Text txtName;

    // Start is called before the first frame update
    void Start()
    {
        //DataManager.Instance.OnSelectLog.AddListener(this.UpdateText);
        //DataManager.Instance.OnLoadSetting.AddListener(this.ChangeValue);
        //DataManager.Instance.OnUpdateValue.AddListener(this.UpdateValue);
        this.StartValue();
    }


    public void StartValue() {
        for(int i = 0; i < this.bars.Count; i++) {
            this.bars[i].gameObject.SetActive(true);
            this.bars[i].txtName.text = "";
            this.bars[i].txtCurrent.text = "";
            this.bars[i].txtProgress.text = "0";
            this.bars[i].txtTotal.text = "";
            this.bars[i].SetArc(0);
        }
        this.txtName.text = "";
    }



    //DataManager EventListener
    public void OnSelectLog(LogData data)
    {
        this.log = data;
        this.obsId = 0;

    }

    public void OnLoadSetting(List<ToxinData> toxinDatas) 
    {
        var toxins = toxinDatas;
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
        this.scrollContainer.GetComponent<RectTransform>().DOSizeDelta(new Vector2(1230, (scrollHeight / 7) + 76), 0);


        string areaName = (obsId != 0) ? DataManager.Instance.GetAreaNameById(obsId) : this.log.areaName;
        string obsName = (obsId != 0) ? DataManager.Instance.GetObsNameById(obsId) : this.log.obsName;
        this.txtName.text = $"{areaName} - {obsName} 실시간 상태";

        DataManager.Instance.RequestOnSelectToxin(0);
    }
    
    public void OnIntervalToxinValue(List<ToxinData> toxinDatas)
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
        DataManager.Instance.RequestOnUpdateChart();
    }
    
    int obsId = 0;

    public void OnSelectObs(int obsId)
    {
        this.obsId = obsId;
        this.log = null;
    }
}
