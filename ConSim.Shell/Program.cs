//
//  Program.cs
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
using System.Reflection;
using ConSim.Lib.Interfaces;
using ConSim.Lib.Events;
using ConSim.Lib.Classes;
#endregion

namespace ConSim.Shell
{
  class MainClass
  {
    /* VARIABLES */
    #region Variables
    /// <summary>
    /// The current lesson.
    /// </summary>
    private static clsLesson currentLesson;
    /// <summary>
    /// Shorthand for newline.
    /// </summary>
    private static string nl = Environment.NewLine;
    #endregion

    /* ARGUMENT CODE */
    #region Argument Code
    /// <summary>
    /// The entry point of the program, where the program control starts and 
    /// ends.
    /// </summary>
    /// <param name="args">The command-line arguments.
    ///  -l [jsonfile]
    ///  -h help
    ///  -v version
    /// </param>
    public static void Main (string[] args)
    {

      if (args.Length == 0) {
        Console.Write (help ());
        return;
      }
        
      switch (args [0].ToString().Trim()) {
      case "-l":
        currentLesson = getLesson (args [1].ToString ());
        lessonLoop ();
        break;
      case "-h":
        Console.Write (help ());
        break;
      case "-v":
        Console.Write (version ());
        break;
      default:
        Console.WriteLine ("Command not recognized, see help documentation:\n");
        Console.Write(help ());
        break;
      }
    }
    /// <summary>
    /// Displays the help section.
    /// </summary>
    private static string help()
    {
      return
        @"ConSim " + version() + nl
      + @"------------------------------------------------------------" + nl
      + @" -h Displays this help screen" + nl 
      + @" -l [lessonjson] Opens a lesson" + nl
      + @" -v Displays version information" + nl;
    }
    /// <summary>
    /// Displays the version.
    /// </summary>
    private static string version()
    {
      return "ConSim - " + 
        Assembly.GetExecutingAssembly ().GetName ().Version.ToString () + nl;
    }
    /// <summary>
    /// Gets the lesson.
    /// </summary>
    /// <returns>The lesson.</returns>
    /// <param name="lessonJSON">Lesson JSON</param>
    private static ConSim.Lib.Classes.clsLesson getLesson(string lessonJSON)
    {
      // If you don't provide a full path
      // to clsLesson it errors out due 
      // to a requirement of loadAssembly
      if (File.Exists (lessonJSON)) {
        FileInfo f = new FileInfo (lessonJSON);
        return new ConSim.Lib.Classes.clsLesson (f.FullName);
      }

      throw new FileNotFoundException ("Could not find: " + lessonJSON);
    }
    #endregion

    /* INTERNALS */
    #region Internals
    /// <summary>
    /// Gets the allowed commands.
    /// </summary>
    /// <returns>The allowed commands.</returns>
    private static string getAllowedCommands()
    {
      string result = "";

      char[] trimChars = new char[2];
      trimChars [0] = ' ';
      trimChars [1] = ',';

      foreach (string s in currentLesson.activeTask.allowedCommands) {
        result += s + ", ";
      }

      if(result != "")
        return result.TrimEnd(trimChars);
      
      return "All";
    }
    /// <summary>
    /// Gets the available commands.
    /// </summary>
    /// <returns>The available commands.</returns>
    private static string getAvailableCommands()
    {
      string result = "";

      char[] trimChars = new char[2];
      trimChars [0] = ' ';
      trimChars [1] = ',';

      foreach (string s in currentLesson.availableCommands()) {
        result += s + ", ";
      }

      if(result != "")
        return result.TrimEnd(trimChars);

      return "";
    }
    /// <summary>
    /// Prints a lesson header.
    /// </summary>
    /// <returns>The header.</returns>
    private static string lessonHeader()
    {
      if (currentLesson.isSandbox == false) {
        return
        nl + @"Lesson: " + currentLesson.Name 
        + @" | Task: " + currentLesson.activeTask.Name + nl + nl
        + @"What To Do: " + nl
        + @"===================================================================" 
        + nl
        + currentLesson.activeTask.ShortDescription + nl + nl
        + currentLesson.activeTask.LongDescription + nl + nl
        + @"Available Commands: " + nl
        + @"==================================================================="
        + nl
        + getAvailableCommands() + nl + nl
        + @"Allowed Commands: " + nl 
        + @"==================================================================="
        + nl 
        + getAllowedCommands ();
      }

      return
      nl + @"Lesson: " + currentLesson.Name + nl + nl
      + @"Available Commands: " + nl
      + @"====================================================================="
      + nl
      + getAvailableCommands () + nl;
    }
    /// <summary>
    /// Filters the line of a command.
    /// </summary>
    /// <returns>The line.</returns>
    /// <param name="line">Line.</param>
    private static string[] filterLine(string[] line) {
      string[] result = new string[(line.Length - 1)];

      foreach (string s in line) {
        if (Array.IndexOf (line, s) != 0) {
          result [Array.IndexOf (line, s) - 1] = s;
        }
      }

      return result;
    }
    /// <summary>
    /// Loop during a lesson until finished.
    /// </summary>
    private static void lessonLoop() {
      while (true == true) {

        ConSim.Lib.Classes.clsTask task = null; 

        if (currentLesson.isSandbox == false) {
          task = currentLesson.activeTask;
        }

        Console.Write (lessonHeader () + nl + nl);
        Console.Write (">");

        string[] line = Console.ReadLine ().Split (' ');
        string command = line [0];
        string[] args = filterLine (line);
        bool attemptTask = false;
        bool boolBreak = false;

        try {
          ConSim.Lib.Interfaces.iModule mod = currentLesson.cmdToiMod (command);

          mod.errorOutputChanged += 
            new EventHandler<iModuleOutputEventArgs>(onErrorOutputChange);
          mod.standardOutputChanged += 
            new EventHandler<iModuleOutputEventArgs>(onStandardOutputChange);
          mod.resultCodeChanged += 
            new EventHandler<iModuleOutputEventArgs>(onResultChange);

          attemptTask = currentLesson.attemptTask (command, args, mod);

          mod.errorOutputChanged -= onErrorOutputChange;
          mod.standardOutputChanged -= onStandardOutputChange;
          mod.resultCodeChanged -= onResultChange;

        } catch (Exception ex) {
          currentLesson.lastErrorOutput = ex.Message;
          Console.WriteLine (nl + ex.Message);
        }
          
        // Lesson has been completed
        if (attemptTask && currentLesson.isSandbox == false) {
          Console.Write (nl + "Congratulations! You passed the lesson!");
          boolBreak = true;
        }

        // Task was completed but still more tasks to go
        if (attemptTask == false
            && currentLesson.isSandbox == false
            && task.Equals (currentLesson.activeTask) == false) {

          Console.Write (nl + "Congratulations! You passed this task!");
        }

        // Attempt was unsuccessful
        if (attemptTask == false
            && currentLesson.isSandbox == false
            && task.Equals (currentLesson.activeTask)) {

          Console.Write (nl + "Unfortunately this was not the expected " +
            "output :(. Try Again!");
        }
          
        if (currentLesson.isSandbox == false) {
          Console.ReadKey ();
          Console.Clear ();
        }

        if (boolBreak)
          break;
      }
    }
    #endregion

    /* EVENTS */
    #region Events
    /// <summary>
    /// Event is fired when the standard output changes of a module.
    /// </summary>
    /// <param name="sender">Sender.</param>
    /// <param name="e">E.</param>
    private static void onStandardOutputChange(object sender, 
      iModuleOutputEventArgs e) 
    {
      Console.WriteLine(e.output);
    }
    /// <summary>
    /// Event is fired when the error output changes of a module.
    /// </summary>
    /// <param name="sender">Sender.</param>
    /// <param name="e">E.</param>
    private static void onErrorOutputChange(object sender, 
      iModuleOutputEventArgs e) 
    {
      Console.WriteLine(e.output);
    }
    /// <summary>
    /// Event is fired when a result code is given to the module.
    /// </summary>
    /// <param name="sender">Sender.</param>
    /// <param name="e">E.</param>
    private static void onResultChange(object sender, 
      iModuleOutputEventArgs e) {
      // We do not want to print result codes in this shell.
    }
    #endregion
  }
}
