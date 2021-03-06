﻿//
//  clsLesson.cs
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
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using ConSim.Lib.Interfaces;

#endregion

namespace ConSim.Lib.Classes
{
  /// <summary>
  /// Represents a collection of tasks and allowed
  /// modules
  /// </summary>
  [DataContract]
  public class clsLesson
  {

    /* DATA STRUCTURE */
    #region Data Structure

    /// <summary>
    /// Name of the lesson.
    /// </summary>
    [DataMember]
    public readonly string Name;
    /// <summary>
    /// The version of the lesson.
    /// </summary>
    [DataMember]
    public readonly string Version;
    /// <summary>
    /// The allowed modules in this lesson.
    /// </summary>
    [DataMember]
    public readonly List<clsModule> AllowedModules;
    /// <summary>
    /// A collection of tasks that make the lesson.
    /// </summary>
    [DataMember]
    public readonly List<Classes.clsTask> Tasks;
    /// <summary>
    /// Maps a module class to an iModule.
    /// </summary>
    [IgnoreDataMember]
    private Dictionary<string, Interfaces.iModule> ModuleMap;
    /// <summary>
    /// Keeps track of the currently
    /// active task.
    /// </summary>
    [IgnoreDataMember]
    public Classes.clsTask activeTask;
    /// <summary>
    /// The last standard output from an attempt.
    /// </summary>
    [IgnoreDataMember]
    public string lastStandardOutput = "";
    /// <summary>
    /// The last error output from an attempt.
    /// </summary>
    [IgnoreDataMember]
    public string lastErrorOutput = "";
    /// <summary>
    /// The last result code.
    /// </summary>
    [IgnoreDataMember]
    public int lastResultCode = 0;
    /// <summary>
    /// Tracks the array 
    /// </summary>
    [IgnoreDataMember]
    private int currentTaskNo {
      get {
        return Tasks.IndexOf (this.activeTask); 
      }
    }
    /// <summary>
    /// Gets a value indicating whether we're on the last task.
    /// </summary>
    /// <value><c>true</c> if is last task; otherwise, <c>false</c>.</value>
    [IgnoreDataMember]
    private bool isLastTask {
      get {
        return currentTaskNo == (Tasks.Count - 1);
      }
    }
    /// <summary>
    /// If there are no tasks, sandbox mode is assumed
    /// which allows you to use the environment freely
    /// </summary>
    /// <value><c>true</c> if is sandbox; otherwise, <c>false</c>.</value>
    [IgnoreDataMember]
    public bool isSandbox {
      get {
        return Tasks.Count == 0;
      }
    }
    /// <summary>
    /// The lesson path.
    /// </summary>
    [IgnoreDataMember]
    private readonly string lessonpath;

    #endregion

    /* API */
    #region API

    /// <summary>
    /// Returns the interface when given the module class.
    /// </summary>
    /// <returns>The module interface</returns>
    /// <param name="m">clsModule</param>
    public Interfaces.iModule clsToiMod(clsModule m)
    {
      try
      {
        return ModuleMap[m.filename];
      } catch (Exception) {
        throw new ArgumentOutOfRangeException 
          ("ERROR: Module is not in the list of loaded modules");
      }
    }

    /// <summary>
    /// Finds the module when given the command string.
    /// </summary>
    /// <returns>The module interface</returns>
    /// <param name="command">Command.</param>
    public Interfaces.iModule cmdToiMod(string command)
    {
      
      foreach(clsModule m in AllowedModules) {

        // Sandbox mode support.
        if (m.commands.Contains (command) && isSandbox)
          return ModuleMap[m.filename];

        // If the module contains the command and it exists
        // in the allowed commands for the task return it.
        if (m.commands.Contains(command) 
            && activeTask.allowedCommands.Contains(command))
          return ModuleMap[m.filename];
        // If the module contains the command and the
        // allowed commands for the task is an empty
        // list, assume all commands are allowed in this task.
        if (m.commands.Contains (command)
            && activeTask.allowedCommands.Count == 0)
          return ModuleMap [m.filename];
      }

      throw new ArgumentException ("ERROR: command " + command + " not found!");
    }

    /// <summary>
    /// Save the data in JSON format to the specified filepath.
    /// </summary>
    /// <param name="filepath">Filepath.</param>
    public void save(string filepath)
    {
      using (FileStream f = new FileStream (filepath, FileMode.Create)) {
        DataContractJsonSerializer ser = 
          new DataContractJsonSerializer (typeof(clsLesson));
        
        ser.WriteObject (f, this);
        f.Close ();
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Classes.clsLesson"/> class.
    /// < /summary>
    /// <param name="filepath">Path to a valid json lesson.</param>
    public clsLesson (string filepath)
    {
      clsLesson newLesson = null;

      using (FileStream f = new FileStream(filepath, FileMode.Open))
      {
        DataContractJsonSerializer ser = 
          new DataContractJsonSerializer(typeof(clsLesson));

        newLesson = (clsLesson)ser.ReadObject(f);

       }

      //Set readonly variables
      this.Name = newLesson.Name;
      this.Tasks = (List<clsTask>)newLesson.Tasks;
      this.Version = newLesson.Version;
      this.AllowedModules = newLesson.AllowedModules;
      this.lessonpath = Path.GetDirectoryName (filepath);
      this.ModuleMap = loadAllowedModules();

      try {
        this.activeTask = this.Tasks[0];
      } catch (Exception) {
        //WARNING: No tasks in this lesson, sandbox mode is active!
      }

    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Classes.clsLesson"/> class.
    /// </summary>
    /// <param name="Name">Name.</param>
    /// <param name="Version">Version.</param>
    /// <param name="AllowedModules">Allowed modules.</param>
    /// <param name="Tasks">Tasks.</param>
    /// <param name="LessonDirectory">Root directory of the lesson 
    /// (used to load modules)</param>
    public clsLesson (string Name, string Version, List<clsTask> Tasks, 
      List<clsModule> Modules, string LessonDirectory,
      bool commandToTask = false) {
     
      this.Name = Name;
      this.Version = Version;
      this.Tasks = Tasks;
      this.AllowedModules = Modules;
      this.lessonpath = LessonDirectory;
      this.ModuleMap = loadAllowedModules ();

      try {
        this.activeTask = this.Tasks [0];
      } catch (Exception) {
        //WARNING: No tasks in this lesson, sandbox mode is active!
      }
    }

    /// <summary>
    /// Attempts the task. Returns true if the lesson is finished
    /// a false return means you should refresh the task.
    /// </summary>
    /// <returns><c>true</c>, if task was attempted, 
    /// <c>false</c> otherwise.</returns>
    /// <param name="command">Command.</param>
    /// <param name="args">Arguments.</param>
    public bool attemptTask(string command, string[] args, 
      Interfaces.iModule mod)
    {
      cleanOnRun ();

      string disallowedArg = disallowedCheck (args);

      if (disallowedArg != null) {
        throw new ArgumentException
          ("ERROR: Your command contains a disallowed argument: " 
          + disallowedArg);
      }

      if (mod.unsupportedCommand(command, args))
      {
        throw new ArgumentException 
          ("This command is unsupported by the module");
      }

      mod.run (command, args);

      string comparison = mod.standardOutput ();

      lastStandardOutput = mod.standardOutput ();
      lastErrorOutput = mod.errorOutput ();
      lastResultCode = mod.resultCode ();

      if (isSandbox)
        return false;

      if (activeTask.errorToTask)
        comparison = mod.errorOutput ();
      if (activeTask.commandToTask) {
        comparison = command + " ";

        foreach (string s in args) {
          comparison += s + " ";
        }

        comparison = comparison.TrimEnd ();
      }
      if (activeTask.errorToTask && activeTask.commandToTask)
        comparison = mod.errorOutput ()
        + Environment.NewLine
        + Environment.NewLine
        + command;

      if (activeTask.hasPassed (comparison) && isLastTask)
        return true;

      if (activeTask.hasPassed (comparison) && isLastTask == false)
      {
        activeTask = Tasks [currentTaskNo + 1];
        return false;
      }

      if (activeTask.hasPassed (comparison) == false)
        return false;

      throw new Exception ("An unknown exception ocurred in the lesson flow");
    } 

    /// <summary>
    /// Lists all available commands.
    /// </summary>
    /// <returns>The commands.</returns>
    public List<string> availableCommands()
    {
      List<string> retval = new List<string> ();

      foreach (clsModule mod in AllowedModules) {
        retval.AddRange (mod.commands);
      }

      return retval;
    }

    #endregion

    /* INTERNALS */
    #region Internals

    /// <summary>
    /// Loads the allowed modules when given
    /// a list of clsModule.
    /// </summary>
    /// <returns>The allowed modules.</returns>
    /// <param name="Modules">Modules.</param>
    private Dictionary<string, Interfaces.iModule> loadAllowedModules()
    {
      Dictionary<string, Interfaces.iModule> mm = 
        new Dictionary<string, iModule> ();

      string tempPath = Path.GetTempPath ();


      foreach (clsModule m in AllowedModules) {
        string src = lessonpath + Path.DirectorySeparatorChar + "Modules" + 
          Path.DirectorySeparatorChar + m.filename;

        string dest = tempPath + m.filename;

        if (File.Exists (dest)) {
          try {
            File.Copy (src, dest, true);
          } catch (IOException) {
            // Couldn't copy the file probably
            // Because it is already in use.
          }
        } else {
          File.Copy (src, dest);
        }

        Assembly DLL = Assembly.LoadFile (dest);
        Type moduleType = DLL.GetType (m.gettype);

        ConSim.Lib.Interfaces.iModule mod = 
          (ConSim.Lib.Interfaces.iModule)Activator.CreateInstance (moduleType);

        mm.Add (m.filename, mod);
      }

      return mm;
    }

    /// <summary>
    /// Cleans before a run.
    /// </summary>
    private void cleanOnRun()
    {
      lastErrorOutput = "";
      lastStandardOutput = "";
    }

    /// <summary>
    /// Checks the arguments against the disallowed list
    /// of the task.
    /// </summary>
    /// <returns>null if allowed, the problem argument if disallowed</returns>
    /// <param name="args">Arguments.</param>
    private string disallowedCheck(string[] args)
    {
      if (activeTask == null)
        return null;

      foreach (string s in args) {
        if (activeTask.disallowedStrings.Contains (s))
          return s;
      }

      return null;
    }
    #endregion
  }
}

