using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Onthesys;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Unity.VisualScripting;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class TMLocalArea : MonoBehaviour
{
    public int id;
    public TMP_Text txtName;
    public GameObject btnLocation;
    public TMLocalList localList;

    void Start()
    {
        localList = transform.parent.GetComponentInParent<TMLocalList>();
        localList.onAlarmUpdate.AddListener(ImageColorChange);
        //DataManager.Instance.OnAlarmUpdate.AddListener(this.ImageColorChange);
    }

    private void ImageColorChange(List<LogData> alarmLogs)
    {
        var lala = alarmLogs.Where(t => t.obsId == id);

        Color color;
        if (lala.Count(t => t.status != 0) > 0)
            color = Color.red;
        else
            color = (lala.Count(t => t.status == 0) > 0 ? Color.yellow : new Color(69 / 255f, 177 / 255f, 226 / 255f));

        foreach (Transform child in btnLocation.transform)
        {
            var image = child.GetComponent<Image>();
            image.color = color;
        }
    }
    public void OnClickBtn() 
    {
        DataManager.Instance.RequestOnSelectObs(id);
    }
}
