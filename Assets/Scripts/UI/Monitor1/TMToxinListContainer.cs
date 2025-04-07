using Onthesys;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class TMToxinListContainer : MonoBehaviour
{
    public UnityEvent<int> onSelectObs;
    public UnityEvent<List<ToxinData>> onLoadSetting;
    public UnityEvent<List<ToxinData>> onIntervalToxinValue;
    public UnityEvent<List<ToxinData>> onToxinStatusChange;
    public UnityEvent<List<ToxinData>> onChangeToxin;



    public void OnSelectObs(int obsId) => onSelectObs.Invoke(obsId);
    public void OnLoadSetting(List<ToxinData> toxinDatas) => onLoadSetting.Invoke(toxinDatas);
    public void OnIntervalToxinValue(List<ToxinData> toxinDatas) => onIntervalToxinValue.Invoke(toxinDatas);
    public void OnToxinStatusChange(List<ToxinData> toxinDatas) => onToxinStatusChange.Invoke(toxinDatas);
    public void OnChangeToxin(List<ToxinData> toxinDatas) => onChangeToxin.Invoke(toxinDatas);
}