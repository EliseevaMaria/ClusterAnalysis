using System;
using System.IO;
using Newtonsoft.Json;
using ClusterAnalysis.Helpers;

namespace ClusterAnalysis
{
    class Program
    {
        static void Main(string[] args)
        {
            CountryStatistics[] countryStatistics;

            using (StreamReader sr = new StreamReader(@"..\..\CountryData.txt"))
            {
                string serializedStatistics = sr.ReadToEnd();
                countryStatistics =
                    JsonConvert.DeserializeObject<CountryStatistics[]>(serializedStatistics);
            }

            ConsoleWriter.WriteSystemMessage("Original data:");
            ConsoleWriter.WriteCoutryStatistics(countryStatistics);

            var normalizator = new Normalizator(countryStatistics);
            normalizator.Normalize(100);
            ConsoleWriter.WriteSystemMessage("Normalized data:");
            ConsoleWriter.WriteCoutryStatistics(countryStatistics);

            ConsoleWriter.WriteSystemMessage("Drawing the diagram for displaying normalized data...");
            ChartCreator.CreateChart(countryStatistics, "clusters");
            ConsoleWriter.WriteSystemMessage("Saved on the app resources directory as 'chart.png'.");

            var nMathHelper = new NMathHelper(countryStatistics);

            nMathHelper.GetAnalysisResults();

            nMathHelper.GetCopheneticCorrelations();
            
            int clusterCount;
            do
            {
                ConsoleWriter.WriteSystemMessage($"Set clusters amount ({countryStatistics.Length} max):");
                clusterCount = Convert.ToInt32(Console.ReadLine());
                if (clusterCount > countryStatistics.Length)
                    clusterCount = countryStatistics.Length;
                Console.WriteLine();

                nMathHelper.GetClustersByAmount(clusterCount);
            }
            while (Console.ReadLine() != "");

            double clusterDistance;
            do
            {
                ConsoleWriter.WriteSystemMessage("Set distance");
                clusterDistance = Convert.ToDouble(Console.ReadLine());
                Console.WriteLine();

                nMathHelper.GetClustersByDistance(clusterDistance);
            }
            while (Console.ReadLine() != "");

            Console.ReadLine();
        }
    }
}
