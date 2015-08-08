using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomeSecretProject
{
    public static class Program
	{
        private static readonly Dictionary<string, Action<InputParameters, string>> actions = new Dictionary<string, Action<InputParameters, string>>
        {
            {"-f", ParseFilename},
            {"-p", ParsePower},
            {"-t", ParseTL},
            {"-m", ParseML}
        };

		static void Main(string[] args)
		{
            var argsQueue = new Queue<string>(args);
		    var inputParameters = new InputParameters();
		    try
		    {
		        while (argsQueue.Count > 0)
		        {
		            var keyValue = ParseNextArg(argsQueue);
		            actions[keyValue.Item1](inputParameters, keyValue.Item2);
		        }
                if (inputParameters.InputFilename == null)
                    throw new Exception("MUST provide us with -f");
		    }
		    catch (Exception ex)
		    {
		        Console.WriteLine("Error: " + ex.Message);
                Environment.Exit(111);
		    }
            //todo call game and return output
		}

	    private static void ParseFilename(InputParameters input, string value)
	    {
	        input.InputFilename = value;
	    }

        private static void ParsePower(InputParameters input, string value)
        {
            input.PowerPhrases.Add(value);
        }

	    private static void ParseTL(InputParameters input, string value)
	    {
	        input.TimeLimitSec = int.Parse(value);
	    }

        private static void ParseML(InputParameters input, string value)
        {
            input.MemoryLimitMb = int.Parse(value);
        }

	    private static Tuple<string, string> ParseNextArg(Queue<string> args)
        {
            var key = args.Dequeue();
            if (!actions.ContainsKey(key))
                throw new Exception("What is this '" + key + "'?");
            if (args.Count == 0)
                throw new Exception("Must provide value after " + key);
            var value = args.Dequeue();
            return Tuple.Create(key, value);
        }
    }

    public class InputParameters
    {
        public InputParameters()
        {
            PowerPhrases = new List<string>();
            TimeLimitSec = 60;
            MemoryLimitMb = 1024;
        }

        public string InputFilename { get; set; }
        public int MemoryLimitMb { get; set; }
        public int TimeLimitSec { get; set; }
        public IList<string> PowerPhrases { get; set; }
    }
}
