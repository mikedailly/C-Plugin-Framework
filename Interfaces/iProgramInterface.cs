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
    public interface iProgramInterface
    {
        /// <summary>
        ///     Simple callback into actual program
        /// </summary>
        /// <param name="_number">something we'll add to the string as "proof" it's done</param>
        /// <returns>the result</returns>
        string Callback(int _number);
    }
}
