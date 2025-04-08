using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Onthesys;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;

public class TMNuclearList : MonoBehaviour
{
    
    public List<TMP_Text> names;
    public List<TMP_Text> greens;
    public List<TMP_Text> reds;
    public List<TMP_Text> yellows;
    public List<TMP_Text> purples;

    private const float MinFillAmount = 0.01f; // 최소 fillAmount 값

    // Start is called before the first frame update
    void Start()
    {
        //DataManager.Instance.OnAlarmUpdateNuclear.AddListener(this.OnAlarmUpdateNuclear);
    }

    public void OnAlarmUpdateNuclear(List<KeyValuePair<int, AlarmCount>> nuclearAlarms) {
        var list = nuclearAlarms;
        var i = 0;
        var green = 0;
        var yellow = 0;
        var red = 0;
        var purple = 0;

        foreach (var obs in list)
        {
            names[i].text = DataManager.Instance.GetAreaNameById(obs.Key);
            var txtGreen = greens[i];
            var txtYellow = yellows[i];
            var txtRed = reds[i];
            var txtPurple = purples[i];

            DOTween.To(() => green, x => green = x, obs.Value.GetGreen(), 1).OnUpdate(() => {
                txtGreen.text = green.ToString();
            });
            DOTween.To(() => yellow, x => yellow = x, obs.Value.GetYellow(), 1).OnUpdate(() => {
                txtYellow.text = yellow.ToString();
            });
            DOTween.To(() => red, x => red = x, obs.Value.GetRed(), 1).OnUpdate(() => {
                txtRed.text = red.ToString();
            });
            DOTween.To(() => purple, x => purple = x, obs.Value.GetPurple(), 1).OnUpdate(() => {
                txtPurple.text = purple.ToString();
            });
            i++;
        }
    }

    public void OnClickBtn(int id) {
        DataManager.Instance.RequestOnSelectArea(id + 5);    
    }


}
