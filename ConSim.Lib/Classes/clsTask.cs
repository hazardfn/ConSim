//
//  clsTask.cs
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

namespace ConSim.Lib.Classes
{
  /// <summary>
  /// Represents a task within a lesson.
  /// </summary>
  [DataContract]
  public class clsTask
  {
    /* DATA STRUCTURE */
    #region Data Structure
    /// <summary>
    /// The name of the task.
    /// </summary>
    [DataMember]
    public readonly string Name;
    /// <summary>
    /// The short description of the task.
    /// </summary>
    [DataMember]
    public readonly string ShortDescription;
    /// <summary>
    /// The long description can for example provide hints or the expected result.
    /// </summary>
    [DataMember]
    public readonly string LongDescription;
    /// <summary>
    /// The expected result is matched to the console output.
    /// </summary>
    [DataMember]
    public readonly object ExpectedResult; // Not shown to the user, if you think it is important add it to the long description.
    /// <summary>
    /// The allowed commands for this task.
    /// </summary>
    [DataMember]
    public readonly List<string> allowedCommands;
    /// <summary>
    /// A list of disallowed strings that can be embedded within other commands
    /// for example: IF exists 'dir' (echo 'dir') contains the following
    /// commands: IF, exists, echo - echo might not appear in the allowed
    /// commands list but this would be allowed if IF appeared in the list.
    /// 
    /// If you add echo in this list this command would not be run.
    /// </summary>
    [DataMember]
    public readonly List<string> disallowedStrings;
    /// <summary>
    /// If lazy matching is true you can check if the output
    /// contains the expected result.
    /// </summary>
    [DataMember]
    public readonly bool lazyMatching = false;
    /// <summary>
    /// Send the command when checking "hasPassed"
    /// for modules that produce no output you can 
    /// match the command instead.
    /// </summary>
    [DataMember]
    public readonly bool commandToTask = false;
    /// <summary>
    /// Send the error when checking "hasPassed"
    /// for tasks that involve simulating errors
    /// you may count an error as a pass.
    /// </summary>
    [DataMember]
    public readonly bool errorToTask = false;
    #endregion

    /* API */
    #region API
    /// <summary>
    /// Returns a true if the result supplied is the result expected.
    /// </summary>
    /// <returns><c>true</c>, if passed, <c>false</c> otherwise.</returns>
    /// <param name="result">Result.</param>
    public bool hasPassed(object result) {
      try {
        if (lazyMatching)
         return result.ToString ().Contains (ExpectedResult.ToString());
        if(Convert.ChangeType(ExpectedResult, result.GetType()).Equals(result))
         return true;
      } catch (NullReferenceException) {
        return false;
      }

      return false;
    }

    /// <summary>
    /// Save the data in JSON format to the specified filepath.
    /// </summary>
    /// <param name="filepath">Filepath.</param>
    public void save(string filepath)
    {
      using (FileStream f = new FileStream (filepath, FileMode.Create)) {
        DataContractJsonSerializer ser = new DataContractJsonSerializer (typeof(clsTask));
        ser.WriteObject (f, this);
        f.Close ();
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Classes.clsTask"/> class.
    /// < /summary>
    /// <param name="filepath">Path to a valid json task.</param>
    public clsTask (string filepath)
    {
      using (FileStream f = new FileStream (filepath, FileMode.Open)) {
        DataContractJsonSerializer ser = new DataContractJsonSerializer (typeof(clsTask));
        clsTask newTask = (clsTask)ser.ReadObject (f);

        // Set readonly variables
        this.ExpectedResult = newTask.ExpectedResult;
        this.LongDescription = newTask.LongDescription;
        this.Name = newTask.Name;
        this.ShortDescription = newTask.ShortDescription;
        this.lazyMatching = newTask.lazyMatching;
        this.commandToTask = newTask.commandToTask;
        this.errorToTask = newTask.errorToTask;
        this.allowedCommands = newTask.allowedCommands;
        this.disallowedStrings = newTask.disallowedStrings;

        if (this.allowedCommands == null)
          this.allowedCommands = new List<string> ();
        if (this.disallowedStrings == null)
          this.disallowedStrings = new List<string> ();
        
        f.Close ();
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Classes.clsTask"/> class.
    /// </summary>
    /// <param name="Name">Name.</param>
    /// <param name="ShortDescription">Short description.</param>
    /// <param name="LongDescription">Long description.</param>
    /// <param name="ExpectedResult">Expected result.</param>
    /// <param name="Module">Module used to complete this task.</param>
    public clsTask (string Name, string ShortDescription, string LongDescription, 
      object ExpectedResult, bool lazyMatching = false, 
      bool commandToTask = false, bool errorToTask = false,
      List<string> allowedCommands = null, List<string> disallowedStrings = null)
    {
      this.ExpectedResult  = ExpectedResult;
      this.LongDescription = LongDescription;
      this.Name = Name;
      this.ShortDescription = ShortDescription;
      this.lazyMatching = lazyMatching;
      this.commandToTask = commandToTask;
      this.errorToTask = errorToTask;
      this.allowedCommands = allowedCommands;
      this.disallowedStrings = disallowedStrings;

      if (this.allowedCommands == null)
        this.allowedCommands = new List<string> ();
      if (this.disallowedStrings == null)
        this.disallowedStrings = new List<string> ();
    }
      
    #endregion
  }
}

