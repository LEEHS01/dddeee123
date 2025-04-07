using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Onthesys;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine.Events;

public class TMThreeCirclePercent : MonoBehaviour
{
    public List<GameObject> maps = new List<GameObject>();
    private List<KeyValuePair<string, int>> list;

    public List<Image> ringCharts;
    public List<TextMeshProUGUI> txtPercents;
    public List<TextMeshProUGUI> txtMapName;

    private const float MinFillAmount = 0.01f; // 최소 fillAmount 값

    public UnityEvent OnUpdateValue;

    // Start is called before the first frame update
    void Start()
    {
        //DataManager.Instance.OnAlarmUpdateMonthly.AddListener(this.OnUpdate);
    }

    public void OnUpdate(List<KeyValuePair<string, int>> top5AlarmsForMonth) { 
        var dict = top5AlarmsForMonth.ToDictionary();
        this.list = top5AlarmsForMonth.ToList();

        var sum = dict.Values.Sum();
        var duration = 1f;
        var rotation = MinFillAmount;

        if (sum == 0)
        {
            var percent = duration / (float)list.Count;
            for (int i = 0; i < list.Count; i++)
            {
                ringCharts[i].DOFillAmount(percent, duration);
                txtPercents[i].text = "0%";

                if (i != 0)
                {
                    ringCharts[i].transform.DOLocalRotate(new Vector3(0, 0, rotation), duration);
                }
                txtMapName[i].text = list[i].Key;
                rotation -= (360 * percent);
            }

        }
        else
        {
            for (int i = 0; i < list.Count; i++)
            {
                var p = ((float)list[i].Value / (float)sum);
                var setPercent = p < MinFillAmount ? MinFillAmount : p;
                ringCharts[i].DOFillAmount(setPercent, duration);
                txtPercents[i].text = (int)(setPercent * 100) + "%";

                if (i != 0)
                {
                    ringCharts[i].transform.DOLocalRotate(new Vector3(0, 0, rotation), duration);
                }
                txtMapName[i].text = list[i].Key;
                rotation -= (360 * setPercent);
            }
        }
    }

    public List<int> GetValues()
    {
        if (list != null)
            return list.Select(t => t.Value).ToList();
        else
            return new List<int> { 1, 1, 1, 1, 1 };
    }

    public void OnClickList(int listId) 
    {
        Debug.Log(this.list[listId].Key);

        this.maps.ForEach(maps =>
        {
            maps.SetActive(false);
        });
        var idx = DataManager.Instance.GetAreaIdByName(this.list[listId].Key) - 1;

        this.maps[idx].SetActive(true);
        DataManager.Instance.RequestOnSelectArea(idx);
    }


}
