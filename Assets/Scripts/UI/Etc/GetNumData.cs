using UnityEngine;
using TMPro;
using System.Linq;
using Onthesys;
using DG.Tweening;
using System.Collections.Generic;

public class GetNumData : MonoBehaviour
{
    public GameObject threeCirclePercentObject; // TMThreeCirclePercent를 가지고 있는 게임 오브젝트
    public TextMeshProUGUI label; // 값을 표시할 TextMeshProUGUI 컴포넌트
    public int orderNumber; // 1에서 5 사이의 숫자
    public int displayType; // 1, 2 또는 3 입력 (1: 1의 자릿수, 2: 10의 자릿수, 3: 100의 자릿수)

    private TMThreeCirclePercent threeCirclePercent;

    void Start()
    {
        // TMThreeCirclePercent 컴포넌트를 가져옵니다.
        threeCirclePercent = threeCirclePercentObject.GetComponent<TMThreeCirclePercent>();
        threeCirclePercent.OnUpdateValue.AddListener(this.UpdateLabelData);

        // orderNumber가 1에서 5 사이인지 확인합니다.
        if (orderNumber < 1 || orderNumber > 5)
        {
            Debug.LogError("Order number must be between 1 and 5.");
        }

        // displayType이 1, 2 또는 3인지 확인합니다.
        if (displayType < 1 || displayType > 3)
        {
            Debug.LogError("Display type must be either 1, 2, or 3.");
        }

        //DataManager.Instance.OnUpdate.AddListener(this.UpdateLabelData);

        //DataManager.Instance.OnAlarmUpdate.AddListener(this.UpdateLabelData);
    }


    void UpdateLabelData(/*List<LogData> alarmDatas*/)
    {
        // TMThreeCirclePercent로부터 값을 가져옵니다.
        //var values = new float[] { threeCirclePercent.no1, threeCirclePercent.no2, threeCirclePercent.no3, threeCirclePercent.no4, threeCirclePercent.no5 };
        var values = threeCirclePercent.GetValues();

        // 값과 인덱스의 쌍을 생성합니다.
        var valueIndexPairs = values.Select((value, index) => new { Value = value, Index = index + 1 }).ToList();

        // 값을 내림차순으로 정렬합니다.
        var sortedValueIndexPairs = valueIndexPairs.OrderByDescending(pair => pair.Value).ToList();

        // orderNumber에 해당하는 값을 선택합니다.
        var selectedValue = sortedValueIndexPairs[orderNumber - 1].Value;

        label.text = "" + Mathf.Floor(selectedValue / Mathf.Pow(10, displayType - 1)) % 10;


    }

    public void ForcedUpdateView(int selectedValue)
    {
        label.text = "" + Mathf.Floor(selectedValue / Mathf.Pow(10, displayType - 1)) % 10;
    }
}
