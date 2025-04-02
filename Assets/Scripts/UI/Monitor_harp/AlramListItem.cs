using Onthesys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

internal class AlramListItem : MonoBehaviour
{
    // 알람 데이터
    public LogData data;

    public TMP_Text txtTime;
    public TMP_Text txtDesc;
    public TMP_Text txtMap;
    public TMP_Text txtArea;
    public TMP_Text txtToxin;

    // 클릭 이벤트 - UiManager로 전달
    public void OnClick()
    {
        UiManager.Instance.Invoke(UiEventType.SelectAlarm, data);
    }
}

