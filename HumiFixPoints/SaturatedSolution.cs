//===================================================================
// 
// Equilibrium relative humidities in air versus temperature 
// 
// Lewis Greenspan
// Journal of research of the NBS
// Vol 81A (1977)
// 
//===================================================================

namespace HumiFixPoints
{
    public class SaturatedSolution
    {
        private readonly Salt salt;
        private double a0, a1, a2, a3;
        private double t_min, t_max;

        public SaturatedSolution(Salt salt)
        {
            this.salt = salt;
            PopulateCoefficients();
        }

        public Salt Salt => salt;

        public double GetHumidityFor(double temperature)
        {
            if (temperature < t_min) return double.NaN;
            if (temperature > t_max) return double.NaN;
            return a0 + (a1 * temperature) + (a2 * temperature * temperature) + (a3 * temperature * temperature * temperature);
        }

        private void PopulateCoefficients()
        {
            switch (Salt)
            {
                case Salt.None:
                    a0 = double.NaN;
                    a1 = double.NaN;
                    a2 = double.NaN;
                    a3 = double.NaN;
                    t_min = double.NaN;
                    t_max = double.NaN;
                    break;
                case Salt.NaCl:
                    a0 = 75.5164;
                    a1 = 0.0398321;
                    a2 = -0.265459E-2;
                    a3 = 0.2848E-4;
                    t_min = 0;
                    t_max = 80;
                    break;
                case Salt.MgCl2:
                    a0 = 33.6686;
                    a1 = -0.00797397;
                    a2 = -0.108988E-2;
                    a3 = 0.0;
                    t_min = 0;
                    t_max = 99.4;
                    break;
                case Salt.H2O:
                    a0 = 100.0;
                    a1 = 0.0;
                    a2 = 0.0;
                    a3 = 0.0;
                    t_min = 0;
                    t_max = 100;
                    break;
                case Salt.KCl:
                    a0 = 88.619;
                    a1 = -0.19334;
                    a2 = 0.899706E-3;
                    a3 = 0.0;
                    t_min = 0;
                    t_max = 90;
                    break;
            }
        }
    }

    public enum Salt
    {
       None,
       NaCl,
       MgCl2,
       KCl,
       H2O
    }
}
