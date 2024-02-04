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
using Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PluginFramework
{
    static class Program
    {
        private static ProgramInterface Interface;
        private static List<FunctionStore> Functions;


        #region Program Interface callbacks
        public static string TestCallback(int _value)
        {
            return "Test: " + _value.ToString();
        }
        #endregion


        #region Plugin manager

        // ******************************************************************************************************************
        /// <summary>
        ///     Add a function
        /// </summary>
        /// <param name="o"></param>
        // ******************************************************************************************************************
        public static void AddFunction(object o)
        {            
            List<MethodInfo> methods = PluginLoader.Find<Function>(o);
            foreach (MethodInfo m in methods)
            {
                FunctionStore fs = null;

                foreach (CustomAttributeData a in m.CustomAttributes)
                {
                    if (a.AttributeType == typeof(Function))
                    {
                        string names = (string)a.ConstructorArguments[0].Value;
                        string args = (string)a.ConstructorArguments[1].Value;
                        string desc = (string)a.ConstructorArguments[2].Value;
                        fs = new FunctionStore(names, args, desc, m, o);
                    }
                }

                if (fs != null)
                {
                    Functions.Add(fs);
                }
            }
        }

        // **********************************************************************************************************************************
        /// <summary>
        ///     Load all our plugins
        /// </summary>
        // **********************************************************************************************************************************
        public static void LoadPlugins()
        {
            // Create program interface
            Interface = new ProgramInterface();
            // Initialise Plugin scanner
            PluginLoader.Init();
            Functions = new List<FunctionStore>();


            string RootPath = PluginLoader.AssemblyDirectory();
            string[] files = Directory.GetFiles(RootPath, "*.dll");
            foreach (string file in files)
            {
                List<object> plugins = PluginLoader.LoadAssembly<iPluginExample>(file);
                if (plugins != null)
                {
                    foreach (object o in plugins)
                    {
                        iPluginExample Plugin = o as iPluginExample;
                        if (Plugin != null)
                        {
                            string name = Plugin.Init(Interface);
                            Console.WriteLine("Plugin '" + name + "' loaded");

                            List<MethodInfo> methods = PluginLoader.Find<Function>(o);
                            AddFunction(o);
                        }
                    }
                }
            }
        }

        #endregion


        // **********************************************************************************************************************************
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        // **********************************************************************************************************************************
        [STAThread]
        static void Main()
        {
            LoadPlugins();

            // Call the test "Plot" function
            object[] parameters = new object[3] { 1, 2, 0xffffffff };
            Functions[0].Method.Invoke(Functions[0].Object, parameters);


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
