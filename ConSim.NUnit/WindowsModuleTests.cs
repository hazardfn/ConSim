//
//  WindowsModule.Tests.cs
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
using ConSim.Lib.Events;
#endregion

namespace ConSim.Windows.Module
{
  [TestFixture ()]
  public class WindowsModuleTests
  {

    /* Test Variables */
    private static readonly string testDir = 
      AppDomain.CurrentDomain.BaseDirectory + "testdir";

    private static string output;

    private void onStandardChange(object sender, iModuleOutputEventArgs e) {
      output += e.output.ToString();
    }

    private void onErrorChange(object sender, iModuleOutputEventArgs e) {
      output += e.output.ToString();
    }

    /* TESTS */
    #region Tests
    [Test ()]
    public void TestUnsupported()
    {
      iModule mod = new WindowsModule ();

      Assert.True(mod.unsupportedCommand("nslookup", new string[0]));
    }
    /// <summary>
    /// Tests the nslookup output.
    /// </summary>
    [Test ()]
    public void TestNsLookupOutput()
    {
      iModule mod = new WindowsModule ();

      string[] args = new string[1];
      args [0] = "host";

      mod.run ("nslookup", args);

      // The return code may indicate
      // the tests are not being run in an environment
      // with cmd (e.g. Anything but Windows).
      if (mod.resultCode() != 2) {
        // Light check just to ensure some related output came back.
        Assert.AreEqual (mod.standardOutput ().Contains ("Address"), true);
      }
    }

    /// <summary>
    /// Tests the ping output.
    /// </summary>
    [Test ()]
    public void TestPingOutput()
    {
      iModule mod = new WindowsModule ();

      string[] args = new string[1];
      args[0] = "127.0.0.1";

      mod.run ("ping", args);

      // The return code may indicate
      // the tests are not being run in an environment
      // with cmd (e.g. Anything but Windows).
      if (mod.resultCode() != 2) {
        // Light check just to ensure some related output came back.
        Assert.AreEqual (mod.standardOutput ().Contains ("Pinging"), true);
      }
    }

    [Test ()]
    public void TestTracertOutput()
    {
      iModule mod = new WindowsModule ();

      string[] args = new string[1];
      args [0] = "127.0.0.1";

      mod.run ("tracert", args);

      // The return code may indicate
      // the tests are not being run in an environment
      // with cmd (e.g. Anything but Windows).
      if (mod.resultCode() != 2) {
        // Light check just to ensure some related output came back.
        Assert.AreEqual (mod.standardOutput ().Contains ("Tracing"), true);
      }
    }

    /// <summary>
    /// Tests the help output.
    /// </summary>
    [Test ()]
    public void TestHelpOutput()
    {
      iModule mod = new WindowsModule ();

      mod.run ("help", new string[0]);

      // The return code may indicate
      // the tests are not being run in an environment
      // with cmd (e.g. Anything but Windows).
      if (mod.resultCode() != 2) {
        // Light check just to ensure some related output came back.
        Assert.AreEqual (mod.standardOutput ().Contains ("HELP"), true);
      }
    }

    /// <summary>
    /// Tests the mk dir and RM dir functions.
    /// </summary>
    [Test ()]
    public void TestMkDirAndRMDir()
    {
      // Cleanup
      if (Directory.Exists (testDir))
        Directory.Delete (testDir);

      iModule mod = new WindowsModule ();

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

      args = new string[3];
      args [0] = "/s";
      args [1] = "/q";
      args [2] = testDir;

      mod.run ("rmdir", args);

      // The return code may indicate
      // the tests are not being run in an environment
      // with bash (Windows, some distros).
      if (mod.resultCode() != 2) {
        // Check the directory no longer exists indicating success
        // Double check the return code is the expected 0
        Assert.AreEqual (Directory.Exists (testDir), false);
        Assert.AreEqual (mod.resultCode (), 0);
      }
    }
    #endregion
  }
}

