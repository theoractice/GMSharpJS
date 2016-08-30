using EdgeJs;
using RGiesecke.DllExport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GMSharpJS
{
    class AsyncJS
    {
        class GmlEvent
        {
            internal double gml;
            internal string param;
        }

        static List<GmlEvent> evtList = new List<GmlEvent>();

        [DllExport("event_invoke_async", CallingConvention = CallingConvention.Cdecl)]
        public static string Invoke(double gml, string param)
        {
            lock (evtList)
            {
                evtList.Add(new GmlEvent { gml = gml, param = param });
            }
            return "true";
        }

        [DllExport("event_do_sync", CallingConvention = CallingConvention.Cdecl)]
        public static string DoEvents()
        {
            foreach (var evt in evtList)
            {
                NativeMethods.GMLProxy(evt.gml, evt.param);
            }
            return "true";
        }

        [DllExport("event_clear", CallingConvention = CallingConvention.Cdecl)]
        public static string ClearEvents()
        {
            lock (evtList)
            {
                evtList.Clear();
            }
            return "true";
        }

        static readonly object @lock = new object();

        [DllExport("monitor_enter", CallingConvention = CallingConvention.Cdecl)]
        public static string MonitorEnter()
        {
            Monitor.Enter(@lock);
            return "true";
        }

        [DllExport("monitor_exit", CallingConvention = CallingConvention.Cdecl)]
        public static string MonitorExit()
        {
            Monitor.Exit(@lock);
            return "true";
        }

        [DllExport("timer", CallingConvention = CallingConvention.Cdecl)]
        public static string JSTimer(double interval, double gml, string msg)
        {
            Task.Run(() => JSFunctions.jsFunc(new
            {
                directive = "Timer",
                interval,
                gml,
                onMessage = (Func<object, Task<object>>)(async (e) =>
                {
                    NativeMethods.GMLProxy(gml, msg);
                    return "true";
                })
            }));
            return "true";
        }
    }
}
