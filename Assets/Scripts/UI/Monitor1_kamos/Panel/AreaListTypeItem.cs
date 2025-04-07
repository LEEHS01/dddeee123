using Onthesys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;


public class AreaListTypeItem : MonoBehaviour
{
    public int areaId;
    public string areaName;

    TMP_Text lblAreaName;
    Dictionary<ToxinStatus, TMP_Text> lblAlarmCounts;

    private void OnValidate()
    {
        lblAreaName = transform.Find("TitleName_Button").GetComponentInChildren<TMP_Text>();
        lblAreaName.text = areaName;
    }


    private void Start()
    {
        lblAreaName = transform.Find("TitleName_Button").GetComponentInChildren<TMP_Text>();

        var signalLamps = transform.Find("SignalLamps");
        lblAlarmCounts = new() {
            {ToxinStatus.Green  , signalLamps.Find("SignalLamp_Green").GetComponentInChildren<TMP_Text>() },
            {ToxinStatus.Yellow , signalLamps.Find("SignalLamp_Yellow").GetComponentInChildren<TMP_Text>() },
            {ToxinStatus.Red    , signalLamps.Find("SignalLamp_Red").GetComponentInChildren<TMP_Text>() },
            {ToxinStatus.Purple , signalLamps.Find("SignalLamp_Purple").GetComponentInChildren<TMP_Text>() },
        };

        //Test코드
        lblAreaName.text = areaName;

        //lblAreaName.text = "테스트 입력";
        //lblAlarmCounts[ToxinStatus.Green].text = "11";
        //lblAlarmCounts[ToxinStatus.Yellow].text = "12";
        //lblAlarmCounts[ToxinStatus.Red].text = "13";
        //lblAlarmCounts[ToxinStatus.Purple].text = "14";        
    }


    public void SetAreaData(int areaId, string areaName, Dictionary<ToxinStatus, int> obsStatus) 
    {
        lblAreaName.text = areaName;
        foreach (var pair in obsStatus)
        {
            lblAlarmCounts[pair.Key].text = pair.Value.ToString();
        }
    }
    public void SetAreaData(int areaId, string areaName, int greenObs, int yellowObs, int redObs, int purpleObs)
        => SetAreaData(areaId, areaName, new() {
            { ToxinStatus.Green, greenObs },
            { ToxinStatus.Yellow, yellowObs },
            { ToxinStatus.Red, redObs },
            { ToxinStatus.Purple, purpleObs },
        });



    
}