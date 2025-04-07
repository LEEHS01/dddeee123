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
    private LogData log;                      // 알람 로그 데이터
    private List<ToxinBar2> bars = new();      // 독성 바 목록
    public RectTransform scrollContainer;      // 스크롤 컨테이너
    public GameObject barPrefab;               // Bar 프리팹
    public Transform barParent;                // Bar가 추가될 부모 객체
    public TMP_Text txtName;                   // 이름 텍스트
    public SetTime txtTime;                    // 로그 시간 텍스트 1
    public SetTime txtTime2;                   // 로그 시간 텍스트 2

    void Start()
    {
        InitializeBars();
        UiManager.Instance.Register(UiEventType.SelectAlarm, OnSelectLog);
        UiManager.Instance.Register(UiEventType.ChangeAlarmSensorList, OnLoadAlarmData);
    }

    private void InitializeBars()
    {
        // 기존 바 제거 및 리스트 초기화
        foreach (Transform child in barParent)
        {
            Destroy(child.gameObject);
        }
        bars.Clear();
    }

    private void CreateBar(ToxinData data)
    {
        GameObject newBar = Instantiate(barPrefab, barParent);
        ToxinBar2 barComponent = newBar.GetComponent<ToxinBar2>();
        if (barComponent != null)
        {
            barComponent.toxinName.text = data.hnsName;
            barComponent.warning.text = data.warning.ToString();
            barComponent.current.text = data.GetLastValue().ToString();
            barComponent.data = data;
            bars.Add(barComponent);
        }
    }

    private void OnSelectLog(object data)
    {
        if (data is LogData logData)
        {
            log = logData;
            txtName.text = $"{log.areaName} - {log.obsName}";
            txtTime.SetText(log.time);       
            txtTime2.SetText(log.time);     
            ShowFocus();
        }
    }


    private void OnLoadAlarmData(object data)
    {
        if (data is List<ToxinData> toxinDatas)
        {
            float scrollHeight = 0;

            // 기존 Bar 초기화
            InitializeBars();

            foreach (var toxinData in toxinDatas)
            {
                if (toxinData.on)  // 활성화된 유해물질만 처리
                {
                    CreateBar(toxinData);
                    var lastBar = bars.Last();

                    // 상태 아이콘 설정
                    SetStatusIcon(lastBar, toxinData.status);

                    scrollHeight += lastBar.GetComponent<RectTransform>().rect.height;
                }
            }

            // 스크롤 컨테이너 높이 조정
            scrollContainer.sizeDelta = new Vector2(1230, (scrollHeight / 7) + 76);
        }
    }


    private void SetStatusIcon(ToxinBar2 bar, ToxinStatus status)
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

