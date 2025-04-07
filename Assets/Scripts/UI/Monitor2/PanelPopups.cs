using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Onthesys;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;

public class PanelPopups : MonoBehaviour
{
    public List<GameObject> popups;

    void Start() {
        this.popups.ForEach(i=>i. SetActive(true));

        DOVirtual.DelayedCall(0.1f,
            () => this.popups.ForEach(i=>i. SetActive(false)));
    }
}
