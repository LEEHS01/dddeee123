using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Onthesys;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using System.ComponentModel;
using System;

public class TMToxinList : MonoBehaviour
{
    private int id;
    public List<TMToxinBar> bars;
    public RectTransform scrollContainer;
    TMToxinListContainer toxinListContainer;
    //public ToxinStatus status;

    private float scrollHeight;
    // Start is called before the first frame update
    void Start()
    {
        toxinListContainer = GetComponentInParent<TMToxinListContainer>();
        toxinListContainer.onSelectObs.AddListener(this.OnSelectObs);
        toxinListContainer.onLoadSetting.AddListener(this.OnLoadSetting);
        toxinListContainer.onIntervalToxinValue.AddListener(this.OnIntervalToxinValue);
        toxinListContainer.onToxinStatusChange.AddListener(this.OnToxinStatusChange);
        toxinListContainer.onChangeToxin.AddListener(this.OnChangeToxin);
    }

    public void OnSelectObs(int id)
    {
        this.id = id;
        this.bars.ForEach(bar =>
        {
            try
            {
                bar.gameObject.SetActive(false);
            }
            catch (Exception ex) {
                Debug.LogException(ex);
                Debug.LogError($"TMToxinList - OnSelectObs {bar}");
                Debug.LogError($"TMToxinList - OnSelectObs {bar.gameObject}");
            }
        });
    }
    public void OnLoadSetting(List<ToxinData> toxinDatas)
    {
        var lHns = toxinDatas;
        scrollHeight = 0;
        for (int i = 0; i < bars.Count; i++)
        {
            try
            {
                bars[i].OnLoadSetting(lHns[i]);
                
                scrollHeight += this.GetComponent<RectTransform>().rect.height;
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                Debug.LogError($"TMToxinList - OnLoadSetting {bars[i]}");
            }
        }

        this.scrollContainer.GetComponent<RectTransform>().DOSizeDelta(new Vector2(500, scrollHeight), 0);
    }

    public void OnChangeToxin(List<ToxinData> toxinDatas)
    {
        var lHns = toxinDatas;
        scrollHeight = 0;

        for (int i = 0; i < bars.Count; i++)
        {
            if (lHns[i].on == false)
            {
                this.bars[i].gameObject.SetActive(false);
                continue;
            }

            this.bars[i].gameObject.SetActive(true);
            this.bars[i].name.text = lHns[i].hnsName;
            this.bars[i].warning.text = lHns[i].warning.ToString();
            this.bars[i].current.text = lHns[i].GetLastValue().ToString();

            var lcahrt = lHns[i].values.Select(t => t / lHns[i].warning).ToList();
            this.bars[i].line.UpdateControlPoints(lcahrt);
            scrollHeight += this.bars[i].GetComponent<RectTransform>().rect.height;
        }
        this.scrollContainer.GetComponent<RectTransform>().DOSizeDelta(new Vector2(500, scrollHeight), 0);
    }

    public void OnIntervalToxinValue(List<ToxinData> toxinDatas)
    {
        var lHns = toxinDatas;
        if (lHns.Count > 0)
        {
            for (int i = 0; i < bars.Count; i++)
            {
                this.bars[i].warning.text = lHns[i].warning.ToString();
                this.bars[i].current.text = lHns[i].GetLastValue().ToString();
                var lcahrt = lHns[i].values.Select(t => t / lHns[i].warning).ToList();
                this.bars[i].line.UpdateControlPoints(lcahrt);
            }
            this.scrollContainer.GetComponent<RectTransform>().DOSizeDelta(new Vector2(500, scrollHeight), 0);
        }
    }

    public void OnToxinStatusChange(List<ToxinData> toxinDatas)
    {
        int chk = 0;
        try
        {
            var lHns = toxinDatas;
            if (lHns.Count <= 0)
                return;

            for (int i = 0; i < bars.Count; i++)
            {
                chk = i;
                switch (lHns[i].status)
                {
                    case ToxinStatus.Green:
                        this.bars[i].alaStatus.color = Color.green;
                        break;
                    case ToxinStatus.Yellow:
                        this.bars[i].alaStatus.color = Color.yellow;
                        break;
                    case ToxinStatus.Red:
                        this.bars[i].alaStatus.color = Color.red;
                        break;
                    case ToxinStatus.Purple:    //수정한 부분
                        this.bars[i].alaStatus.color = new Color32(0x6C, 0x00, 0xE2, 0xFF);
                        break;
                    default:
                        this.bars[i].alaStatus.color = Color.green;
                        break;
                }
            }
        }
        catch(System.Exception ex)
        {
            var a = chk;
            Debug.LogException(ex);
        }
    }

    public void ShowFocus() {
        this.gameObject.GetComponent<RectTransform>().DOAnchorPosY(0, 0);
    }

    public void HideFocus() {
        this.gameObject.GetComponent<RectTransform>().DOAnchorPosY(-600, 0);
    }

}
