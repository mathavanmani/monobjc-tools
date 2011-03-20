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
using System.IO;

namespace Monobjc.Tools.Xcode
{
    public class PBXCopyFilesBuildPhase : PBXBuildPhase
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref = "PBXCopyFilesBuildPhase" /> class.
        /// </summary>
        public PBXCopyFilesBuildPhase()
        {
            this.DstPath = string.Empty;
            this.DstSubfolderSpec = 16;
        }

        /// <summary>
        ///   Gets or sets the DST path.
        /// </summary>
        /// <value>The DST path.</value>
        public String DstPath { get; set; }

        /// <summary>
        ///   Gets or sets the DST subfolder spec.
        /// </summary>
        /// <value>The DST subfolder spec.</value>
        public int DstSubfolderSpec { get; set; }

        /// <summary>
        ///   Gets the nature.
        /// </summary>
        /// <value>The nature.</value>
        public override PBXElementType Nature
        {
            get { return PBXElementType.PBXCopyFilesBuildPhase; }
        }

        /// <summary>
        ///   Gets the description.
        /// </summary>
        /// <value>The description.</value>
        public override string Description
        {
            get { return "CopyFilesBuildPhase"; }
        }

        /// <summary>
        ///   Accepts the specified visitor.
        /// </summary>
        /// <param name = "visitor">The visitor.</param>
        public override void Accept(IPBXVisitor visitor)
        {
            visitor.Visit(this);

            if (this.Files != null)
            {
                foreach (PBXFileReference file in this.Files)
                {
                    file.Accept(visitor);
                }
            }
        }

        /// <summary>
        ///   Writes this element to the writer.
        /// </summary>
        /// <param name = "writer">The writer.</param>
        /// <param name = "map">The map.</param>
        public override void WriteTo(TextWriter writer, IDictionary<IPBXElement, string> map)
        {
            writer.writeElementPrologue(map, this);
            writer.WriteAttribute("buildActionMask", this.BuildActionMask);
            writer.WriteAttribute("dstPath", this.DstPath);
            writer.WriteAttribute("dstSubfolderSpec", this.DstSubfolderSpec);
            writer.WriteReferences(map, "files", this.Files);
            writer.WriteAttribute("runOnlyForDeploymentPostprocessing", this.RunOnlyForDeploymentPostprocessing);
            writer.writeElementEpilogue();
        }
    }
}