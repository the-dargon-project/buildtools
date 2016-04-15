#load "helpers.csx"

using System;

public class BuildCommand : ICommand {
   public string Name { get { return "build"; } }

   public int Execute(Options options) {
      var projectName = options.GetProperty("ProjectName");
      if (projectName == null) {

      }
      Console.WriteLine("Building Project: " + (projectName ?? "[unspecified]"));
      Console.WriteLine("Build Tools: " + options.GetProperty("BuildToolsDir"));
      Console.WriteLine("Projects: " + options.GetProperty("ProjectsRootDir"));
      Util.Exec("echo hi");
      return 0;
   }
}