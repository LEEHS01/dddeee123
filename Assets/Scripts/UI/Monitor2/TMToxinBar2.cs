using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Onthesys;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using System;

public class TMToxinBar2 : MonoBehaviour
{
    public TMP_Text toxinName;
    public TMP_Text current;
    public TMP_Text warning;
    public UILineRenderer line;
    public List<GameObject> statusIcon;
    public DateTime dt;
    public ToxinData data;

    public void OnClickList() {
        DataManager.Instance.RequestOnSelectLogToxin(dt, this.data.hnsid);
    }
}
