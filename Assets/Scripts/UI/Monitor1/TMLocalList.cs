using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Onthesys;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using System.Collections;
using UnityEngine.Events;

public class TMLocalList : MonoBehaviour
{
    public List<TMLocalArea> areas;

    public UnityEvent<List<LogData>> onAlarmUpdate;

    void Awake()
    {
    }

    void Start()
    {
        Debug.Log("조건 만족 후 실행");
        //DataManager.Instance.OnInit.AddListener(OnInit);
        //DataManager.Instance.OnAlarmUpdate.AddListener(OnAlarmUpdate);

        Debug.Log("관측소 정보 지정 완료");

    }

    public void OnAlarmUpdate(List<LogData> logDatas) 
    {
        onAlarmUpdate.Invoke(logDatas);
    }
    public void OnInit(List<ObsData> obss)
    {
        Debug.Log("조건 만족 후 실행");
        for (int i = 0; i < areas.Count; i++)
        {
            this.areas[i].id = obss[i].id;
            this.areas[i].txtName.text = obss[i].obsName.ToString();
        }

        Debug.Log("관측소 정보 지정 완료");


        for (var i = 0; i < transform.childCount; i++)
            transform.GetChild(i).gameObject.SetActive(false);
    }
}
