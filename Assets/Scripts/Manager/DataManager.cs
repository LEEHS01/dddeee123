using DG.Tweening;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace Onthesys
{
    public class DataManager : MonoBehaviour
    {
        #region [※ UnityEvents 유니티이벤트 ※]
        /// <summary>
        /// 홈 버튼을 누를 시 발생하는 이벤트
        /// </summary>
        [SerializeField] private UnityEvent OnHome;
        /// <summary>
        /// 지역을 선택할 시 발생하는 이벤트
        /// </summary>
        [SerializeField] private UnityEvent<int> OnSelectArea;
        /// <summary>
        /// 관측소 선택 시 발생하는 이벤트
        /// </summary>
        [SerializeField] private UnityEvent<int> OnSelectObs;
        /// <summary>
        /// 보드의 진행 단계에 따라 발생하는 이벤트
        /// TmMachineStep에서 사용 중
        /// </summary>
        [SerializeField] private UnityEvent<int> OnUpdateStep;
        /// <summary>
        /// 센서의 값이 바뀔 때 발생하는 걸로 추측 중
        /// TMToxinList에서 받아들이는 것으로 보임
        /// Invoke가 없는 상태...
        /// </summary>
        [SerializeField] private UnityEvent<List<ToxinData>> OnChangeToxin;

        /// <summary>
        /// GetToxinValueLast을 통해 CurrentToxi값이 ToxinData.values에 적용됐을 때 발생
        /// TMToxinList, TMToxinList3에서 리스너 등록
        /// </summary>
        [SerializeField] private UnityEvent<List<ToxinData>> OnIntervalToxinValue;
        /// <summary>
        /// 센서 정보들의 제원정보과 '현재' 그래프값을 모두 로드했을 때 발생.
        /// TMToxinList, TMToxinList3에서 리스너를 등록해서 toxinDatas의 값을 띄우고 있음
        /// </summary>
        [SerializeField] private UnityEvent<List<ToxinData>> OnLoadSetting;
        /// <summary>
        /// 활성화된 알람 발생 당시의 '과거' 그래프, 센서값을 모두 로드했을 때 발생.
        /// TMToxinList2서 과거 그래프 정보를 띄워주고 있음.
        /// </summary>
        [SerializeField] private UnityEvent<List<ToxinData>> OnLoadAlarmData;
        /// <summary>
        /// 활성화된 알람을 선택했을 때, 해당 알람의 정보를 보여주고자 발생
        /// </summary>
        [SerializeField] private UnityEvent<LogData> OnSelectLog;
        /// <summary>
        /// 세부 차트가 담긴 팝업창 토글 이벤트.
        /// TmPopupDetailToxin을 활성화함.
        /// </summary>
        [SerializeField] private UnityEvent<DateTime, ToxinData> OnSelectChart;

        /// <summary>
        /// TMToxinList2 내에 TMToxinBar2 클릭 시, 발생하는 이벤트.
        /// TmPopupDetailToxin2을 띄워줌
        /// </summary>
        [SerializeField] private UnityEvent<DateTime, ToxinData> OnSelectLogToxin;
        /// <summary>
        /// OnSelectToxin에서 받은 int 값을 hns객체로 반환해 TMDetailToxinBar에 전달.
        /// </summary>
        [SerializeField] private UnityEvent<ToxinData> OnSelectToxin;
        /// <summary>
        /// TMDetailToxinBar에서 차트를 그리기 위한 이벤트로, 호출구조가 조금 복잡하다.
        /// OnUpdateValue.Invoke > TMToxinList3.UpdateValue > TMDetailToxinBar > OnUpdateCurrentValue > line.UpdateControlPoints()
        /// </summary>
        [SerializeField] private UnityEvent OnUpdateChart;
        /// <summary>
        /// CCTV 선택시 발생하는 이벤트. 관측소 정보 하나만을 전달한다.
        /// </summary>
        [SerializeField] private UnityEvent<ObsData> OnSelectCCTV;
        /// <summary>
        /// A모니터에서 장비 선택시 발생하는 이벤트로 추측.
        /// 현재는 어떤 참조도 없어서 어떤 기능인지 판단하기 어려움
        /// </summary>
        [SerializeField] private UnityEvent<ObsData, int> OnSelectMachine;

        /// <summary>
        /// 알람 리스트 변경으로 인해 센서들의 상태가 변경됐을 때 발생
        /// </summary>
        [SerializeField] private UnityEvent<List<ToxinData>> OnToxinStatusChange;
        /// <summary>
        /// 첫 1회 및 알람 현황 변경 사항으로 발생하는 이벤트.
        /// 알람리스트의 변경 사항을 UI에서 확인할 수 있도록 함
        /// </summary>
        [SerializeField] private UnityEvent<List<LogData>> OnAlarmUpdate;

        //이 밑으로는 OnAlarmUpdate과 발생 시점이 동일함 이 점에 주의할 것.
        //OnAlarmUpdateMap, OnAlarmUpdateOcean, OnAlarmUpdateNuclear의 데이터는
        //OnAlarmUpdate를 가공해서 얻을 수 있기에 추후에 통합 가능.
        [SerializeField] private UnityEvent<List<KeyValuePair<string, int>>> OnAlarmUpdateMonthly;
        [SerializeField] private UnityEvent<List<KeyValuePair<string, AlarmData>>> OnAlarmUpdateYearly;
        [SerializeField] private UnityEvent<List<KeyValuePair<int, AlarmData>>> OnAlarmUpdateMap;
        [SerializeField] private UnityEvent<List<KeyValuePair<int, AlarmData>>> OnAlarmUpdateOcean;
        [SerializeField] private UnityEvent<List<KeyValuePair<int, AlarmData>>> OnAlarmUpdateNuclear;

        /// <summary>
        /// A모니터 TMBarList 아래의 알람Summary UI를 초기화하기 위한 이벤트
        /// </summary>
        [SerializeField] private UnityEvent<List<AlarmSummaryModel>> OnChangeSummary;
        /// <summary>
        /// HNS 센서의 제원 값을 수정하는 이벤트
        /// SetToxinDataProperty로 이어진다.
        /// </summary>
        [SerializeField] private UnityEvent<(int toxinId, float value, UpdateColumn column)> OnSetToxinProp = new();
        /// <summary>
        /// DataManager의 초기화 종료시 발생하는 이벤트입니다.
        /// 현재는 TMLocalList에서 사용되고 있습니다.
        /// </summary>
        [SerializeField] private UnityEvent<List<ObsData>> OnInit;



        #endregion [※ UnityEvents 유니티이벤트 ※]

        #region [※ UnityEvents Capsulize 유니티이벤트 캡슐화※]
        public void RequestOnSelectToxin(int toxinId) {
            try
            {
                OnSelectToxin.Invoke(toxinDatas[toxinId]);
            }
            catch (Exception ex) 
            {
                Debug.LogException(ex);
                Debug.LogError($"요청값 : {toxinId} 센서 수 : {toxinDatas.Count}");
            
            }
    }
        public void RequestOnSelectToxin(DateTime dt, int alaToxinId) => OnSelectChart.Invoke(dt, alaToxinDatas[alaToxinId]);
        public void RequestOnSelectCCTV(int obsId) => OnSelectCCTV.Invoke(obss[obsId - 1]);
        public void RequestOnSetToxinProp(int toxinId, float value, UpdateColumn column) => OnSetToxinProp.Invoke((toxinId, value, column));

        public void RequestOnHome() => OnHome.Invoke();
        public void RequestOnSelectObs(int obsId) => OnSelectObs.Invoke(obsId);
        public void RequestOnSelectArea(int areaId) => OnSelectArea.Invoke(areaId);
        public void RequestOnSelectLog(int logId) => OnSelectLog.Invoke(alarmLogs.Find(log => log.idx == logId));
        public void RequestOnSelectLogToxin(DateTime dt, int toxinId) => OnSelectLogToxin.Invoke(dt, toxinDatas.Find(toxin => toxin.hnsid == toxinId));

        public void RequestOnUpdateChart() => OnUpdateChart.Invoke();

        #endregion [※ UnityEvents Capsulize 유니티이벤트 캡슐화※]

        #region [※ DataStructs 자료구조 ※]
        /// <summary>
        /// 관측소 데이터 리스트
        /// </summary>
        private List<ObsData> obss = new();
        /// <summary>
        /// 활성화된 알람(로그) 리스트
        /// </summary>
        private List<LogData> alarmLogs = new();
        /// <summary>
        /// 현재 관측소 내 센서 리스트
        /// </summary>
        private List<ToxinData> toxinDatas = new();
        /// <summary>
        /// 로그 조회에 사용되는 관측소 내 센서 리스트
        /// </summary>
        private List<ToxinData> alaToxinDatas = new();

        /// <summary>
        /// 월간 상위 5개 지역(obs)
        /// 지역(obs)-알람 수
        /// </summary>
        private Dictionary <string, int> top5AlarmsForMonth = new();
        /// <summary>
        /// 연간 상위 5개 지역(obs)
        /// 지역(obs)-클래스별 알람 수
        /// </summary>
        private Dictionary <string, AlarmData> top5AlarmsForYear = new();
        /// <summary>
        /// 각 지역별 알람 현황
        /// 사용처는 ButtonWithPointAnimation에서 알람 유무에 따라 지역 아이콘 색깔 표시
        /// </summary>
        private Dictionary <int, AlarmData> mapAlarms = new();
        /// <summary>
        /// 해양 지역 알람 현황
        /// 사용처는 TMOceanList에 데이터 표시.
        /// mapAlarms에서 간단한 데이터 정제를 통해 얻게 바꿔야할 듯
        /// </summary>
        private Dictionary <int, AlarmData> oceanAlarms = new();
        /// <summary>
        /// 발전소 지역 알람 현황
        /// 사용처는 TMNuclearList에 데이터 표시.
        /// mapAlarms에서 간단한 데이터 정제를 통해 얻게 바꿔야할 듯
        /// </summary>
        private Dictionary <int, AlarmData> nuclearAlarms = new();
        #endregion [※ DataStructs 자료구조 ※]


        public static DataManager Instance;
        /// <summary>
        /// 시스템 초기화 여부 
        /// </summary>
        bool isInit = true;
        /// <summary>
        /// 현재 UI에서 선택한 관측소의 ID값
        /// </summary>
        int CurrentObsId = -1;
        /// <summary>
        /// DB 관리자 객체입니다. 처리하고자 하는 콜백을 넘겨주면 데이터 쿼리문을 대신 수행하고 콜백을 처리해줍니다.
        /// </summary>
        DbManager dbManager;
        /// <summary>
        /// Window 관리자 객체입니다. 다중 디스플레이를 적용하고 관련한 처리를 제공합니다.
        /// </summary>
        WindowManager windowManager;

        /// <summary>
        /// 반복 수행 함수
        /// </summary>
        private void LoopByInterval()
        {
            //주기적으로 알람 리스트 업데이트
            dbManager.GetAlarmLogsActivated(UpdateAlarmLogs);

            //선택한 관측소가 있다면
            if (CurrentObsId > 0)
                //보드 진행상태 갱신
                dbManager.GetSensorStep(CurrentObsId,
                        step => OnUpdateStep.Invoke(step));
            

            //주기적으로 재귀 실행
            DOVirtual.DelayedCall(1f, LoopByInterval);
        }
        private void LoopByIntervalTrend() 
        {
            //선택한 관측소가 있다면
            if (CurrentObsId > 0)
                //센서들의 그래프에 최근 계측값 추가
                dbManager.GetToxinValueLast(CurrentObsId, models => {

                    toxinDatas.ForEach(item =>
                    item.UpdateValue(
                        models.Where(t => t.boardidx == item.boardid && t.hnsidx == item.hnsid).FirstOrDefault()
                        ));

                    OnIntervalToxinValue.Invoke(toxinDatas);
                });

            //주기적으로 재귀 실행
            DOVirtual.DelayedCall(Option.TREND_TIME_INTERVAL * 60, LoopByIntervalTrend);
        }

        #region [※ Initialize 초기화 ※]
        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;

                //DBManager 객체 생성 및 저장
                new GameObject() { name = "DBManager" }.transform.SetParent(transform);
                dbManager = transform.Find("DBManager").AddComponent<DbManager>();
                //WindowManager 객체 생성 및 저장
                new GameObject() { name = "WindowManager" }.transform.SetParent(transform);
                windowManager = transform.Find("WindowManager").AddComponent<WindowManager>();

                Instance.Init();
                DontDestroyOnLoad(Instance);
            }
            else
            {
                Destroy(this);
            }
        }

        public void Init()
        {
            //초기에 관측소 정보 로드 
            dbManager.GetObss(obss => {
                obss.ForEach(item => this.obss.Add(item));

                DOVirtual.DelayedCall(0.1f, () => OnInit.Invoke(obss));
            });
            //초기에 활성화된 알람 정보 로드
            dbManager.GetAlarmLogsActivated(logs =>
                    logs.ForEach(item => this.alarmLogs.Add(item))
                );

            //LoopByInterval 시작.
            DOVirtual.DelayedCall(1, this.LoopByInterval);
            DOVirtual.DelayedCall(1, this.LoopByIntervalTrend);
        }

        void Start()
        {
            //듀얼 모니터 적용
            bool ret = windowManager.ActivateDualDisplay();

            //홈 선택 시, 선택헀던 관측소 및 toxinDatas 해제
            OnHome.AddListener(() =>
            {
                CurrentObsId = -1;
                this.toxinDatas.Clear();
            });

            //지역 선택 시, 선택헀던 관측소 및 toxinDatas 해제
            OnSelectArea.AddListener(i => {
                CurrentObsId = -1;
                //Debug.Log("지역 선택됨: " + i);
                this.toxinDatas.Clear();
            });

            //지역 선택 시, 해당 지역 요약데이터 적용
            OnSelectArea.AddListener(areaId =>
                dbManager.GetAlarmSummary(areaId, datas =>
                    OnChangeSummary.Invoke(datas)));


            // TODO : 알람로그 선택 시, 과거 로그를 트렌드 창에 불러오는 UpdateChartDataByLog는 의도되지 않은 기능 같음. 추후 해결 필요
            //알람 로그 선택 시, 현재 시간대의 ChartData를 UpdateChartData 처리
            OnSelectLog.AddListener(log =>
                //해당 관측소의 현재 데이터 >>> 차트
                dbManager.GetToxinData(log.obsId, datas => {

                    Debug.Log($"[OnSelectLog 실행됨] obsId: {log.obsId}");
                    datas.ForEach(data => toxinDatas.Add(data));

                    dbManager.GetChartValue(
                        log.obsId, DateTime.Now.AddHours(-Option.TREND_DURATION_REALTIME), DateTime.Now, Option.TREND_TIME_INTERVAL, datas =>
                        {
                            UpdateChartData(log.obsId, datas);
                            Debug.Log($"[OnSelectLog] 차트 데이터 업데이트 완료: {log.obsId}");

                            OnSelectObs.Invoke(log.obsId);
                        });
                }));


            //알람 로그 선택 시, 해당 시간대의 ChartData를 UpdateChartDataByLog로 처리
            OnSelectLog.AddListener(log =>
                //알람 로그 당시의 데이터 >>> 차트
                dbManager.GetToxinData(log.obsId, datas =>
                {

                    this.alaToxinDatas.Clear();
                    datas.ForEach(model => this.alaToxinDatas.Add(model));

                    dbManager.GetChartValue(
                        log.obsId, log.time.AddHours(-Option.TREND_DURATION_REALTIME), log.time, Option.TREND_TIME_INTERVAL, datas =>
                        {
                            UpdateChartDataByLog(log, log.time.AddHours(-Option.TREND_DURATION_REALTIME), Option.TREND_TIME_INTERVAL, datas);
                        });
                }));

            //관측소 선택 시, toxinDatas를 해당 관측소의 정보로 갱신
            OnSelectObs.AddListener(obsId => {

                CurrentObsId = obsId;

                dbManager.GetToxinData(obsId, datas => {
                    datas.ForEach(model => toxinDatas.Add(model));

                    dbManager.GetChartValue(
                        obsId, DateTime.Now.AddHours(-Option.TREND_DURATION_REALTIME), DateTime.Now, Option.TREND_TIME_INTERVAL, datas => {

                            //차트 데이터 적용
                            UpdateChartData(obsId, datas);

                            //센서 값 호출
                            OnLoadSetting.Invoke(toxinDatas);

                        });
                });
            });


            //TMSetting의 toxin 재원값 변경에 따른 처리
            OnSetToxinProp.AddListener(command => {
                ToxinData toxin = toxinDatas[command.toxinId];

                switch (command.column)
                {
                    case UpdateColumn.USEYN:
                        toxin.on = command.value != 0f;
                        break;
                    case UpdateColumn.INSPECTIONFLAG:
                        toxin.fix = command.value != 0f;
                        break;
                    case UpdateColumn.ALAHIVAL:
                        toxin.serious = command.value;
                        break;
                    case UpdateColumn.ALAHIHIVAL:
                        toxin.warning = command.value;
                        break;
                    case UpdateColumn.ALAHIHISEC:
                        toxin.duration = command.value;
                        break;
                    default: throw new Exception("ERROR! Not Notified column detected!");
                }

                dbManager.SetToxinDataProperty(CurrentObsId, toxin, command.column, () => { });
            });

            /*TEST VALUE!!*/
            //TODO : TMSetting에서 임시로 가져온 처리부분이라서 추후 수정 필요
            OnAlarmUpdate.AddListener(logs =>
                dbManager.GetAlarmSummary(1/*TEST VALUE!!*/, datas =>
                    OnChangeSummary.Invoke(datas)));
            /*TEST VALUE!!*/
        }
        #endregion [※ Initialize 초기화 ※]

        #region [※ Data Control 데이터 제어 ※]

        /// <summary>
        /// 넘겨받은 알람 리스트 중 기존에 갖고 있던 알람 리스트와 변경 사항이 있는지 확인합니다.
        /// 있다면, 기존 알람 리스트를 덮어쓰고, 연간알람, 월간알람, 지역별 알람을 최신화, OnAlarmUpdate를 발생시킵니다.
        /// </summary>
        /// <param name="logs">기존 데이터들과 비교할 최신 알람 데이터들</param>
        void UpdateAlarmLogs(List<LogData> logs)
        {
            //그냥 반환값을 bool로 만들고 싶은 맘이 굴뚝같음.
            //변경사항이 있는지?
            var chkAlarm = false;

            var idxList = logs
                .Select(t => t.idx)
                .ToList();

            var newIdxs = idxList
                .Except(this.alarmLogs.Select(t => t.idx))
                .ToList();

            if (newIdxs.Count > 0)
            {
                foreach (var idx in newIdxs)
                {
                    var model = logs.FirstOrDefault(t => t.idx == idx);
                    this.alarmLogs.Add(model);
                }
                chkAlarm = true;
            }

            var turnOffList = this.alarmLogs
                .Select(t => t.idx)
                .Except(idxList)
                .ToList();

            if (turnOffList.Count > 0)
            {
                foreach (var idx in turnOffList)
                {
                    var i = alarmLogs.FindIndex(t => t.idx == idx);
                    alarmLogs.RemoveAt(i);
                }
                chkAlarm = true;
            }

            //Debug.LogError($"({chkAlarm} || {isInit} == ?");
            //첫 1회, 또는 활성화된 알람 목록에 변경사항 발생 시
            if (chkAlarm || isInit)
            {
                //Debug.LogError("(chkAlarm || isInit == true");
                isInit = false;

                //전체 알람, 해안 알람, 핵발전소 알람 최신화
                this.UpdateAlarmSummary();

                //연간 알람 최신화
                dbManager.GetAlarmMonthly(datas =>
                {
                    var dicMonthly = obss.GroupBy(item => item.areaName)
                                            .ToDictionary(t => t.Key, t => 0);
                    foreach (var map in datas)
                        dicMonthly[map.areanm] = map.cnt;

                    this.top5AlarmsForMonth = dicMonthly.OrderByDescending(t => t.Value)
                                                        .Take(5)
                                                        .ToDictionary(t => t.Key, t => t.Value);

                    OnAlarmUpdateMonthly.Invoke(top5AlarmsForMonth.ToKeyValueList());
                });
                //월간 알람 적용
                dbManager.GetAlarmYearly(datas =>
                {
                    var dicYearly = obss.GroupBy(item => item.areaName)
                                            .ToDictionary(t => t.Key, t => new AlarmData());
                    foreach (var map in datas)
                    {
                        dicYearly[map.areanm].yellow = map.ala0;
                        dicYearly[map.areanm].red = map.ala1 + map.ala2;
                    }

                    this.top5AlarmsForYear = dicYearly.OrderByDescending(t => t.Value.GetRedYellow())
                                                        .Take(5)
                                                        .ToDictionary(t => t.Key, t => t.Value);

                    OnAlarmUpdateYearly.Invoke(top5AlarmsForYear.ToKeyValueList());

                    //알람 리스트 최신화
                    OnAlarmUpdate.Invoke(alarmLogs);
                });


            }
        }

        /// <summary>
        /// mapAlarms, oceanAlarms, nuclearAlarms 최신화
        /// </summary>
        void UpdateAlarmSummary()
        {
            var dicCurrentAlarm = obss.GroupBy(item => item.areaId)
                                       .ToDictionary(t => t.Key, t => new AlarmData());

            var NuclearIdx = obss.Where(t => t.type == AreaType.Nuclear)
                                  .Select(t => t.areaId)
                                  .Distinct()
                                  .ToList();

            var OceanIdx = obss.Where(t => t.type == AreaType.Ocean)
                                .Select(t => t.areaId)
                                .Distinct()
                                .ToList();

            foreach (var item in dicCurrentAlarm)
            {
                var name = GetAreaNameById(item.Key);
                var larea = this.obss
                    .Where(t => t.areaName.Equals(name))
                    .Select(e => e.id);
                var lala = this.alarmLogs
                    .Where(t => t.areaName.Equals(name))
                    .Select(e => new {
                        e.obsId,
                        e.status
                    });

                var nonAlarmcut = larea
                        .Except(lala.Select(t => t.obsId))
                        .Count();
                Debug.Log(lala);
                item.Value.green = nonAlarmcut;
                item.Value.yellow = lala.Count(t => t.status == 1);
                item.Value.red = lala.Count(t => t.status == 2);
                item.Value.purple = lala.Count(t => t.status == 0);
            }

            this.mapAlarms = dicCurrentAlarm;
            this.oceanAlarms = dicCurrentAlarm.Where(t => OceanIdx.Contains(t.Key)).ToDictionary(t => t.Key, t => t.Value);
            this.nuclearAlarms = dicCurrentAlarm.Where(t => NuclearIdx.Contains(t.Key)).ToDictionary(t => t.Key, t => t.Value);

            OnAlarmUpdateMap.Invoke(mapAlarms.ToKeyValueList());
            OnAlarmUpdateOcean.Invoke(oceanAlarms.ToKeyValueList());
            OnAlarmUpdateNuclear.Invoke(nuclearAlarms.ToKeyValueList());


            if (this.CurrentObsId > 0)
                this.UpdateToxinStatus();
        }

        /// <summary>
        /// ToxinData.status 최신화
        /// </summary>
        void UpdateToxinStatus()
        {
            this.toxinDatas.ForEach(t => t.status = ToxinStatus.Green);
            foreach (var ala in alarmLogs.Where(t => t.obsId == CurrentObsId))
            {
                if (ala.status == 0)
                {
                    toxinDatas.Where(t => t.boardid == ala.boardId && t.status != ToxinStatus.Red && t.status != ToxinStatus.Yellow).ToList()
                        .ForEach(t => t.status = ToxinStatus.Purple);
                }
                else if (ala.status == 1)
                {
                    toxinDatas.Where(t => t.boardid == ala.boardId && t.status != ToxinStatus.Red && t.status != ToxinStatus.Purple).ToList()
                        .ForEach(t => t.status = ToxinStatus.Yellow);
                }
                else if (ala.status == 2)
                {
                    toxinDatas.Where(t => t.boardid == ala.boardId && t.status != ToxinStatus.Yellow && t.status != ToxinStatus.Purple).ToList()
                        .ForEach(t => t.status = ToxinStatus.Red);
                }
            }
            OnToxinStatusChange.Invoke(toxinDatas);
        }

        /// <summary>
        /// 특정 관측소의 현재 시간을 기준으로 차트데이터를 구성합니다.
        /// </summary>
        /// <param name="obsId">관측소 ID</param>
        /// <param name="datas"> dbManager.GetChartValue로 얻은 값입니다. </param>
        /// <exception cref="Exception"></exception>
        void UpdateChartData(int obsId, List<ChartDataModel> datas)
        {
            toxinDatas.ForEach(model =>
            {
                if (datas.Count <= 0) throw new Exception("LoadObsToxin - GetChartValue - UpdateChartData : 얻은 데이터의 원소 수가 0입니다. 차트를 정상적으로 표시할 수 없습니다. GetChartValue의 범위를 확인해주세요.");

                var values = datas.Where(t => t.boardidx == model.boardid && t.hnsidx == model.hnsid).Select(t => t.val).ToList();
                model.values = values;
            });

            alarmLogs.Where(t => t.obsId == obsId).ToList().ForEach(ala =>
            {
                if (ala.status == 0)
                {
                    toxinDatas
                    .Where(t => t.boardid == ala.boardId && t.status != ToxinStatus.Red).ToList()
                    .ForEach(t => t.status = ToxinStatus.Yellow);
                }
                else
                {
                    toxinDatas
                    .FirstOrDefault(t => t.boardid == ala.boardId && t.hnsid == ala.hnsId)
                    .status = ToxinStatus.Red;
                }
            });

            CurrentObsId = obsId;
            OnLoadSetting.Invoke(toxinDatas);
        }

        /// <summary>
        /// 특정 알람을 기준으로 차트데이터를 구성합니다. UpdateChartData과는 다른 저장공간인 alaToxinDatas를 사용합니다.
        /// </summary>
        /// <param name="log">해당 알람 정보</param>
        /// <param name="startDt">시작 시간</param>
        /// <param name="intervalMin">차트 시간 단위</param>
        /// <param name="entity"></param>
        /// <exception cref="Exception"></exception>
        void UpdateChartDataByLog(LogData log, DateTime startDt, int intervalMin, List<ChartDataModel> entity)
        {
            int countExpected = Mathf.FloorToInt((Option.TREND_DURATION_LOG * 60f) / Option.TREND_TIME_INTERVAL);

            foreach (var model in alaToxinDatas)
            {
                if (entity.Count <= 0)
                    throw new Exception("UpdateChartDataByLog - List<ChartDataModel> entity.count == 0 !!!");

                var lval = entity.Where(t => t.boardidx == model.boardid && t.hnsidx == model.hnsid).OrderBy(e => e.obsdt).Select(e => new
                {
                    dt = DateTime.Parse(e.obsdt),
                    e.val,
                    e.aival,
                    difval = Math.Abs(e.val - e.aival)
                }).ToList();

                for (int i = 1; i <= countExpected; i++)
                {
                    var time = Truncate(startDt.AddMinutes(intervalMin * i), TimeSpan.FromMinutes(1.0));

                    var values = lval.FirstOrDefault(t => t.dt == time);

                    if (values == null)
                    {
                        model.values.Add(0f);
                        model.aiValues.Add(0f);
                        model.diffValues.Add(0f);
                    }
                    else
                    {
                        //Debug.LogError($"values [{i}] : {values.val}");
                        model.values.Add(values.val);
                        model.aiValues.Add(values.aival);
                        model.diffValues.Add(values.difval);
                    }
                }
            }

            if (log.status != 0)
            {
                alaToxinDatas.FirstOrDefault(t => t.boardid == log.boardId && t.hnsid == log.hnsId).status = ToxinStatus.Red;
            }
            else
            {
                alaToxinDatas.Where(t => t.boardid == log.boardId).ToList().ForEach(t => t.status = ToxinStatus.Yellow);
            }

            OnLoadAlarmData.Invoke(alaToxinDatas);
        }

        #endregion [※ Data Control 데이터 제어 ※]

        DateTime Truncate(DateTime dateTime, TimeSpan timeSpan)
        {
            if (timeSpan == TimeSpan.Zero)
                return dateTime;
            if (dateTime == DateTime.MinValue ||
                dateTime == DateTime.MaxValue)
                return dateTime;
            return dateTime.AddTicks(-(dateTime.Ticks % timeSpan.Ticks));
        }

        public int GetAreaIdByName(string areaName) => this.obss.FirstOrDefault(t => t.areaName.Equals(areaName)).areaId;
        public string GetAreaNameById(int areaId)
        {
            ObsData obsData = this.obss.FirstOrDefault(t => t.areaId == areaId);
            try
            {
                return obsData.areaName;
            }
            catch (Exception ex)
            {
                string msg = $"GetAreaNameBysd에서 해당 이름을 구하지 못했습니다. areaId : {areaId}";
                Debug.LogError(msg);
                Debug.LogException(ex);
                return msg;
            }
        }
        public string GetObsNameById(int obsid) => this.obss.FirstOrDefault(t => t.id == obsid).obsName;

        public string GetAreaNameByObsId(int obsid) => this.obss.FirstOrDefault(t => t.id == obsid).areaName;

    }
}
