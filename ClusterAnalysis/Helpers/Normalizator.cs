using System;
using System.Collections.Generic;
using System.Linq;

namespace ClusterAnalysis
{
    public class Normalizator
    {
        public CountryStatistics[] CountryStatistics
        {
            get;
        }

        public Normalizator(CountryStatistics[] countryStatistics)
        {
            this.CountryStatistics = countryStatistics;
        }

        public void Normalize(int maxValue)
        {
            int countryStatisticsCount = this.CountryStatistics.Length;
            var meatStatistics = this.CountryStatistics.Select(s => s.MeatConsumption).ToArray();
            this.GetNormalizedValues(meatStatistics, maxValue);

            var butterStatistics = this.CountryStatistics.Select(s => s.ButterConsumption).ToArray();
            this.GetNormalizedValues(butterStatistics, maxValue);

            var sugarStatistics = this.CountryStatistics.Select(s => s.SugarConsumption).ToArray();
            this.GetNormalizedValues(sugarStatistics, maxValue);

            var deathStatistics = this.CountryStatistics.Select(s => s.DeathLevel).ToArray();
            this.GetNormalizedValues(deathStatistics, maxValue);

            for (int i = 0; i < countryStatisticsCount; i++)
            {
                this.CountryStatistics[i].MeatConsumption = meatStatistics[i];
                this.CountryStatistics[i].ButterConsumption = butterStatistics[i];
                this.CountryStatistics[i].SugarConsumption = sugarStatistics[i];
                this.CountryStatistics[i].DeathLevel = deathStatistics[i];
            }
        }

        private void GetNormalizedValues(double[] metrics, int maxValue)
        {
            int countryStatisticsCount = this.CountryStatistics.Length;
            double minMetrics = metrics.Min();
            for (int i = 0; i < countryStatisticsCount; i++)
                metrics[i] -= minMetrics;

            double maxMetrics= metrics.Max();
            double factor = maxValue / maxMetrics;
            for (int i = 0; i < countryStatisticsCount; i++)
                metrics[i] *= factor;
        }
    }
}
