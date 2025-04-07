using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Onthesys
{
    public class LogData
    {
        public int obsId;
        public int boardId;
        public DateTime time;
        public string areaName;
        public string obsName;
        public int hnsId;
        public string hnsName;
        public int status;
        public float? value;
        public int idx;

        public LogData(int obsid, int boardid, string areaName, string obsName, int hnsId, string hnsName, DateTime dt, int status, float? val, int idx)
        {
            this.obsId = obsid;
            this.boardId = boardid;
            this.areaName = areaName;
            this.obsName = obsName;
            this.hnsId = hnsId;
            this.hnsName = hnsName;
            this.time = dt;
            this.status = status;
            this.value = val;
            this.idx = idx;
        }
    }

}


