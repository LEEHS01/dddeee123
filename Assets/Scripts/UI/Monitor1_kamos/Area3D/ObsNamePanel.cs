using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;


public class ObsNamePanel : MonoBehaviour
{
    ModelProvider modelProvider => UiManager.Instance.modelProvider;

    public int obsId = -1;
    TMP_Text lblObsName;

    private void Start()
    {
        UiManager.Instance.Register(UiEventType.NavigateObs, OnNavigateObs);

        lblObsName = GetComponentInChildren<TMP_Text>();
    }

    private void OnNavigateObs(object obj) 
    {
        if (obj is not int obsId) return;

        lblObsName.text = modelProvider.GetObs(obsId).obsName;
    }

}
