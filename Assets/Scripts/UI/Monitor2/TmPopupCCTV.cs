using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Onthesys;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using System;
using System.Net;
using UnityEngine.Networking;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using UMP;

public class TmPopupCCTV : MonoBehaviour
{
    public TMP_Text txtName;
    public UniversalMediaPlayer video;
    public GameObject loading;
    public ARVideoCanvasHelper gui;
    private ObsData data;

    void Start() {
        //DataManager.Instance.OnSelectCCTV.AddListener(this.OnSelectCCTV);
        this.gui.gameObject.SetActive(false);
        this.loading.SetActive(false);
    }

    public void OnSelectCCTV(ObsData area) {
        this.data = area;
        this.txtName.text = area.areaName + " - " + area.obsName;
        this.gui.gameObject.SetActive(true);
        this.video.AddBufferingEvent((progress)=>{
            if(progress == 100) this.loading.SetActive(false);
            else {
                this.gui.gameObject.SetActive(true);
                this.loading.SetActive(true);
            }
        });
    }

    public void OnVideoUp() {
        Debug.Log("up");
        SendPTZCommandAsync("up", 2, 1000);
    }

    public void OnVideoDown() {
        Debug.Log("down");
        SendPTZCommandAsync("down", 2, 1000);
    }

    public void OnVideoLeft() {
        Debug.Log("left");
        SendPTZCommandAsync("left", 2, 1000);
    }

    public void OnVideoRight() {
        Debug.Log("right");
        SendPTZCommandAsync("right", 2, 1000);
    }

    public void OnVideoIn() {
        Debug.Log("in");
        SendPTZCommandAsync("zoomin", 6, 1000);
    }

    public void OnVideoOut() {
        Debug.Log("out");
        SendPTZCommandAsync("zoomout", 6, 1000);
    }

    public void OnVideoEvent() {
        //Debug.Log("MediaPlayer " + mp.name + " generated event: " + eventType.ToString());
        
    }

    private async Task SendPTZCommandAsync(string direction, int speed, int timeout)
    {
        //string requestUrl = $"http://115.91.85.42:50081/httpapi/SendPTZ?action=sendptz&PTZ_CHANNEL=1&PTZ_MOVE={direction},{speed}&PTZ_TIMEOUT={timeout}";
        string requestUrl = $"http://192.168.1.109:50081/httpapi/SendPTZ?action=sendptz&PTZ_CHANNEL=1&PTZ_MOVE={direction},{speed}&PTZ_TIMEOUT={timeout}";
        //url2 = $"http://192.168.1.108:50080/httpapi/SendPTZ?action=sendptz&PTZ_CHANNEL=1&PTZ_MOVE={direction},{speed}&PTZ_TIMEOUT={timeout}";
        HttpClient client = new HttpClient();
        
            // 사용자 이름과 비밀번호 설정
            var byteArray = Encoding.ASCII.GetBytes("admin:HNS_qhdks_!Q@W3");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            try
            {
                // GET 요청
                HttpResponseMessage response = await client.GetAsync(requestUrl);

                // 예외 발생
                response.EnsureSuccessStatusCode();

                // 응답 본문 읽기
                string responseBody = await response.Content.ReadAsStringAsync();
                Debug.Log(responseBody);

                // 요청 성공 메시지
                //MessageBox.Show($"Command {direction} sent successfully.");
            }
            catch (HttpRequestException httpEx)
            {
                Debug.Log($"HttpRequestException: {httpEx.Message}");
                Console.WriteLine($"HttpRequestException: {httpEx.Message}");
            }
            catch (Exception ex)
            {
                Debug.Log($"Error sending PTZ command: {ex.Message}");
                Console.WriteLine($"Exception: {ex.Message}");
            }
        
        /*
        string requestUrl = $"http://115.91.85.42:50081/httpapi/SendPTZ?action=sendptz&PTZ_CHANNEL=1&PTZ_MOVE={direction},{speed}&PTZ_TIMEOUT={timeout}";
        UnityWebRequest www = UnityWebRequest.Get(requestUrl);
        www.SetRequestHeader("Authorization", "admin:HNS_qhdks_!Q@W3");
        www.SendWebRequest();

        var byteArray = Encoding.ASCII.GetBytes("admin:HNS_qhdks_!Q@W3");
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

        */
    }


}
