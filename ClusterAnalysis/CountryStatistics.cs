using System;
using CenterSpace.NMath.Core;

namespace ClusterAnalysis
{
    public class CountryStatistics
    {
        public string Country
        {
            get;
            set;
        }

        public double MeatConsumption { get; set; }

        public double ButterConsumption { get; set; }

        public double SugarConsumption { get; set; }

        public double DeathLevel { get; set; }

        public DoubleVector GetDoubleVector()
        {
            return new DoubleVector(this.MeatConsumption, this.ButterConsumption, 
                                    this.SugarConsumption, this.DeathLevel);
        }
    }
}
