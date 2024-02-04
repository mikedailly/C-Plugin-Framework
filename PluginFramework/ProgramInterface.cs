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
//  If you have any additions or improvements, please consider pushing them back for everyone to use and enjoy.
//
// ******************************************************************************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interfaces;

namespace PluginFramework
{
    // ******************************************************************************************************************************************************
    /// <summary>
    ///     This class is passed to all plugins so that they can call "back" into the main program
    /// </summary>
    // ******************************************************************************************************************************************************
    class ProgramInterface : iProgramInterface
    {
        public string Callback(int _number)
        {
            return Program.TestCallback(_number);
        }
    }
}
