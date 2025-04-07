using Onthesys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class PopupAlarmFactory : MonoBehaviour
{
    GameObject popupAlarmPrefab;
    List<PopupAlarm> popupAlarms = new();

    private void Start()
    {
        popupAlarmPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefab/MonitorA/PopupAlarm.prefab");
    }

    private void Update()
    {
        //DEBUG!
        if (Input.GetKeyDown(KeyCode.F3))
        {
            CreatePopupAlarm(new AlarmLogModel()
            {
                alacode = 2,
                aladt = DateTime.Now.ToString(),
                areanm = "인천",
                obsnm = "능내리",
                hnsnm = "클로로포름",
                currval = 105,
                alahihival = 100,

            });

        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            CreatePopupAlarm(new AlarmLogModel()
            {
                alacode = 1,
                aladt = DateTime.Now.ToString(),
                areanm = "인천",
                obsnm = "능내리",
                hnsnm = "클로로포름",
                currval = 105,
                alahihival = 100,

            });

        }
        if (Input.GetKeyDown(KeyCode.F1))
        {
            CreatePopupAlarm(new AlarmLogModel()
            {
                alacode = 0,
                aladt = DateTime.Now.ToString(),
                areanm = "인천",
                obsnm = "능내리",
                hnsnm = "클로로포름",
                currval = 105,
                alahihival = 100,

            });

        }
        
        
        //모든 ㅍ팝업 닫기
        if (Input.GetKeyDown(KeyCode.F10))
        {
            popupAlarms.ForEach(pa => Destroy(pa.gameObject));
            popupAlarms.Clear();
        }
    }



    void CreatePopupAlarm(List<AlarmLogModel> alarmLogs) => alarmLogs.ForEach(al => CreatePopupAlarm(al));
    void CreatePopupAlarm(AlarmLogModel alarmLog)
    {
        GameObject newPopup = Instantiate(popupAlarmPrefab, transform);
        PopupAlarm newPopupComp = newPopup.GetComponent<PopupAlarm>();
        {
            newPopupComp.InitAlarmLog(alarmLog);
            newPopupComp.transform.parent = transform;
            newPopupComp.transform.localPosition = Vector2.zero;
        }

        // 화면 경계 계산
        RectTransform canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        float halfWidth = canvasRect.rect.width / 2;
        float halfHeight = canvasRect.rect.height / 2;

        //이전 팝업들과 겹치지 않는 자리를 찾을 때까지 반복
        bool isPlaced = true;
        do {
            isPlaced = true;

            //기존 팝업들과 비교해서
            foreach (PopupAlarm popupAlarm in popupAlarms)
            {
                float distance = Vector3.Distance(newPopup.transform.localPosition, popupAlarm.transform.localPosition);
                //너무 가깝다면
                if (distance < 10f)
                {
                    newPopup.transform.localPosition += Vector3.one * 15f;
                    isPlaced = false;

                    // 화면 경계를 넘으면 반대편으로 이동
                    Vector3 localPos = newPopup.transform.localPosition;

                    if (localPos.x > halfWidth) localPos.x = -halfWidth;
                    else if (localPos.x < -halfWidth) localPos.x = halfWidth;

                    if (localPos.y > halfHeight) localPos.y = -halfHeight;
                    else if (localPos.y < -halfHeight) localPos.y = halfHeight;

                    break;
                }
            }
        } while (!isPlaced);


        popupAlarms.Add(newPopupComp);
    }
    public void RemovePopupAlarm(PopupAlarm popupAlarm) => popupAlarms.Remove(popupAlarm);

}
