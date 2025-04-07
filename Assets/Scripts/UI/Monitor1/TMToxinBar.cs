using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Onthesys;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;

public class TMToxinBar : MonoBehaviour
{
    public TMP_Text name;
    public TMP_Text current;
    public TMP_Text warning;
    public Image alaStatus;
    public UILineRenderer line;

    public bool OnLoadSetting(ToxinData toxinData)
    {

        this.gameObject.SetActive(toxinData.on);
        if (toxinData.on)
        {
            this.name.text = toxinData.hnsName;
            this.warning.text = toxinData.warning.ToString();
            this.current.text = toxinData.GetLastValue().ToString();

            var lcahrt = toxinData.values.Select(t => t / toxinData.warning).ToList();
            this.line.UpdateControlPoints(lcahrt);

            switch (toxinData.status)
            {
                case ToxinStatus.Green:
                    this.alaStatus.color = Color.green;
                    break;
                case ToxinStatus.Yellow:
                    this.alaStatus.color = Color.yellow;
                    break;
                case ToxinStatus.Red:
                    this.alaStatus.color = Color.red;
                    break;
                case ToxinStatus.Purple:    //수정한 부분
                    this.alaStatus.color = new Color32(0x6C, 0x00, 0xE2, 0xFF);
                    break;
                default:
                    this.alaStatus.color = Color.green;
                    break;
            }

            return true;
        }
        return false;
    }
    public void OnChangeToxin(ToxinData toxinData)
    {

    }
    public void OnIntervalToxinValue(ToxinData toxinData)
    {

    }
    public void OnToxinStatusChange(ToxinData toxinData)
    {

    }

}
