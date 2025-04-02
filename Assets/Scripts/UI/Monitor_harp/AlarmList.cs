using Onthesys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class AlarmList : MonoBehaviour
{

    // 기존 코드와 동일하게 유지
    private List<LogData> list; // 알람 데이터 목록

    private int areaIndex = -1; // 선택된 지역 인덱스
    private int statusIndex = -1; // 선택된 상태 인덱스

    void Start()
    {
        // 알람 리스트 변경 이벤트 구독
        UiManager.Instance.Register(UiEventType.ChangeAlarmList, OnUpdateAlarmList);

        //관측소 네비게이션 이벤트 구독
        UiManager.Instance.Register(UiEventType.NavigateObs, OnNavigateObs);
        UiManager.Instance.Register(UiEventType.NavigateHome, OnNavigateHome);
        UiManager.Instance.Register(UiEventType.NavigateArea, OnNavigateArea);
    }


    // 알람 리스트 업데이트 이벤트 핸들러
    private void OnUpdateAlarmList(object data)
    {
        if (data is List<LogData> logs)
        {
            list = logs;
            // 향후 알람 리스트 업데이트 로직 추가 가능
        }
    }

    // 화면 전환 핸들러들
    private void OnNavigateObs(object obsId)
    {
        gameObject.SetActive(true);
    }

    private void OnNavigateHome(object _)
    {
        gameObject.SetActive(false);
    }

    private void OnNavigateArea(object areaId)
    {
        gameObject.SetActive(false);
    }

    // 알람 항목 클릭 시 발생하는 이벤트 (UI 버튼에 연결)
    public void OnAlarmSelected(int alarmIndex)
    {
        if (list != null && alarmIndex < list.Count)
        {
            UiManager.Instance.Invoke(UiEventType.SelectAlarm, list[alarmIndex]);
        }
    }

    // 알람 필터링 (드롭다운 메뉴에 연결)
    public void OnAreaFilterChanged(int areaIndex)
    {
        this.areaIndex = areaIndex;
        // 필터링 로직 추가 가능
    }

    public void OnStatusFilterChanged(int statusIndex)
    {
        this.statusIndex = statusIndex - 1; // 기존 코드와 일치
                                            // 필터링 로직 추가 가능
    }
}

