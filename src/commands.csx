#load "options.csx"

public static class Dispatcher {
   public static int Dispatch(ICommand[] commands, Options options) {
      foreach (var command in commands) {
         if (command.Name == options.Command) {
            return command.Execute(options);
         }
      }
      Console.Error.WriteLine("Could not find command '" + options.Command + "'.");
      SummarizeAvailableCommands(commands);
      return 1;
   }

   public static void SummarizeAvailableCommands(ICommand[] commands) {
      var commandsString = string.Join(", ", commands.Select(StringifyCommand));
      if (string.IsNullOrWhiteSpace(commandsString)) {
         commandsString = "(none)";
      }
      Console.WriteLine("Available Commands: " + commandsString);
   }

   public static string StringifyCommand(ICommand command) {
      return command.Name;
   }
}

public interface ICommand {
   string Name { get; }
   int Execute(Options options);
}

public class LambdaCommand : ICommand {
   private readonly Func<Options, int> _execute; 

   public LambdaCommand(string name, Func<Options, int> execute) {
      Name = name;
      _execute = execute;
   }

   public string Name { get; private set; }

   public int Execute(Options options) {
      return _execute(options);
   }
}