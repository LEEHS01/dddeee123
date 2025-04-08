using Onthesys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        dbManager = GetComponent<DbManager>();
        uiManager = GetComponent<UiManager>();
    }
    #endregion [Instantiating]


    #region [DataStructs]

    List<ObsData> obss;
    List<AlarmCount> alarms;
    List<ToxinData> toxins;
    List<ToxinData> toxinsLog;
    List<LogData> logDataList;  // 로그 데이터 추가
    //List<Area> areas;

    #endregion [DataStructs]

    public List<LogData> GetAlarms()
    {
        throw new NotImplementedException();
    }

    public LogData GetAlarm(int alarmId)
    {
        throw new NotImplementedException();
    }

    public ToxinStatus GetAreaStatus(int areaId)
    {
        throw new NotImplementedException();
    }


    public List<(int areIdx, string areaName)> GetAreaDatas()
    {
        throw new NotImplementedException();
    }

    public ObservatoryModel GetObsData(int obsId)
    {
        throw new NotImplementedException();
    }

    public List<ObservatoryModel> GetObsDatas()
    {
        throw new NotImplementedException();
    }

    public List<ObservatoryModel> GetObsDatasByAreaId(int areaId)
    {
        throw new NotImplementedException();
    }

    public ObsData GetObs(int obsId)
    {
        throw new NotImplementedException();
    }

    public List<ObsData> GetObss()
    {
        throw new NotImplementedException();
    }

    public List<ObsData> GetObssByAreaId(int areaId)
    {
        throw new NotImplementedException();
    }

    public ToxinStatus GetObsStatus(int obsId)
    {
        throw new NotImplementedException();
    }

    public List<AreaData> GetAreas()
    {
        throw new NotImplementedException();
    }

    public AreaData GetArea(int areaId)
    {
        throw new NotImplementedException();
    }



    public ToxinData GetToxin(int sensorId)
    {
        throw new NotImplementedException();
    }

    public List<ToxinData> GetToxins()
    {
        throw new NotImplementedException();
    }

    public List<ToxinData> GetToxinsInLog()
    {
        throw new NotImplementedException();
    }





    public List<AlarmMontlyModel> GetAlarmMonthly()
    {
        throw new NotImplementedException();
    }

    public List<AlarmYearlyModel> GetAlarmYearly()
    {
        throw new NotImplementedException();
    }

    public List<AlarmSummaryModel> GetAlarmSummary()
    {
        throw new NotImplementedException();
    }

    public Dictionary<int, ToxinStatus> GetAreaStatus()
    {
        throw new NotImplementedException();
    }
}
