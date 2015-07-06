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
#endregion

namespace ConSim.Shell
{
  class MainClass
  {
    public static void Main (string[] args)
    {
       
    }

    private string help()
    {
      return
        @"ConSim " + Assembly.GetExecutingAssembly().GetName().Version.ToString()
      + @"------------------------------------------------------------"
      + @" -h Displays this help screen" 
      + @" -l [lessondir] Opens a lesson"
      + @" - ";
    }
  }
}
