using DG.Tweening;
using Onthesys;
using OpenCover.Framework.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using static UnityEditorInternal.ReorderableList;

public class AreaGraph : MonoBehaviour 
{
    ModelProvider modelProvider => UiManager.Instance.modelProvider;

    TMP_Text lblAreaTitle;
    List<TMP_Text> lblObsNames = new();
    List<TMP_Text> lblValues = new();
    List<Transform> histograms = new();

    List<ObsData> obssInArea = new();

    Vector3 defaultPos;
    private void Start()
    {
        UiManager.Instance.Register(UiEventType.NavigateHome, OnNavigateHome);
        UiManager.Instance.Register(UiEventType.NavigateArea, OnNavigateArea);
        UiManager.Instance.Register(UiEventType.NavigateObs, OnNavigateObs);
        UiManager.Instance.Register(UiEventType.ChangeSummary, OnChangeSummary);

        lblAreaTitle = transform.Find("Text (TMP) (1)").GetComponent<TMP_Text>();

        Transform indexes = transform.Find("Panel_Frame").Find("Histogram_Index");
        lblObsNames.Add(indexes.Find("Index_A").GetComponentInChildren<TMP_Text>());
        lblObsNames.Add(indexes.Find("Index_B").GetComponentInChildren<TMP_Text>());
        lblObsNames.Add(indexes.Find("Index_C").GetComponentInChildren<TMP_Text>());

        Transform values = transform.Find("Panel_Frame").Find("Panel_Count");
        lblValues.Add(values.Find("Text").GetComponent<TMP_Text>());
        lblValues.Add(values.Find("Text (1)").GetComponent<TMP_Text>());
        lblValues.Add(values.Find("Text (2)").GetComponent<TMP_Text>());
        lblValues.Add(values.Find("Text (3)").GetComponent<TMP_Text>());
        lblValues.Add(values.Find("Text (4)").GetComponent<TMP_Text>());

        Transform histograms = transform.Find("Panel_Histogram");
        for (var i = 0; i < histograms.childCount; i++)
            this.histograms.Add(histograms.GetChild(i));

        defaultPos = transform.position;
    }

    private void OnChangeSummary(object obj)
    {
        //초기화
        foreach (Transform histo in histograms)
            foreach(Transform bar in histo)
                bar.transform.localScale = new Vector3(1f, 0f, 1f);


        //값 넣기
        List<AlarmSummaryModel> summarys= modelProvider.GetAlarmSummary();

        if (summarys.Count == 0) return;

        int maxValue = summarys.Max(summary => summary.cnt);

        for (int i = lblValues.Count - 1; i >= 0; i--)
            lblValues[i].text = "" +  Mathf.RoundToInt((float)maxValue * i / (lblValues.Count - 1));

        foreach (AlarmSummaryModel summary in summarys) 
        {
            Transform tHistogram = histograms[summary.month + 1];
            Transform tBar;
            if (summary.obsidx == obssInArea[0].id)
                tBar = tHistogram.Find("Histogram05_Turquoise");
            else if (summary.obsidx == obssInArea[1].id)
                tBar = tHistogram.Find("Histogram05_Orange");
            else if (summary.obsidx == obssInArea[2].id)
                tBar = tHistogram.Find("Histogram05_Green");
            else throw new Exception("AreaGraph - OnChangeSummary : Summary 내의 정보가 지역 내 관측소들과 일치하지 않습니다. 잘못된 데이터가 입력됐습니다.");

            tBar.localScale = new Vector3(1f, (float)summary.cnt / maxValue, 1f);
        }

    }

    private void OnNavigateArea(object obj)
    {
        //this.gameObject.SetActive(false);
        SetAnimation(defaultPos + new Vector3(0f, +400f), 1f);

        if (obj is not int areaId) return;

        this.obssInArea = modelProvider.GetObssByAreaId(areaId);

        for(int i = 0; i < obssInArea.Count; i++)
            lblObsNames[i].text = obssInArea[i].obsName;

        AreaData area = modelProvider.GetArea(areaId);
        lblAreaTitle.text = area.areaName + " 지역 알람 발생";
    }
    private void OnNavigateHome(object obj)
    {
        //this.gameObject.SetActive(true);
        SetAnimation(defaultPos, 1f);
    }
    private void OnNavigateObs(object obj)
    {
        //this.gameObject.SetActive(false);
        SetAnimation(defaultPos, 1f);
    }

    void SetAnimation(Vector3 toPos, float duration)
    {
        Vector3 fromPos = GetComponent<RectTransform>().position;
        DOTween.To(() => fromPos, x => fromPos = x, toPos, duration).OnUpdate(() => {
            GetComponent<RectTransform>().position = fromPos;
        });

    }
}