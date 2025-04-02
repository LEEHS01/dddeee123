using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Onthesys;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine.Events;

public class TMSettingListItem : MonoBehaviour
{
    public int id;
    public ToxinData data;
    public Toggle checkbox;
    public TMP_Text txtName;
    public TMP_InputField txtHi;
    public TMP_InputField txtHihi;
    public TMP_InputField txtDuration;
    public Toggle toggle;

    void Start()
	{
        
    }
}
