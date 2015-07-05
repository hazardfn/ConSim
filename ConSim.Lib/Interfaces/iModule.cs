//
//  iModule.cs
//
//  Author:
//       Howard Beard-Marlowe <howardbm@live.se>
//
//  Copyright (c) 2015 Howard Beard-Marlowe
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

/* INCLUDES */
#region Includes
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
#endregion

namespace Interfaces
{
  /// <summary>
  /// Represents a pluggable DLL which exposes 
  /// a command.
  /// </summary>
  public interface iModule
  {
    /* API */
    #region API
    /// <summary>
    /// Name of the module.
    /// </summary>
    /// <returns>The name.</returns>
    string getName();
    /// <summary>
    /// Gets the version of the module.
    /// </summary>
    /// <returns>The version.</returns>
    string getVersion();
    /// <summary>
    /// Gets a list of accepted commands
    /// </summary>
    List<string> Commands();
    /// <summary>
    /// Runs the module when supplied with a list of args.
    /// </summary>
    /// <param name="args">Arguments.</param>
    void run(string command, string[] args); 
    /// <summary>
    /// Returns the output of the command last run.
    /// </summary>
    /// <returns>The output.</returns>
    string standardOutput();
    /// <summary>
    /// Returns the error output of the command last run.
    /// </summary>
    /// <returns>The output.</returns>
    string errorOutput();
    /// <summary>
    /// Should contain an exit code if your module relays to a process
    /// </summary>
    /// <returns>The code.</returns>
    int returnCode();
    #endregion
  }
}

