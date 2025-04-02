using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Onthesys;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using System;

public class TmPopupDetailToxin2 : MonoBehaviour
{
    private ToxinData data;
    public TMP_Text txtName;
    public TMP_Text txtCurrent;
    public TMP_Text txtTotal;
    public UILineRenderer2 line;
    public List<TMP_Text> hours;
    public List<TMP_Text> verticals;

    public GameObject statusGreen;
    public GameObject statusRed;
    public GameObject statusYellow;
    public GameObject statusPurple;

    void Start() {
       // DataManager.Instance.OnSelectLogToxinEvent.AddListener(this.OnUpdate);
    }

    public void OnSelectLogToxin(DateTime dt, ToxinData toxin) {
        if(toxin == null) return;

        data = toxin;
        txtName.text = toxin.hnsName;
        txtCurrent.text = Math.Round(toxin.GetLastValue(), 2).ToString();
        txtTotal.text = Math.Round(data.warning, 2).ToString();
        
        var max = data.warning < data.values.Max() ? data.values.Max() : data.warning;
        var lcahrt = data.values.Select(t => t / max).ToList();
        line.UpdateControlPoints(lcahrt);

        statusGreen    .SetActive(toxin.status == ToxinStatus.Green);
        statusRed      .SetActive(toxin.status == ToxinStatus.Red);
        statusYellow   .SetActive(toxin.status == ToxinStatus.Yellow);
        statusPurple   .SetActive(toxin.status == ToxinStatus.Purple);

        SetMins(dt);
        SetVertical(max);
    }

    void SetMins(DateTime dt)
    {
        DateTime startDt = dt.AddHours(-4);
        var turm = (dt - startDt).TotalMinutes / this.hours.Count;

        for (int i = 0; i < this.hours.Count; i++)
        {
            var t = dt.AddMinutes(-(turm * i));
            this.hours[i].text = t.ToString("HH:mm");
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


}
