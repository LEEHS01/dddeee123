using NUnit.Framework;
using Onthesys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



public interface ModelProvider
{
    public ObsData GetObs(int obsId);
    public List<ObsData> GetObss();
    public List<ObsData> GetObssByAreaId(int areaId);
    public ToxinStatus GetObsStatus(int obsId);

    public List<AreaData> GetAreas();
    public AreaData GetArea(int areaId);
    public ToxinStatus GetAreaStatus(int areaId);

    public ToxinData GetToxin(int sensorId);
    public List<ToxinData> GetToxins();
    public List<ToxinData> GetToxinsInLog();

    public List<AlarmCount> GetAlarms();
    public AlarmCount GetAlarm(int alarmId);

    public List<AlarmMontlyModel> GetAlarmMonthly();
    public List<AlarmYearlyModel> GetAlarmYearly();
    public List<AlarmSummaryModel> GetAlarmSummary();

    public Dictionary<int, ToxinStatus> GetAreaStatus();


}
