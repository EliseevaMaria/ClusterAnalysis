using System;
using System.IO;
using CenterSpace.NMath.Core;
using CenterSpace.NMath.Stats;
using NMathClusterAnalysis = CenterSpace.NMath.Stats.ClusterAnalysis;
using Newtonsoft.Json;
using System.Linq;
using System.Collections.Generic;
using Generic = System.Collections.Generic;

namespace ClusterAnalysis
{
    class Program
    {
        static void Main(string[] args)
        {
            CountryStatistics[] countryStatistics;

            using (StreamReader sr = new StreamReader("CountryData.txt"))
            {
                string serializedStatistics = sr.ReadToEnd();
                countryStatistics =
                    JsonConvert.DeserializeObject<CountryStatistics[]>(serializedStatistics);
            }

            var normalizator = new Normalizator(countryStatistics);
            normalizator.Normalize(100);

            // TODO: Form a diagram with two random metrics.

            DoubleVector[] vectors = new DoubleVector[countryStatistics.Length];
            for (int i = 0; i < vectors.Length; i++)
                vectors[i] = countryStatistics[i].GetDoubleVector();

            double[,] dataMatrix = new double[countryStatistics.Length, 4];
            for (int i = 0; i < countryStatistics.Length; i++)
            {
                dataMatrix[i, 0] = countryStatistics[i].MeatConsumption;
                dataMatrix[i, 1] = countryStatistics[i].ButterConsumption;
                dataMatrix[i, 2] = countryStatistics[i].SugarConsumption;
                dataMatrix[i, 3] = countryStatistics[i].DeathLevel;
            }
            DoubleMatrix doubleMatrix = new DoubleMatrix(dataMatrix);

            string[] distanceFunctions = { "EuclideanDistance", "PowerDistance", "MaximumDistance" };
            string[] linkageFunctions = { "CompleteLinkage", "WeightedAverageLinkage", "CentroidLinkage" };
            var powerDistance = new Distance.PowerDistance(4, 4);
            NMathClusterAnalysis[,] analysis = new NMathClusterAnalysis[3, 3]
            {
                {
                    new NMathClusterAnalysis(doubleMatrix, Distance.EuclideanFunction, Linkage.CompleteFunction),
                    new NMathClusterAnalysis(doubleMatrix, Distance.EuclideanFunction, Linkage.WeightedAverageFunction),
                    new NMathClusterAnalysis(doubleMatrix, Distance.EuclideanFunction, Linkage.CentroidFunction),
                },
                {
                    new NMathClusterAnalysis(doubleMatrix, powerDistance.Function, Linkage.CompleteFunction),
                    new NMathClusterAnalysis(doubleMatrix, powerDistance.Function, Linkage.WeightedAverageFunction),
                    new NMathClusterAnalysis(doubleMatrix, powerDistance.Function, Linkage.CentroidFunction),
                },
                {
                    new NMathClusterAnalysis(doubleMatrix, Distance.MaximumFunction, Linkage.CompleteFunction),
                    new NMathClusterAnalysis(doubleMatrix, Distance.MaximumFunction, Linkage.WeightedAverageFunction),
                    new NMathClusterAnalysis(doubleMatrix, Distance.MaximumFunction, Linkage.CentroidFunction),
                },
            };

            Console.WriteLine("Cophenetic correlations:");
            Tuple<int, int> minIndex = null;
            Tuple<int, int> maxIndex = null;
            double minCorrelation = double.MaxValue;
            double maxCorrelation = double.MinValue;
            double[,] correlations = new double[3, 3];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    double correlation =
                        StatsFunctions.Correlation(analysis[i, j].Distances, analysis[i, j].CopheneticDistances);
                    correlations[i, j] = correlation;
                    if (correlation < minCorrelation)
                    {
                        minCorrelation = correlation;
                        minIndex = new Tuple<int, int>(i, j);
                    }
                    if (correlation > maxCorrelation)
                    {
                        maxCorrelation = correlation;
                        maxIndex = new Tuple<int, int>(i, j);
                    }
                    Console.WriteLine(correlation + " ");
                }
                Console.WriteLine();
            }

            Console.WriteLine("Minimal cophenetic correlation:");
            var minClusterAnalysis = analysis[minIndex.Item1, minIndex.Item2];
            Console.WriteLine($"{distanceFunctions[minIndex.Item1]} and {linkageFunctions[minIndex.Item2]}");
            Console.WriteLine();

            Console.WriteLine("Maximal cophenetic correlation:");
            var maxClusterAnalysis = analysis[maxIndex.Item1, maxIndex.Item2];
            Console.WriteLine($"{distanceFunctions[maxIndex.Item1]} and {linkageFunctions[maxIndex.Item2]}");
            Console.WriteLine();

            ClusterSet clustersBySetNumber = null;
            do
            {
                Console.WriteLine($"Set clusters number ({maxClusterAnalysis.N} max):");
                int clusterCount = Convert.ToInt32(Console.ReadLine());
                if (clusterCount > 20)
                    clusterCount = 20;
                Console.WriteLine();

                clustersBySetNumber = maxClusterAnalysis.CutTree(clusterCount);
                PrintClusterCountries(countryStatistics, clustersBySetNumber);
            }
            while (Console.ReadLine() != "");

            ClusterSet clustersByDistance = null;
            do
            {
                Console.WriteLine($"Set distance ({maxClusterAnalysis.N} max):");
                double clusterDistance = Convert.ToDouble(Console.ReadLine());
                Console.WriteLine();

                clustersByDistance = maxClusterAnalysis.CutTree(clusterDistance);
                PrintClusterCountries(countryStatistics, clustersByDistance);
            }
            while (Console.ReadLine() != "");

            // TODO: Form the dendrogram.

            // TODO: Form the metrics diagram with clusters (gscatter).

            Console.ReadLine();
        }

        private static void PrintClusterCountries(CountryStatistics[] countryStatistics, ClusterSet clustersBySetNumber)
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
