//
//  LessonTests.cs
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
using NUnit.Framework;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
#endregion

namespace ConSim.NUnit
{
  [TestFixture()]
  public class LessonTests
  {
    /* Location Variables */
    #region Location Variables
    private static readonly string baseDirectory     = AppDomain.CurrentDomain.BaseDirectory;
    private static readonly string taskJSON          = baseDirectory + "/Lessons/TestLesson/Tasks/TestTask.json";
    private static readonly string lessonJSON        = baseDirectory + "/Lessons/TestLesson/TestLesson.json";
    private static readonly string sandboxLessonJSON = baseDirectory + "/Lessons/TestSandboxLesson/TestSandboxLesson.json";
    private static readonly string lessonDir         = Path.GetDirectoryName (lessonJSON);
    private static readonly string sandboxLessonDir  = Path.GetDirectoryName (sandboxLessonJSON);
    #endregion

    /* Test Variables */
    #region Test Variables
    private static readonly string Name       = "TestLesson";
    private static readonly string Version    = "TEST";
    private static readonly string ModuleType = "Modules.TestModule";
    private static readonly string DLLName    = "ConSim.Test.Module.dll";

    private static readonly string lessonModule = lessonDir + "/Modules/" + DLLName;

    private static readonly Classes.clsTask task     = new Classes.clsTask(taskJSON);
    private static readonly Classes.clsModule module = new Classes.clsModule(ModuleType, DLLName);
    #endregion

    /* TESTS */
    #region Tests
    /// <summary>
    /// Tests the sandbox lesson flow.
    /// </summary>
    [Test ()]
    public void TestSandboxLessonFlow()
    {
      // Copy the module file for sandbox tests
      if (Directory.Exists (sandboxLessonDir + "/Modules/") == false)
        Directory.CreateDirectory (sandboxLessonDir + "/Modules/");
      
      File.Copy (lessonModule, sandboxLessonDir + "/Modules/" + DLLName, true);

      List<Classes.clsTask> Tasks = new List<Classes.clsTask> ();
      List<Classes.clsModule> AllowedModules = new List<Classes.clsModule> ();

      AllowedModules.Add (module);

      Classes.clsLesson lesson = new Classes.clsLesson (Name, Version, Tasks, AllowedModules, sandboxLessonDir);
      lesson.save (sandboxLessonJSON);

      lesson = new Classes.clsLesson (sandboxLessonJSON);

      string[] args = new string[1];

      //Test multiple commands and ensure the expected output
      args[0] = "1";
      Assert.AreEqual(lesson.attemptTask("increment", args), false);
      Assert.AreEqual (lesson.lastStandardOutput, "2");
      args[0] = "random";
      Assert.AreEqual (lesson.attemptTask ("increment", args), false);
      Assert.AreEqual (lesson.lastErrorOutput, "Unexpected format in arguments");
    }


    /// <summary>
    /// Tests the lesson write functionality.
    /// </summary>
    [Test ()]
    public void TestLessonWrite()
    {
      List<Classes.clsTask> Tasks = new List<Classes.clsTask> ();
      List<Classes.clsModule> AllowedModules = new List<Classes.clsModule> ();

      Tasks.Add (task);
      AllowedModules.Add (module);

      Classes.clsLesson lesson = new Classes.clsLesson (Name, Version, Tasks, AllowedModules,lessonDir);
      lesson.save (lessonJSON);

      lesson = new Classes.clsLesson (lessonJSON);

      // Test that the lesson was read back correctly
      Assert.AreEqual (lesson.Name, Name);
      Assert.AreEqual (lesson.Version, Version);
      Assert.AreEqual (lesson.activeTask.ExpectedResult, Tasks [0].ExpectedResult);
      Assert.AreEqual (lesson.activeTask.LongDescription, Tasks [0].LongDescription);
      Assert.AreEqual (lesson.activeTask.Name, Tasks [0].Name);
      Assert.AreEqual (lesson.activeTask.ShortDescription, Tasks [0].ShortDescription);
      Assert.AreEqual (lesson.AllowedModules [0].filename, module.filename);
      Assert.AreEqual (lesson.AllowedModules [0].gettype, module.gettype);
    }

    /// <summary>
    /// Tests the loading of modules.
    /// </summary>
    [Test ()]
    public void TestModuleLoad()
    {
      Classes.clsLesson lesson = new Classes.clsLesson (lessonJSON);

      Interfaces.iModule testMod = lesson.clsToiMod(lesson.AllowedModules[0]);

      // Test the loaded module returns expected name
      // and version.
      Assert.AreEqual (testMod.getName (), "TestModule");
      Assert.AreEqual (testMod.getVersion (), "TEST");

      string[] args = new string[1];
      args[0] = "1";

      testMod.run ("increment", args);

      // Test that the increment module returns the
      // expected result.
      Assert.AreEqual (testMod.standardOutput(), "2");

      args[0] = "Invalid";

      testMod.run ("increment", args);

      // Test that the increment module outputs to
      // the error stream when given a weird
      // value.
      Assert.AreEqual (testMod.errorOutput(), "Unexpected format in arguments");
    }

    /// <summary>
    /// Tests the lesson flow.
    /// </summary>
    [Test ()]
    public void TestLessonFlow()
    {
      List<Classes.clsTask> Tasks = new List<Classes.clsTask> ();
      List<Classes.clsModule> AllowedModules = new List<Classes.clsModule> ();

      Tasks.Add (new Classes.clsTask (task.Name, task.ShortDescription, task.LongDescription, 2));
      Tasks.Add (new Classes.clsTask (task.Name, task.ShortDescription, task.LongDescription, 3));

      AllowedModules.Add (module);

      Classes.clsLesson lesson = new Classes.clsLesson (Name, Version, Tasks, AllowedModules, lessonDir);
      lesson.save (lessonJSON);

      lesson = new Classes.clsLesson (lessonJSON);

      string[] args = new string[1];

      //Test a failed try returns false
      args[0] = lesson.activeTask.ExpectedResult.ToString();
      Assert.AreEqual(lesson.attemptTask("increment", args), false);

      //Test a weird value still returns false
      args[0] = "random";
      Assert.AreEqual (lesson.attemptTask ("increment", args), false);

      //Test the right value returns false but advances the task
      args[0] = (Convert.ToInt32(lesson.activeTask.ExpectedResult) - 1).ToString();
      Assert.AreEqual (lesson.attemptTask ("increment", args), false);
      Assert.AreNotEqual (lesson.activeTask.ExpectedResult, 2);

      //Test the final task completion returns true
      args[0] = (Convert.ToInt32(lesson.activeTask.ExpectedResult) - 1).ToString();
      Assert.AreEqual (lesson.attemptTask ("increment", args), true);
    }
    #endregion
  }
}

