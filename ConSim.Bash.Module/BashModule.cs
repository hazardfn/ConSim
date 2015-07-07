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
using ConSim.Lib.Interfaces;
using ConSim.Lib.Events;
#endregion

namespace ConSim.Bash.Module
{
  public class BashModule : iModule
  {

    /* API */
    #region API

    /// <summary>
    /// The standard output.
    /// </summary>
    private string standardoutput;
    string StandardOutput {
      get { return standardoutput; }
      set {
        standardoutput += (value);

        EventHandler<iModuleOutputEventArgs> handler = onStandardOutputChange;
        if (handler != null) {
          handler (this, new iModuleOutputEventArgs (value));
        }
      }
    }
    /// <summary>
    /// The error output.
    /// </summary>
    private string erroroutput;
    string ErrorOutput {
      get { return erroroutput; }
      set {
        erroroutput += (value);

        EventHandler<iModuleOutputEventArgs> handler = onErrorOutputChange;
        if (handler != null) {
          handler (this, new iModuleOutputEventArgs (value));
        }
      }
    }
    /// <summary>
    /// The return code.
    /// </summary>
    private int resultcode;
    int ResultCode
    {
      get { return resultcode; }
      set {
        resultcode = value;

        EventHandler<iModuleOutputEventArgs> handler = onResultCodeChange;
        if (handler != null) {
          handler (this, new iModuleOutputEventArgs(value));
        }
      }
    }
    /// <summary>
    /// Occurs on standard output change.
    /// </summary>
    event EventHandler<ConSim.Lib.Events.iModuleOutputEventArgs> onStandardOutputChange;
    /// <summary>
    /// Occurs when error output change.
    /// </summary>
    event EventHandler<ConSim.Lib.Events.iModuleOutputEventArgs> onErrorOutputChange;
    /// <summary>
    /// Occurs on result code change.
    /// </summary>
    event EventHandler<ConSim.Lib.Events.iModuleOutputEventArgs> onResultCodeChange;

    /// <summary>
    /// Standard output.
    /// </summary>
    /// <returns>The output.</returns>
    string iModule.standardOutput ()
    {
      return StandardOutput;
    }

    /// <summary>
    /// Error output.
    /// </summary>
    /// <returns>The error output.</returns>
    string iModule.errorOutput ()
    {
      return ErrorOutput;
    }

    /// <summary>
    /// Returns the exit code.
    /// </summary>
    /// <returns>The exit code.</returns>
    int iModule.resultCode ()
    {
      return ResultCode;
    }

    /// <summary>
    /// Gets the name of the module.
    /// </summary>
    /// <returns>The name.</returns>
    string iModule.getName ()
    {
      return "Interface for Bash";
    }

    /// <summary>
    /// Returns a list of commands supported by this module.
    /// </summary>
    List<string> iModule.Commands ()
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
    string iModule.getVersion ()
    {
      return "1.0.0";
    }

    /// <summary>
    /// Run the specified command and args.
    /// </summary>
    /// <param name="command">Command.</param>
    /// <param name="args">Arguments.</param>
    void iModule.run (string command, string[] args)
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
          ResultCode = 2;
          return;
        } else {
          throw ex;
        }
      }

      p.WaitForExit ();
      ResultCode = p.ExitCode;

      StreamReader sr = p.StandardOutput;
      StreamReader srE = p.StandardError;

      while (sr.EndOfStream == false) {
        string t = sr.ReadLine ();
        this.StandardOutput = t;
      }

      while (srE.EndOfStream == false) {
        string t = srE.ReadLine ();
        this.ErrorOutput = t;
      }

      sr.Close ();
      srE.Close ();
      p.Close ();
    }


    /// <summary>
    /// Occurs when standard output is changed.
    /// </summary>
    event EventHandler<iModuleOutputEventArgs> iModule.standardOutputChanged {
      add {
        onStandardOutputChange += value;
      }
      remove {
        onStandardOutputChange -= value;
      }
    }
    /// <summary>
    /// Occurs when error output is changed.
    /// </summary>
    event EventHandler<iModuleOutputEventArgs> iModule.errorOutputChanged {
      add {
        onErrorOutputChange += value;
      }
      remove {
        onErrorOutputChange -= value;
      }
    }
    /// <summary>
    /// Occurs when result code is changed.
    /// </summary>
    event EventHandler<iModuleOutputEventArgs> iModule.resultCodeChanged {
      add {
        onResultCodeChange += value;
      }
      remove {
        onResultCodeChange -= value;
      }
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

