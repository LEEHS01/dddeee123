using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Onthesys;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine.Events;

public class TMMeshColor : MonoBehaviour
{
    private List<Color> originColors = new List<Color>();
    public Color color;
    public MeshRenderer mesh;

    public UnityEvent OnMouseClick;

    void Start() {
        for(int j = 0; j < mesh.materials.Length; j++) {
            this.originColors.Add(mesh.materials[j].color);
        }
    }

    void OnMouseEnter() {
        for(int j = 0; j < mesh.materials.Length; j++) {
            mesh.materials[j].color = color;
        }
    }

    void OnMouseExit()
    {
        for(int j = 0; j < mesh.materials.Length; j++) {
            mesh.materials[j].color = this.originColors[j];
        }
    }

    void OnMouseUp()
    {
        this.OnMouseClick.Invoke();
    }
}
