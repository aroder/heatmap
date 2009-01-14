using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdamRoderick.HeatMap
{
    public interface IEncoder
    {
        string Encode(HeatPoint[] points);
        HeatPoint[] Decode(string encodedString);
    }
}
