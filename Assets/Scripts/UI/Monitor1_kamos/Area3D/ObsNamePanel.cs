using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;


public class ObsNamePanel : MonoBehaviour
{
    public int obsId = -1;
    TMP_Text lblObsName;

    private void Start()
    {
        UiManager.Instance.Register(UiEventType.NavigateObs, OnNavigateObs);

        lblObsName = GetComponentInChildren<TMP_Text>();
    }

    private void OnNavigateObs(object obs) 
    {
        string txt = "능내리";
        ///TODO
        obsId = obsId;
        lblObsName.text = txt;
    }

}
