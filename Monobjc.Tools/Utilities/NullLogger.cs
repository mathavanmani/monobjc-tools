﻿//
// This file is part of Monobjc, a .NET/Objective-C bridge
// Copyright (C) 2007-2014 - Laurent Etiemble
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
namespace Monobjc.Tools.Utilities
{
    /// <summary>
    ///   Implementation of <see cref = "IExecutionLogger" /> that does nothing.
    /// </summary>
    public class NullLogger : IExecutionLogger
    {
        /// <summary>
        ///   Logs the debug message.
        /// </summary>
        /// <param name = "message">The message.</param>
        public void LogDebug(string message) {}

        /// <summary>
        ///   Logs the info message.
        /// </summary>
        /// <param name = "message">The message.</param>
        public void LogInfo(string message) {}

        /// <summary>
        ///   Logs the warning message.
        /// </summary>
        /// <param name = "message">The message.</param>
        public void LogWarning(string message) {}

        /// <summary>
        ///   Logs the error message.
        /// </summary>
        /// <param name = "message">The message.</param>
        public void LogError(string message) {}
    }
}