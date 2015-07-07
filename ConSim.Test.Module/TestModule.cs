//
//  TestModule.cs
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
using System.Collections;
using System.Collections.Generic;
using ConSim.Lib.Interfaces;
using ConSim.Lib.Events;
#endregion

namespace ConSim
{
  public class TestModule : iModule
  {
    /* API */
    #region API
    /// <summary>
    /// The result from the run.
    /// </summary>
    private string result;
    public string Result {
      get { return result; }
      set {
        result += (value);

        EventHandler<iModuleOutputEventArgs> handler = onStandardOutputChange;
        if (handler != null) {
          handler (this, new iModuleOutputEventArgs (value));
        }
      }
    }
    /// <summary>
    /// Any error output.
    /// </summary>
    private string error;
    public string Error
    {
      get { return error; }
      set {
        error += (value);
 
        EventHandler<iModuleOutputEventArgs> handler = onErrorOutputChange;
        if (handler != null) {
          handler (this, new iModuleOutputEventArgs (value));
        }
      }
    }
    /// <summary>
    /// Occurs on standard output change.
    /// </summary>
    event EventHandler<iModuleOutputEventArgs> onStandardOutputChange;
    /// <summary>
    /// Occurs when error output change.
    /// </summary>
    event EventHandler<iModuleOutputEventArgs> onErrorOutputChange;
    /// <summary>
    /// Occurs on result code change.
    /// </summary>
    event EventHandler<iModuleOutputEventArgs> onResultCodeChange;

    /// <summary>
    /// Standard output.
    /// </summary>
    /// <returns>The output.</returns>
    string iModule.standardOutput ()
    {
      return Result;
    }

    /// <summary>
    /// Error output.
    /// </summary>
    /// <returns>The error output.</returns>
    string iModule.errorOutput ()
    {
      return Error;
    }

    /// <summary>
    /// Exit code of any external process.
    /// </summary>
    /// <returns>The return code.</returns>
    int iModule.resultCode ()
    {
      return 0;
    }

    /// <summary>
    /// Gets the module name.
    /// </summary>
    /// <returns>The module name.</returns>
    string iModule.getName ()
    {
      return "TestModule";
    }

    /// <summary>
    /// Gets the module version.
    /// </summary>
    /// <returns>The module version.</returns>
    string iModule.getVersion ()
    {
      return "TEST";
    }

    /// <summary>
    /// List of supported commands.
    /// </summary>
    List<string> iModule.Commands ()
    {
      List<string> commands = new List<string> ();

      commands.Add ("increment");

      return commands;
    }

    /// <summary>
    /// The test module accepts a number and increments it by one.
    /// </summary>
    /// <param name="args">Arguments.</param>
    void iModule.run (string command, string[] args)
    {
      error = "";
      result = "";

      int number = 0;

      try {
        number = Convert.ToInt32(args[0]);
      } catch (FormatException) {
        this.Error = "Unexpected format in arguments";
        return;
      } catch (Exception ex) {
        this.Error = ex.Message;
        return;
      }

      Result = (number + 1).ToString ();
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
  }
}
