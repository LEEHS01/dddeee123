using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;


public class MapArea : MonoBehaviour 
{
    
    /// <summary>
    /// 선택한 지역 ID, NavigateArea이벤트에 따라 변화함.
    /// </summary>
    public int areaId = -1;

    List<List<Vector3>> obssPos;
    List<string> areaTextureRoots;

    List<MapAreaMarker> areaMarkers;
    Image imgArea;



    private void Start()
    {
        UiManager.Instance.Register(UiEventType.NavigateHome, OnNavigateHome);
        UiManager.Instance.Register(UiEventType.NavigateArea, OnNavigateArea);
        UiManager.Instance.Register(UiEventType.NavigateObs, OnNavigateObs);

        areaMarkers = transform.Find("MarkerList").GetComponentsInChildren<MapAreaMarker>(true).ToList();
        imgArea = transform.Find("MapMask").Find("MapImage").GetComponent<Image>();

        string prefabPath = "Assets/Prefab/MonitorA/MapAreaLoadout.prefab";
        obssPos = GetObssPos(prefabPath);
        if (obssPos == null)
            throw new Exception("MapAreaMarker - GetObssPos() returned null!");

        areaTextureRoots = new() {
            "Assets/Arts/Image/GyeongGi.png",
            "Assets/Arts/Image/GangWon.png",
            "Assets/Arts/Image/GyeongGi.png",
            "Assets/Arts/Image/GangWon.png",
            "Assets/Arts/Image/GyeongGi.png",

            "Assets/Arts/Image/GyeongGi.png",
            "Assets/Arts/Image/GyeongGi.png",
            "Assets/Arts/Image/GyeongGi.png",
            "Assets/Arts/Image/GyeongGi.png",
            "Assets/Arts/Image/GyeongGi.png",
        };


        //DEBUG
        for (int i = 0; i < areaMarkers.Count; i++)
        {
            MapAreaMarker areaMarker = areaMarkers[i];
            areaMarker.SetObsData(i + 1, areaMarker.obsName, areaMarker.status);
        }


        gameObject.SetActive(false);
    }


    private void OnNavigateArea(object obj)
    {
        if (obj is int newAreaId) 
        {
            areaId = newAreaId;
            SetByAreaId(areaId);
            this.gameObject.SetActive(true);
        }
    }
    private void OnNavigateHome(object obj)
    {
        this.gameObject.SetActive(false);
    }
    private void OnNavigateObs(object obj)
    {
        this.gameObject.SetActive(false);
    }

    bool SetByAreaId(int areaId) 
    {
        int areaIdx = areaId - 1;
        try
        {
            //지역 배경 설정
            imgArea.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(areaTextureRoots[areaIdx]);

            //지역 마커 설정
            for (int i = 0; i < areaMarkers.Count; i++)
            {
                var areaMarker = areaMarkers[i];
                areaMarker.transform.position = this.transform.position + obssPos[areaIdx][i];
            }

            //서브 타이틀 설정
            //TODO

            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"MapArea - SetByAreaId() Failed! areaId : {areaId}");
            Debug.LogException(ex);
            return false;
        }
    }
    List<List<Vector3>> GetObssPos(string prefabPath)
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

        List<List<Vector3>> list = new List<List<Vector3>>();

        foreach (Transform child in prefab.transform)
        {
            List<Vector3> areaNode = new();
            //Debug.Log("Area : " + child.name);
            foreach (Transform grandChild in child)
            {
                //Debug.Log("obs : " + grandChild.position.ToString());
                areaNode.Add(grandChild.position);
            }
            list.Add(areaNode);
        }

        return list;
    }
}
