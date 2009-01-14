using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdamRoderick.HeatMap
{
    public abstract class Encoder : IEncoder
    {
        protected abstract string EncodingString{ get; }

        public virtual string Encode(HeatPoint[] points)
        {
            int len = EncodingString.Length;
            List<char> chars = new List<char>();
            foreach (HeatPoint p in points)
            {
                int position = p.X * EncodingString.Length + p.Y;
                char char1 = EncodingString[((int)Math.Floor((double)position / len))];
                char char2 = EncodingString[position % len];
                chars.Add(char1);
                chars.Add(char2);
            }
            return new string(chars.ToArray());
        }

        public virtual HeatPoint[] Decode(string encodedString)
        {
            List<HeatPoint> retval = new List<HeatPoint>();
            for (var i = 0; i < encodedString.Length; i += 2)
            {
                char char1 = encodedString[i];
                char char2 = encodedString[i + 1];
                int x = EncodingString.IndexOf(char1);
                int y = EncodingString.IndexOf(char2);
                retval.Add(new HeatPoint(x, y, 0));
            }

            return retval.ToArray();
        }

    }
}
