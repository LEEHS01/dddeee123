using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Onthesys;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;

public class TmArcBar : MonoBehaviour
{
    public int id;

    public TMP_Text txtProgress;
    public TMP_Text txtName;
    public TMP_Text txtTotal;
    public TMP_Text txtMin;
    public TMP_Text txtMax;
    public TMP_Text txtCurrent;
    public Image arc;

    public ToxinData data;

    public void SetArc(float value) {
        arc.DOFillAmount((float)(value), 1);
    }

    public void OnSelectToxin() {
        DataManager.Instance.RequestOnSelectToxin(id);
    }
}
