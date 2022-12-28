using At.Matus.StatisticPod;
using System;

namespace HumiFixPoints
{
    public class CalibrationData
    {
        public CalibrationData(Transmitter[] transmitters, Salt salt)
        {
            this.transmitters = transmitters;
            saturatedSolution = new SaturatedSolution(salt);
            HumidityErrors = new double[transmitters.Length];
            Temperatures = new double[transmitters.Length];
            ComputeProperties();
        }

        public DateTime TimeStamp { get; private set; }
        public double TrueHumidity { get; private set; }
        public double EnsembleTemperature { get; private set; }
        public double EnsembleTemperatureRange { get; private set; }
        public double[] Temperatures { get; private set; }
        public double[] HumidityErrors { get; private set; }

        public string GetCsvLine()
        {
            string line = $"{MmTime.GetMjd(TimeStamp):F5},{EnsembleTemperature:F3},{EnsembleTemperatureRange:F3},{TrueHumidity:F3}";
            for (int i = 0; i < Temperatures.Length; i++)
                line += $",{Temperatures[i]:F3}";
            for (int i = 0; i < HumidityErrors.Length; i++)
                line += $",{HumidityErrors[i]:F3}";
            return line;
        }

        public string GetCsvHeader()
        {
            string line = $"MJD,ensemble temperature (°C),temperature range (°C),true humidity (%)";
            for (int i = 0; i < Temperatures.Length; i++)
                line += $",temperature for {transmitters[i].TransmitterSN} (°C)";
            for (int i = 0; i < HumidityErrors.Length; i++)
                line += $",humidity error for {transmitters[i].TransmitterSN} (%)";
            return line;
        }

        public string GetLogLine()
        {
            string line = $"{TimeStamp.ToString("yyyy-MM-dd HH:mm")}  {EnsembleTemperature:F2}({EnsembleTemperatureRange:F2}) °C > {TrueHumidity:F1} %";
            for (int i = 0; i < HumidityErrors.Length; i++)
                line += $"  Sensor#{i+1}: {HumidityErrors[i]:+0.0;-#.0} %";
            return line;
        }

        private readonly Transmitter[] transmitters;
        private readonly SaturatedSolution saturatedSolution;

        private void ComputeProperties()
        {
            TimeStamp = DateTime.UtcNow;
            StatisticPod ensembleTemperature = new StatisticPod();
            foreach (Transmitter transmitter in transmitters)
                ensembleTemperature.Update(transmitter.AirTemperature);
            EnsembleTemperature = ensembleTemperature.AverageValue;
            EnsembleTemperatureRange = ensembleTemperature.Range;
            TrueHumidity = saturatedSolution.GetHumidityFor(EnsembleTemperature);
            for (int i = 0; i < HumidityErrors.Length; i++)
            {
                HumidityErrors[i] = transmitters[i].AirHumidity - TrueHumidity;
                Temperatures[i] = transmitters[i].AirTemperature;
            }
        }

    }
}
