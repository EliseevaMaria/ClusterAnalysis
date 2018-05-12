using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CenterSpace.NMath.Stats;

namespace ClusterAnalysis.Helpers
{
    public static class ConsoleWriter
    {
        public static void WriteCoutryStatistics(CountryStatistics[] countryStatistics)
        {
            var maxCountryLength = countryStatistics.Max(stat => stat.Country.Length) + 2;
            var maxMeatLength = countryStatistics.Max(stat => stat.MeatConsumption.ToString().Length) + 2;
            var maxButterLength = countryStatistics.Max(stat => stat.ButterConsumption.ToString().Length) + 2;
            var maxSugarLength = countryStatistics.Max(stat => stat.SugarConsumption.ToString().Length) + 2;
            var maxDeathLength = countryStatistics.Max(stat => stat.DeathLevel.ToString().Length) + 2;

            StringBuilder message = new StringBuilder();
            foreach (var countryStats in countryStatistics)
            {
                message.Clear();
                message.Append(countryStats.Country);
                ConsoleWriter.AppendSpaces(message, maxCountryLength - countryStats.Country.Length);

                int meatLength = countryStats.MeatConsumption.ToString().Length;
                message.Append(countryStats.MeatConsumption);
                ConsoleWriter.AppendSpaces(message, maxMeatLength - meatLength);

                int butterLength = countryStats.ButterConsumption.ToString().Length;
                message.Append(countryStats.ButterConsumption);
                ConsoleWriter.AppendSpaces(message, maxButterLength - butterLength);

                int sugarLength = countryStats.SugarConsumption.ToString().Length;
                message.Append(countryStats.SugarConsumption);
                ConsoleWriter.AppendSpaces(message, maxSugarLength - sugarLength);

                int deathLength = countryStats.DeathLevel.ToString().Length;
                message.Append(countryStats.DeathLevel);
                ConsoleWriter.AppendSpaces(message, maxDeathLength - deathLength);

                Console.WriteLine(message);
            }
        }

        private static void AppendSpaces(StringBuilder message, int spacesCount)
        {
            for (int i = 0; i < spacesCount; i++)
                message.Append(" ");
        }

        public static void WriteSystemMessage(string message)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public static void WriteMessage(string message)
        {
            Console.WriteLine(message);
        }

        public static void WriteClusterCountries(CountryStatistics[] countryStatistics, ClusterSet clustersBySetNumber)
        {
            int clusterCount = clustersBySetNumber.NumberOfClusters;
            for (int i = 0; i < clusterCount; i++)
            {
                List<int> countryIndices = new List<int>();
                for (int j = 0; j < clustersBySetNumber.Clusters.Length; j++)
                    if (clustersBySetNumber.Clusters[j] == i)
                        countryIndices.Add(j);
                List<CountryStatistics> clusterCountries =
                    countryIndices.Select(countryIndex => countryStatistics[countryIndex]).ToList();
                if (clusterCountries.Count > 0)
                    Console.WriteLine($"Cluster {i + 1}:");
                for (int j = 0; j < clusterCountries.Count - 1; j++)
                    Console.Write(clusterCountries[j].Country + ", ");
                Console.Write(clusterCountries[clusterCountries.Count - 1].Country + "\r\n\r\n");
            }
        }
    }
}
