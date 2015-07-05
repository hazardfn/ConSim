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
#endregion

namespace Modules
{
  public class TestModule : Interfaces.iModule
  {
    /* API */
    #region API
    /// <summary>
    /// The result from the run.
    /// </summary>
    string result;
    /// <summary>
    /// Any error output.
    /// </summary>
    string error;

    /// <summary>
    /// Standard output.
    /// </summary>
    /// <returns>The output.</returns>
    string Interfaces.iModule.standardOutput ()
    {
      return result;
    }

    /// <summary>
    /// Error output.
    /// </summary>
    /// <returns>The error output.</returns>
    string Interfaces.iModule.errorOutput ()
    {
      return error;
    }

    /// <summary>
    /// Exit code of any external process.
    /// </summary>
    /// <returns>The return code.</returns>
    int Interfaces.iModule.returnCode ()
    {
      throw new NotImplementedException ();
    }

    /// <summary>
    /// Gets the module name.
    /// </summary>
    /// <returns>The module name.</returns>
    string Interfaces.iModule.getName ()
    {
      return "TestModule";
    }

    /// <summary>
    /// Gets the module version.
    /// </summary>
    /// <returns>The module version.</returns>
    string Interfaces.iModule.getVersion ()
    {
      return "TEST";
    }

    /// <summary>
    /// List of supported commands.
    /// </summary>
    List<string> Interfaces.iModule.Commands ()
    {
      List<string> commands = new List<string> ();

      commands.Add ("increment");

      return commands;
    }

    /// <summary>
    /// The test module accepts a number and increments it by one.
    /// </summary>
    /// <param name="args">Arguments.</param>
    void Interfaces.iModule.run (string command, string[] args)
    {
      int number = 0;

      try {
        number = Convert.ToInt32(args[0]);
      } catch (FormatException) {
        this.error = "Unexpected format in arguments";
      } catch (Exception ex) {
        this.error = ex.Message;
      }

      result = (number + 1).ToString ();
    }
    #endregion
  }
}
