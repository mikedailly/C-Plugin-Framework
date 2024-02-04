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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Diagnostics;

namespace Interfaces
{
	public static class PluginLoader
	{
		internal static Assembly TheAssembly;
		
		/// <summary>Simple look up table for all the files we've tried to load - even if they never worked</summary>
		private static Dictionary<string, Assembly> LoadedAssemblies;

		// *****************************************************************************
		/// <summary>
		///		Init the plugin loader
		/// </summary>
		/// <returns>False for error, True for okay</returns>
		// *****************************************************************************
		public static bool Init()
		{
			LoadedAssemblies = new Dictionary<string, Assembly>();
			return true;
		}

		// ************************************************************************************************
		/// <summary>
		///		Load the assembly (if we haven't already) and add it to our pool
		/// </summary>
		/// <param name="_filename">Full path to the assembly</param>
		/// <returns>
		///		The loaded assembly - or null for error
		/// </returns>
		// ************************************************************************************************
		private static Assembly LoadedAssembly(string _filename)
		{
			Assembly assembly;
			if (LoadedAssemblies.TryGetValue(_filename, out assembly))
			{
				// even DLLs we tried to load and failed are stored, and will return a null
				return assembly;
			}

			try{
				assembly = Assembly.LoadFrom(_filename);
			}catch{
				Console.WriteLine("Error loading " + _filename);
				assembly = null;
			}
			LoadedAssemblies.Add(_filename, assembly);
			return assembly;
		}


		// *****************************************************************************
		/// <summary>
		///		Get the path to the running program
		/// </summary>
		/// <returns>The path to where we are running from</returns>
		// *****************************************************************************
		public static string AssemblyDirectory()
		{
			string codeBase = Assembly.GetExecutingAssembly().Location;
			UriBuilder uri = new UriBuilder(codeBase);
			string path = Uri.UnescapeDataString(uri.Path);
			return Path.GetDirectoryName(path);
		}


		/// **************************************************************************************************
		/// <summary>
		///     Gets a list of supported types, where that type implements our interface
		/// </summary>
		/// <typeparam name="T">The interface we're looking for</typeparam>
		/// <returns>
		///		A list of valid types to look for
		/// </returns>
		/// **************************************************************************************************
		private static List<string> EnumerateTypes<T>()
		{
			return TheAssembly.DefinedTypes
							.Where(type => typeof(T).IsAssignableFrom(type) && !type.IsAbstract)
							.Select(type => type.FullName)
							.ToList();
		}


		/// **************************************************************************************************
		/// <summary>
		///		Look in the assembly for the found types, and
		///	</summary>
		/// <returns>True for okay, false for error</returns>
		/// **************************************************************************************************
		private static List<object> ScanAssembly<iPlugin>()
		{
			List<object> rets = new List<object>();

			// Any types that implement our interface?
			List<string> PluginTypes = EnumerateTypes<iPlugin>();
			if (PluginTypes.Count == 0) return rets;				// nope!

			// We found some, so create an instance of each of the types we found
			List<iPlugin> PluginInterfaces = new List<iPlugin>();
			foreach (string s in PluginTypes)
			{
				Type t = TheAssembly.GetType(s);
				object pkg = Activator.CreateInstance(t);
				rets.Add(pkg);
			}
			return rets;
		}


		// ************************************************************************************************************************
		/// <summary>
		///		Load a single DLL. Each DLL can have multiple iPlugin interface types, so we'll return a list if any are found
		/// </summary>
		/// <param name="_FileName">The full path to the DLL to try and load</param>
		/// <returns>
		///		List of objects that implement the interface we're searching for
		/// </returns>
		// ************************************************************************************************************************
		public static List<object> LoadAssembly<iInterface>(string _FileName)
		{
			TheAssembly = null;
			if (!File.Exists(_FileName)) return null;

			// Have we already loaded this assembly in this app domain?
			bool found = false;
			foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
			{
				if (!a.IsDynamic)
				{
					if (a.Location == _FileName)
					{
						TheAssembly = a;
						found = true;
						break;
					}
				}
			}

			byte[] fileBytes = null;
			if (!found)
			{
				try
				{
					fileBytes = File.ReadAllBytes(_FileName);
                }
                catch
                {
					Console.WriteLine("Error trying to load assembly " + _FileName);
					return null;
                }
				Type plugintype = typeof(iInterface);


				// check to see if this possibly an assembly first of all
				// DOS header starts at 0x0, the DWORD at 0x3c contains a pointer to the PE signature(usually 0x80) which is 4 bytes, 
				// the next 20 bytes is the COFF header and then there is the PE header (at 0x9.The PE header is 224 bytes and contains the 
				// data directory(at 96 bytes into the PE header = 0xf.The 15th entry(at 0x16 is the CLR header descriptor(sometimes called the 
				// COM descriptor, but this does not have anything to do with COM). If this is empty (ie 0 in the 8 bytes from 0x168 to 0x16f) then 
				// the file is not a.NET assembly. If you want to check if it is a COM DLL then you should look to see if it exports GetClassObject.
				int offsetCOFF = BitConverter.ToInt32(fileBytes, 0x3c);
				int magicPE = BitConverter.ToInt32(fileBytes, offsetCOFF);
				if (magicPE != 0x004550) return null;
				offsetCOFF += 4;
				int offsetOptional = offsetCOFF + 20;
				Int16 sizeofOptionalHeader = BitConverter.ToInt16(fileBytes, offsetCOFF + 16);
				Int16 magicOptional = BitConverter.ToInt16(fileBytes, offsetOptional);
				bool isPE32 = (magicOptional == 0x10b);
				int datatDirectoryOffset = offsetOptional + (isPE32 ? 96 : 112);
				Int64 CLRheaderDescriptor = BitConverter.ToInt64(fileBytes, datatDirectoryOffset + 112);
				if (CLRheaderDescriptor == 0) return null;


				// First, does this assembly container a plugin? Scan it before actually loading it in....
				TheAssembly = LoadedAssembly(_FileName);
				if (TheAssembly == null) return null;
			}

			// load all dependant assemblies
			AssemblyName[] RefNames = TheAssembly.GetReferencedAssemblies();
			foreach (var name in RefNames)
			{
				LoadedAssembly(name.FullName);
			}

			// Now search for the plugin in the loaded Assembly
			bool foundtype = false;
			try
			{
				foreach (TypeInfo i in TheAssembly.DefinedTypes)
				{
					Type[] tt = i.GetInterfaces();
					foreach (Type ti in tt)
					{
						if (ti.FullName == typeof(iInterface).FullName)
						{
							foundtype = true;
							break;
						}
					}
					if (foundtype) break;
				}
			}
			catch
			{
				// Assembly doesn't have "DefinedTypes"... so just ignore
				return null;
			}
			if (!foundtype) return null;

			// If we get here, then the assembly implements an "iInterface" somewhere... 
			if (fileBytes != null)
			{
				try
				{
					TheAssembly = Assembly.Load(fileBytes);
				}
				catch
				{
					return null;
				}
			}

			return ScanAssembly<iInterface>();
		}

		// ************************************************************************************************************************
		/// <summary>
		///     Find all the attributes attached to methods in an object
		/// </summary>
		/// <typeparam name="Attribute">The attribute we're looking for</typeparam>
		/// <param name="Obj">The object to look in</param>
		/// <returns>
		///		A list of methods tagged with the requested attribute
		/// </returns>
		// ************************************************************************************************************************
		public static List<MethodInfo> Find<Attribute>(object Obj)
		{
			if (Obj == null) return new List<MethodInfo>();

			MethodInfo[] methods = Obj.GetType()
						 .GetMethods()
						 .Where(m => m.GetCustomAttributes(typeof(Attribute), false).Length > 0)
						 .ToArray();

			return methods.ToList();
		}
	}
}
