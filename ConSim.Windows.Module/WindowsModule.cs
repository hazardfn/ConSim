﻿//
//  WindowsModule.cs
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
using System.Text.RegularExpressions;
using System.Collections;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections.Generic;
#endregion

namespace ConSim.Windows.Module
{
  
  public class WindowsModule : Interfaces.iModule
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
    /// Standard output.
    /// </summary>
    /// <returns>The output.</returns>
    string Interfaces.iModule.standardOutput ()
    {
      return standardOutput;
    }

    /// <summary>
    /// Error output.
    /// </summary>
    /// <returns>The error output.</returns>
    string Interfaces.iModule.errorOutput ()
    {
      return errorOutput;
    }

    /// <summary>
    /// Returns the exit code.
    /// </summary>
    /// <returns>The exit code.</returns>
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
      return "Interface for the Windows Console";
    }

    /// <summary>
    /// List of supported commands.
    /// </summary>
    List<string> Interfaces.iModule.Commands ()
    {
      List<string> commands = new List<string> ();

      commands.Add ("help");
      commands.Add ("md");
      commands.Add ("mkdir");
      commands.Add ("rmdir");
      commands.Add ("nslookup");
      commands.Add ("ping");
      commands.Add ("tracert");


      return commands;
    }

    /// <summary>
    /// Some commands will never finish
    /// outputting causing an infinite loop
    /// that's bad.
    /// 
    /// This is temporary until I think of a 
    /// better way to handle it.
    /// </summary>
    /// <value>True if unsupported.</value>
    private bool unsupportedCommand (string cmdandargs)
    {
      List<string> unsupported = new List<string> ();

      unsupported.Add (@"nslookup -");
      unsupported.Add (@"^nslookup$");
      unsupported.Add (@"ping -t");

      foreach (string s in unsupported) {
        Regex rex = new Regex (s);

        if (rex.IsMatch(cmdandargs))
          return true;
      }

      return false;
    }

    /// <summary>
    /// Gets the version of the module
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
      string preppedArgs = prepareArguments (command, args);

      if (unsupportedCommand(preppedArgs))
      {
        errorOutput = "This command is unsupported by the module";
        Console.WriteLine (errorOutput);
        return;
      }

      string filename = @"cmd.exe";
      string arguments = "/C " + prepareArguments (command, args);

      Process p = prepareProcess (filename, arguments);

      try {
        p.Start ();
      } catch(Win32Exception ex) {
        //Probably not running on windows
        //This return code will pass the tests
        //When running in other environments
        if (notSupported(ex.NativeErrorCode)) {
          returnCode = 2;
          return;
        } else {
          throw ex;
        }
      }

      StreamReader sr = p.StandardOutput;
      StreamReader   srE = p.StandardError;

      while (sr.EndOfStream == false) {
        string t = sr.ReadLine ();
        this.standardOutput += t;
        Console.WriteLine (t);
      }

      while (srE.EndOfStream == false) {
        string t = srE.ReadLine ();
        this.errorOutput += t;
        Console.WriteLine (t);
      }

      sr.Close ();
      srE.Close ();
      p.Close ();

      if (errorOutput != null)
        returnCode = -1;
      if (errorOutput == null)
        returnCode = 0;
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
      string retVal = command + " ";
      foreach(string s in args) {
        retVal += @s + " ";
      }

      return retVal.Trim();
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
              && Environment.OSVersion.VersionString.Contains ("Windows") == false);
    }
    #endregion
  }
}

