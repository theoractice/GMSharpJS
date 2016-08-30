using EdgeJs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GMSharpJS
{
    class NativeMethods
    {
        [DllImport("GMAPIProxy.dll", EntryPoint = "gm_gmlproxy", CallingConvention = CallingConvention.Cdecl)]
        internal static extern double GMLProxy(double gml, string msg);

        [DllImport("GMAPIProxy.dll", EntryPoint = "gm_setinstance", CallingConvention = CallingConvention.Cdecl)]
        internal static extern double SetInstance(); 
    }
}
