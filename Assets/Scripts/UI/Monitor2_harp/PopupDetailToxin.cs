using Assets.Scripts.Data;
using Onthesys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;


internal class PopupDetailToxin : MonoBehaviour
{
    private ToxinData data;
    private DateTime dt;

    public TMP_Text txtName;
    public TMP_Text txtCurrent;
    public TMP_Text txtTotal;
    public List<ChartBar> bars;

    private void Start()
    {
        UiManager.Instance.Register(UiEventType.SelectCurrentSensor, OnSelectChart);
    }

    public void OnSelectChart(object data)
    {
        if (data is not Tuple<DateTime, ToxinData> tupleData)
        {
            return;
        }

        DateTime dt = tupleData.Item1;
        ToxinData toxin = tupleData.Item2;

        if (toxin == null)
        {
            return;
        }

        this.data = toxin;
        this.txtName.text = toxin.hnsName.Replace("\n", string.Empty);
        this.txtCurrent.text = Math.Round(toxin.GetLastValue(), 2).ToString();
        this.txtTotal.text = Math.Round(toxin.warning, 2).ToString();

        // 그래프 바가 제대로 활성화되지 않으면 활성화
        bars.ForEach(bar => bar.gameObject.SetActive(true));

        // 그래프 데이터 추출기 정의 (aiValues, values, diffValues)
        List<Func<ToxinData, List<float>>> valuesExtractors = new()
    {
        toxinData => toxinData.aiValues,
        toxinData => toxinData.values,
        toxinData => toxinData.diffValues,
    };

        if (bars.Count != valuesExtractors.Count)
        {
            Debug.LogWarning($"bars.Count({bars.Count})와 valuesExtractors.Count({valuesExtractors.Count})가 일치하지 않음");
            return;
        }

        // 그래프 데이터 업데이트
        for (int i = 0; i < Mathf.Min(bars.Count, valuesExtractors.Count); i++)
        {
            Func<ToxinData, List<float>> extractFunc = valuesExtractors[i];
            float max = Mathf.Max(this.data.warning, extractFunc(this.data).Max());

            var chartValues = extractFunc(this.data).Select(value => value / max).ToList();
            bars[i].line.UpdateControlPoints(chartValues);
            bars[i].CreatAxis(dt, max);
        }
    }


}

