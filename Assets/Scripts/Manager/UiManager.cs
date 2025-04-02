using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UiManager;

public class UiManager : MonoBehaviour
{
    public static UiManager Instance = null;
    private void Awake()
    {
        if (Instance != null)
        {
            //Debug.LogWarning("UiManagerUiManager.Instance was not null!!!" + Instance.name);
            Destroy(this);
            return;
        }
        Instance = this;
        //Debug.LogWarning("UiManagerUiManager.Instance was null!" + Instance.name);
    }

    private Dictionary<UiEventType, Action<object>> eventHandlers = new();
    public void Register(UiEventType eventType, Action<object> handler)
    {
        if (!eventHandlers.ContainsKey(eventType))
        {
            eventHandlers[eventType] = handler;
        }
        else
        {
            eventHandlers[eventType] += handler;
        }
    }

    public void Unregister(UiEventType eventType, Action<object> handler)
    {
        if (eventHandlers.ContainsKey(eventType))
        {
            eventHandlers[eventType] -= handler;
        }
    }

    public void Invoke(UiEventType eventType, object payload = null)
    {
        if (eventHandlers.ContainsKey(eventType))
        {
            eventHandlers[eventType]?.Invoke(payload);
        }
    }
}
public enum UiEventType
{
    NavigateHome,   //시작 화면으로 이동
    NavigateArea,   //지역 화면으로 이동
    NavigateObs,    //관측소 화면으로 이동

    SelectAlarm,        //알람 선택
    SelectAlarmSensor,  //알람 내 센서 선택
    SelectCurrentSensor,    //센서 모니터링 중 센서 선택
    SelectCCTV,     //관측소 내의 CCTV 선택

    ChangeTrendLine,    //실시간 트렌드 갱신
    ChangehBarChart,    //연간 지역 알람 요약본 업데이트 (기존 OnChangeSummary)
    ChangeAlarmList,    //알람리스트 변동 발생 (기존 OnAlarmUpdated)
    ChangeSensorList,   //센서리스트 변동 발생 (기존 OnLoadSetting)
    ChangeAlarmSensorList,  //과거 알람 센서리스트에 변동 발생
    ChangeSensorStatus, //센서리스트 내 상태 변동 발생 (기존 OnToxinStatusChange)
    ChangeAlarmMonthly, //알람 월간 상위5개 변동 발생
    ChangeAlarmYearly,  //알람 연간 상위5개 변동 발생
    ChangeAlarmMap,     //지역별 알람 현황 변동 발생

    CommitSensorSetting,    //사용자 지정 센서 설정 변경 (기존 OnSetToxinProp)

    //계획 중

    PopupMachineInfo,  //기계 제원 패널 표시
    PopupSetting,  //환경설정 패널 표시

}