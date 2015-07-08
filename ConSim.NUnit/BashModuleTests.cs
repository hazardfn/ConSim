//
//  BashModule.Tests.cs
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
using ConSim.Lib.Interfaces;
#endregion

namespace ConSim.Bash.Module.Tests
{
  [TestFixture ()]
  public class BashModuleTests
  {

    /* Test Variables */
    private static readonly string testDir = 
      AppDomain.CurrentDomain.BaseDirectory + "testdir";


    /* TESTS */
    #region Tests
    /// <summary>
    /// Tests the help output.
    /// </summary>
    [Test ()]
    public void TestHelpOutput()
    {
      iModule mod = new BashModule ();

      mod.run ("help", new string[0]);

      // The return code may indicate
      // the tests are not being run in an environment
      // with bash (Windows, some distros).
      if (mod.resultCode() != 2) {
        // Light check just to ensure some related output came back.
        Assert.AreEqual (mod.standardOutput ().Contains ("help"), true);
      }
    }

    /// <summary>
    /// Tests the mkdir and RM commands.
    /// </summary>
    [Test ()]
    public void TestMkDirAndRM()
    {
      // Cleanup
      if (Directory.Exists (testDir))
        Directory.Delete (testDir);

      iModule mod = new BashModule ();
     
      string[] args = new string[1];
      args [0] = testDir;

      mod.run ("mkdir", args);

      // The return code may indicate
      // the tests are not being run in an environment
      // with bash (Windows, some distros).
      if (mod.resultCode() != 2) {
        // Check the directory exists indicating success
        // Double check the return code is the expected 0
        Assert.AreEqual (Directory.Exists (testDir), true);
        Assert.AreEqual (mod.resultCode (), 0);
      }

      args = new string[2];
      args [0] = "-rf";
      args [1] = testDir;

      mod.run ("rm", args);

      // The return code may indicate
      // the tests are not being run in an environment
      // with bash (Windows, some distros).
      if (mod.resultCode() != 2) {
        // Check the directory no longer exists indicating success
        // Double check the return code is the expected 0
        Assert.AreEqual (Directory.Exists (testDir), false);
        Assert.AreEqual (mod.resultCode(), 0);
      }
      #endregion
    }
  }
}

