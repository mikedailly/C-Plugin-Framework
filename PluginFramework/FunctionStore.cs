// ******************************************************************************************************************************************************
//
//  C# plugin framework (c) Mike Dailly 2020, all rights reserved
//
//  This code is free to use, modify and share with no limitations - copyright is retained.
//
//  Contributors
//  ------------
//  Mike Dailly
//
//
//  If you havce any additions or improvements, please consider pushing them back for everyone to use and enjoy.
//
// ******************************************************************************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PluginFramework
{
    public class FunctionStore
    {
        public string Name;
        public string Args;
        public string Desc;
        public MethodInfo Method;
        public object Object;

        public FunctionStore(string _name, string _args, string _desc, MethodInfo _method, object _object)
        {
            Name = _name;
            Args = _args;
            Desc = _desc;
            Method = _method;
            Object = _object;
        }
    }
}
