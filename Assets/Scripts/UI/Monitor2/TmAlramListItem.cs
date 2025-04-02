using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Onthesys;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
//using static UnityEditor.Progress;

public class TmAlramListItem : MonoBehaviour
{
    public LogData data;
    public TMP_Text txtTime;
    public TMP_Text txtDesc;
    public TMP_Text txtMap;
    public TMP_Text txtArea;
    public TMP_Text txtToxin;
    
    public void SetText(LogData data) {
        this.data = data;
        this.txtTime.text = data.time.ToString("hh:mm");
        this.txtDesc.text = data.hnsName;
        this.txtMap.text = data.areaName;
        this.txtArea.text = data.obsName;

        switch (data.status)
        {
            case 0:
                this.txtToxin.text = "설비이상";
                break;
            case 1:
                this.txtToxin.text = "경계";
                break;
            case 2:
                this.txtToxin.text = "경보";
                break;
        }
        //this.txtToxin.text = data.toxin.values.Last().ToString() + "%";
    }

    public void OnClickList() 
    {
        DataManager.Instance.RequestOnSelectLog(this.data.idx);
    }
}
