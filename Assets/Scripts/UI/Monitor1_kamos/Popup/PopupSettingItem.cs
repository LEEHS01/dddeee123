using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupSettingItem : MonoBehaviour
{
    public Toggle tglVisibility;
    public TMP_Text lblSensorName;

    int sensorId;


    private void Start()
    {
        tglVisibility = GetComponentInChildren<Toggle>();
        lblSensorName = GetComponentInChildren<TMP_Text>();

        tglVisibility.onValueChanged.AddListener(OnValueChanged);
    }

    public void SetItem(int sensorId, string sensorName, bool isVisible) 
    {
        lblSensorName.text = sensorName;
        tglVisibility.isOn = isVisible;
        this.sensorId = sensorId;
    }
    private void OnValueChanged(bool isVisible)
    {
        //Temporary Function
        UiManager.Instance.Invoke(UiEventType.CommitSensorUsing, (sensorId, isVisible));
    }
}