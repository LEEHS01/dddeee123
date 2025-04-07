using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Onthesys;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;

public class TmFollowToggle : MonoBehaviour
{
    public Toggle toggle; 
    public bool nagative;
    public GameObject Cover;
    public List<GameObject> followTargets;
    
    void Start() { 
        toggle.onValueChanged.AddListener(this.OnChangeValue);
    }

    void OnChangeValue(bool on) 
    {
        if (Cover.activeSelf)
            return;

        for(int i = 0; i < this.followTargets.Count; i++) 
        {
            if(this.nagative) 
                this.followTargets[i].SetActive(!on);
            else 
                this.followTargets[i].SetActive(!on); 
        }
    }
}
