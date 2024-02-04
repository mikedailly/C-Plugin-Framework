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
using Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginExample
{
    public class MyFirstExample : iPluginExample
    {
        iProgramInterface mProgInterface;

        public string Init(iProgramInterface _interface)
        {
            mProgInterface = _interface;
            string res = mProgInterface.Callback(12);
            return "My First Plugin: "+res;
        }

        public void Quit()
        {            
        }

        public void Tick()
        {            
        }





        [Function("plot","x,y,color","Simple drawing command")]
        public void PlotAPoint(int _x, int _y, uint _col)
        {

        }

    }
}
