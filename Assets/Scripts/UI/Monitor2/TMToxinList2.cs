using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Onthesys;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Unity.VisualScripting;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using System;

public class TMToxinList2 : MonoBehaviour
{
    private LogData log;
    public List<TMToxinBar2> bars;
    public RectTransform scrollContainer;

    public TMP_Text txtName;
    public SetTime txtTime;
    public SetTime txtTime2;

    // Start is called before the first frame update
    void Start()
    {
        //DataManager.Instance.OnSelectLog.AddListener(this.UpdateText);
        //DataManager.Instance.OnLoadAlarmData.AddListener(this.ChangeValue);
    }

    public void OnSelectLog(LogData data = null)
    {
        this.log = data;
    }

    public void OnLoadAlarmData(List<ToxinData> alaToxinDatas)
    {
		float scrollHeight = 0;
        var datas = alaToxinDatas;
        for (int i = 0; i < this.bars.Count; i++)
        {
            this.bars[i].gameObject.SetActive(datas[i].on);
            if (datas[i].on)
			{
				this.bars[i].statusIcon.ForEach(icon => icon.gameObject.SetActive(false));

                this.bars[i].toxinName.text = datas[i].hnsName;
                this.bars[i].warning.text = datas[i].warning.ToString();
                this.bars[i].current.text = datas[i].GetLastValue().ToString();
                this.bars[i].data = datas[i];
                this.bars[i].dt = log.time;
                var lcahrt = datas[i].values.Select(t => t / datas[i].warning).ToList();
                this.bars[i].line.UpdateControlPoints(lcahrt);

                if(datas[i].status == ToxinStatus.Purple)
                    this.bars[i].statusIcon[3].gameObject.SetActive(true);
                else if(datas[i].status == ToxinStatus.Red)
                    this.bars[i].statusIcon[2].gameObject.SetActive(true);
                else if (datas[i].status == ToxinStatus.Yellow)
                    this.bars[i].statusIcon[1].gameObject.SetActive(true);
                else
                    this.bars[i].statusIcon[0].gameObject.SetActive(true);

                scrollHeight += this.bars[i].GetComponent<RectTransform>().rect.height;
            }
        }
        this.scrollContainer.GetComponent<RectTransform>().DOSizeDelta(new Vector2(500, scrollHeight), 0);

        this.txtTime.SetText(this.log.time);
        this.txtTime2.SetText(this.log.time);
        this.txtName.text = this.log.areaName + " - " + this.log.obsName;
    }

    public void ShowFocus()
    {
        this.gameObject.GetComponent<RectTransform>().DOAnchorPosY(0, 0);
    }

    public void HideFocus()
    {
        this.gameObject.GetComponent<RectTransform>().DOAnchorPosY(-600, 0);
    }

    
}
