using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BmeModels
{
    public class TableItem
    {
        public string Name { get; set; } =  string.Empty;
        public double Value { get; set; }
        public string IconString { get; set; }
    public override string ToString()
    {
            return Name + "U+0020(" + Value + ")" + IconString;

    }
    }
}
