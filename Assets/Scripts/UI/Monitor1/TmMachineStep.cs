using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Onthesys;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using System;
using Newtonsoft.Json.Bson;

public class TmMachineStep : MonoBehaviour
{
    public TMP_Text txtArea;
    public OutlineCtrl outline;
    public List<GameObject> popups;
    public List<GameObject> steps;
    public List<GameObject> covers;
    public GameObject sensor;

    private int id;
    private int step;
    private bool isOnOver;
    void Start()
	{
        //DataManager.Instance.OnSelectArea.AddListener(OnSelectArea);
        //DataManager.Instance.OnSelectObs.AddListener(OnSelectObs);
        //DataManager.Instance.OnUpdateStep.AddListener(OnUpdateStep);
    }

    public void OnSelectArea(int areaid)
    {
        InitView();
    }

    public void OnSelectObs(int obsid)
    {
        id = obsid;
        txtArea.text = DataManager.Instance.GetObsNameById(id);
    }

    public void OnUpdateStep(int step) 
    {
        this.step = step;
        if (isOnOver)
            return;

        if (!covers[0].activeSelf)
        {
            if (this.outline.choiceStage != this.step)
            {
                this.outline.choiceStage = this.step;
                this.outline.SetCurrentStage(this.step);
                this.HidePopup();
                this.UpdatePopup(this.step - 1);
            }
        }
    }

    public void InitView()
    {
        covers.ForEach(t => t.SetActive(true));
        popups.ForEach(t => t.SetActive(false));
        steps.ForEach(t => t.SetActive(false));
        sensor.SetActive(false);
        outline.ResetAllOutlines();
        isOnOver = covers[1].gameObject.activeSelf;
    }

    public void ShowCorver()
    {
        if (covers[1].gameObject.activeSelf)
        {
            covers[1].gameObject.SetActive(false);
        }
        else
        {
            InitView();
        }
        isOnOver = covers[1].gameObject.activeSelf;
    }

    public void ShowPopup()
    {
        if (isOnOver)
            return;

        HidePopup();
        if (covers[0].gameObject.activeSelf == false)
        {
            this.UpdatePopup(step - 1);
        }
    }

    public void UpdatePopup(int idx)
    {
        popups[idx].SetActive(true);
        steps[idx].SetActive(true);
    }

    public void HidePopup()
    {
        popups.ForEach(popup => popup.SetActive(false));
        steps.ForEach(step => step.SetActive(false));
    }

}
