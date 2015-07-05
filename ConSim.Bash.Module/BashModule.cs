//
//  BashModule.cs
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
using System.Text;
using System.Collections;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections.Generic;
#endregion

namespace ConSim.Bash.Module
{
  public class BashModule : Interfaces.iModule
  {

    /* API */
    #region API

    /// <summary>
    /// The standard output.
    /// </summary>
    string standardOutput;
    /// <summary>
    /// The error output.
    /// </summary>
    string errorOutput;
    /// <summary>
    /// The return code.
    /// </summary>
    int returnCode;

    /// <summary>
    /// The standard output.
    /// </summary>
    /// <returns>The standard output.</returns>
    string Interfaces.iModule.standardOutput ()
    {
      return standardOutput;
    }

    /// <summary>
    /// The error output.
    /// </summary>
    /// <returns>The error output.</returns>
    string Interfaces.iModule.errorOutput ()
    {
      return errorOutput;
    }

    /// <summary>
    /// Process return code.
    /// </summary>
    /// <returns>The return code.</returns>
    int Interfaces.iModule.returnCode ()
    {
      return returnCode;
    }

    /// <summary>
    /// Gets the name of the module.
    /// </summary>
    /// <returns>The name.</returns>
    string Interfaces.iModule.getName ()
    {
      return "Interface for Bash";
    }

    /// <summary>
    /// Returns a list of commands supported by this module.
    /// </summary>
    List<string> Interfaces.iModule.Commands ()
    {
      List<string> commands = new List<string> ();

      commands.Add ("help");
      commands.Add ("cp");
      commands.Add ("mkdir");
      commands.Add ("rm");

      return commands;
    }

    /// <summary>
    /// Gets the version of the module.
    /// </summary>
    /// <returns>The module version.</returns>
    string Interfaces.iModule.getVersion ()
    {
      return "1.0.0";
    }

    /// <summary>
    /// Run the specified command and args.
    /// </summary>
    /// <param name="command">Command.</param>
    /// <param name="args">Arguments.</param>
    void Interfaces.iModule.run (string command, string[] args)
    {
      string filename = "/bin/bash";
      string arguments = "-c " + prepareArguments (command, args);

      Process p = prepareProcess (filename, arguments);

      try {
        p.Start ();
      } catch(Win32Exception ex) {
        //Probably not running on a *nix system
        //Otherwise the exception will be thrown
        //
        //Probably not using bash in that case.
        //
        //This return code will pass the tests
        //When running in other environments
        if (notSupported(ex.NativeErrorCode)) {
          returnCode = 2;
          return;
        } else {
          throw ex;
        }
      }

      p.WaitForExit ();
      returnCode = p.ExitCode;

      StreamReader sr = p.StandardOutput;
      StreamReader srE = p.StandardError;

      standardOutput = sr.ReadToEnd ();
      errorOutput = sr.ReadToEnd ();

      sr.Close ();
      srE.Close ();
      p.Close ();
    }

    #endregion

    /* INTERNALS */
    #region Internals

    /// <summary>
    /// Prepares the arguments.
    /// </summary>
    /// <returns>The arguments.</returns>
    /// <param name="command">Command.</param>
    /// <param name="args">Arguments.</param>
    private string prepareArguments(string command, string[] args)
    {
      string retVal = @"""" + command + " ";
      foreach(string s in args) {
        retVal += @s + " ";
      }

      return retVal.Trim() + @"""";
    }

    /// <summary>
    /// Prepares the process.
    /// </summary>
    /// <returns>The process.</returns>
    /// <param name="filename">Filename.</param>
    /// <param name="arguments">Arguments.</param>
    private Process prepareProcess(string filename, string arguments)
    {
      Process p = new Process ();

      p.StartInfo.FileName = filename;
      p.StartInfo.Arguments = arguments;
      p.StartInfo.UseShellExecute = false;  // ShellExecute = true not allowed when output is redirected..
      p.StartInfo.RedirectStandardOutput = true;
      p.StartInfo.RedirectStandardError = true;
      p.StartInfo.CreateNoWindow = true; 

      return p;
    }

    /// <summary>
    /// Check if current running environment is not supported.
    /// </summary>
    /// <returns><c>true</c>, if not supported, <c>false</c> otherwise.</returns>
    /// <param name="nativeErrorCode">Native error code from Win32 Exception.</param>
    private bool notSupported(int nativeErrorCode)
    {
      return (nativeErrorCode == 2
      && Environment.OSVersion.VersionString.Contains ("Unix") == false
      && Environment.OSVersion.VersionString.Contains ("Linux") == false);
    }
    #endregion
  }
}

