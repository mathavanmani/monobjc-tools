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
using System.ComponentModel;

namespace Monobjc.Tools.Xcode
{
    /// <summary>
    /// </summary>
    public enum PBXFileType
    {
        /// <summary>
        /// </summary>
        None = 0,
        /// <summary>
        /// </summary>
        [Description("archive.ar")]
        ArchiveAr,
        /// <summary>
        /// </summary>
        [Description("compiled.mach-o.dylib")]
        CompiledMachODylib,
        /// <summary>
        /// </summary>
        [Description("sourcecode.c.h")]
        SourceCodeCH,
        /// <summary>
        /// </summary>
        [Description("sourcecode.c.c")]
        SourceCodeCC,
        /// <summary>
        /// </summary>
        [Description("sourcecode.c.objc")]
        SourceCodeCObjC,
        /// <summary>
        /// </summary>
        [Description("sourcecode.cpp.objcpp")]
        SourceCodeCppObjCpp,
        /// <summary>
        /// </summary>
        [Description("text")]
        Text,
        /// <summary>
        /// </summary>
        [Description("text.plist")]
        TextPList,
        /// <summary>
        /// </summary>
        [Description("text.plist.strings")]
        TextPlistStrings,
        /// <summary>
        /// </summary>
        [Description("text.rtf")]
        TextRTF,
        /// <summary>
        /// </summary>
        [Description("text.script.sh")]
        TextScriptSh,
        /// <summary>
        /// </summary>
        [Description("text.xcconfig")]
        TextXCConfig,
        /// <summary>
        /// </summary>
        [Description("wrapper.application")]
        WrapperApplication,
        /// <summary>
        /// </summary>
        [Description("wrapper.cfbundle")]
        WrapperCFBundle,
        /// <summary>
        /// </summary>
        [Description("wrapper.framework")]
        WrapperFramework,
    }
}