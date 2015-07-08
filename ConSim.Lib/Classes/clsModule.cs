//
//  clsModule.cs
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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

#endregion

namespace ConSim.Lib.Classes
{
  /// <summary>
  /// A module configuration file
  /// Tells the software which type to load.
  /// </summary>
  [DataContract]
  public class clsModule
  {
    /* DATA STRUTURE */
    #region Data Structure
    /// <summary>
    /// Type that implements iModule.
    /// e.g "Namespace.Class1"
    /// </summary>
    [DataMember]
    public string gettype;

    /// <summary>
    /// Name of the dll file.
    /// </summary>
    [DataMember]
    public string filename;
    #endregion

    /* API */
    #region API
    /// <summary>
    /// Initializes a new instance of the <see cref="Classes.clsModule"/> class.
    /// </summary>
    /// <param name="filepath">Filepath.</param>
    public clsModule(string filepath)
    {
      clsModule newModule = null;

      using (FileStream f = new FileStream (filepath, FileMode.Open)) {
        DataContractJsonSerializer ser = 
          new DataContractJsonSerializer (typeof(clsModule));

        newModule = (clsModule)ser.ReadObject (f);

        f.Close ();
      }

      // Set readonly variables
      this.gettype = newModule.gettype;
      this.filename = newModule.filename;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Classes.clsModule"/> class.
    /// </summary>
    /// <param name="type">Type.</param>
    /// <param name="filename">Filename.</param>
    public clsModule (string type, string filename)
    {
      this.gettype  = type;
      this.filename = filename; 
    }

    /// <summary>
    /// Save the class to JSON.
    /// </summary>
    /// <param name="filepath">Filepath.</param>
    public void save(string filepath)
    {
      using (FileStream f = new FileStream (filepath, FileMode.Create)) {
        DataContractJsonSerializer ser = 
          new DataContractJsonSerializer (typeof(clsModule));

        ser.WriteObject (f, this);
        f.Close ();
      }      
    }

    #endregion

  }
}

