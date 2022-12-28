using At.Matus.StatisticPod;
using System;
using System.Text;

namespace HumiFixPoints
{
    public class Summary
    {
        private readonly StatisticPod stpTemperatureEnsemble = new StatisticPod();
        private readonly StatisticPod stpTemperatureSpread = new StatisticPod();
        private readonly StatisticPod stpTrueHumidity = new StatisticPod();
        private readonly StatisticPod[] stpDeviations;
        private readonly StatisticPod[] stpTemperatures;
        private DateTime recentCalibrationTime;

        public Summary(int sensorNumber)
        {
            stpDeviations = new StatisticPod[sensorNumber];
            stpTemperatures = new StatisticPod[sensorNumber];
            for (int i = 0; i < stpDeviations.Length; i++)
                stpDeviations[i] = new StatisticPod();
            for (int i = 0; i < stpTemperatures.Length; i++)
                stpTemperatures[i] = new StatisticPod();
        }

        public void Update(CalibrationData calData)
        {
            stpTemperatureEnsemble.Update(calData.EnsembleTemperature);
            stpTemperatureSpread.Update(calData.EnsembleTemperatureRange);
            stpTrueHumidity.Update(calData.TrueHumidity);
            for (int i = 0; i < stpDeviations.Length; i++)
                stpDeviations[i].Update(calData.HumidityErrors[i]);
            for (int i = 0; i < stpTemperatures.Length; i++)
                stpTemperatures[i].Update(calData.Temperatures[i]);
            recentCalibrationTime = calData.TimeStamp;
        }

        public void Reset()
        {
            stpTemperatureEnsemble.Restart();
            stpTemperatureSpread.Restart();
            stpTrueHumidity.Restart();
            for (int i = 0; i < stpDeviations.Length; i++)
                stpDeviations[i].Restart();
            for (int i = 0; i < stpTemperatures.Length; i++)
                stpTemperatures[i].Restart();
        }

        public string GetResult()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Timestamp: {recentCalibrationTime.ToString("yyyy-MM-dd HH:mm")} (MJD: {MmTime.GetMjd(recentCalibrationTime):F5})");
            sb.AppendLine($"n = {stpTemperatureEnsemble.SampleSize}");
            sb.AppendLine($"t_ensemble = {stpTemperatureEnsemble.AverageValue:F3}({stpTemperatureEnsemble.StandardDeviation:F3})[{stpTemperatureEnsemble.Range:F3}] °C");
            sb.AppendLine($"t_spread = {stpTemperatureSpread.AverageValue:F3} °C");
            sb.AppendLine($"h_true = {stpTrueHumidity.AverageValue:F2} %");
            for (int i = 0; i < stpTemperatures.Length; i++)
                sb.AppendLine($"- Sensor#{i + 1} t: {stpTemperatures[i].AverageValue:F3}({stpTemperatures[i].StandardDeviation:F3})[{stpTemperatures[i].Range:F3}] °C");
            for (int i = 0; i < stpDeviations.Length; i++)
                sb.AppendLine($"- Sensor#{i + 1} h_error: {stpDeviations[i].AverageValue:+0.00;-#.00}({stpDeviations[i].StandardDeviation:F2})[{stpDeviations[i].Range:F2}] %");
            sb.Append($"=====================================================================");
            return sb.ToString();
        }
    }
}
