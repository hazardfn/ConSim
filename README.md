# Overview | [Travis-CI](https://travis-ci.org/hazardfn/ConSim) :: ![Build Status](https://travis-ci.org/hazardfn/ConSim.svg)
--------
ConSim is a Console Simulator for education purposes - this suite allows you to create lessons with a group of tasks and use them
to illustrate the versaility of a terminal in a closed and controlled environment.

Features
--------
 * Set up multiple lessons consisting of a group of tasks which a student must complete to advance.
 * Supports multiple modules allowing you to, in theory, write your own basic language to demonstrate simple terminal commands.
 * Comes with a Windows module which will forward commands to cmd and print the output.
 * Limit the allowed commands and even filter the arguments to prevent any dangerous commands being run.
 * Comes with a Bash module for forwarding commands to bash.
 * Use the expected result field to control expected output used for passing tasks.
 * Can use the error output to compare to the expected output if you wish to demonstrate errors.
 * Can use the command itself as a comparison if you wish to just check the student put in the right command to pass the task.

Usage
--------
 You may choose to use ConSim.Shell as your basic interpreter or write a more personalised one for your environment,  regardless, the file structure must be as follows:

 ```
 => Lesson
 ==> Lesson.json
 ==> Modules
 ===> [Module DLL's]
 ```

 Lesson.json looks like so:

 ```
 {
  "AllowedModules": [
    {
      "__type": "clsModule:#Classes",
      "filename": "ConSim.Test.Module.dll",
      "gettype": "Modules.TestModule"
    }
  ],
  "Name": "TestLesson",
  "Tasks": [
    {
      "ExpectedResult": "true",
      "LongDescription": "This is a long description",
      "Name": "taskness",
      "ShortDescription": "This is short",
      "allowedCommands": [
    
      ],
      "commandToTask": false,
      "disallowedStrings": [
    
      ],
      "errorToTask": false,
      "lazyMatching": false
    }
  ],
  "Version": "TEST",
}
 ```

 Lesson
---------
 The lesson itself only contains 2 datafields (Name & Version) they are pretty self explanatory:

 Name: Name of the lesson - shown in ConSim.Shell.
 
 Version: Version of the lesson - shown next to the name in ConSim.Shell.


 Allowed Modules
----------
 Allowed modules is an array, multiple entries can be added using the following fields:

 **__type**: You should not change, this is so the json deserializer knows the type to deserialize to.
 
 **filename**: This is the name of the dll file this entry relates to. Must be placed in the modules directory.
 
 **gettype**: This should be the "Namespace.Class" of this module which can be determined in the source code.


 Tasks
----------
 Tasks is also an array and is where the main portion of the activities are defined, see below for field documentation:

 **__type**: You should not change, this is so the json deserializer knows the type to deserialize to.
 
 **ExpectedResult**: The result required from the module/console/terminal before the task is passed.
 
 **LongDescription**: A lengthy description of the task, you can include the expected result here if you wish.
 
 **Name**: The name of the task.
 
 **ShortDescription**: A short description of the task, printed above the long one in ConSim.
 
 **allowedCommands**: A list of allowed commands out of the ones the modules provide. Empty implies all are allowed.
 
 **commandToTask**: Set to true if your expected result analysis is on the command itself as opposed to the output.
 
 **errorToTask**: Set to true if your expected result analysis is on an error that should be returned as opposed to the output.
 
 **lazyMatching**: Set to true if you just want to match part of the output (can be combined with command and error to task).

Loading the TestLesson
--------
 Loading the test lesson with ConSim can be a good way for you to experiment and see how things work. Simply open a  terminal and type the following:

 ```
 ConSim.Shell.exe -l "ConSim.NUnit/bin/[Release | Debug]/Lessons/TestLesson/TestLesson.json"
 ```
 ![Example](http://imageshack.com/a/img673/3586/A1RBZ2.png)

 The expected result is 2, the test module increments an argument by 1 so supplying the command "increment 1" should  pass the task.
