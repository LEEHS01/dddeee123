using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Onthesys;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;

public class TmAlramList : MonoBehaviour
{
    public TmAlramListItem itemPrefab;
    public GameObject itemContainer;
    private List<LogData> list;
    public List<TmAlramListItem> items;

    public TMP_Dropdown dropdownMap;
    public TMP_Dropdown dropdownStatus;
    
    private int areaidx = -1;
    private int statusIdx = -1;

    void Start()
    {
        this.itemPrefab.gameObject.SetActive(false);
        this.dropdownMap.onValueChanged.AddListener(this.OnDropdownMapValueChanged);
        this.dropdownStatus.onValueChanged.AddListener(this.OnDropdownStatusValueChanged);

        //DataManager.Instance.OnAlarmUpdate.AddListener(this.OnAlarmUpdate);
    }

    public void OnAlarmUpdate(List<LogData> alarmLogs)
    {
        if (this.areaidx >= 0 || this.statusIdx >= 0)
            this.UpdateFilter();
        else
        {
            list = alarmLogs;
            UpdateText();
        }
    }
    private void OnDropdownMapValueChanged(int value) 
	{
        this.areaidx = value;
        
        this.UpdateFilter();
    }

    private void OnDropdownStatusValueChanged(int value) 
	{
        this.statusIdx = value - 1;
        
        this.UpdateFilter();
    }

    public void UpdateText()
    {

        for (int i = 0; i < this.items.Count; i++) {
            DestroyImmediate(this.items[i].gameObject);
        }
        this.items.Clear();
        
        float height = this.list.Count * this.itemPrefab.GetComponent<RectTransform>().sizeDelta.y;
        for(int i = 0; i < this.list.Count; i++) {
            TmAlramListItem item = Instantiate(this.itemPrefab);
            item.gameObject.SetActive(true);
            item.transform.SetParent(this.itemContainer.transform);
            item.SetText(this.list[i]);
            this.items.Add(item);
        }
        this.itemContainer.GetComponent<RectTransform>().sizeDelta = 
            new Vector2(this.itemContainer.GetComponent<RectTransform>().sizeDelta.x, height);
    }

    private void UpdateFilter() {
        for(int i = 0; i < this.items.Count; i++) {
            TmAlramListItem item = this.items[i];
            item.gameObject.SetActive(true);

            if(this.areaidx > 0) {
                if(DataManager.Instance.GetAreaIdByName(item.data.areaName) != this.areaidx) {
                    item.gameObject.SetActive(false);
                }
            }

            if(this.statusIdx > -1) {
                if(item.data.status != this.statusIdx)
                {
                    item.gameObject.SetActive(false);
                }
            }
        }
    }
    
    public void OnClickList(int listId) 
    {
        //DataManager.Instance.OnSelectObs.Invoke(this.list[listId].obsId);
    }

    public void OnClickOrder(string order) {
        this.list.Sort((a, b)=>{
            if(order == AlramOrder.TIME_UP.ToString()) 
            {
                return b.time.CompareTo(a.time);
            } 
            else if(order == AlramOrder.TIME_DOWN.ToString()) 
            {
                return a.time.CompareTo(b.time);
            } 
            else if(order == AlramOrder.STATUS_UP.ToString()) 
            {
                return b.hnsName.CompareTo(a.hnsName);
            } 
            else if(order == AlramOrder.STATUS_DOWN.ToString()) 
            {
                return a.hnsName.CompareTo(b.hnsName);
            } 
            else if(order == AlramOrder.MAP_UP.ToString()) 
            {
                return b.areaName.CompareTo(a.areaName);
            } 
            else if(order == AlramOrder.MAP_DOWN.ToString()) 
            {
                return a.areaName.CompareTo(b.areaName);
            } 
            else if(order == AlramOrder.AREA_UP.ToString()) 
            {
                return b.obsName.CompareTo(a.obsName);
            } 
            else if(order == AlramOrder.AREA_DOWN.ToString()) 
            {
                return a.obsName.CompareTo(b.obsName);
            } 
            return 0;
        });
        this.UpdateText();
        this.UpdateFilter();
    }

}


public enum AlramOrder {
    TIME_UP,
    TIME_DOWN,
    STATUS_UP,
    STATUS_DOWN,
    MAP_UP,
    MAP_DOWN,
    AREA_UP,
    AREA_DOWN,
    VALUE_UP,
    VALUE_DOWN
}