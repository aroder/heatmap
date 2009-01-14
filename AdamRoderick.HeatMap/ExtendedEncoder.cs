using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdamRoderick.HeatMap
{
    public class ExtendedEncoder : Encoder
    {
        protected override string EncodingString
        {
            get { return "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-.`~!@#$%^&*()-_=+[{]};:<>?|"; }
        }
    }
}
