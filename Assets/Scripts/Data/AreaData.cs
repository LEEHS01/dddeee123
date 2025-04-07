using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class AreaData
{
    public int areaIdx;
    public string areaName;
    public AreaType areaType;


    public enum AreaType
    {
        Ocean,
        Nuclear
    }
}
