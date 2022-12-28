using CommandLine;
using CommandLine.Text;

namespace HumiFixPoints
{
    public class Options
    {
        [Option("ports", DefaultValue = "COM1", HelpText = "Comma separated port names.")]
        public string PortNames { get; set; }

        [Option("prefix", DefaultValue = "HumFix", HelpText = "Prefix for file names.")]
        public string Prefix { get; set; }

        [Option("NaCl", DefaultValue = false, HelpText = "Fix point: saturated NaCl solution.")]
        public bool NaCl { get; set; }

        [Option("MgCl2", DefaultValue = false, HelpText = "Fix point: saturated MgCl2 solution.")]
        public bool MgCl2 { get; set; }

        [Option("KCl", DefaultValue = false, HelpText = "Fix point: saturated KCl solution.")]
        public bool KCl { get; set; }

        [Option("H2O", DefaultValue = false, HelpText = "Fix point: pure water.")]
        public bool H2O { get; set; }

        [Option("comment", DefaultValue = "---", HelpText = "User supplied comment string.")]
        public string UserComment { get; set; }

        [Option('s', "summary", DefaultValue = 3, HelpText = "Time interval for combining data. In hours.")]
        public int SummaryHours { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            string AppName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            string AppVer = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

            HelpText help = new HelpText
            {
                Heading = new HeadingInfo($"{AppName}, version {AppVer}"),
                Copyright = new CopyrightInfo("Michael Matus", 2022),
                AdditionalNewLineAfterOption = false,
                AddDashesToOption = true
            };
            string preamble = "Program to calibrate E+E humidity transmitters using saturated salt solutions. " +
                "Measurement results are logged in a file. A salt solution must be choosen.";
            help.AddPreOptionsLine(preamble);
            help.AddPreOptionsLine("");
            help.AddPreOptionsLine($"Usage: {AppName} [options]");
            help.AddPostOptionsLine("");
            help.AddOptions(this);
            return help;
        }
    }
}
