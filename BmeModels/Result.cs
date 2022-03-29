using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BmeModels
{
    public class Result
    {
        public string Month { get; set; } = String.Empty;
        public double Income { get; set; }
        public double Expenses { get; set; }
        public double MonthResult { get; set; }
        public double MonthResultAcc { get; set; }
    }
}
