using System;
using System.Collections.Generic;

namespace HumiFixPoints
{
    public class TransmitterSet
    {
        public TransmitterSet(string[] ports)
        {
            List<Transmitter> transmittersList = new List<Transmitter>();
            foreach (string port in ports)
                transmittersList.Add(new Transmitter(port));
            Transmitters = transmittersList.ToArray();
        }

        public TransmitterSet(string portsCS) : this(ParseCsvPorts(portsCS)) { }

        private static string[] ParseCsvPorts(string portsCS)
        {
            char[] seps = { ' ', ',', ';' };
            return portsCS.Split(seps, StringSplitOptions.RemoveEmptyEntries);
        }

        public Transmitter[] Transmitters { get; }
        public int SensorNumber => Transmitters.Length;

        public void Update()
        {
            foreach (var tm in Transmitters)
                tm.Update();
        }

        public void Reset()
        {
            foreach (var tm in Transmitters)
                tm.Reset();
        }

    }
}
