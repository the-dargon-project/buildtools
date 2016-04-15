#load "commands.csx"

public class Solution {
   public string Name { get; set; }
   public string Path { get; set; }
   public ICommand[] Commands { get; set; }
}