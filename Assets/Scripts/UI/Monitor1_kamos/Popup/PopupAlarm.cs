using DG.Tweening;
using JetBrains.Annotations;
using NUnit.Framework;
using Onthesys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupAlarm : MonoBehaviour 
{
    Button btnClose, btnAlarmTransition;
    Image imgSignalLamp, imgSignalLight;
    TMP_Text lblTitle, lblSummary;//, lblAwareTransition;

    Tween intervalThread;

    LogData alarmLog = null;
    int passMins = 0;
    const float intervalValue = 10f;    //60f = 1min
    static Dictionary<ToxinStatus, Color> statusColorDic = new();

    static PopupAlarm()
    {
        Dictionary<ToxinStatus, string> rawColorSets = new() {
            { ToxinStatus.Green,    "#3EFF00"},
            { ToxinStatus.Yellow,   "#FFF600"},
            { ToxinStatus.Red,      "#FF0000"},
            { ToxinStatus.Purple,   "#6C00E2"},
        };

        Color color;
        foreach (var pair in rawColorSets)
            if (ColorUtility.TryParseHtmlString(htmlString: pair.Value, out color))
                statusColorDic[pair.Key] = color;
    }

    private void Start()
    {
        Transform titleLine = transform.Find("TitleLine");
        lblTitle = titleLine.Find("lblTitle").GetComponent<TMP_Text>();
        btnClose = titleLine.Find("btnClose").GetComponent<Button>();
        btnClose.onClick.AddListener(OnCloseAlarm);

        Transform titleCircle = titleLine.Find("Icon_EventPanel_TitleCircle");
        imgSignalLamp = titleCircle.Find("imgSignalLamp").GetComponentInChildren<Image>();
        imgSignalLight = titleCircle.Find("imgSignalLight").GetComponentInChildren<Image>();

        btnAlarmTransition = transform.Find("btnAlarmTransition").GetComponent<Button>();
        btnAlarmTransition.onClick.AddListener(OnClickAlarmTransition);

        lblSummary = transform.Find("lblSummary").GetComponent<TMP_Text>();
        //lblAwareTransition = transform.Find("lblAwareTransition").GetComponent<TMP_Text>();

        UiManager.Instance.Register(UiEventType.ChangeAlarmList, OnChangeAlarmList);

        gameObject.SetActive(false);
    }

    void OnChangeAlarmList(object obj)
    {
        if (obj is not List<LogData> alarmLogs) throw new Exception("PopupAlarm.cs can't accept event type : ChangeAlarmList with no List<AlarmCount>");

        InitAlarmLog(alarmLogs[0]);
    }

    public void InitAlarmLog(LogData data)
    {
        if (this.alarmLog == data) return;

        passMins = 0;
        alarmLog = data;
        IntervalUpdateView();

        intervalThread?.Kill();
        intervalThread = DOVirtual.DelayedCall(intervalValue,IntervalUpdateView).SetUpdate(true);
        intervalThread.SetLoops(99999, LoopType.Restart);

        //gameObject.SetActive(false);

        gameObject.SetActive(true);
    }

    void IntervalUpdateView()
    {
        passMins++;

        //lblSummary 설정
        {
            DateTime logDt = alarmLog.time;
            string passTimeString = "#ERROR!";
            if (passMins < 60)
            {
                passTimeString = $"{passMins} 분";
            }
            else if (passMins < 60 * 24)
            {
                passTimeString = $"{passMins / 60} 시간";
            }
            else
            {
                passTimeString = $"{passMins / 60 / 24} 일";
            }

            lblSummary.text = "" +
                $"발생 지점 : {alarmLog.areaName} - {alarmLog.obsName}\n" +
                $"발생 시각 : {logDt:yy/MM/dd HH:mm}({passTimeString} 전)\n\n";

            if ((ToxinStatus)alarmLog.status == ToxinStatus.Purple)
            {
                lblSummary.text +=
                    $"설비 이상 : {"보드 " + alarmLog.boardId}";
            }
            else
            {
                lblSummary.text +=
                    $"원인 물질 : {alarmLog.hnsName}\n" +
                    $"측정 값 : {alarmLog.value.Value} / {alarmLog.value}";
            }
        }

        //alacode에 맞게 lblTitle, imgSignalLamp 변경
        switch (alarmLog.status)
        {
            case 0: //설비이상
                lblTitle.text = $"설비이상 발생 : {alarmLog.areaName} - {alarmLog.obsName} 보드 {alarmLog.boardId}번";
                imgSignalLamp.color = statusColorDic[ToxinStatus.Purple];
                break;
            case 1: //경계
                lblTitle.text = $"경계 알람 발생 : {alarmLog.areaName} - {alarmLog.obsName} {alarmLog.hnsName}";
                imgSignalLamp.color = statusColorDic[ToxinStatus.Yellow];
                break;
            case 2: //경보
                lblTitle.text = $"경보 알람 발생 : {alarmLog.areaName} - {alarmLog.obsName} {alarmLog.hnsName}";
                imgSignalLamp.color = statusColorDic[ToxinStatus.Red];
                break;
        }
        imgSignalLight.color = imgSignalLamp.color;
    }


    private void OnClickAlarmTransition()
    {
        UiManager.Instance.Invoke(UiEventType.SelectAlarm, alarmLog);
        UiManager.Instance.Invoke(UiEventType.NavigateObs, alarmLog.obsId);
    }
    private void OnCloseAlarm()
    {
        gameObject.SetActive(false);
    }
}
