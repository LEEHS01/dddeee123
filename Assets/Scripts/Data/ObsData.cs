using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;
using UnityEngine.PlayerLoop;
//using static UnityEditor.Progress;
//using static UnityEngine.Rendering.DebugUI;

namespace Onthesys
{
    public class ObsData
    {
        public int id;
        public AreaData.AreaType type;
        public int areaId;
        public string areaName;
        public string obsName;
        public int step;

        public string src_video1 = "rtsp://admin:HNS_qhdks_!Q@W3@115.91.85.42/video1?profile=high";
        public string src_video2 = "rtsp://admin:HNS_qhdks_!Q@W3@115.91.85.42/video1?profile=high";
        public string src_video_up = "";
        public string src_video_down = "";
        public string src_video_left = "";
        public string src_video_right = "";

        public ObsData(string mapName, int areaidx, string areaName, AreaData.AreaType type, int id)
        {
            this.areaName = mapName;
            this.areaId = areaidx;
            this.obsName = areaName;
            this.type = type;
            this.id = id;
            this.step = UnityEngine.Random.Range(0, 5);
        }

        private void UpdateStep(string step)
        {
            if (step != null)
            {
                switch (step.Trim())
                {
                    case "0020":
                        this.step = 1;
                        break;
                    case "0021":
                        this.step = 2;
                        break;
                    case "0023":
                        this.step = 3;
                        break;
                    case "0024":
                        this.step = 4;
                        break;
                    case "0025":
                        this.step = 5;
                        break;
                    default:
                        this.step = 5;
                        break;
                }
            }
        }
    }


    public enum ToolStatus
    {
        STAY_0,
        START_1,
        PRE_2,
        wORK_3,
        WASH_4
    }

    
}


