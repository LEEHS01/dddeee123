using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Onthesys;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;

public class TMTop5List : MonoBehaviour
{
    public List<GameObject> maps = new List<GameObject>();
    private List<KeyValuePair<string, AlarmData>> list;
    public List<TMP_Text> names;
    public List<TMP_Text> reds;
    public List<TMP_Text> yellows;
        
    private const float MinFillAmount = 0.01f; // 최소 fillAmount 값

    // Start is called before the first frame update
    void Start()
    {
        //DataManager.Instance.OnAlarmUpdateYearly.AddListener(this.OnAlarmUpdateYearly);
    }

    public void OnAlarmUpdateYearly(List<KeyValuePair<string, AlarmData>> top5AlarmsForYear) 
    {
        this.list = top5AlarmsForYear.ToList();

        var red = 0;
        var yellow = 0;
        for (int i = 0; i < this.list.Count; i++)
        {
            names[i].text = list[i].Key;
            var txtRed = reds[i];
            var txtYellow = yellows[i];

            DOTween.To(() => red, x => red = x, list[i].Value.GetRed(), 1).OnUpdate(() => {
                txtRed.text = red.ToString();
            });
            DOTween.To(() => yellow, x => yellow = x, list[i].Value.GetYellow(), 1).OnUpdate(() => {
                txtYellow.text = yellow.ToString();
            });

            red = 0;
            yellow = 0;
        }
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
