using System;
using System.Linq;

namespace ClusterAnalysis
{
    public class DistanceCounter
    {
        public double GetPowerDistance(CountryStatistics countryStatistics1, CountryStatistics countryStatistics2, int power)
        {
            double meat = Math.Pow(countryStatistics1.MeatConsumption - countryStatistics2.MeatConsumption, power);
            double butter = Math.Pow(countryStatistics1.ButterConsumption - countryStatistics2.ButterConsumption, power);
            double sugar = Math.Pow(countryStatistics1.SugarConsumption - countryStatistics2.SugarConsumption, power);
            double death = Math.Pow(countryStatistics1.DeathLevel - countryStatistics2.DeathLevel, power);
            
            double backPower = (double)(1.0m / power);
            return Math.Pow(meat + butter + sugar + death, backPower);
        }

        public double GetEuclideanDistance(CountryStatistics countryStatistics1, CountryStatistics countryStatistics2)
        {
            return this.GetPowerDistance(countryStatistics1, countryStatistics2, 2);
        }

        public double GetChebychevDistance(CountryStatistics countryStatistics1, CountryStatistics countryStatistics2)
        {
            double[] metricsAbs =
            {
                Math.Abs(countryStatistics1.MeatConsumption - countryStatistics2.MeatConsumption),
                Math.Abs(countryStatistics1.ButterConsumption - countryStatistics2.ButterConsumption),
                Math.Abs(countryStatistics1.SugarConsumption - countryStatistics2.SugarConsumption),
                Math.Abs(countryStatistics1.DeathLevel - countryStatistics2.DeathLevel),
            };

            return metricsAbs.Max();
        }
    }
}
