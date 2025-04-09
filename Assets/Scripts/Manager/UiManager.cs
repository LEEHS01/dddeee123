using Onthesys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UiManager;

public class UiManager : MonoBehaviour
{
    public ModelProvider modelProvider => ModelManager.Instance;

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

    private void Update()
    {
        //DEBUG
        if (Input.GetKeyDown(KeyCode.F1))
        {
            this.Invoke(UiEventType.ChangeAlarmList, new List<LogData>(){
                new(0, 0, "인천", "능내리", 5, "클로로포름", DateTime.Now, 0, 105f, -1, 100, 80),
            });
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            this.Invoke(UiEventType.ChangeAlarmList, new List<LogData>(){
                new(0, 0, "인천", "능내리", 5, "클로로포름", DateTime.Now, 1, 105f, -1, 100, 80),
            });

        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            this.Invoke(UiEventType.ChangeAlarmList, new List<LogData>(){
                new(0, 0, "인천", "능내리", 5, "클로로포름", DateTime.Now, 2, 105f, -1, 100, 80),
            });
        }
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
    Initiate,       //주요 컨트롤 객체들의 초기화 완료

    NavigateHome,   //시작 화면으로 이동
    NavigateArea,   //지역 화면으로 이동
    NavigateObs,    //관측소 화면으로 이동

    SelectAlarm,        //알람 선택
    SelectAlarmSensor,  //알람 내 센서 선택
    SelectCurrentSensor,    //센서 모니터링 중 센서 선택
    SelectCCTV,     //관측소 내의 CCTV 선택
    SelectSettingObs,   //환경설정 창 내에서 관측소 선택


    ChangeTrendLine,    //실시간 트렌드 갱신
    ChangeSummary,    //연간 지역 알람 요약본 업데이트 (기존 OnChangeSummary)
    ChangeAlarmList,    //알람리스트 변동 발생 (기존 OnAlarmUpdated)
    ChangeSensorList,   //센서리스트 변동 발생 (기존 OnLoadSetting)
    ChangeAlarmSensorList,  //과거 알람 센서리스트에 변동 발생
    ChangeSensorStatus, //센서리스트 내 상태 변동 발생 (기존 OnToxinStatusChange)
    ChangeAlarmMonthly, //알람 월간 상위5개 변동 발생
    ChangeAlarmYearly,  //알람 연간 상위5개 변동 발생
    ChangeAlarmMap,     //지역별 알람 현황 변동 발생

    CommitSensorUsing, //환경설정 - 센서 표시 변경
    CommitBoardFixing,      //환경설정 - 보드 수정 변경
    CommitCCTVUrl,          //환경설정 - CCTV URL 변경
    CommitPopupAlarmCondition,      //환경설정 - 팝업 알람 조건 변경

    //계획 중

    PopupMachineInfo,  //기계 제원 패널 표시
    PopupSetting,  //환경설정 패널 표시

}