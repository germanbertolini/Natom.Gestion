using AspNetCore.Reporting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Natom.Extensions
{
    public static class NetCoreReportingExtensions
    {
        public static void EnableExternalImages(this LocalReport report)
        {
            BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
            FieldInfo field = report.GetType().GetField("localReport", bindFlags);
            object rptObj = field.GetValue(report);
            Type type = rptObj.GetType();
            PropertyInfo pi = type.GetProperty("EnableExternalImages");
            pi.SetValue(rptObj, true, null);
        }
    }
}
