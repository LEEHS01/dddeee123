using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Onthesys;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;

public class TMPopupMachine : MonoBehaviour
{
    private ObsData area;
    
    // Start is called before the first frame update
    void Start()
    {
        //DataManager.Instance.OnSelectArea.AddListener(this.UpdateText);
    }

    public void UpdateText(ObsData data = null)
    {
        //this.obs = data;
    }
}
