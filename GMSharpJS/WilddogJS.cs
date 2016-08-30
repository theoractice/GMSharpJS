using EdgeJs;
using RGiesecke.DllExport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GMSharpJS
{
    /// <summary>
    /// 由于C#制作dll导出函数的特性，目前还是把全部方法写在同一个类里。
    /// </summary>
    class WilddogJS
    {
        class MessageContext
        {
            internal string path;
            internal string type;
            internal string msg;
        }

        static Queue<MessageContext> msgQueue = new Queue<MessageContext>();

        [DllExport("wilddog_init", CallingConvention = CallingConvention.Cdecl)]
        public static string Init(string appId)
        {
            Task.Run(() => JSFunctions.jsFunc(new { directive = "Init", appId })).Wait();
            return "true";
        }

        [DllExport("wilddog_set", CallingConvention = CallingConvention.Cdecl)]
        public static string Set(string path, string data)
        {
            Task.Run(() => JSFunctions.jsFunc(new { directive = "Set", path, data }));
            return "true";
        }

        [DllExport("wilddog_update", CallingConvention = CallingConvention.Cdecl)]
        public static string WilddogUpdate(string path, string data)
        {
            Task.Run(() => JSFunctions.jsFunc(new { directive = "Update", path, data }));
            return "true";
        }

        [DllExport("wilddog_subscribe", CallingConvention = CallingConvention.Cdecl)]
        public static string Subscribe(string path, string type)
        {
            Task.Run(() =>
            {
                JSFunctions.jsFunc(new
                {
                    directive = "Subscribe",
                    path = path,
                    type = type,
                    onMessage = (Func<object, Task<object>>)(async (e) =>
                    {
                        try
                        {
                            dynamic evt = e;
                            lock (msgQueue)
                            {
                                msgQueue.Enqueue(new MessageContext
                                {
                                    path = evt.path,
                                    type = evt.type,
                                    msg = evt.msg
                                });

                            }
                        }
                        catch (Exception) { }
                        return "true";
                    })
                });
            });
            return "true";
        }

        [DllExport("wilddog_on", CallingConvention = CallingConvention.Cdecl)]
        public static string On(string path, string type, string format, double gml)
        {
            Task.Run(() =>
            {
                JSFunctions.jsFunc(new
                {
                    directive = "On",
                    path = path,
                    type = type,
                    onMessage = (Func<object, Task<object>>)(async (e) =>
                    {
                        try
                        {
                            if (format == "") format = "{0},{1},{2}";
                            dynamic evt = e;
                            NativeMethods.GMLProxy(gml, string.Format(format, evt.path, evt.type, evt.msg));
                            return "true";
                        }
                        catch (Exception)
                        {
                            return "false";
                        }
                    })
                });
            });
            return "true";
        }

        [DllExport("wilddog_pull", CallingConvention = CallingConvention.Cdecl)]
        public static string Pull(string format)
        {
            string ret;
            if (format == "") format = "{0},{1},{2}";
            lock (msgQueue)
            {
                if (msgQueue.Count == 0)
                    ret = "empty";
                else
                {
                    var e = msgQueue.Dequeue();
                    ret = string.Format(format, e.path, e.type, e.msg);
                }
            }
            return ret;
        }

        [DllExport("wilddog_remove", CallingConvention = CallingConvention.Cdecl)]
        public static string Remove(string path)
        {
            Task.Run(() => JSFunctions.jsFunc(new { directive = "Remove", path }));
            return "true";
        }
    }
}
