using Onthesys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEditor;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditorInternal.ReorderableList;

public class PopupSetting : MonoBehaviour
{
    Button btnClose;

    GameObject pnlTabObs, pnlTabAlarm, pnlTabCCTV;
    Button btnTabObs, btnTabAlarm, btnTabCCTV;
    TMP_Dropdown ddlArea, ddlObs;

    //TabObs
    Toggle tglBoardToxin, tglBoardChemical;

    TMP_Text lblPositionValue;
    List<PopupSettingItem> visibilityItems;

    //TabAlarm
    Dictionary<ToxinStatus,Toggle> tglAlarmPopup;

    //TabCCTV
    Dictionary<CCTVUrlType, TMP_InputField> txtCCTV;

    //선택한 지역 값
    int areaId, obsId;

    //팝업 시 사용할 기본 위치
    Vector3 defaultPos;

    public enum CCTVUrlType
    {
        Equipment,
        Outdoor,
        ControlUp,
        ControlDown,
        ControlLeft,
        ControlRight,

    }

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
                btnTabCCTV = conTabButtons.Find("btnCCTV").GetComponent<Button>();
                btnTabCCTV.onClick.AddListener(() => OnOpenTab(pnlTabCCTV));
            }

            Transform conTabPanels = transform.Find("conTabPanels");
            {
                pnlTabObs = conTabPanels.Find("tabObs").gameObject;
                pnlTabAlarm = conTabPanels.Find("tabAlarm").gameObject;
                pnlTabCCTV = conTabPanels.Find("tabCCTV").gameObject;
            }

            Transform conSelectObs = transform.Find("conSelectObs");
            {
                ddlArea = conSelectObs.Find("ddlArea").GetComponent<TMP_Dropdown>();
                ddlArea.onValueChanged.AddListener(OnSelectArea);
                ddlObs = conSelectObs.Find("ddlObs").GetComponent<TMP_Dropdown>();
                ddlObs.onValueChanged.AddListener(OnSelectObs);
            }
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

            Transform obsInspection = pnlTabObs.transform.Find("ObsInspection");
            lblPositionValue = obsInspection.Find("lblPositionValue").GetComponent<TMP_Text>();
            //txtCCTVEquipment = obsInspection.Find("txtCCTVEquipment").GetComponent<TMP_InputField>();
            //txtCCTVOutdoor = obsInspection.Find("txtCCTVOutdoor").GetComponent<TMP_InputField>();

            visibilityItems = transform.GetComponentsInChildren<PopupSettingItem>().ToList();

            for (int i = 0; i < visibilityItems.Count; i++)
                visibilityItems[i].SetItem(i, "독성도", true);
        }

        //tabAlarm 구성요소
        {
            Transform toggleAlarmPopup = pnlTabAlarm.transform.Find("ToggleAlarmPopup");
            tglAlarmPopup = new() {
                {ToxinStatus.Yellow,    toggleAlarmPopup.Find("Serious")        .GetComponentInChildren<Toggle>() },
                {ToxinStatus.Red,       toggleAlarmPopup.Find("Warning")        .GetComponentInChildren<Toggle>() },
                {ToxinStatus.Purple,    toggleAlarmPopup.Find("Malfunction")    .GetComponentInChildren<Toggle>() },
            };

        }

        //tabCCTV 구성요소
        {
            Transform conCCTVUrls = pnlTabCCTV.transform.Find("conCCTVUrls");
            txtCCTV = new() {
                {CCTVUrlType.Equipment,     conCCTVUrls.Find("lblStreamEquipment")   .GetComponentInChildren<TMP_InputField>()},
                {CCTVUrlType.Outdoor,       conCCTVUrls.Find("lblStreamEquipment")   .GetComponentInChildren<TMP_InputField>()},
                {CCTVUrlType.ControlUp,     conCCTVUrls.Find("lblControlUp")        .GetComponentInChildren<TMP_InputField>()},
                {CCTVUrlType.ControlDown,   conCCTVUrls.Find("lblControlDown")      .GetComponentInChildren<TMP_InputField>()},
                {CCTVUrlType.ControlLeft,   conCCTVUrls.Find("lblControlLeft")      .GetComponentInChildren<TMP_InputField>()},
                {CCTVUrlType.ControlRight,  conCCTVUrls.Find("lblControlRight")     .GetComponentInChildren<TMP_InputField>()},
            };
        }

        UiManager.Instance.Register(UiEventType.PopupSetting, OnPopupSetting);

        btnClose = transform.Find("Btn_Close").GetComponent<Button>();
        btnClose.onClick.AddListener(OnCloseSetting);

        defaultPos = transform.position;

        OnOpenTab(this.pnlTabObs);
        gameObject.SetActive(false);

        //Debug!
        UiManager.Instance.Register(UiEventType.CommitSensorUsing, tuple => Debug.LogWarning("CommitSensorUsing occured : " + tuple));
    }
    
    #region [Basic Function]
    private void OnOpenTab(GameObject targetTab)
    {
        Sprite sprTabOn = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Arts/UI/UIAssets/Textures/Components/Form/Btn_Search_p.png");
        Sprite sprTabOff = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Arts/UI/UIAssets/Textures/Components/Form/Btn_Search_n.png");

        btnTabObs   .GetComponentInChildren<Image>().sprite = pnlTabObs == targetTab? sprTabOn : sprTabOff;
        btnTabAlarm .GetComponentInChildren<Image>().sprite = pnlTabAlarm == targetTab? sprTabOn : sprTabOff;
        btnTabCCTV  .GetComponentInChildren<Image>().sprite = pnlTabCCTV == targetTab? sprTabOn : sprTabOff;

        pnlTabObs.SetActive(pnlTabObs == targetTab);
        pnlTabAlarm.SetActive(pnlTabAlarm == targetTab);
        pnlTabCCTV.SetActive(pnlTabCCTV == targetTab);
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

        //DEBUG
        this.obsId = obsId; 
    }

    #endregion [Basic Function]

    #region [DropdownList]
    private void LoadAreaList()
    {
        //TODO

        //지역 정보 수신
        List<object> areaList = new();

        //ddl에 삽입
        ddlArea.ClearOptions();
        ddlArea.AddOptions(new List<TMP_Dropdown.OptionData>()
        {
            new("인천"),
            new("평택"),
            new("평택"),
            new("평택"),
            new("평택"),

            new("평택"),
            new("평택"),
            new("평택"),
            new("평택"),
            new("평택"),
        });

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
        //TODO

        //areaId를 통해 관측소 정보 수신
        List<object> obsList = new();

        //ddl에 삽입
        ddlObs.ClearOptions();
        ddlObs.AddOptions(new List<TMP_Dropdown.OptionData>()
        {
            new("능내리"),
            new("A동"),
            new("B동"),
        });

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

    private void OnTogglePopup(ToxinStatus status, bool isOn)
    {
        //Temporary Function
        UiManager.Instance.Invoke(UiEventType.CommitPopupToggle, (status, isOn));
    }


    #endregion [Alarm Tab]
    
    #region [CCTV Tab]

    private void OnChangeCCTV(CCTVUrlType urlType, string url)
    {
        //Temporary Function
        UiManager.Instance.Invoke(UiEventType.CommitCCTVUrl, (obsId, urlType, url));
    }

    #endregion [CCTV Tab]
}