#load "solution.csx"
#load "commands-build.csx"

public static class IncludeGlobals {
   public static IReadOnlyList<string> ScriptArgs { get; set; }
   public static Solution Solution { get; set; }
}

IncludeGlobals.ScriptArgs = Env.ScriptArgs;

public static class Export {
   public static void Solution(string Name, ICommand[] Commands) {
      var options = Options.From(IncludeGlobals.ScriptArgs);
      var solutionDir = options.GetPropertyOrThrow("SolutionDir");
      IncludeGlobals.Solution = new Solution {
         Name = Name,
         Commands = Commands,
         Path = solutionDir
      };
   }
}