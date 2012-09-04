//
// This file is part of Monobjc, a .NET/Objective-C bridge
// Copyright (C) 2007-2012 - Laurent Etiemble
//
// Monobjc is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// any later version.
//
// Monobjc is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Monobjc.  If not, see <http://www.gnu.org/licenses/>.
//
using System;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using Monobjc.Tools.Generator.Model.Database;
using Monobjc.Tools.Generator.Tasks;
using Monobjc.Tools.Generator.Tasks.Conversion;
using Monobjc.Tools.Generator.Tasks.General;
using Monobjc.Tools.Generator.Tasks.Generation;
using Monobjc.Tools.Generator.Tasks.Output;
using Monobjc.Tools.Generator.Tasks.Patching;
using Monobjc.Tools.Generator.Utilities;
using NDesk.Options;

namespace Monobjc.Tools.Generator
{
    public static class Program
    {
        public static void Help()
        {
            Console.WriteLine();
            Console.WriteLine("Monobjc Code Generator");
            Console.WriteLine();
            Console.WriteLine("Usage:");
            Console.WriteLine("MonobjcGenerate [--help] [-t|--tasks=XXX] [-d|--dir=YYY] -s|--set=ZZZ");
            Console.WriteLine();
            Console.WriteLine("\t-h|--help           : Display this help");
            Console.WriteLine("\t-t|--tasks=value    : A comma separated list of task to execute ('retrieve', 'extract', 'generate', 'copy'");
            Console.WriteLine("\t-d|--dir=value      : The directory where to copy generated code");
            Console.WriteLine("\t-s|--set=value      : The documentation set to use ('SnowLeopard', 'Lion', 'MountainLion')");
            Console.WriteLine();
        }

        public static void Main(string[] args)
        {
            int verbose = 0;
            String tasks = null;
            String targetDir = null;
            String docSet = "MountainLion";

            // Create an option set
            OptionSet p = new OptionSet().
                Add("h|help", v => tasks = "help").
                Add("v", v => ++verbose).
                Add("t=|tasks=", v => tasks = v).
                Add("d=|dir=", v => targetDir = v).
                Add("s=|set=", v => docSet = v);
            p.Parse(args);

            // A DocSet is mandatory
            if (String.IsNullOrEmpty(docSet))
            {
                Help();
                return;
            }

            // Load DocSet properties
            NameValueCollection settings = new NameValueCollection();
            settings.Load(docSet + ".properties");

            // Create an execution context
            TaskContext context = new TaskContext();
            context.Settings = settings;
            context.TypeManager = new TypeManager();

            // Create the execution manager
            TaskManager manager = new TaskManager();
            manager.Context = context;
            manager.AddTask(new LoadTask("Load"));

            // Prepare each task
            tasks = tasks ?? String.Empty;
            foreach (String task in tasks.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).Distinct())
            {
                switch (task)
                {
                    case "validate":
                        manager.AddTask(new ValidateTask("Validate"));
                        break;
                    case "retrieve":
                        manager.AddTask(new RetrieveTask("Download"));
                        break;
                    case "extract":
                        manager.AddTask(new CleanClassicTask("Clean Classic HTML"));
                        manager.AddTask(new ReplaceTextTask("Patch HTML", EntryFolderType.Html));
                        manager.AddTask(new Html2XhtmlTask("HTML 2 XHTML"));
                        manager.AddTask(new ReplaceTextTask("Patch XHTML", EntryFolderType.Xhtml));
                        manager.AddTask(new DeprecateTask("Deprecate"));
                        manager.AddTask(new Xhtml2XmlTask("XHTML 2 XML"));
                        manager.AddTask(new CSharp2XmlTask("Code 2 XML"));
                        manager.AddTask(new DeprecateTask("Deprecate"));
                        manager.AddTask(new PatchXmlTask("Patch XML"));
                        break;
                    case "generate":
                        manager.AddTask(new GenerateCodeTask("Generate"));
                        manager.AddTask(new ReplaceTextTask("Patch Generated Code 1/4", EntryFolderType.Generated, ".cs"));
                        manager.AddTask(new ReplaceTextTask("Patch Generated Code 2/4", EntryFolderType.Generated, ".Class.cs"));
                        manager.AddTask(new ReplaceTextTask("Patch Generated Code 3/4", EntryFolderType.Generated, ".Constants.cs"));
                        manager.AddTask(new ReplaceTextTask("Patch Generated Code 4/4", EntryFolderType.Generated, ".Constructors.cs"));
                        break;
                    case "copy":
                        if (String.IsNullOrEmpty(targetDir))
                        {
                            Console.WriteLine("Directory must be provided");
                            Help();
                            Environment.Exit(-1);
                        }
                        manager.AddTask(new CopyInPlaceTask("Coyy In Place", targetDir));
                        manager.AddTask(new GenerateInfoTask("Generate Assembly Info", targetDir));
                        break;
                    case "analyze":
                        manager.AddTask(new DumpDelegateMethodsTask("Search Delegate Methods"));
                        manager.AddTask(new DumpStaticInitializersTask("Search Static Initializers"));
                        manager.AddTask(new ValidateTask("Validate URLs"));
                        manager.AddTask(new DumpDeprecatedTask("Search for Deprecated API"));
                        manager.AddTask(new CopyURLTask("Copy Deprecated URLs"));
                        manager.AddTask(new DumpMissingTask("Search for missing entities"));
                        manager.AddTask(new DumpAllNamesTask("List All Names"));
                        manager.AddTask(new DumpEmptySignatureTask("Empty Signatures"));
                        break;

                    case "dump":
                        manager.AddTask(new DumpGenerationTask("Generation Result"));
						break;

                        //	
                        // This code is reserved when developing new task
                        //
                    case "dev":
                        // ...
                        break;

                    default:
                        Help();
                        Environment.Exit(-1);
                        break;
                }
            }

            manager.Execute();
        }

        public static void Load(this NameValueCollection collection, String filename)
        {
            foreach (String line in File.ReadLines(filename))
            {
                if (String.IsNullOrEmpty(line) || line.StartsWith(";") || line.StartsWith("#") || line.StartsWith("'"))
                {
                    continue;
                }
                if (!line.Contains("="))
                {
                    continue;
                }
                String[] parts = line.Split(new string[] { "=" }, 2, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 2)
                {
                    continue;
                }
                collection.Add(parts[0], parts[1]);
            }
        }
    }
}