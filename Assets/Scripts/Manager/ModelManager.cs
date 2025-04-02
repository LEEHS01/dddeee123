using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Manager
{
    internal class ModelManager
    {
        public class AlarmLogModel
        {
            public int alarmId { get; set; }
            public string alarmAt { get; set; }
            public string obsName { get; set; }
            public string areaName { get; set; }
            public int sensorId { get; set; }
            public int obsId { get; set; }
            public int boardId { get; set; }
            public float? seriousThreshold { get; set; }
            public float? warningThreshold { get; set; }
            public float? current { get; set; }
            public string sensorName { get; set; }
            public string turnOffFlag { get; set; }
            public int alarmCode { get; set; }
        }

        public class AlarmMontlyModel
        {
            public string areaName;
            public int alarmCount;
        }

        public class AlarmSummaryModel
        {
            public int obsId;
            public int month;
            public int alarmCount;
        }

        public class AlarmYearlyModel
        {
            public string areaName;
            public int malfunctionCount;
            public int warningCount;
            public int seriousCount;
        }

        public class ChartDataModel
        {
            public string musureAt;
            public int? hnsidx;
            public int? obsidx;
            public int? boardidx;

            public float sensorValue;   //Value
            public float aiPrediction;
        }

        public class CurrentDataModel
        {
            public int sensorId;
            public int boardId;
            public string statusCode;
            public float? current;
            public float warningThreshold;
            public float seriousThreshold;
            public string isUsing;
            public string isFixed;
        }

        public class HnsResourceModel
        {
            public int sensorId;
            public int obsId;
            public int boardId;
            public string sensorName;
            public string isUsing;
            public float? seriousThreshold;
            public float? warningThreshold;
            public float? warningDuration;
            public string isFixed;
        }

        public class ObservatoryModel
        {
            public string areaName;
            public int areaId;
            public string obsName;
            public int areaType;
            public int obsId;
        }

        public class ObsSensorStepModel
        {
            public int obsId;
            public string toxinStep;
            public string chemiStep;
        }

    }

    
}
