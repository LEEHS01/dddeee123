using Onthesys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;


public class AlarmList : MonoBehaviour
{

    // 기존 코드와 동일하게 유지
    private List<LogData> list; // 알람 데이터 목록

    public AlarmListItem itemPrefab;
    public GameObject itemContainer;

    public List<AlarmListItem> items;
    public TMP_Dropdown dropdownMap;
    public TMP_Dropdown dropdownStatus;

    private int areaIndex = -1; // 선택된 지역 인덱스
    private int statusIndex = -1; // 선택된 상태 인덱스

    void Start()
    {

        this.itemPrefab.gameObject.SetActive(false);
        // 알람 리스트 변경 이벤트 구독
        UiManager.Instance.Register(UiEventType.ChangeAlarmList, OnUpdateAlarmList);
    }

    // 알람 리스트 업데이트 이벤트 핸들러
    private void OnUpdateAlarmList(object data)
    {
        if (data is List<LogData> logs)
        {
            list = logs;
            UpdateText();
            // 향후 알람 리스트 업데이트 로직 추가 가능
        }
    }

    private void UpdateText()
    {
        for (int i = 0; i < this.items.Count; i++)
        {
            DestroyImmediate(this.items[i].gameObject);
        }
        this.items.Clear();

        float height = this.list.Count * this.itemPrefab.GetComponent<RectTransform>().sizeDelta.y;
        for (int i = 0; i < this.list.Count; i++)
        {
            AlarmListItem item = Instantiate(this.itemPrefab);
            item.gameObject.SetActive(true);
            item.transform.SetParent(this.itemContainer.transform);
            item.SetText(this.list[i]);
            this.items.Add(item);
        }
        this.itemContainer.GetComponent<RectTransform>().sizeDelta =
            new Vector2(this.itemContainer.GetComponent<RectTransform>().sizeDelta.x, height);
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

