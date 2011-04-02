//
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
using Monobjc.Tools.Utilities;

namespace Monobjc.Tools.Xcode
{
    public class PBXFileReference : PBXFileElement
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref = "PBXFileReference" /> class.
        /// </summary>
        public PBXFileReference() {}

        /// <summary>
        ///   Initializes a new instance of the <see cref = "PBXFileReference" /> class.
        /// </summary>
        /// <param name = "name">The name.</param>
        public PBXFileReference(string name) : base(name) {}

        /// <summary>
        ///   Gets or sets the file encoding.
        /// </summary>
        /// <value>The file encoding.</value>
        public PBXFileEncoding FileEncoding { get; set; }

        /// <summary>
        ///   Gets or sets the type of the explicit file.
        /// </summary>
        /// <value>The type of the explicit file.</value>
        public PBXFileType ExplicitFileType { get; set; }

        /// <summary>
        ///   Gets or sets the last type of the known file.
        /// </summary>
        /// <value>The last type of the known file.</value>
        public PBXFileType LastKnownFileType { get; set; }

        /// <summary>
        ///   Gets or sets the line ending.
        /// </summary>
        /// <value>The line ending.</value>
        public PBXLineEnding LineEnding { get; set; }

        /// <summary>
        ///   Gets or sets the path.
        /// </summary>
        /// <value>The path.</value>
        public String Path { get; set; }

        /// <summary>
        ///   Gets or sets the source tree.
        /// </summary>
        /// <value>The source tree.</value>
        public PBXSourceTree SourceTree { get; set; }

        /// <summary>
        ///   Gets the elemnt's nature.
        /// </summary>
        /// <value>The nature.</value>
        public override PBXElementType Nature
        {
            get { return PBXElementType.PBXFileReference; }
        }

        /// <summary>
        ///   Accepts the specified visitor.
        /// </summary>
        /// <param name = "visitor">The visitor.</param>
        public override void Accept(IPBXVisitor visitor)
        {
            visitor.Visit(this);
        }

        /// <summary>
        ///   Writes this element to the writer.
        /// </summary>
        /// <param name = "writer">The writer.</param>
        /// <param name = "map">The map.</param>
        public override void WriteTo(TextWriter writer, IDictionary<IPBXElement, string> map)
        {
            writer.WritePBXElementPrologue(2, map, this);
            if (this.FileEncoding != PBXFileEncoding.Default)
            {
                writer.WritePBXProperty(3, map, "fileEncoding", (int) this.FileEncoding);
            }
            if (this.ExplicitFileType != PBXFileType.None)
            {
                writer.WritePBXProperty(3, map, "explicitFileType", this.ExplicitFileType.ToDescription());
            }
            if (this.LastKnownFileType != PBXFileType.None)
            {
                writer.WritePBXProperty(3, map, "lastKnownFileType", this.LastKnownFileType.ToDescription());
            }
            if (this.LineEnding != PBXLineEnding.Unix)
            {
                writer.WritePBXProperty(3, map, "lineEnding", (int) this.LineEnding);
            }
            writer.WritePBXProperty(3, map, "name", this.Name);
            writer.WritePBXProperty(3, map, "path", this.Path);
            writer.WritePBXProperty(3, map, "sourceTree", this.SourceTree.ToDescription());
            writer.WritePBXElementEpilogue(2);
        }
    }
}