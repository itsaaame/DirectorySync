using CommandLine;

namespace DirectorySync
{
    public class MainOptions
    {
        [Option(shortName: 'i', longName: "input", Required = true, HelpText = "path to the source folder you want to have the mirror of")]
        public string Input { get; set; }

        [Option(shortName: 'o', longName: "output", Required = true, HelpText = "path to the destination folder where you want the source to be mirrored")]
        public string Output { get; set; }

        [Option(shortName: 'l', longName: "logger", Required = true, HelpText = "path to the log file where all of the mirror changes are documented")]
        public string Logger { get; set; }

        [Option(shortName: 'p', longName: "period", Required = true, HelpText = "period of time in seconds; how often you want your source folder to be mirrored")]
        public int Period { get; set; }

    }
    class Program
    {
        static void Main(string[] args)
        {
            string source, destination, logger;
            int period;
            CommandLine.Parser.Default.ParseArguments<MainOptions>(args)
            .WithParsed<MainOptions>(o =>
            {
                source = o.Input;
                destination = o.Output;
                period = o.Period;
                logger = o.Logger;
                SleepyFileWatcher watcher = new(source, destination, logger, period);
                watcher.Watch();
            });

        }
    }

  
}
