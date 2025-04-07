using Onthesys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ModelManager : MonoBehaviour, ModelProvider
{

    #region [Singleton]
    public static ModelProvider Instance = null;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }
    #endregion [Singleton]

    #region [Instantiating]
    public DbManager dbManager;
    public UiManager uiManager;

    private void Start()
    {
        dbManager = GetComponent<DbManager>();
        uiManager = GetComponent<UiManager>();
    }
    #endregion [Instantiating]


    #region [DataStructs]

    List<ObsData> obss;
    List<AlarmData> alarms;
    List<ToxinData> toxins;
    List<ToxinData> toxinsLog;
    //List<Area> areas;

    #endregion [DataStructs]






    public List<(int areIdx, string areaName)> GetAreaDatas()
    {
        throw new NotImplementedException();
    }

    public ObservatoryModel GetObsData(int obsId)
    {
        throw new NotImplementedException();
    }

    public List<ObservatoryModel> GetObsDatas()
    {
        throw new NotImplementedException();
    }

    public List<ObservatoryModel> GetObsDatasByAreaId(int areaId)
    {
        throw new NotImplementedException();
    }

}
