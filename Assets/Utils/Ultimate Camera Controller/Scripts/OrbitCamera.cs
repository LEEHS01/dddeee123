using System;
using System.Collections.Generic;
using DG.Tweening;
using Onthesys;
using UnityEngine;
using UnityEngine.EventSystems;

public class OrbitCamera : MonoBehaviour
    {
        public GameObject target;
        public float distance;
        public float distanceMax;
        public float distanceMin;

        public float xSpeed = 250.0f;
        public float ySpeed = 120.0f;

        public float yMinLimit = -20;
        public float yMaxLimit = 80;

        float x = 0.0f;
        float y = 0.0f;

        public List<GameObject> deactiveTargets;

        void Awake() {
            
        }

        void Start()
        {
            //DataManager.Instance.OnSelectArea.AddListener(this.ResetPos);
            var angles = transform.eulerAngles;
            x = angles.y;
            y = angles.x;
        }

        public void ResetPos(int idx)
        {
            this.transform.localPosition = new Vector3(-12, 19, -33);
            this.transform.rotation = Quaternion.Euler(12, 2.5f, 0);
            var angles = transform.eulerAngles;
            x = angles.y;
            y = angles.x;
            this.distance = distanceMax;
        }

        float prevDistance;

        void LateUpdate()
        {
            if(EventSystem.current.IsPointerOverGameObject()) return;

            if (distance < 2) distance = 2;
            distance -= Input.GetAxis("Mouse ScrollWheel") * 10;
            if(distance < this.distanceMin) distance = this.distanceMin;
            if(distance > this.distanceMax) distance = this.distanceMax;
            if (target && (Input.GetMouseButton(0) || Input.GetMouseButton(1)))
            {
                var pos = Input.mousePosition;
                var dpiScale = 1f;
                if (Screen.dpi < 1) dpiScale = 1;
                if (Screen.dpi < 200) dpiScale = 1;
                else dpiScale = Screen.dpi / 200f;

                if (pos.x < 380 * dpiScale && Screen.height - pos.y < 250 * dpiScale) return;

                x += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
                y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

                y = ClampAngle(y, yMinLimit, yMaxLimit);
                var rotation = Quaternion.Euler(y, x, 0);
                var position = rotation * new Vector3(0.0f, 0.0f, -distance) + target.transform.position;
                transform.rotation = rotation;
                transform.position = position;
                this.DeactiveTargets();   
            } else {
                this.ActiveTargets();
            }

            if (Math.Abs(prevDistance - distance) > 0.001f)
            {
                prevDistance = distance;
                var rot = Quaternion.Euler(y, x, 0);
                var po = rot * new Vector3(0.0f, 0.0f, -distance) + target.transform.position;
                transform.rotation = rot;
                transform.position = po;
            }
        }

        static float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360)
                angle += 360;
            if (angle > 360)
                angle -= 360;
            return Mathf.Clamp(angle, min, max);
        }

        private void DeactiveTargets() {
            for(int i = 0; i < this.deactiveTargets.Count; i++) {
                this.deactiveTargets[i].SetActive(false);
            }
        }

        private void ActiveTargets() {
            for(int i = 0; i < this.deactiveTargets.Count; i++) {
                this.deactiveTargets[i].SetActive(true);
            }
        }
    }
