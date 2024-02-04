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

namespace Interfaces
{
    // ****************************************************************************************************************
    /// <summary>
    ///     The Plugin interface
    /// </summary>
    // ****************************************************************************************************************
    public interface iPluginExample
    {
        /// <summary>Called to initialise</summary>
        /// <returns>The name of the plugin</returns>
        string Init(iProgramInterface _interface);

        /// <summary>Called once per "tick"</summary>
        void Tick();

        /// <summary>Called to quit - free up unmanaged resources etc</summary>
        void Quit();
    }
}


