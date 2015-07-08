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
using ConSim.Lib;
#endregion

namespace ConSim.NUnit
{
  [TestFixture()]
  public class LessonTests
  {
    /* Location Variables */
    #region Location Variables
    private static readonly char ps                  = 
      Path.DirectorySeparatorChar;
    
    private static readonly string baseDirectory     = 
      AppDomain.CurrentDomain.BaseDirectory;

    private static readonly string taskJSON          = 
      baseDirectory + ps + "Lessons" + ps + "TestLesson" + ps + "Tasks" + 
      ps + "TestTask.json";

    private static readonly string lessonJSON        = 
      baseDirectory + ps + "Lessons" + ps + "TestLesson" + 
      ps + "TestLesson.json";

    private static readonly string sandboxLessonJSON = 
      baseDirectory + ps + "Lessons" + ps + "TestSandboxLesson" + 
      ps + "TestSandboxLesson.json";

    private static readonly string lessonDir         = 
      Path.GetDirectoryName (lessonJSON);

    private static readonly string sandboxLessonDir  = 
      Path.GetDirectoryName (sandboxLessonJSON);
    #endregion

    /* Test Variables */
    #region Test Variables
    private static readonly string Name       = "TestLesson";
    private static readonly string Version    = "TEST";
    private static readonly string ModuleType = "ConSim.TestModule";
    private static readonly string DLLName    = "ConSim.Test.Module.dll";

    private static readonly string lessonModule = lessonDir + ps + "Modules" + 
      ps + DLLName;

    private static readonly ConSim.Lib.Classes.clsTask task     = 
      new ConSim.Lib.Classes.clsTask(taskJSON);

    private static List<string> commands = new List<string>();
    private static ConSim.Lib.Classes.clsModule module;
    #endregion

    [TestFixtureSetUp ()]
    public void setupFixture()
    {
      commands.Add ("increment");
      module = new ConSim.Lib.Classes.clsModule(ModuleType, DLLName, commands);
    }

    /* TESTS */
    #region Tests
    /// <summary>
    /// Tests the sandbox lesson flow.
    /// </summary>
    [Test ()]
    public void TestSandboxLessonFlow()
    {
      // Copy the module file for sandbox tests
      if (Directory.Exists (sandboxLessonDir + ps + "Modules" + ps) == false)
        Directory.CreateDirectory (sandboxLessonDir + ps + "Modules" + ps);
      
      File.Copy (lessonModule, sandboxLessonDir + ps + "Modules" + 
        ps + DLLName, true);

      List<ConSim.Lib.Classes.clsTask> Tasks = 
        new List<ConSim.Lib.Classes.clsTask> ();

      List<ConSim.Lib.Classes.clsModule> AllowedModules = 
        new List<ConSim.Lib.Classes.clsModule> ();

      AllowedModules.Add (module);

      ConSim.Lib.Classes.clsLesson lesson = 
        new ConSim.Lib.Classes.clsLesson (Name, Version, Tasks, AllowedModules, 
          sandboxLessonDir);

      lesson.save (sandboxLessonJSON);

      lesson = new ConSim.Lib.Classes.clsLesson  (sandboxLessonJSON);

      string[] args = new string[1];

      //Test multiple commands and ensure the expected output
      args[0] = "1";
      Assert.AreEqual(lesson.attemptTask("increment", args, 
        lesson.clsToiMod(module)), false);
      Assert.AreEqual (lesson.lastStandardOutput, "2");
      args[0] = "random";
      Assert.AreEqual (lesson.attemptTask ("increment", args, 
        lesson.clsToiMod(module)), false);
      Assert.AreEqual (lesson.lastErrorOutput, 
        "Unexpected format in arguments");
    }


    /// <summary>
    /// Tests the lesson write functionality.
    /// </summary>
    [Test ()]
    public void TestLessonWrite()
    {
      List<ConSim.Lib.Classes.clsTask> Tasks = 
        new List<ConSim.Lib.Classes.clsTask> ();
      List<ConSim.Lib.Classes.clsModule> AllowedModules = 
        new List<ConSim.Lib.Classes.clsModule> ();

      Tasks.Add (task);
      AllowedModules.Add (module);

      ConSim.Lib.Classes.clsLesson lesson = 
        new ConSim.Lib.Classes.clsLesson (Name, Version, Tasks, AllowedModules,lessonDir);
      lesson.save (lessonJSON);

      lesson = new ConSim.Lib.Classes.clsLesson (lessonJSON);

      // Test that the lesson was read back correctly
      Assert.AreEqual (lesson.Name, Name);
      Assert.AreEqual (lesson.Version, Version);
      Assert.AreEqual (lesson.activeTask.ExpectedResult, 
        Tasks [0].ExpectedResult);
      Assert.AreEqual (lesson.activeTask.LongDescription, 
        Tasks [0].LongDescription);
      Assert.AreEqual (lesson.activeTask.Name, Tasks [0].Name);
      Assert.AreEqual (lesson.activeTask.ShortDescription, 
        Tasks [0].ShortDescription);
      Assert.AreEqual (lesson.AllowedModules [0].filename, module.filename);
      Assert.AreEqual (lesson.AllowedModules [0].gettype, module.gettype);
    }

    /// <summary>
    /// Tests the loading of modules.
    /// </summary>
    [Test ()]
    public void TestModuleLoad()
    {
      ConSim.Lib.Classes.clsLesson lesson = 
        new ConSim.Lib.Classes.clsLesson (lessonJSON);

      ConSim.Lib.Interfaces.iModule testMod = 
        lesson.clsToiMod(lesson.AllowedModules[0]);

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
      List<ConSim.Lib.Classes.clsTask> Tasks = 
        new List<ConSim.Lib.Classes.clsTask> ();

      List<ConSim.Lib.Classes.clsModule> AllowedModules = 
        new List<ConSim.Lib.Classes.clsModule> ();

      Tasks.Add (
        new ConSim.Lib.Classes.clsTask (task.Name, task.ShortDescription, 
          task.LongDescription, "2")
      );

      Tasks.Add (
        new ConSim.Lib.Classes.clsTask (task.Name, task.ShortDescription, 
          task.LongDescription, "3")
      );

      AllowedModules.Add (module);

      ConSim.Lib.Classes.clsLesson lesson = 
        new ConSim.Lib.Classes.clsLesson (Name, Version, Tasks, AllowedModules, 
          lessonDir);
      
      lesson.save (lessonJSON);

      lesson = new ConSim.Lib.Classes.clsLesson (lessonJSON);

      string[] args = new string[1];

      //Test a failed try returns false
      args[0] = lesson.activeTask.ExpectedResult.ToString();
      Assert.AreEqual(lesson.attemptTask("increment", args, 
        lesson.clsToiMod(module)), false);

      //Test a weird value still returns false
      args[0] = "random";
      Assert.AreEqual (lesson.attemptTask ("increment", args, 
        lesson.clsToiMod(module)), false);

      //Test the right value returns false but advances the task
      args[0] = (Convert.ToInt32(lesson.activeTask.ExpectedResult) - 1).
        ToString();
      Assert.AreEqual (lesson.attemptTask ("increment", args, 
        lesson.clsToiMod(module)), false);
      
      Assert.AreNotEqual (lesson.activeTask.ExpectedResult, 2);

      //Test the final task completion returns true
      args[0] = (Convert.ToInt32(lesson.activeTask.ExpectedResult) - 1).
        ToString();

      Assert.AreEqual (lesson.attemptTask ("increment", args, 
        lesson.clsToiMod(module)), true);
    }

    [Test ()]
    public void TestCommandMatch() {
      List<ConSim.Lib.Classes.clsTask> Tasks = 
        new List<ConSim.Lib.Classes.clsTask> ();

      List<ConSim.Lib.Classes.clsModule> AllowedModules = 
        new List<ConSim.Lib.Classes.clsModule> ();

      Tasks.Add (
        new ConSim.Lib.Classes.clsTask (task.Name, task.ShortDescription, 
          task.LongDescription, "increment 6",false,false,true, false, null, 
          null)
      );

      AllowedModules.Add (module);

      ConSim.Lib.Classes.clsLesson lesson = 
        new ConSim.Lib.Classes.clsLesson (Name, Version, Tasks, AllowedModules, 
          lessonDir);

      string[] args = new string[1];

      //Fail Test
      args[0] = "4";
      Assert.AreEqual (lesson.attemptTask ("increment", args, 
        lesson.clsToiMod (module)), false);
      //Success Test
      args[0] = "6";
      Assert.AreEqual (lesson.attemptTask ("increment", args, 
        lesson.clsToiMod (module)), true);

    }
    #endregion
  }
}

