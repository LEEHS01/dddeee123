using Onthesys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AreaListYearItem : MonoBehaviour
{
    int areaId;

    TMP_Text lblAreaName, lblWarnAndSer, lblMalfunction;
    Button btnNavigateArea; 

    private void Start()
    {
        lblAreaName = transform.Find("Text (TMP) Content").GetComponent<TMP_Text>();
        lblWarnAndSer = transform.Find("Text (TMP) Alert").GetComponent<TMP_Text>();
        lblMalfunction = transform.Find("Text (TMP) Wrong").GetComponent<TMP_Text>();

        btnNavigateArea = GetComponent<Button>();
        btnNavigateArea.onClick.AddListener(OnClick);
    }


    public void SetAreaData(int areaId, string areaName, AlarmCount alarmCount) 
    {
        this.areaId = areaId;
        lblAreaName.text = areaName;
        lblWarnAndSer.text = "" +(alarmCount.yellow + alarmCount.red);
        lblMalfunction.text = "" + alarmCount.purple;
    }

    void OnClick()
    {
        UiManager.Instance.Invoke(UiEventType.NavigateArea, areaId);
    }
}