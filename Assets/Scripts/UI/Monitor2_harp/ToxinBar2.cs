using Onthesys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;


internal class ToxinBar2 : MonoBehaviour 
{
    public TMP_Text toxinName;
    public TMP_Text current;
    public TMP_Text warning;
    public UILineRenderer line;
    public List<GameObject> statusIcon;
    public DateTime dt;
    public ToxinData data;

    public void OnClickList()
    {
        UiManager.Instance.Invoke(UiEventType.SelectAlarmSensor, (dt, this.data.hnsid));
    }
}

