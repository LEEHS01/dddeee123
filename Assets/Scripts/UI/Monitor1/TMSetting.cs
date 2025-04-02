using UnityEngine;
using Onthesys;
using System.Collections.Generic;
using static UnityEngine.Rendering.DebugUI;
using System;
using System.Security.Cryptography;

public class TMSetting : MonoBehaviour
{
    public List<TMSettingListItem> list;
    public bool isInited = false;

    void Start() 
    {
        if (isInited) return;

        var data = new List<ToxinData>();//DataManager.Instance.toxinDatas;
        Action<int, float, UpdateColumn> onAction = DataManager.Instance.RequestOnSetToxinProp;

        if (data.Count > 0)
        {
            for (int i = 0; i < list.Count; i++)
            {
                TMSettingListItem item = this.list[i];

                item.id = i;
                item.data = data[i];
                item.txtName.text = data[i].hnsName;

                //Warning 임계값
                item.txtHihi.text = data[i].warning.ToString();
                item.txtHihi.onValueChanged.AddListener((txt) => 
                    onAction(item.id, float.Parse(txt), UpdateColumn.ALAHIHIVAL));

                item.txtHihi.onValueChanged.AddListener((txt) =>
                    Debug.Log($"Update Response: {item.id} {txt}"));

                //Serious 임계값
                item.txtHi.text = data[i].serious.ToString();
                item.txtHi.onValueChanged.AddListener((txt) => 
                    onAction(item.id, float.Parse(txt), UpdateColumn.ALAHIVAL));

                //임계시간
                item.txtDuration.text = data[i].duration.ToString();
                item.txtDuration.onValueChanged.AddListener((txt) => 
                    onAction(item.id, float.Parse(txt), UpdateColumn.ALAHIHISEC));

                //사용 여부
                item.checkbox.isOn = data[i].on;
                item.checkbox.onValueChanged.AddListener((on) => 
                    onAction(item.id, on ? 1f : 0, UpdateColumn.USEYN));

                //문제 해결 여부
                item.toggle.isOn = data[i].fix;
                item.toggle.onValueChanged.AddListener((on) => 
                    onAction(item.id, on ? 1f : 0, UpdateColumn.INSPECTIONFLAG));
                item.toggle.gameObject.SetActive(i <= 1);
            }
            isInited = true;
        }
    }

	public void OnLoadSetting(List<ToxinData> toxinDatas)
    {
        var data = toxinDatas;// new List<ToxinData>();//DataManager.Instance.toxinDatas;
        for (int i = 0; i < list.Count; i++)
        {
            TMSettingListItem item = this.list[i];
            item.data = data[i];
            item.txtName.text = data[i].hnsName;
            item.txtHihi.text = data[i].warning.ToString();
            item.txtHi.text = data[i].serious.ToString();
            item.checkbox.isOn = data[i].on;
            item.toggle.isOn = data[i].fix;
        }


        if (isInited) return;
        Action<int, float, UpdateColumn> onAction = DataManager.Instance.RequestOnSetToxinProp;

        if (data.Count > 0)
        {
            for (int i = 0; i < list.Count; i++)
            {
                TMSettingListItem item = this.list[i];

                item.id = i;
                item.data = data[i];
                item.txtName.text = data[i].hnsName;

                //Warning 임계값
                item.txtHihi.text = data[i].warning.ToString();
                item.txtHihi.onValueChanged.AddListener((txt) =>
                    onAction(item.id, float.Parse(txt), UpdateColumn.ALAHIHIVAL));

                item.txtHihi.onValueChanged.AddListener((txt) =>
                    Debug.Log($"Update Response: {item.id} {txt}"));

                //Serious 임계값
                item.txtHi.text = data[i].serious.ToString();
                item.txtHi.onValueChanged.AddListener((txt) =>
                    onAction(item.id, float.Parse(txt), UpdateColumn.ALAHIVAL));

                //임계시간
                item.txtDuration.text = data[i].duration.ToString();
                item.txtDuration.onValueChanged.AddListener((txt) =>
                    onAction(item.id, float.Parse(txt), UpdateColumn.ALAHIHISEC));

                //사용 여부
                item.checkbox.isOn = data[i].on;
                item.checkbox.onValueChanged.AddListener((on) =>
                    onAction(item.id, on ? 1f : 0, UpdateColumn.USEYN));

                //문제 해결 여부
                item.toggle.isOn = data[i].fix;
                item.toggle.onValueChanged.AddListener((on) =>
                    onAction(item.id, on ? 1f : 0, UpdateColumn.INSPECTIONFLAG));
                item.toggle.gameObject.SetActive(i <= 1);
            }
            isInited = true;
        }


    }

}
