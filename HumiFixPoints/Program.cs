using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;

namespace HumiFixPoints
{
    class Program
    {
        /****************************************************************************************/
        // mimic an Arduino sketch
        // many globals are needed

        private static int LOG_INTERVALL = 5;           // in minutes, must be divisor of 60
        private static int LOG_INTERVALL_TOLERANCE = 4; // in seconds
        private static int DISCARD_FIRST = 4;           // number of readings not logged
        private static TransmitterSet transmitterSet;
        private static Summary summary;
        private static Options options;
        private static int logNumber;
        private static DateTime summaryStartTime;
        private static string csvFileName;
        private static string logFileName;

        static void Main(string[] args)
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            Console.OutputEncoding = Encoding.UTF8;

            Setup(args);
            while (true)
            {
                Loop();
            }
        }

        /****************************************************************************************/

        private static void Setup(string[] args)
        {
            summaryStartTime = DateTime.UtcNow;
            logNumber = 0;
            options = new Options();
            CommandLine.Parser.Default.ParseArgumentsStrict(args, options);
            if(GetSaltFromOption()==Salt.None)
            {
                Console.WriteLine("No fix point given!");
                Environment.Exit(1);
            }
            transmitterSet = new TransmitterSet(options.PortNames);
            summary = new Summary(transmitterSet.SensorNumber);
            csvFileName = GenerateBaseFileName() + ".csv";
            logFileName = GenerateBaseFileName() + ".log";
            Console.WriteLine(GetHeaderText());
        }

        /****************************************************************************************/

        private static void Loop()
        {
            UpdateSensorValues();
            if (TimeForLogging(DateTime.UtcNow))
            {
                logNumber++;
                CalibrationData calData = new CalibrationData(transmitterSet.Transmitters, GetSaltFromOption());
                WriteDataToConsole(calData);
                if (logNumber > DISCARD_FIRST)
                {
                    summary.Update(calData);
                    WriteCsvToFile(calData, csvFileName);
                }

                TimeSpan timeSpan = DateTime.UtcNow - summaryStartTime;
                if (timeSpan.TotalHours >= options.SummaryHours)
                {
                    summaryStartTime = DateTime.UtcNow;
                    Console.WriteLine($"***** Summary for the last {options.SummaryHours} h saved in {logFileName} *****");
                    WriteRoundupToFile(summary, logFileName);
                    summary.Reset();
                }
                Thread.Sleep(1000 * LOG_INTERVALL_TOLERANCE);
                ResetSensorValues();
            }
        }

        /****************************************************************************************/

        private static void WriteRoundupToFile(Summary roundup, string fileName)
        {
            try
            {
                StreamWriter writer = new StreamWriter(fileName, true, Encoding.UTF8);
                if (IsTextFileEmpty(fileName))
                    writer.WriteLine(GetHeaderText());
                writer.WriteLine(roundup.GetResult());
                writer.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine($"{fileName} could not be updated!");
            }
        }

        /****************************************************************************************/

        private static void WriteCsvToFile(CalibrationData calData, string fileName)
        {
            try
            {
                StreamWriter writer = new StreamWriter(fileName, true, Encoding.UTF8);
                if (IsTextFileEmpty(fileName))
                    writer.WriteLine(calData.GetCsvHeader());
                writer.WriteLine(calData.GetCsvLine());
                writer.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine($"{fileName} could not be updated!");
            }
        }

        /****************************************************************************************/

        private static bool IsTextFileEmpty(string fileName)
        {
            var info = new FileInfo(fileName);
            if (info.Length == 0)
                return true;
            // only if your use case can involve files with 1 or a few bytes of content.
            if (info.Length < 6)
            {
                var content = File.ReadAllText(fileName);
                return content.Length == 0;
            }
            return false;
        }

        /****************************************************************************************/

        private static void WriteDataToConsole(CalibrationData calData)
        {
            string flag = string.Empty;
            if (logNumber <= DISCARD_FIRST)
                flag = "  discarded!";
            Console.WriteLine($"{calData.GetLogLine()}{flag}");
        }

        /****************************************************************************************/

        private static void UpdateSensorValues()
        {
            transmitterSet.Update();
            Thread.Sleep(900);
        }

        /****************************************************************************************/

        private static void ResetSensorValues() => transmitterSet.Reset();

        /****************************************************************************************/

        private static bool TimeForLogging(DateTime timeStamp)
        {
            int minutes = timeStamp.Minute;
            int seconds = timeStamp.Second;
            if (seconds <= (LOG_INTERVALL_TOLERANCE - 1))
            {
                if (minutes % LOG_INTERVALL == 0)
                {
                    return true;
                }
            }
            return false;
        }

        /****************************************************************************************/

        private static Salt GetSaltFromOption()
        {
            // the order determines the priority in case of multiple choices
            if (options.MgCl2) return Salt.MgCl2;
            if (options.NaCl) return Salt.NaCl;
            if (options.KCl) return Salt.KCl;
            if (options.H2O) return Salt.H2O;
            return Salt.None;
        }

        /****************************************************************************************/

        private static string GetHeaderText()
        {
            string AppName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            string AppVer = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{AppName}, version {AppVer}");
            sb.AppendLine($"User comment: {options.UserComment}");
            sb.AppendLine($"Fixed point solution: {GetSaltFromOption()}");
            sb.AppendLine($"Averaging intervall {LOG_INTERVALL} min");
            sb.AppendLine($"Discard first {DISCARD_FIRST} values ({LOG_INTERVALL*DISCARD_FIRST} min)");
            sb.AppendLine($"Summarize at all {options.SummaryHours} h");
            sb.AppendLine($"Data file {csvFileName}"); 
            sb.AppendLine($"Summary file {logFileName}");
            sb.AppendLine($"{transmitterSet.SensorNumber} transmitter(s)");
            for (int i = 0; i < transmitterSet.SensorNumber; i++)
            {
                sb.AppendLine($"- Sensor#{i + 1}: {transmitterSet.Transmitters[i]}");
            }
            sb.Append($"=====================================================================");
            return sb.ToString();
        }

        /****************************************************************************************/

        private static string GenerateBaseFileName()
        {
            string str = options.Prefix;
            str += summaryStartTime.ToString("_yyyyMMddHHmm_");
            str += GetSaltFromOption();
            return str;
        }
    }
}
