using At.Matus.StatisticPod;
using Bev.Instruments.EplusE.EExx;
using System.Threading;

namespace HumiFixPoints
{
    public class Transmitter
    {
        private const int delay = 100;
        private readonly EExx device;
        private readonly StatisticPod airTemperature;
        private readonly StatisticPod airHumidity;

        public Transmitter(string sensorIp)
        {
            device = new EExx(sensorIp);
            airTemperature = new StatisticPod($"[t] {device.InstrumentID}");
            airHumidity = new StatisticPod($"[h] {device.InstrumentID}");
        }

        public double AirTemperature => airTemperature.AverageValue;
        public double AirTemperatureRange => airTemperature.Range;
        public double AirHumidity => airHumidity.AverageValue;
        public double AirHumidityRange => airHumidity.Range;
        public string TransmitterID => device.InstrumentID;
        public string TransmitterSN => device.InstrumentSerialNumber;

        public void Update()
        {
            MeasurementValues values = device.GetValues();
            airTemperature.Update(values.Temperature);
            airHumidity.Update(values.Humidity);
            Thread.Sleep(delay);
        }

        public void Reset()
        {
            airTemperature.Restart();
            airHumidity.Restart();
        }

        public override string ToString() => $"{TransmitterID}";
    }
}
