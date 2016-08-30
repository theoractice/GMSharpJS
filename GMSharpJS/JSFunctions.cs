using EdgeJs;
using RGiesecke.DllExport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GMSharpJS
{
    class JSFunctions
    {
        internal static Func<object, Task<object>> jsFunc = Edge.Func(
        @"
            var Wilddog = require('wilddog');
            var ref; 

            return function (arg, cb)
            {
                switch(arg.directive)
                {
                    case 'Timer':
                        setTimeout(function() {
                            arg.onMessage({},
                            function(error, result)
                            {
                                if (error) throw error;
                            });
                        }, arg.interval);
                        break;
                    case 'Init':
                        ref = new Wilddog('https://'+arg.appId+'.wilddogio.com/');
                        break;
                    case 'Set':
                        ref.child(arg.path).set(arg.data);
                        break;
                    case 'Update':
                        ref.child(arg.path).update(arg.data);
                        break;
                    case 'Subscribe':
                    case 'On':
                        ref.child(arg.path).on(
                            arg.type, 
                            function(datasnapshot)
                            {
                                arg.onMessage(
                                {
                                    path : datasnapshot.key(),
                                    type : arg.type,
                                    msg : datasnapshot.val()
                                },
                                function(error, result)
                                {
                                    if (error) throw error;
                                });
                            }, 
                            function(error) { cb(error, null); }
                        );
                        break;
                    case 'Remove':
                        ref.child(arg.path).remove();
                        break;
                    default:
                        break;
                }
                cb(); 
            }
        ");
    }
}
