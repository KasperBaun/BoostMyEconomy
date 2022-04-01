using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BmeModels
{
    public class ChartData
    {
        public double[] Data { get; set; } = new double[10];
        public string[] Labels { get; set; } = new string[10];
    }
}
