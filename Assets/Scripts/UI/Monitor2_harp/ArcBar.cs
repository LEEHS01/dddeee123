using DG.Tweening;
using Onthesys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


internal class ArcBar : MonoBehaviour
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



    // 아크 이미지 fill 값 설정 메서드 (기존과 동일)
    public void SetArc(float value)
    {
        arc.DOFillAmount((float)(value), 1);
    }

    // 센서 선택 이벤트 - UiManager로 전달
    public void OnSelectToxin()
    {
        UiManager.Instance.Invoke(UiEventType.SelectCurrentSensor, id);
    }
}

