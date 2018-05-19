using System;
using AccordDistance = Accord.Math.Distance;
using CenterSpace.NMath.Core;
using CenterSpace.NMath.Stats;
using NMathClusterAnalysis = CenterSpace.NMath.Stats.ClusterAnalysis;

namespace ClusterAnalysis.Helpers
{
    public class NMathHelper
    {
        private CountryStatistics[] countryStatistics;

        private DoubleMatrix doubleMatrix;

        private NMathClusterAnalysis[,] analysisResults;

        private string[] distanceFunctions;
        private string[] DistanceFunctions => this.distanceFunctions ??
            (this.distanceFunctions = new string[] { "Euclidean distance function", "Mahalanobis distance function", "Maximum distance function" });

        private string[] linkageFunctions;
        private string[] LinkageFunctions => this.linkageFunctions ??
            (this.linkageFunctions = new string[] { "Complete linkage function", "Average weighted linkage function", "Centroid linkage function" });

        private NMathClusterAnalysis bestClusterAnalysis;

        public NMathHelper(CountryStatistics[] countryStatistics)
        {
            this.countryStatistics = countryStatistics;

            DoubleVector[] vectors = new DoubleVector[countryStatistics.Length];
            for (int i = 0; i < vectors.Length; i++)
                vectors[i] = countryStatistics[i].GetDoubleVector();

            double[,] dataMatrix = new double[countryStatistics.Length, 4];
            for (int i = 0; i < countryStatistics.Length; i++)
            {
                dataMatrix[i, 0] = countryStatistics[i].SugarConsumption;
                dataMatrix[i, 1] = countryStatistics[i].DeathLevel;
                dataMatrix[i, 2] = countryStatistics[i].MeatConsumption;
                dataMatrix[i, 3] = countryStatistics[i].ButterConsumption;
            }
            this.doubleMatrix = new DoubleMatrix(dataMatrix);
        }

        public void GetClustersByAmount(int clusterCount)
        {
            ClusterSet clustersByAmount = this.bestClusterAnalysis.CutTree(clusterCount);
            ConsoleWriter.WriteClusterCountries(countryStatistics, clustersByAmount);
            ChartCreator.CreateChart(countryStatistics, clustersByAmount, "clustersByAmount");
        }

        public void GetClustersByDistance(double clusterDistance)
        {
            ClusterSet clustersByDistance = this.bestClusterAnalysis.CutTree(clusterDistance);
            ConsoleWriter.WriteClusterCountries(countryStatistics, clustersByDistance);
            ChartCreator.CreateChart(countryStatistics, clustersByDistance, "clustersByDistance");
        }

        public void GetAnalysisResults()
        {
            if (this.doubleMatrix == null)
                return;

            Distance.Function MahalanobisDistanceFunction = new Distance.Function(this.MahalanobisDistance);
            this.analysisResults = new NMathClusterAnalysis[3, 3]
            {
                {
                    new NMathClusterAnalysis(this.doubleMatrix, Distance.EuclideanFunction, Linkage.CompleteFunction),
                    new NMathClusterAnalysis(this.doubleMatrix, Distance.EuclideanFunction, Linkage.WeightedAverageFunction),
                    new NMathClusterAnalysis(this.doubleMatrix, Distance.EuclideanFunction, Linkage.CentroidFunction),
                },
                {
                    new NMathClusterAnalysis(this.doubleMatrix, MahalanobisDistanceFunction, Linkage.CompleteFunction),
                    new NMathClusterAnalysis(this.doubleMatrix, MahalanobisDistanceFunction, Linkage.WeightedAverageFunction),
                    new NMathClusterAnalysis(this.doubleMatrix, MahalanobisDistanceFunction, Linkage.CentroidFunction),
                },
                {
                    new NMathClusterAnalysis(this.doubleMatrix, Distance.MaximumFunction, Linkage.CompleteFunction),
                    new NMathClusterAnalysis(this.doubleMatrix, Distance.MaximumFunction, Linkage.WeightedAverageFunction),
                    new NMathClusterAnalysis(this.doubleMatrix, Distance.MaximumFunction, Linkage.CentroidFunction),
                },
            };
        }

        private double MahalanobisDistance(DoubleVector xVector, DoubleVector yVector)
        {
            int n = xVector.Length;

            double averageX = 0;
            foreach (double xValue in xVector)
                averageX += xValue;
            averageX /= n;

            double averageY = 0;
            foreach (double yValue in yVector)
                averageY += yValue;
            averageY /= n;

            double[,] covariance = new double[n, n];
            double x;

            for (int i = 0; i < n; i++)
            {
                for (int k = 0; k < n; k++)
                {
                    double a = xVector[i] - averageX;
                    double b = yVector[k] - averageY;
                    x = a * b;
                    covariance[i, k] = x;
                }
            }

            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    covariance[i, j] = 1 / covariance[i, j];

            return AccordDistance.Mahalanobis(xVector.ToArray(), yVector.ToArray(), covariance);
        }

        public void GetCopheneticCorrelations()
        {
            ConsoleWriter.WriteSystemMessage("Cophenetic correlations:");

            Tuple<int, int> minCopheneticCorrelationIndex = null;
            Tuple<int, int> maxCopheneticCorrelationIndex = null;
            double minCorrelation = double.MaxValue;
            double maxCorrelation = double.MinValue;
            double[,] correlations = new double[3, 3];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    double correlation =
                        StatsFunctions.Correlation(this.analysisResults[i, j].Distances, this.analysisResults[i, j].CopheneticDistances);
                    correlations[i, j] = correlation;
                    if (correlation < minCorrelation)
                    {
                        minCorrelation = correlation;
                        minCopheneticCorrelationIndex = new Tuple<int, int>(i, j);
                    }
                    if (correlation > maxCorrelation)
                    {
                        maxCorrelation = correlation;
                        maxCopheneticCorrelationIndex = new Tuple<int, int>(i, j);
                    }
                    ConsoleWriter.WriteMessage($"{this.DistanceFunctions[i]}, {this.LinkageFunctions[j]}: {correlation}");
                }
                Console.WriteLine();
            }

            ConsoleWriter.WriteSystemMessage("Minimal cophenetic correlation:");
            ConsoleWriter.WriteMessage(distanceFunctions[minCopheneticCorrelationIndex.Item1] +
                " and " + linkageFunctions[minCopheneticCorrelationIndex.Item2]);

            ConsoleWriter.WriteSystemMessage("Maximal cophenetic correlation:");
            ConsoleWriter.WriteMessage(distanceFunctions[maxCopheneticCorrelationIndex.Item1] +
                " and " + linkageFunctions[maxCopheneticCorrelationIndex.Item2]);

            this.bestClusterAnalysis = this.analysisResults[maxCopheneticCorrelationIndex.Item1, maxCopheneticCorrelationIndex.Item2];
        }
    }
}
