using Newtonsoft.Json;
using NUnit.Framework;
using Onthesys;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
//using static UnityEditor.Progress;

/// <summary>
/// DB 관리자 객체입니다. API 요청과정을 통해 DB에서 쿼리문과 프로시저를 수행하고
/// 데이터를 가져오는 작업을 담당합니다.
/// </summary>
public class DbManager : MonoBehaviour
{
    public static DbManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;
    }

    #region [공개 메서드]
    /// <summary>
    /// 관측소 데이터를 가져옵니다.
    /// </summary>
    /// <param name="callback"></param>
    public void GetObss(Action<List<ObsData>> callback) 
        => StartCoroutine(GetObssFunc(callback));
    /// <summary>
    /// 현재 활성화된 알람들을 가져옵니다.
    /// </summary>
    /// <param name="callback"></param>
    public void GetAlarmLogsActivated(Action<List<LogData>> callback)
        => StartCoroutine(GetAlarmLogsActivatedFunc(callback));
    /// <summary>
    /// 월간 알람 정보를 가져옵니다.
    /// </summary>
    /// <param name="callback"></param>
    public void GetAlarmMonthly(Action<List<AlarmMontlyModel>> callback)
        => StartCoroutine(GetAlarmMonthlyFunc(callback));
    /// <summary>
    /// 연간 알람 정보를 가져옵니다.
    /// </summary>
    /// <param name="callback"></param>
    public void GetAlarmYearly(Action<List<AlarmYearlyModel>> callback)
        => StartCoroutine(GetAlarmYearlyFunc(callback));
    /// <summary>
    /// 특정 관측소의 보드의 진행 상태(ex "0020", "0021"...)을 가져옵니다. 1~5까지 있습니다.
    /// 원래 오염도 보드와 화학물질 보드 둘 다 별개의 진행 상태를 갖지만, 현재는 오염도 보드의 진행 상태만을 반환합니다.
    /// </summary>
    /// <param name="obsId">관측소ID</param>
    /// <param name="callback"></param>
    public void GetSensorStep(int obsId, Action<int> callback)
        => StartCoroutine(GetSensorStepFunc(obsId, callback));
    /// <summary>
    /// 특정 관측소의 센서들이 가장 최근에 계측한 값들을 가져옵니다.
    /// </summary>
    /// <param name="obsId">관측소ID</param>
    /// <param name="callback"></param>
    public void GetToxinValueLast(int obsId, Action<List<CurrentDataModel>> callback)
        => StartCoroutine(GetToxinValueLastFunc(obsId, callback));
    /// <summary>
    /// 특정 관측소의 센서들의 정보와 설정값등을 가져옵니다.
    /// </summary>
    /// <param name="obsId"></param>
    /// <param name="callback"></param>
    public void GetToxinData(int obsId, Action<List<ToxinData>> callback)
        => StartCoroutine(GetToxinDataFunc(obsId, callback));
    /// <summary>
    /// 특정 관측소의 기록들을 사용해 차트를 작성하기 위한 데이터로 정제합니다.
    /// 시간 범위, 측정 간격(분 단위)을 정할 수 있습니다.
    /// </summary>
    /// <param name="obsId">관측소ID</param>
    /// <param name="startDt">시작시간</param>
    /// <param name="endDt">종료시간</param>
    /// <param name="intervalMin">계측 간격(분 단위)</param>
    /// <param name="callback"></param>
    public void GetChartValue(int obsId, DateTime startDt, DateTime endDt, int intervalMin, Action<List<ChartDataModel>> callback)
        => StartCoroutine(GetChartValueFunc(obsId, startDt, endDt, intervalMin, callback));
    /// <summary>
    /// 특정 지역의 월별 알람 갯수를 반환합니다.
    /// </summary>
    /// <param name="areaId">지역ID</param>
    /// <param name="callback"></param>
    public void GetAlarmSummary(int areaId, Action<List<AlarmSummaryModel>> callback)
        => StartCoroutine(GetAlarmSummaryFunc(areaId, callback));
    /// <summary>
    /// 특정 센서의 행을 선택해 편집하는 기능입니다. TMSetting이 사용하고 있으며,
    /// 센서 활성화 여부, Fix여부?, 경계(Serious == hi) 임계값, 경고(Warning == hihi) 임계값, 지속시간?(hihisec?)
    /// 을 편집할 수 있습니다.
    /// </summary>
    /// <param name="obsId">관측소ID</param>
    /// <param name="toxinData">센서 정보</param>
    /// <param name="column">편집 대상 행</param>
    /// <param name="callback"></param>
    public void SetToxinDataProperty(int obsId, ToxinData toxinData, UpdateColumn column, Action callback)
        => StartCoroutine(SetToxinDataPropertyFunc(obsId, toxinData, column, callback));

    /// <summary>
    /// 지역 정보들을 가져옵니다.
    /// </summary>
    /// <param name="areaId">지역ID</param>
    /// <param name="callback"></param>
    public void GetAreas(Action<List<AreaData>> callback)
        => StartCoroutine(GetAreasFunc(callback));
    #endregion

    #region [내부 열거자 함수]
    IEnumerator GetObssFunc(Action<List<ObsData>> callback)
    {
        List<ObsData> obss = new();

        #region 관측소 정보 Load
        var query = "EXEC GET_OBS;";
        yield return StartCoroutine(ResponseAPIString(QueryType.SELECT.ToString(), query, (response) =>
        {
            Debug.Log("API OBS Response: " + response);
            var entity = JsonConvert.DeserializeObject<List<ObservatoryModel>>(response);

            entity.ForEach(item =>
                obss.Add(
                    new ObsData(item.areanm, item.areaidx, item.obsnm, (AreaData.AreaType)item.areatype, item.obsidx)
                    ));

        }));
        #endregion

        callback(obss);
    }
    IEnumerator GetAlarmLogsActivatedFunc(Action<List<LogData>> callback)
    {
        List<LogData> alarmLogs = new();

        #region 실시간 알람 리스트 Load
        var query = "EXEC GET_CURRENT_ALARM_LOG;";
        yield return StartCoroutine(ResponseAPIString(QueryType.SELECT.ToString(), query, (response) =>
        {
            Debug.Log("API Alarm Response: " + response);
            var entity = JsonConvert.DeserializeObject<List<AlarmLogModel>>(response);

            entity.ForEach(item =>
            alarmLogs.Add(
                new LogData(item.obsidx, item.boardidx, item.areanm, item.obsnm, item.hnsidx, item.hnsnm, Convert.ToDateTime(item.aladt), item.alacode, item.currval, item.alaidx, 
                item.alahival.HasValue? item.alahival.Value : 0f, 
                item.alahihival.HasValue? item.alahihival.Value : 0f
                )));
        }));
        #endregion

        callback(alarmLogs);
    }
    IEnumerator GetAlarmMonthlyFunc(Action<List<AlarmMontlyModel>> callback)
    {
        //List<AlarmMontlyModel> alarmList = new();

        #region 알람 Monthly Update
        var query = "EXEC GET_ALARM_MONTHLY;";
        yield return StartCoroutine(ResponseAPIString(QueryType.SELECT.ToString(), query, (response) =>
        {
            Debug.Log("API ALARM MONTHLY Response: " + response);
            var entity = JsonConvert.DeserializeObject<List<AlarmMontlyModel>>(response);
            //entity.ForEach(item => alarmList.Add(item));
            callback(entity);
        }));
        #endregion

    }
    IEnumerator GetAlarmYearlyFunc(Action<List<AlarmYearlyModel>> callback)
    {
        List<AlarmYearlyModel> alarmList = new();

        #region 알람 Monthly Update
        var query = "EXEC GET_ALARM_YEARLY;";
        yield return StartCoroutine(ResponseAPIString(QueryType.SELECT.ToString(), query, (response) =>
        {
            Debug.Log("API ALARM YEARLY Response: " + response);
            var entity = JsonConvert.DeserializeObject<List<AlarmYearlyModel>>(response);
            entity.ForEach(item =>
                alarmList.Add(item));
        }));
        #endregion

        callback(alarmList);
    }
    IEnumerator GetSensorStepFunc(int obsId, Action<int> callback)
    {
        Debug.Log("API GetSensorStep");
        var query = $"EXEC GET_SENSOR_STEP @obsid = {obsId};";
        yield return StartCoroutine(ResponseAPIString(QueryType.SELECT.ToString(), query, (response) =>
        {
            Debug.Log("API GET SENSOR Response: " + response);
            var entity = JsonConvert.DeserializeObject<List<ObsSensorStepModel>>(response);

            int step = entity.Count > 0 ? ConvertToStepIdx(entity[0].toxistep) : 5;
            callback(step);
        }));
    }
    IEnumerator GetToxinValueLastFunc(int obsId, Action<List<CurrentDataModel>> callback)
    {
        List<CurrentDataModel> curDatas = new();

        var query = $"EXEC GET_CURRENT_TOXI @obsidx = {obsId}";
        yield return StartCoroutine(ResponseAPIString(QueryType.SELECT.ToString(), query, (response) =>
        {
            //Debug.Log("HNS Toxin Value Response: " + response);
            var entity = JsonConvert.DeserializeObject<List<CurrentDataModel>>(response);
            entity.ForEach(item =>
                curDatas.Add(item));

        }));
        callback(curDatas);
    }
    IEnumerator GetToxinDataFunc(int obsId, Action<List<ToxinData>> callback)
    {
        List<ToxinData> toxinDatas = new();

        var query = $"EXEC GET_SETTING @obsidx = {obsId};";
        yield return StartCoroutine(ResponseAPIString(QueryType.SELECT.ToString(), query, (response) =>
        {
            Debug.Log("API HNS Response: " + response);
            var entity = JsonConvert.DeserializeObject<List<HnsResourceModel>>(response);

            entity.ForEach(model =>
                toxinDatas.Add(new ToxinData(model))
            );

        }));

        callback(toxinDatas);
    }
    IEnumerator GetChartValueFunc(int obsId, DateTime startDt, DateTime endDt, int intervalMin, Action<List<ChartDataModel>> callback)
    {
        List<ChartDataModel> chartData = new();

        var startdt = startDt.ToString("yyyyMMddHHmm00");
        var enddt = endDt.ToString("yyyyMMddHHmm00");
        var query = $"EXEC GET_CHARTVALUE @obsidx = {obsId}, @start_dt = {startdt}, @end_dt = {enddt}, @interval = {intervalMin};";
        yield return StartCoroutine(ResponseAPIString(QueryType.SELECT.ToString(), query, (response) =>
        {
            Debug.Log("HNS Chart Response: " + response);
            var entity = JsonConvert.DeserializeObject<List<ChartDataModel>>(response);
            chartData = entity;
        }));

        callback(chartData);
    }
    IEnumerator GetAlarmSummaryFunc(int areaId, Action<List<AlarmSummaryModel>> callback)
    {
        var query = $"EXEC GET_ALARM_SUMMARY @areaid = {areaId};";

        yield return StartCoroutine(ResponseAPIString(QueryType.SELECT.ToString(), query, (response) =>
        {
            Debug.Log("API Summary Response: " + response);
            var entity = JsonConvert.DeserializeObject<List<AlarmSummaryModel>>(response);
            callback(entity);
        }));

    }
    IEnumerator SetToxinDataPropertyFunc(int obsId, ToxinData toxinData, UpdateColumn column, Action callback)
    {
        var toxin = toxinData;

        var query = "UPDATE TB_HNS SET ";
        var setQuery = string.Empty;
        var whereQuery = $" WHERE OBSIDX = {obsId} AND BOARDIDX = {toxin.boardid} AND HNSIDX = {toxin.hnsid}";
        var value = string.Empty;
        switch (column)
        {
            case UpdateColumn.USEYN:
                value = toxin.on ? "1" : "0";
                break;
            case UpdateColumn.ALAHIHIVAL:
                value = toxin.warning.ToString();
                break;
            case UpdateColumn.INSPECTIONFLAG:
                value = toxin.fix ? "1" : "0";
                break;
            case UpdateColumn.ALAHIVAL:
                value = toxin.serious.ToString();
                break;
            case UpdateColumn.ALAHIHISEC:
                value = toxin.duration.ToString();
                break;
            default:
                break;
        }
        setQuery = $"{column.ToString()} = {value}";

        query = query + setQuery + whereQuery;
        yield return StartCoroutine(ResponseAPIString(QueryType.SELECT.ToString(), query, (response) =>
        {
            Debug.Log("Update Response: " + response);
            callback();
        }));
    }
    IEnumerator GetAreasFunc(Action<List<AreaData>> callback)
    {
        List<AreaData> list = new();
        var query = $"SELECT * FROM TB_AREA;";

        yield return StartCoroutine(ResponseAPIString(QueryType.SELECT.ToString(), query, (response) =>
        {
            Debug.Log("API Summary Response: " + response);
            var entity = JsonConvert.DeserializeObject<List<AreaDataModel>>(response);
            entity.ForEach(model => list.Add(AreaData.FromAreaDataModel(model)));
        }));
        callback(list);
    }
    #endregion

    int ConvertToStepIdx(string code)
    {
        switch (code.Trim())
        {
            case "0020":
                return 1;
            case "0021":
                return 2;
            case "0023":
                return 3;
            case "0024":
                return 4;
            case "0025":
                return 5;
            default:
                return 5;
        }
    }

    /// <summary>
    /// DB서버를 연결해주는 API서버에 쿼리문을 전달한 뒤, 응답을 전달받는 함수입니다.
    /// </summary>
    /// <param name="type">쿼리 유형입니다. QueryType.SELECT.ToString() 같은 방식으로 사용합니다.</param>
    /// <param name="query">쿼리문 내용입니다.</param>
    /// <param name="callback"></param>
    /// <returns></returns>
    IEnumerator ResponseAPIString(string type, string query, System.Action<string> callback)
    {
        var data = new
        {
            SQLType = type,
            SQLquery = query
        };
        var json = JsonConvert.SerializeObject(data, Formatting.Indented);
        // JSON 데이터를 바이트 배열로 변환
        byte[] jsonToSend = new UTF8Encoding().GetBytes(json);

        // UnityWebRequest를 POST 메서드로 생성
        UnityWebRequest request = new UnityWebRequest(Option.url, "POST");
        request.uploadHandler = new UploadHandlerRaw(jsonToSend);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // 요청 보내기
        yield return request.SendWebRequest();

        // 응답 처리
        if (request.result == UnityWebRequest.Result.Success)
        {
            // 요청 성공 시 응답 본문 출력
            callback(request.downloadHandler.text);
        }
        else
        {
            // 오류 처리
            callback($"Error: {request.error}");
        }
    }
    
    enum QueryType
    {
        SELECT,
        UPDATE
    }


}

public enum UpdateColumn
{
    USEYN,
    ALAHIHIVAL,
    ALAHIVAL,
    ALAHIHISEC,
    INSPECTIONFLAG
}