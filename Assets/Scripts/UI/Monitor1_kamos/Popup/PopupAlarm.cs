using DG.Tweening;
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

    AlarmLogModel alarmLog = null;
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
    }

    public void InitAlarmLog(AlarmLogModel data)
    {
        alarmLog = data;
        DOVirtual.DelayedCall(0.1f, IntervalUpdateView);
        //gameObject.SetActive(false);
    }

    void IntervalUpdateView()
    {
        passMins++;

        //lblSummary 설정
        {
            DateTime logDt = Convert.ToDateTime(alarmLog.aladt);
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
                $"발생 지점 : {alarmLog.areanm} - {alarmLog.obsnm}\n" +
                $"발생 시각 : {logDt:yy/MM/dd HH:mm}({passTimeString} 전)\n\n";

            if (alarmLog.alacode == 0)
            {
                lblSummary.text +=
                    $"설비 이상 : {"보드 " + alarmLog.boardidx}";
            }
            else
            {
                lblSummary.text +=
                    $"원인 물질 : {alarmLog.hnsnm}\n" +
                    $"측정 값 : {alarmLog.currval} / {alarmLog.alahihival}";
            }
        }

        //alacode에 맞게 lblTitle, imgSignalLamp 변경
        switch (alarmLog.alacode)
        {
            case 0: //설비이상
                lblTitle.text = $"설비이상 발생 : {alarmLog.areanm} - {alarmLog.obsnm} 보드 {alarmLog.boardidx}번";
                imgSignalLamp.color = statusColorDic[ToxinStatus.Purple];
                break;
            case 1: //경계
                lblTitle.text = $"경계 알람 발생 : {alarmLog.areanm} - {alarmLog.obsnm} {alarmLog.hnsnm}";
                imgSignalLamp.color = statusColorDic[ToxinStatus.Yellow];
                break;
            case 2: //경보
                lblTitle.text = $"경보 알람 발생 : {alarmLog.areanm} - {alarmLog.obsnm} {alarmLog.hnsnm}";
                imgSignalLamp.color = statusColorDic[ToxinStatus.Red];
                break;
        }
        imgSignalLight.color = imgSignalLamp.color;



        DOVirtual.DelayedCall(intervalValue, IntervalUpdateView);
    }


    private void OnClickAlarmTransition()
    {
        UiManager.Instance.Invoke(UiEventType.SelectAlarm, alarmLog);
        UiManager.Instance.Invoke(UiEventType.NavigateObs, alarmLog.obsidx);
    }
    private void OnCloseAlarm()
    {
        PopupAlarmFactory alarmFactory;
        if (transform.parent.TryGetComponent(out alarmFactory)) 
        {
            alarmFactory.RemovePopupAlarm(this);
        }

        Destroy(gameObject);
    }
}
