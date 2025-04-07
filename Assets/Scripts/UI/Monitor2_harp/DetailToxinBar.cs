using Onthesys;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;


internal class DetailToxinBar : MonoBehaviour
{

    private ToxinData toxinData;
    private DateTime aladt;
    public TMP_Text txtName;
    public UILineRenderer2 line;
    public List<TMP_Text> hours;
    public List<TMP_Text> verticals;
    public GameObject btnDetail;

    //private int periodDays = 1;
    public TMP_Dropdown periodDropdown;

    //테스트용
    private List<float> chartData = new List<float>();
    private List<DateTime> timeStamps = new List<DateTime>(); // 시간 데이터 
    private int maxDataPoints = 20; // 최대 데이터 포인트 수

    void Start()
    {
        Initialize();
        InitializeDropdown();

        UiManager.Instance.Register(UiEventType.SelectCurrentSensor, OnSelectLog);
        UiManager.Instance.Register(UiEventType.SelectAlarmSensor, OnSelectToxin);
        UiManager.Instance.Register(UiEventType.ChangeTrendLine, OnChangeTrendLine); // 이벤트 등록



        AddDataPoint(0.5f);  // 첫 번째 더미
        AddDataPoint(0.6f);  // 두 번째 더미
        StartCoroutine(AddRandomDataRoutine()); //테스트
    }

    private void Initialize()
    {
        SetDynamicHours(1);
        this.txtName.text = "";
        this.btnDetail.SetActive(true);
    }

    #region 테스트용 실시간 갱신 그래프
    private IEnumerator AddRandomDataRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f); // 1초마다 갱신

            // 랜덤값 추가
            float randomValue = UnityEngine.Random.Range(0f, 1f);
            Debug.Log($"Generated random value: {randomValue}");

            // 데이터 추가 및 갱신
            AddDataPoint(randomValue);

            // 현재 시간 갱신
            UpdateXAxis();
        }
    }

    private void AddDataPoint(float newValue)
    {

        // 최대 데이터 포인트를 초과하면 가장 오래된 데이터 제거
        if (chartData.Count >= maxDataPoints)
        {
            chartData.RemoveAt(0);
            timeStamps.RemoveAt(0);
        }

        // 새로운 데이터 추가
        chartData.Add(newValue);
        timeStamps.Add(DateTime.Now);

        // 데이터 포인트가 최소 2개 이상일 때만 업데이트
        if (chartData.Count >= 2)
        {
            line.UpdateControlPoints(chartData);
            UpdateXAxis();
        }
        else
        {
            Debug.LogWarning("그래프를 그리기 위한 최소 점 개수가 부족합니다.");
        }
    }

    private void UpdateXAxis()
    {
        DateTime currentTime = DateTime.Now;
        for (int i = 0; i < hours.Count; i++)
        {
            if (i < timeStamps.Count)
            {
                // timeStamps 리스트에 있는 시간 표시
                hours[i].text = timeStamps[i].ToString("HH:mm:ss");
            }
            else
            {
                // 최신 시간으로 채워주기
                DateTime time = currentTime.AddSeconds(-1 * (hours.Count - i - 1));
                hours[i].text = time.ToString("HH:mm:ss");
            }
        }
    }
    #endregion

    private void InitializeDropdown()
    {
        if (periodDropdown == null)
        {
            Debug.LogError("Dropdown 객체가 할당x");
            return;
        }

        var options = new List<string> { "1일", "7일", "30일" };
        periodDropdown.ClearOptions();
        periodDropdown.AddOptions(options);

        periodDropdown.onValueChanged.AddListener(value =>
        {
            int periodDays = GetSelectedPeriodDays();
            Debug.Log($"기간 선택: {periodDays}일");
            UiManager.Instance.Invoke(UiEventType.ChangeTrendLine, periodDays);
        });

        Debug.Log("Dropdown 초기화 완료");
    }

   
    public void OnSelectLog(object data)
    {
        if (data is LogData logData)
        {
            this.aladt = logData.time;
            UpdateChartData(1);
        }
    }

    public void OnSelectToxin(object data)
    {
        if (data is ToxinData toxinData)
        {
            this.toxinData = toxinData;
            this.txtName.text = toxinData.hnsName;
            this.btnDetail.SetActive(true);
            
            int periodDays = GetSelectedPeriodDays();
            UpdateChartData(periodDays);
        }
    }

    private int GetSelectedPeriodDays()
    {
        int periodDays = 1;
        switch (periodDropdown.value)
        {
            case 0:
                periodDays = 1;  // 1일
                break;
            case 1:
                periodDays = 7;  // 7일
                break;
            case 2:
                periodDays = 30; // 30일
                break;
        }
        return periodDays;
    }

    private void UpdateChartData(int periodDays)
    {
        if (toxinData == null)
        {
            Debug.LogWarning("ToxinData is null. 랜덤 데이터로 그래프를 초기화합니다.");
            line.UpdateControlPoints(chartData);
            SetDynamicHours(periodDays);
            return;
        }

        var convertedData = ConvertToChartData(toxinData);
        line.UpdateControlPoints(convertedData);
        SetDynamicHours(periodDays);
    }


        private List<float> ConvertToChartData(ToxinData toxin)
    {
        if (toxin == null) throw new Exception("DetailToxinBar.toxinData is null. Cannot draw the graph!");

        var max = toxin.values.Max();
        var lchart = new List<float>();
        if (max > toxin.warning)
        {
            lchart = toxin.values.Select(t => t / max).ToList();
        }
        else
        {
            max = toxin.warning;
            lchart = toxin.values.Select(t => t / toxin.warning).ToList();
        }
        SetVertical(max);
        return lchart;
    }

    private void SetDynamicHours(int periodDays)
    {
        DateTime endDt = DateTime.Now;
        DateTime startDt = endDt.AddDays(-periodDays);
        var interval = (endDt - startDt).TotalMinutes / this.hours.Count;

        for (int i = 0; i < this.hours.Count; i++)
        {
            var t = endDt.AddMinutes(-(interval * i));
            this.hours[i].text = t.ToString("MM-dd HH:mm");
        }
    }

    private void SetVertical(float max)
    {
        var verticalMax = ((max + 1) / (verticals.Count - 1));

        for (int i = 0; i < this.verticals.Count; i++)
        {
            this.verticals[i].text = Math.Round((verticalMax * i), 2).ToString();
        }
    }

    public void OnDetailSelect()
    {
        if (toxinData == null)
        {
            Debug.LogWarning("No toxin data available.");
            return;
        }

        UiManager.Instance.Invoke(UiEventType.SelectAlarmSensor, (this.aladt, this.toxinData.hnsid));
    }

    private void OnChangeTrendLine(object data)
    {
        if (data is int periodDays)
        {
            Debug.Log($"ChangeTrendLine 이벤트 수신: {periodDays}일");
            SetDynamicHours(periodDays);
            UpdateChartData(periodDays);
        }
    }


   

}

