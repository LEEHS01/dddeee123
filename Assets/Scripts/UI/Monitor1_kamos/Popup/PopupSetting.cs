using Onthesys;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UMP.Services.Helpers;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditorInternal.ReorderableList;

public class PopupSetting : MonoBehaviour
{
    ModelProvider modelProvider => UiManager.Instance.modelProvider;

    #region[UI Componenets]
    Button btnClose;

    GameObject pnlTabObs, pnlTabAlarm;//, pnlTabCCTV;
    Button btnTabObs, btnTabAlarm;//, btnTabCCTV;
    TMP_Dropdown ddlArea, ddlObs;

    //TabObs
    Toggle tglBoardToxin, tglBoardChemical;
    TMP_InputField txtCCTVEquipment, txtCCTVOutdoor;
    List<PopupSettingItem> chlUsingSensors;

    //TabAlarm
    Slider sldAlarmPopup;
    TMP_InputField txtDbAddress;
    #endregion [UI Components]

    #region [Variables]
    //선택한 지역 값
    int areaId, obsId;

    //팝업 시 사용할 기본 위치
    Vector3 defaultPos;
    #endregion [Variables]


    private void Start()
    {
        //PopupSetting 구성요소
        { 
            Transform conTabButtons = transform.Find("conTabButtons");
            {
                btnTabObs = conTabButtons.Find("btnObs").GetComponent<Button>();
                btnTabObs.onClick.AddListener(() => OnOpenTab(pnlTabObs));
                btnTabAlarm = conTabButtons.Find("btnAlarm").GetComponent<Button>();
                btnTabAlarm.onClick.AddListener(() => OnOpenTab(pnlTabAlarm));
                //btnTabCCTV = conTabButtons.Find("btnCCTV").GetComponent<Button>();
                //btnTabCCTV.onClick.AddListener(() => OnOpenTab(pnlTabCCTV));
            }

            Transform conTabPanels = transform.Find("conTabPanels");
            {
                pnlTabObs = conTabPanels.Find("tabObs").gameObject;
                pnlTabAlarm = conTabPanels.Find("tabAlarm").gameObject;
                //pnlTabCCTV = conTabPanels.Find("tabCCTV").gameObject;
            }

            btnClose = transform.Find("Btn_Close").GetComponent<Button>();
            btnClose.onClick.AddListener(OnCloseSetting);
        }
        
        //tabObs 구성요소
        { 
            Transform toggleBoard = pnlTabObs.transform.Find("ToggleBoard");
            {
                tglBoardToxin = toggleBoard.Find("BoardToxin").GetComponentInChildren<Toggle>();
                tglBoardToxin.onValueChanged.AddListener(isFixing => OnToggleBoard(true, isFixing));
                tglBoardChemical = toggleBoard.Find("BoardChemical").GetComponentInChildren<Toggle>();
                tglBoardChemical.onValueChanged.AddListener(isFixing => OnToggleBoard(false, isFixing));
            }

            Transform conSelectObs = pnlTabObs.transform.Find("conSelectObs");
            {
                ddlArea = conSelectObs.Find("ddlArea").GetComponent<TMP_Dropdown>();
                ddlArea.onValueChanged.AddListener(OnSelectArea);
                ddlObs = conSelectObs.Find("ddlObs").GetComponent<TMP_Dropdown>();
                ddlObs.onValueChanged.AddListener(OnSelectObs);
            }

            Transform conCCTVUrls = pnlTabObs.transform.Find("conCCTVUrls");

            txtCCTVEquipment = conCCTVUrls.Find("lblStreamEquipment").GetComponentInChildren<TMP_InputField>();
            txtCCTVEquipment.onValueChanged.AddListener(url => OnChangeCCTV(false, url));
            txtCCTVOutdoor = conCCTVUrls.Find("lblStreamEquipment").GetComponentInChildren<TMP_InputField>();
            txtCCTVOutdoor.onValueChanged.AddListener(url => OnChangeCCTV(true, url));

            chlUsingSensors = transform.GetComponentsInChildren<PopupSettingItem>().ToList();

            for (int i = 0; i < chlUsingSensors.Count; i++)
                chlUsingSensors[i].SetItem(i, "불러오는 중...", true);
        }

        //tabAlarm 구성요소
        {
            //Transform toggleAlarmPopup = pnlTabAlarm.transform.Find("ToggleAlarmPopup");
            //tglAlarmPopup = new() {
            //    {ToxinStatus.Yellow,    toggleAlarmPopup.Find("Serious")        .GetComponentInChildren<Toggle>() },
            //    {ToxinStatus.Red,       toggleAlarmPopup.Find("Warning")        .GetComponentInChildren<Toggle>() },
            //    {ToxinStatus.Purple,    toggleAlarmPopup.Find("Malfunction")    .GetComponentInChildren<Toggle>() },
            //};
            Transform slideAlarmPopup = pnlTabAlarm.transform.Find("SlideAlarmPopup");
            sldAlarmPopup = slideAlarmPopup.GetComponentInChildren<Slider>();
            sldAlarmPopup.onValueChanged.AddListener(OnAlarmSliderChanged);

            Transform dbAddressContainer = pnlTabAlarm.transform.Find("DbAddress");
            txtDbAddress = dbAddressContainer.GetComponentInChildren<TMP_InputField>();
            txtDbAddress.text = Option.url;
            txtDbAddress.onValueChanged.AddListener(OnChangeDbAddress);
        }


        UiManager.Instance.Register(UiEventType.PopupSetting, OnPopupSetting);
        UiManager.Instance.Register(UiEventType.NavigateObs, OnNavigateObs);
        UiManager.Instance.Register(UiEventType.ChangeSensorList, OnChangeSensorList);

        defaultPos = transform.position;

        //초기화 진행
        OnOpenTab(this.pnlTabObs);
        gameObject.SetActive(false);
        LoadAreaList();
        LoadObs(1);

        //Debug!
        UiManager.Instance.Register(UiEventType.CommitSensorUsing, tuple => Debug.LogWarning("CommitSensorUsing occured : " + tuple));
    }

    private void OnChangeSensorList(object obj)
    {
        List<ToxinData> toxins = modelProvider.GetToxins();

        for (int i = 0; i < toxins.Count; i++)
        {
            ToxinData toxin = toxins[i];
            PopupSettingItem item = chlUsingSensors[i];
            item.SetItem(i, toxin.hnsName, toxin.on);
        }

        ToxinData toxinBoard = toxins[0];
        tglBoardToxin.isOn = !toxinBoard.fix;

        ToxinData chemiBoard = toxins[1];
        tglBoardChemical.isOn = !chemiBoard.fix;

    }
    

    #region [Basic Function]
    private void OnOpenTab(GameObject targetTab)
    {
        Sprite sprTabOn = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Arts/UI/UIAssets/Textures/Components/Form/Btn_Search_p.png");
        Sprite sprTabOff = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Arts/UI/UIAssets/Textures/Components/Form/Btn_Search_n.png");

        btnTabObs   .GetComponentInChildren<Image>().sprite = pnlTabObs == targetTab? sprTabOn : sprTabOff;
        btnTabAlarm .GetComponentInChildren<Image>().sprite = pnlTabAlarm == targetTab? sprTabOn : sprTabOff;
        //btnTabCCTV  .GetComponentInChildren<Image>().sprite = pnlTabCCTV == targetTab? sprTabOn : sprTabOff;

        pnlTabObs.SetActive(pnlTabObs == targetTab);
        pnlTabAlarm.SetActive(pnlTabAlarm == targetTab);
        //pnlTabCCTV.SetActive(pnlTabCCTV == targetTab);
    }
    private void OnPopupSetting(object obj)
    {
        transform.position = defaultPos;
        gameObject.SetActive(true);
        LoadAreaList();
    }
    private void OnCloseSetting()
    {
        gameObject.SetActive(false);
    }
    private void OnNavigateObs(object obj) 
    {
        if (obj is not int obsId) return;

        this.obsId = obsId; 
        
    }

    #endregion [Basic Function]

    #region [DropdownList]
    private void LoadAreaList()
    {
        //지역 정보 수신
        List<AreaData> areas = modelProvider.GetAreas();

        //ddl에 삽입
        ddlArea.ClearOptions();

        List<TMP_Dropdown.OptionData> dropdownItems = new();
        areas.ForEach(area => dropdownItems.Add(new(area.areaName)));
        ddlArea.AddOptions(dropdownItems);

        //기본 지역 설정
        OnSelectArea(0);
    }
    private void OnSelectArea(int idx)
    {
        areaId = idx + 1;
        LoadObs(areaId);
        ddlObs.value = 0;
    }
    private void LoadObs(int areaId)
    {
        //areaId를 통해 관측소 정보 수신
        List<ObsData> obss = modelProvider.GetObssByAreaId(areaId);

        //ddl에 삽입
        ddlObs.ClearOptions();

        List<TMP_Dropdown.OptionData> dropdownItems = new();
        obss.ForEach(obs => dropdownItems.Add(new(obs.obsName)));
        ddlObs.AddOptions(dropdownItems);
    }
    private void OnSelectObs(int idx)
    {
        //TODO
        //Dropdown 인덱스를 opsId로 변환하는 과정이 필요함. 
        //int obsId = idx + 1; XX
    }
    #endregion [DropdownList]

    #region [Obs Tab]

    //기존 PopupSettingItem이 가진 Button에 삽입될 예정
    private void OnToggleSensor(int sensorId, bool isUsing)
    {
        //Temporary Function
        UiManager.Instance.Invoke(UiEventType.CommitSensorUsing, (obsId, sensorId, isUsing));
    }
    private void OnToggleBoard(bool isToxinBoard, bool isFixing)
    {
        //Temporary Function
        UiManager.Instance.Invoke(UiEventType.CommitBoardFixing, (obsId, isToxinBoard, isFixing));
    }

    #endregion [Obs Tab]
    
    #region [Alarm Tab]


    private void OnAlarmSliderChanged(float value)
    {
        int selection = 4;
        int choosenIdx = Mathf.RoundToInt(value * (selection-1));

        float normalizedSliderValue = (float)choosenIdx / (float)(selection - 1);
        sldAlarmPopup.SetValueWithoutNotify(normalizedSliderValue);

        UiManager.Instance.Invoke(UiEventType.CommitPopupAlarmCondition, (ToxinStatus)choosenIdx);
    }





    private void OnChangeDbAddress(string dbAddress) 
    {
        PlayerPrefs.SetString("dbAddress", dbAddress);
        Option.url = dbAddress;
    }

    #endregion [Alarm Tab]

    #region [CCTV Tab]

    private void OnChangeCCTV(bool isOutdoor, string url)
    {
        //Temporary Function
        UiManager.Instance.Invoke(UiEventType.CommitCCTVUrl, (obsId, isOutdoor, url));
    }

    #endregion [CCTV Tab]





}