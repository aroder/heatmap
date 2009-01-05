using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace AdamRoderick.HeatMap
{
    public static class Util
    {
        public static void Log(string msg)
        {
            Debug.WriteLine(string.Format("{0}\t{1}", DateTime.Now.ToString(), msg));
        }
        public static void Log(Exception ex)
        {
            Log(ExceptionStringBuilder(ex));
        }

        private static string ExceptionStringBuilder(Exception ex)
        {
            return ExceptionStringBuilder(ex, 0);
        }
        /// <summary>
        /// Recursive method to build a full detail of the supplied exception and all inner exceptions.
        /// </summary>
        /// <param name="ex">The Exception to document.</param>
        /// <param name="TabLevel">The number of tab levels to prefix exception detail with. 
        /// When this method is called from outside itself this parameter should be zero.</param>
        /// <returns></returns>
        private static string ExceptionStringBuilder(Exception ex, int TabLevel)
        {
            string tabPrefix = string.Empty;
            for (int i = 0; i < TabLevel; i++)
            {
                tabPrefix += "\t";
            } // for
            System.Text.StringBuilder detail = new System.Text.StringBuilder();
            detail.AppendLine(tabPrefix + "EXCEPTION OCCURED AT " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());
            detail.AppendLine(tabPrefix + "\tMESSAGE");
            detail.AppendLine(tabPrefix + "\t-------");
            detail.AppendLine(tabPrefix + "\t" + ex.Message);
            detail.AppendLine();
            detail.AppendLine(tabPrefix + "\tSOURCE");
            detail.AppendLine(tabPrefix + "\t------");
            detail.AppendLine(tabPrefix + "\t" + ex.Source);
            detail.AppendLine();
            detail.AppendLine(tabPrefix + "\tSTACKTRACE");
            detail.AppendLine(tabPrefix + "\t----------");
            detail.AppendLine(tabPrefix + "\t" + ex.StackTrace);
            detail.AppendLine();
            if (ex.InnerException != null)
            {
                detail.AppendLine(tabPrefix + "\tINNER EXCEPTION");
                detail.AppendLine(tabPrefix + "\t---------------");
                detail.AppendLine(tabPrefix + "\t" + ex.InnerException.ToString());
                detail.AppendLine(tabPrefix + "-----------------------------------------------------");
                detail.AppendLine(tabPrefix + "-----------------------------------------------------");
                detail.AppendLine(string.Empty);
                detail.Append(ExceptionStringBuilder(ex.InnerException, ++TabLevel));
            } // if            
            detail.AppendLine(string.Empty);
            return detail.ToString();
        } // ExceptionStringBuilder

    }
}
