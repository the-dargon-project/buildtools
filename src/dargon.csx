#load "helpers.csx"
#load "dargon-build.csx"
#load "solution.csx"
#load "commands.csx"

public static class Program {
   public static void Run(IReadOnlyList<string> args, Solution solution) {
      try {
         var options = Options.From(args).WithSolution(solution);
         int errorCode = Dispatcher.Dispatch(solution.Commands, options);
         Util.Exit(errorCode);
      } catch (Exception e) {
         Console.Error.WriteLine();
         Console.Error.WriteLine("== Error ==");
         Console.Error.WriteLine(e);
      }
   }
}