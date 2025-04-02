using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Onthesys;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using System;

public class TmPopupDetailToxin : MonoBehaviour
{
    private ToxinData data;
    public TMP_Text txtName;
    public TMP_Text txtCurrent;
    public TMP_Text txtTotal;
    public List<TMChartBar> bars;
    void Start() 
	{
        //DataManager.Instance.OnSelectChart.AddListener(this.OnUpdate);
    }

    public void OnSelectChart(DateTime dt, ToxinData toxin)
    {
        if (toxin == null) return;
        this.data = toxin;
        this.txtName.text = toxin.hnsName.Replace("\n", string.Empty); ;
        this.txtCurrent.text = Math.Round(toxin.GetLastValue(), 2).ToString();
        this.txtTotal.text = Math.Round(toxin.warning, 2).ToString();

        bars.ForEach(bar => bar.gameObject.SetActive(true));

        //aiValues, values, diffValues ������ bar ��ü�� ������ �Է�
        List<Func<ToxinData, List<float>>> valuesExtractors = new() {
            toxinData => toxinData.aiValues,
            toxinData => toxinData.values,
            toxinData => toxinData.diffValues,
        };

        if (bars.Count != valuesExtractors.Count)
            Debug.LogWarning("TmPopupDetailToxin.OnSelectChart() ���� �� bars.Count != valuesExtractors.Count���� �߰��߽��ϴ�.\n �̴� �ǵ����� ���� ó���� ���ɼ��� ������ Ȯ�����ּ���.");
        
        for (int i = 0; i < Mathf.Min(bars.Count, valuesExtractors.Count); i--) 
        {
            Func<ToxinData, List<float>> extractFunc = valuesExtractors[i];

            float max = data.warning < extractFunc(data).Max() ? extractFunc(data).Max() : data.warning;
            var lcahrt = extractFunc(data).Select(t => t / max).ToList();
            bars[i].line.UpdateControlPoints(lcahrt);
            bars[i].CreatAxis(dt, max);
        }

    }


}
