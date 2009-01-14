using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using AdamRoderick.HeatMap;
using System.Text;

namespace CenStatsHeatMap.UI
{
    public partial class _Default : Page
    {
        protected void Button1_Click(object sender, EventArgs e)
        {
            string[] pixelPoints = ToEncodeTextBox.Text.Split(new char[] {','});
            List<HeatPoint> points = new List<HeatPoint>();
            foreach (string p in pixelPoints)
            {
                string[] coords = p.Split(new char[] {'x'});
                points.Add(new HeatPoint(int.Parse(coords[0]), int.Parse(coords[1]), 0));
            }

            
            //Google Simple
            IEncoder enc2 = new ExtendedEncoder();
            try
            {
                MyExtendedEncodedLabel.Text = enc2.Encode(points.ToArray());
            }
            catch (IndexOutOfRangeException ex) {
                MyExtendedEncodedLabel.Text = "Too big"; 
            }

            StringBuilder sb2 = new StringBuilder();
            foreach (HeatPoint hp in enc2.Decode(ToDecodeTextBox.Text)) this.AppendToStringBuilder(sb2, hp);
            MyExtendedDecodedLabel.Text = sb2.ToString();


            
            //Google Extended
            IEncoder enc = new GoogleExtendedEncoder();
            try
            {
                GoogleExtendedEncodedLabel.Text = enc.Encode(points.ToArray());
            } catch (IndexOutOfRangeException ex)
            {
                GoogleExtendedEncodedLabel.Text = "Too big";
            }

            StringBuilder sb = new StringBuilder();
            foreach (HeatPoint hp in enc.Decode(ToDecodeTextBox.Text)) this.AppendToStringBuilder(sb, hp);
            GoogleExtendedDecodedLabel.Text = sb.ToString();
        }

        private void AppendToStringBuilder(StringBuilder sb, HeatPoint hp)
        {
            sb.Append("[");
            sb.Append(hp.X.ToString());
            sb.Append("x");
            sb.Append(hp.Y.ToString());
            sb.Append("]");
            sb.Append(",");            
        }
    }
}
