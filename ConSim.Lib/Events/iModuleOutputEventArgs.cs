﻿//
//  iModuleOutputEventArgs.cs
//
//  Author:
//       IEUser <${AuthorEmail}>
//
//  Copyright (c) 2015 IEUser
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
using System;

namespace ConSim.Lib.Events
{
  public class iModuleOutputEventArgs : EventArgs
  {
    /// <summary>
    /// Output from a module.
    /// </summary>
    private readonly object _output;

    /// <summary>
    /// Initializes a new instance of the 
    /// <see cref="ConSim.Lib.Events.iModuleOutputEventArgs"/> class.
    /// </summary>
    /// <param name="output">Output.</param>
    public iModuleOutputEventArgs (object output)
    {
      _output = output;
    }

    /// <summary>
    /// Gets the output.
    /// </summary>
    /// <value>The output.</value>
    public object output
    {
      get { return _output; }
    }
  }
}

