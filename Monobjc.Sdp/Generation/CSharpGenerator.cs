﻿//
// This file is part of Monobjc, a .NET/Objective-C bridge
// Copyright (C) 2007-2011 - Laurent Etiemble
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Monobjc.Tools.Sdp.Model;

namespace Monobjc.Tools.Sdp.Generation
{
    /// <summary>
    /// Generate a wrapper by using the C# language.
    /// </summary>
    public class CSharpGenerator : Generator
    {
        protected override string Generate(GenerationContext context)
        {
            StringBuilder builder = new StringBuilder();

            GenerateHeader(builder);
            GenerateUsings(builder);

            // Output namespace
            builder.AppendLine("namespace Monobjc.ScriptingBridge." + context.Prefix);
            builder.AppendLine("{");

            // Output enumerations
            GenerateEnumerations(builder, context);

            // Output application class
            @class appCls = context.Classes.FirstOrDefault(c => c.name == "application");
            if (appCls == null)
            {
                throw new NotSupportedException("No application found");
            }
            GenerateClass(builder, context, appCls);

            // Output other classes
            foreach (@class cls in context.Classes.Where(c => c.name != "application"))
            {
                GenerateClass(builder, context, cls);
            }

            builder.AppendLine("}");

            return builder.ToString();
        }

        private static void GenerateHeader(StringBuilder builder)
        {
            builder.AppendLine("//");
            builder.AppendLine("// This file was generated by the Monobjc SDP tool");
            builder.AppendLine("//");
            builder.AppendLine("// DO NOT ALTER UNLESS YOU KNOW WHAT YOU DO");
            builder.AppendLine("//");
        }

        private static void GenerateUsings(StringBuilder builder)
        {
            builder.AppendLine("using System;");
            builder.AppendLine("using Monobjc;");
            builder.AppendLine("using Monobjc.Foundation;");
            builder.AppendLine("using Monobjc.AppKit;");
            builder.AppendLine("using Monobjc.ScriptingBridge;");
            builder.AppendLine();
        }

        private static void GenerateClass(StringBuilder builder, GenerationContext context, @class cls)
        {
            String className = context.ConvertType(cls.name, false);
            String baseClassName;
            if (cls.name == "application")
            {
                baseClassName = "SBApplication";
            }
            else
            {
                baseClassName = String.IsNullOrEmpty(cls.inherits) ? "SBObject" : context.ConvertType(cls.inherits, false);
            }

            builder.AppendLine("\t/// <summary>");
            builder.AppendFormat("\t/// {0}", cls.description);
            builder.AppendLine();
            builder.AppendLine("\t/// </summary>");

            builder.AppendFormat("\tpublic partial class {0} : {1}", className, baseClassName);
            builder.AppendLine();
            builder.AppendLine("\t{");

            // TODO: Add doc
            builder.AppendFormat("\t\tpublic {0}() : base() {{}}", className);
            builder.AppendLine();
            builder.AppendLine();
            // TODO: Add doc
            builder.AppendFormat("\t\tpublic {0}(IntPtr pointer) : base(pointer) {{}}", className);
            builder.AppendLine();
            builder.AppendLine();

            // Output elements
            builder.AppendLine("#region ----- Elements -----");
            foreach (element element in context.GetElementsFor(cls).OrderBy(e => e.type))
            {
                // TODO: Add doc
                String elementName;
                String elementMessage;
                @class typeCls = context.Classes.FirstOrDefault(c => String.Equals(c.name, element.type));
                if (typeCls != null)
                {
                    elementName = NamingHelper.GenerateDotNetName(String.Empty, typeCls.plural ?? typeCls.name);
                    elementMessage = NamingHelper.GenerateObjCName(typeCls.plural ?? typeCls.name);
                }
                else
                {
                    elementName = NamingHelper.GenerateDotNetName(String.Empty, element.type);
                    elementMessage = NamingHelper.GenerateObjCName(element.type);
                }
                builder.AppendFormat("\t\tpublic SBElementArray {0}", elementName);
                builder.AppendLine();
                builder.AppendLine("\t\t{");
                builder.AppendFormat("\t\t\tget {{ return ObjectiveCRuntime.SendMessage<SBElementArray>(this, \"{0}\"); }}", elementMessage);
                builder.AppendLine();
                builder.AppendLine("\t\t}");
                builder.AppendLine();
            }
            builder.AppendLine("#endregion");
            builder.AppendLine();

            // Output properties
            builder.AppendLine("#region ----- Properties -----");
            foreach (property property in context.GetPropertiesFor(cls).OrderBy(p => p.name))
            {
                String propertyType = context.ConvertType(property.type, true);
                String propertyName = NamingHelper.GenerateDotNetName(String.Empty, property.name);

                // TODO: Add doc
                builder.AppendFormat("\t\tpublic {0} {1}", propertyType, propertyName);
                builder.AppendLine();
                builder.AppendLine("\t\t{");

                // Generate getter
                switch (property.access)
                {
                    case propertyAccess.r:
                    case propertyAccess.rw:
                        {
                            String message = NamingHelper.GenerateObjCName(property.name);
                            builder.AppendFormat("\t\t\tget {{ return ObjectiveCRuntime.SendMessage<{0}>(this, \"{1}\"); }}", propertyType, message);
                            builder.AppendLine();
                            break;
                        }
                    default:
                        break;
                }

                // Generate setter
                switch (property.access)
                {
                    case propertyAccess.rw:
                    case propertyAccess.w:
                        {
                            String message = "set" + propertyName + ":";
                            builder.AppendFormat("\t\t\tset {{ ObjectiveCRuntime.SendMessage(this, \"{0}\", value); }}", message);
                            builder.AppendLine();
                            break;
                        }
                    default:
                        break;
                }

                builder.AppendLine("\t\t}");
                builder.AppendLine();
            }
            builder.AppendLine("#endregion");
            builder.AppendLine();

            // Output methods
            builder.AppendLine("#region ----- Commands -----");
            IEnumerable<command> commands = context.GetCommandsFor(cls);
            foreach (command command in commands.OrderBy(c => c.name))
            {
                String returnType = command.result != null ? context.ConvertType(command.result.type1, true) : "void";

                String methodName = NamingHelper.GenerateDotNetName(String.Empty, command.name);
                if (command.parameter != null)
                {
                    methodName = command.parameter.Aggregate(methodName, (name, p) => name += NamingHelper.GenerateDotNetName(String.Empty, p.name));
                }
                // TODO: Add doc
                String signature = String.Format("\t\tpublic {0} {1}(", returnType, methodName);
                List<String> parameters = new List<String>();
                if (command.parameter != null)
                {
                    foreach (parameter parameter in command.parameter)
                    {
                        String parameterName = NamingHelper.GenerateObjCName(parameter.name);
                        parameters.Add(context.ConvertType(parameter.type1, true) + " " + GenerationContext.ConvertParameterName(parameterName));
                    }
                }
                signature += String.Join(", ", parameters.ToArray());
                signature += ")";

                builder.Append(signature);
                builder.AppendLine();
                builder.AppendLine("\t\t{");

                // TODO: Add invocation

                builder.AppendLine("\t\t}");
                builder.AppendLine();
            }
            builder.AppendLine("#endregion");
            builder.AppendLine();

            builder.AppendLine("\t}");
            builder.AppendLine();
        }

        private static void GenerateEnumerations(StringBuilder builder, GenerationContext context)
        {
            foreach (enumeration enumeration in context.Enumerations)
            {
                GenerateEnumeration(builder, context, enumeration);
            }
        }

        private static void GenerateEnumeration(StringBuilder builder, GenerationContext context, enumeration enumeration)
        {
            String enumerationName = NamingHelper.GenerateDotNetName(context.Prefix, enumeration.name);

            //builder.AppendLine("\t/// <summary>");
            //builder.AppendFormat("\t/// {0}", enumeration.description);
            //builder.AppendLine();
            //builder.AppendLine("\t/// </summary>");

            builder.AppendFormat("\tpublic enum {0} : uint", enumerationName);
            builder.AppendLine();
            builder.AppendLine("\t{");
            IEnumerable<enumerator> values = enumeration.Items.Where(i => i is enumerator).Cast<enumerator>();
            foreach (enumerator value in values)
            {
                String valueName = NamingHelper.GenerateDotNetName(enumerationName, value.name);
                //builder.AppendLine("\t\t/// <summary>");
                //builder.AppendFormat("\t\t/// {0}", value.description);
                //builder.AppendLine();
                //builder.AppendLine("\t\t/// </summary>");

                if (!String.IsNullOrEmpty(value.code))
                {
                    builder.AppendFormat("\t\t{0} = 0x{1}, // '{2}'", valueName, NamingHelper.ToUInt32(value.code).ToString("X8"), value.code);
                }
                else
                {
                    builder.AppendFormat("\t\t{0},", valueName);
                }
                builder.AppendLine();
            }
            builder.AppendLine("\t}");
            builder.AppendLine();
        }
    }
}