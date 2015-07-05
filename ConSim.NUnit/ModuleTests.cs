//
//  ModuleTests.cs
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
#endregion

namespace ConSim.NUnit
{
  [TestFixture()]
  public class ModuleTests
  {
    /* TESTS */
    #region Tests
    /// <summary>
    /// Tests the test module (Ironic I know).
    /// </summary>
    [Test()]
    public void ModuleTestResult()
    {
      Interfaces.iModule testMod = new Modules.TestModule ();

      string[] args = new string[1];
      args[0] = "1";

      testMod.run ("increment", args); 

      // Test we get expected results from the 
      // Interface.
      Assert.AreEqual (testMod.getName (), "TestModule");
      Assert.AreEqual (testMod.getVersion (), "TEST");
      Assert.AreEqual (testMod.standardOutput(), "2");

      args[0] = "Invalid";

      testMod.run ("increment", args);

      // Test we get data in the error output when using
      // unexpected data.
      Assert.AreEqual (testMod.errorOutput(), "Unexpected format in arguments");
    }
    #endregion
  }
}

