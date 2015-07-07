﻿//
//  TaskTests.cs
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
using ConSim.Lib.Classes;
#endregion

namespace ConSim.NUnit
{
  [TestFixture ()]
  public class TaskTest
  {
    /* Test Variables */
    private static readonly string baseDir  = AppDomain.CurrentDomain.BaseDirectory;
    private static readonly string taskJSON = baseDir + "/Lessons/TestLesson/Tasks/TestTask.json";

    private static readonly string ExpectedResult   = "true";
    private static readonly string LongDescription  = "This is a long description";
    private static readonly string ShortDescription = "This is short";
    private static readonly string Name             = "taskness";
    private static readonly ConSim.Lib.Classes.clsTask task    = new ConSim.Lib.Classes.clsTask (Name,ShortDescription,LongDescription,ExpectedResult);


    /* TESTS */
    #region Tests
    /// <summary>
    /// Tests the task write functionality.
    /// </summary>
    [Test ()]
    public void TestTaskWrite ()
    {
      task.save (taskJSON);
      ConSim.Lib.Classes.clsTask newTask = new ConSim.Lib.Classes.clsTask (taskJSON);

      // Assert that the values on read-back are the same
      Assert.AreEqual (newTask.ExpectedResult, ExpectedResult);
      Assert.AreEqual (newTask.LongDescription, LongDescription);
      Assert.AreEqual (newTask.ShortDescription, ShortDescription);
      Assert.AreEqual (newTask.Name, Name);
    }

    /// <summary>
    /// Tests the ability the read an existing task.
    /// </summary>
    [Test ()]
    public void TestExistingTaskRead ()
    {
      ConSim.Lib.Classes.clsTask newTask = new ConSim.Lib.Classes.clsTask (taskJSON);

      // Assert that the values on read-back are the same
      Assert.AreEqual (newTask.ExpectedResult, "true");
      Assert.AreEqual (newTask.LongDescription, "This is a long description");
      Assert.AreEqual (newTask.ShortDescription, "This is short");
      Assert.AreEqual (newTask.Name, "taskness");
    }
    #endregion
  }
}

