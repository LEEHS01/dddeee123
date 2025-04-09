using DG.Tweening;
using Onthesys;
using OpenCover.Framework.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.Timeline.Actions;
using UnityEngine;

public class ModelManager : MonoBehaviour, ModelProvider
{

    #region [Singleton]
    public static ModelProvider Instance = null;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }
    #endregion [Singleton]

    #region [Instantiating]
    public DbManager dbManager;
    public UiManager uiManager;

    private void Start()
    {
        //Get Core Components
        dbManager = GetComponent<DbManager>();
        uiManager = GetComponent<UiManager>();

        //Load Datas
        dbManager.GetObss(obss => obss.ForEach(obs => this.obss.Add(obs)));
        dbManager.GetAreas(areas => areas.ForEach(area => this.areas.Add(area)));
        dbManager.GetAlarmLogsActivated(logs => logs.ForEach(log => this.logDataList.Add(log)));
        dbManager.GetAlarmMonthly(monthModels => monthModels.ForEach(model => this.alarmMonthly.Add(
            (GetAreaByName(model.areanm).areaId, model.cnt)
            )));
        dbManager.GetAlarmYearly(yearModels => yearModels.ForEach(model => this.alarmYearly.Add(
            (GetAreaByName(model.areanm).areaId, new(0, model.ala1, model.ala2, model.ala0))
            )));

        //Register Events
        uiManager.Register(UiEventType.SelectAlarm, OnSelectAlarm);
        uiManager.Register(UiEventType.NavigateArea, OnNavigateArea);
        uiManager.Register(UiEventType.NavigateObs, OnNavigateObs);
        uiManager.Register(UiEventType.Initiate, OnInitiate);

        AwaitInitiating();
    }


    int initTryCount = 0;
    private void AwaitInitiating()
    {
        initTryCount++;
        if (initTryCount > 50)
        {
            Debug.LogError("ModelManager - AwaitInitiating : 5초가 지난 뒤에도 값이 초기화되지 않았습니다. 이는 초기화 작업 중 치명적인 문제가 있을 가능성을 나타냅니다.");

            UiManager.Instance.Invoke(UiEventType.Initiate);
            return;
        }


        bool isInitiated = obss.Count != 0 && areas.Count != 0;

        if (!isInitiated)
            DOVirtual.DelayedCall(0.1f, AwaitInitiating);
        else
            UiManager.Instance.Invoke(UiEventType.Initiate);
    }

    #endregion [Instantiating]

    #region [Processing]




    void GetTrendValueProcess() 
    {
        dbManager.GetToxinValueLast(currentObsId, currents => 
        {
            if (currents.Count != toxins.Count) Debug.LogWarning("ModelManager - GetTrendValueProcess : currents와 toxins 간의 길이 불일치.");

            for (int i = 0; i <= toxins.Count; i++) 
            {
                ToxinData toxin = toxins[i];
                CurrentDataModel current = currents[i];
                toxin.UpdateValue(current);
            }
            uiManager.Invoke(UiEventType.ChangeTrendLine);
        });

        //지속적으로 재귀 호출
        DOVirtual.DelayedCall(/*Option.TREND_TIME_INTERVAL*/1, GetTrendValueProcess);
    }

  

    #endregion [Processing]

    #region [EventListener]

    private void OnNavigateArea(object obj)
    {
        if (obj is not int areaId) return;

        alarmSummarys.Clear();

        dbManager.GetAlarmSummary(areaId, summarys =>
        {
            summarys.ForEach(summary => alarmSummarys.Add(summary));

            UiManager.Instance.Invoke(UiEventType.ChangeSummary);
        });

    }

    private void OnNavigateObs(object obj)
    {
        if (obj is not int obsId) return;

        currentObsId = obsId;

        dbManager.GetToxinData(obsId, toxins =>
        {
            this.toxins.Clear();
            this.toxins.AddRange(toxins);

            UiManager.Instance.Invoke(UiEventType.ChangeSensorList);
        });

        DateTime endTime = DateTime.Now;
        DateTime startTime = endTime.AddHours(-3);

        dbManager.GetChartValue(obsId, startTime, endTime, Option.TREND_TIME_INTERVAL, chartDatas =>
        {
            toxins.ForEach(model =>
            {
                if (chartDatas.Count <= 0) throw new Exception("LoadObsToxin - GetChartValue - UpdateChartData : 얻은 데이터의 원소 수가 0입니다. 차트를 정상적으로 표시할 수 없습니다. GetChartValue의 범위를 확인해주세요.");

                var values = chartDatas
                    .Where(t => t.boardidx == model.boardid && t.hnsidx == model.hnsid)
                    .Select(t => t.val).ToList();
                model.values = values;
            });

            logDataList.Where(t => t.obsId == obsId).ToList().ForEach(ala =>
            {
                if (ala.status == 0)
                {
                    toxins
                    .Where(t => t.boardid == ala.boardId && t.status != ToxinStatus.Red).ToList()
                    .ForEach(t => t.status = ToxinStatus.Yellow);
                }
                else
                {
                    toxins
                    .FirstOrDefault(t => t.boardid == ala.boardId && t.hnsid == ala.hnsId)
                    .status = ToxinStatus.Red;
                }
            });

            currentObsId = obsId;

            UiManager.Instance.Invoke(UiEventType.ChangeTrendLine);
        });
    }


    private void OnSelectAlarm(object obj)
    {
        if (obj is not int alarmId) return;

        LogData log = logDataList.Find(logData => logData.idx == alarmId);

        if (log == null) throw new Exception("ModelManager - OnSelectAlarm : 선택한 로그의 정보를 찾지 못했습니다.");

        logToxins.Clear();
        dbManager.GetToxinData(log.obsId, toxins =>
        {
            toxins.ForEach(toxin => this.logToxins.Add(toxin));

            // 선택된 관측소 ID를 UI에 전달
            UiManager.Instance.Invoke(UiEventType.SelectAlarmSensor, log.obsId);

            // 독성/센서 데이터 리스트 전달 → ToxinList3에서 OnLoadSetting 호출됨
            UiManager.Instance.Invoke(UiEventType.ChangeSensorList, this.logToxins);
        });

        //TODO
    }

    private void OnUpdateAlarm(object obj)
    {
        dbManager.GetAlarmLogsActivated(logs =>
        {
            logs.ForEach(log => this.logDataList.Add(log));

            UiManager.Instance.Invoke(UiEventType.ChangeAlarmList);

        });
    }

    private void OnInitiate(object obj)
    {
        GetTrendValueProcess();
        /*
        dbManager.GetAlarmLogsActivated(logs =>
        {
            logs.ForEach(log => this.logDataList.Add(log));

            UiManager.Instance.Invoke(UiEventType.ChangeAlarmList, this.logDataList);
        });*/

    }

    #endregion [EventListener]

    #region [DataStructs]

    int currentObsId = -1;

    List<ObsData> obss = new();
    List<ToxinData> toxins = new();
    List<ToxinData> logToxins = new();
    List<LogData> logDataList = new();
    List<AreaData> areas = new();

    List<AlarmSummaryModel> alarmSummarys = new();
    List<(int areaId, int count)> alarmMonthly = new();
    List<(int areaId, AlarmCount counts)> alarmYearly = new();

    #endregion [DataStructs]

    #region [ModelProvider]
    public ObsData GetObs(int obsId) => obss.Find(obs => obs.id == obsId);

    public List<ObsData> GetObss() => obss;

    public List<ObsData> GetObssByAreaId(int areaId) => obss.FindAll(obs => obs.areaId == areaId);

    public ToxinStatus GetObsStatus(int obsId)
    {
        ToxinStatus status = ToxinStatus.Green;

        //해당 관측소의 로그를 모두 가져옴
        List<LogData> obsLogs = logDataList.FindAll(log => log.obsId == obsId);
        List<ToxinStatus> transitionCondition;

        //로그를 순회하며 가장 높은 경고 단계를 탐색
        foreach (var log in obsLogs)
            switch (log.status)
            {
                case 1: //경고
                    transitionCondition = new() { ToxinStatus.Green };
                    if (transitionCondition.Contains(status))
                        status = ToxinStatus.Yellow;
                    break;
                case 2: //경보
                    transitionCondition = new() { ToxinStatus.Green, ToxinStatus.Yellow };
                    if (transitionCondition.Contains(status))
                        status = ToxinStatus.Red;
                    break;
                case 0: //설비이상
                    transitionCondition = new() { ToxinStatus.Green, ToxinStatus.Yellow, ToxinStatus.Red };
                    if (transitionCondition.Contains(status))
                        status = ToxinStatus.Purple;
                    break;
                default:
                    throw new Exception("사전에 정의되지 않은 에러 코드를 사용하고 있습니다. 오류 코드는 다음의 범위 안에 있어야 합니다. (0,1,2) \n 입력된 오류 코드:" + log.status);
            }

        //반환
        return status;
    }

    public List<AreaData> GetAreas() => areas;

    public AreaData GetArea(int areaId) => areas.Find(area => area.areaId == areaId);
    public ToxinStatus GetAreaStatus(int areaId)
    {
        ToxinStatus highestStatus = ToxinStatus.Green;

        //지역 내 관측소들을 순회하며 가장 높은 수준의 알람을 탐색
        var obssInArea = GetObssByAreaId(areaId);
        obssInArea.ForEach(obs =>
            highestStatus = (ToxinStatus)Math.Max((int)highestStatus, (int)GetObsStatus(obs.id))
        );

        return highestStatus;
    }

    public ToxinData GetToxin(int sensorId) => sensorId != 1 ? toxins.Find(toxin => toxin.hnsid == sensorId) : toxins[1];

    public List<ToxinData> GetToxins() => toxins;

    public List<ToxinData> GetToxinsInLog() => logToxins;
    public List<LogData> GetAlarms() => logDataList;

    public LogData GetAlarm(int alarmId) => logDataList.Find(log => log.idx == alarmId);

    public List<(int areaId, int count)> GetAlarmMonthly() => alarmMonthly;

    public List<(int areaId, AlarmCount counts)> GetAlarmYearly() => alarmYearly;

    public List<AlarmSummaryModel> GetAlarmSummary() => alarmSummarys;

    public AlarmCount GetObsStatusCountByAreaId(int areaId)
    {
        AlarmCount obsCounts = new(0, 0, 0, 0);

        //지역 내 관측소들을 순회하며 갯수를 세기
        var obssInArea = GetObssByAreaId(areaId);
        obssInArea.ForEach(obs => {
            ToxinStatus obsStatus = GetObsStatus(obs.id);
            switch (obsStatus)
            {
                case ToxinStatus.Green: obsCounts.green++; break;
                case ToxinStatus.Yellow: obsCounts.yellow++; break;
                case ToxinStatus.Red: obsCounts.red++; break;
                case ToxinStatus.Purple: obsCounts.purple++; break;
            }
        });

        return obsCounts;
    }

    public ObsData GetObsByName(string obsName) => obss.Find(obs => obs.obsName == obsName);

    public AreaData GetAreaByName(string areaName) => areas.Find(area => area.areaName == areaName);

    public ToxinStatus GetSensorStatus(int obsId, int boardId, int sensorId)
    {
        ToxinStatus highestStatus = ToxinStatus.Green;

        //지역 내 관측소들을 순회하며 해당 센서의 가장 높은 수준의 알람을 탐색
        ObsData obs = GetObs(obsId);
        List<LogData> sensorLogs = GetAlarms().FindAll(log => log.hnsId == sensorId && log.obsId == obsId && log.boardId == boardId);

        sensorLogs.ForEach(log => {

            ToxinStatus logStatus = log.status != 0 ? (ToxinStatus)log.status : ToxinStatus.Purple;

            highestStatus = (ToxinStatus)Math.Max((int)highestStatus, (int)logStatus);
            }
        );
        return highestStatus;
    }
    #endregion [ModelProvider]
}
