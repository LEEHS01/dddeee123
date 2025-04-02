using DG.Tweening;
using Onthesys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;


internal class ToxinList2 : MonoBehaviour
{
    private LogData log;              // 알람 로그 데이터
    public List<TMToxinBar2> bars;    // 독성 바 목록
    public RectTransform scrollContainer;  // 스크롤 컨테이너
    public TMP_Text txtName;         // 이름 텍스트
    public SetTime txtTime;          // 시간 표시 컴포넌트
    public SetTime txtTime2;         // 시간 표시 컴포넌트 (추가)

    void Start()
    {
        UiManager.Instance.Register(UiEventType.SelectAlarm, OnSelectLog);
        UiManager.Instance.Register(UiEventType.ChangeAlarmSensorList, OnLoadAlarmData);
    }

    private void OnSelectLog(object data)
    {
        if (data is LogData logData)
        {
            this.log = logData;
            ShowFocus(); // 포커스 표시
        }
    }

    private void OnLoadAlarmData(object data)
    {

    }

    // 포커스 표시/숨김 메서드
    public void ShowFocus()
    {
        this.gameObject.GetComponent<RectTransform>().DOAnchorPosY(0, 0);
    }

    public void HideFocus()
    {
        this.gameObject.GetComponent<RectTransform>().DOAnchorPosY(-600, 0);
    }
}

