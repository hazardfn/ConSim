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
using System.Reflection;
using Classes;
#endregion

namespace ConSim.Shell
{
  class MainClass
  {

    private static clsLesson currentLesson;
    private static string nl = Environment.NewLine;

    public static void Main (string[] args)
    {
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

    private static string help()
    {
      return
        @"ConSim " + version() + nl
      + @"------------------------------------------------------------" + nl
      + @" -h Displays this help screen" + nl 
      + @" -l [lessonjson] Opens a lesson" + nl
      + @" -v Displays version information" + nl;
    }

    private static string version()
    {
      return Assembly.GetExecutingAssembly ().GetName ().Version.ToString () + nl;
    }

    private static clsLesson getLesson(string lessonJSON)
    {
      return new clsLesson(lessonJSON);
    }

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

    private static string lessonHeader()
    {
      return
        @"Lesson: " + currentLesson.Name + " " + currentLesson.Version + nl
      + @"------------------------------------------------------------" + nl + nl
      + @"Current Task: " + currentLesson.activeTask.Name + nl
      + @"---" + nl
      + currentLesson.activeTask.ShortDescription + nl
      + currentLesson.activeTask.LongDescription + nl
      + @"Avail. Commands: " + getAvailableCommands () + nl
      + @"Allowed Commands: " + getAllowedCommands ();

    }

    private static string[] filterLine(string[] line) {
      string[] result = new string[(line.Length - 1)];

      foreach (string s in line) {
        if (Array.IndexOf (line, s) != 0) {
          result [Array.IndexOf (line, s) - 1] = s;
        }
      }

      return result;
    }

    private static void lessonLoop() {
      while (true == true) {

        clsTask task = currentLesson.activeTask;

        Console.Write (lessonHeader() + nl + nl);
        Console.Write (">");
        string[] line = Console.ReadLine ().Split (' ');
        string command = line [0];
        string[] args = filterLine (line);

        // Lesson has been completed
        if (currentLesson.attemptTask (command, args)) {
          if (currentLesson.lastStandardOutput != null)
            Console.Write (currentLesson.lastStandardOutput + nl);
          if (currentLesson.lastErrorOutput != null)
            Console.Write (currentLesson.lastErrorOutput + nl);

          Console.Write (nl + "Congratulations! You passed the lesson!");
          Console.ReadKey ();
          Console.Clear ();
          break;
        }

        // Task was completed but still more tasks to go
        if (currentLesson.attemptTask (command, args) == false
            && task.Equals (currentLesson.activeTask) == false) {
          if (currentLesson.lastStandardOutput != null)
            Console.Write (currentLesson.lastStandardOutput + nl);
          if (currentLesson.lastErrorOutput != null)
            Console.Write (currentLesson.lastErrorOutput + nl);

          Console.Write (nl + "Congratulations! You passed this task!");
          Console.ReadKey ();
          Console.Clear ();
        }

        // Attempt was unsuccessful
        if (currentLesson.attemptTask (command, args) == false
            && task.Equals (currentLesson.activeTask)) {
          if (currentLesson.lastStandardOutput != null)
            Console.Write (currentLesson.lastStandardOutput + nl);
          if (currentLesson.lastErrorOutput != null)
            Console.Write (currentLesson.lastErrorOutput + nl);

          Console.Write (nl + "Unfortunately this was not the expected output :(. Try Again!");
          Console.ReadKey ();
          Console.Clear ();

        }
      }
    }
  }
}
