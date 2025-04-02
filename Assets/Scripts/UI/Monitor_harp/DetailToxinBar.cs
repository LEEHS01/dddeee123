using Onthesys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;


internal class DetailToxinBar : MonoBehaviour
{

    public TMP_Text txtName;         // 센서 이름 텍스트
    public TMP_Text txtCurrent;      // 현재값 텍스트
    public TMP_Text txtTotal;        // 전체값 텍스트
    public UILineRenderer line;      // 라인 차트
    public List<GameObject> statusIcon;  // 상태 아이콘 리스트
    private DateTime dt;              // 시간 정보
    public ToxinData data;           // 센서 데이터

    // 센서 상세 정보 클릭 이벤트 - UiManager로 전달
    public void OnClickList()
    {
        // (DateTime, int) 튜플 형태로 데이터 전달
        UiManager.Instance.Invoke(UiEventType.SelectAlarmSensor, (dt, this.data.hnsid));
    }
}

