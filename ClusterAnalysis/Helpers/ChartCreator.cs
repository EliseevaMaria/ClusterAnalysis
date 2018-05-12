using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using CenterSpace.NMath.Stats;

namespace ClusterAnalysis.Helpers
{
    public class ChartCreator
    {
        private Chart chart;

        private string name;

        private Color[] pointColors = new Color[20]
        {
            Color.Blue,
            Color.Red,
            Color.Yellow,
            Color.Teal,
            Color.Green,
            Color.Indigo,
            Color.Purple,
            Color.Black,
            Color.Pink,
            Color.LightSalmon,
            Color.Navy,
            Color.Firebrick,
            Color.DarkOrange,
            Color.Olive,
            Color.Lime,
            Color.Gray,
            Color.Khaki,
            Color.OrangeRed,
            Color.SaddleBrown,
            Color.Aqua
        };

        public ChartCreator(string metricsX, string metricsY, string name)
        {
            this.chart = new Chart()
            {
                Width = 700,
                Height = 700
            };

            var chartArea = new ChartArea();
            chartArea.AxisX.Title = metricsX;
            chartArea.AxisX.Minimum = 0;
            chartArea.AxisX.Maximum = 100;
            chartArea.AxisY.Title = metricsY;
            chartArea.AxisY.Minimum = 0;
            chartArea.AxisY.Maximum = 100;
            this.chart.ChartAreas.Add(chartArea);

            var series = new Series()
            {
                ChartType = SeriesChartType.Point
            };
            this.chart.Series.Add(series);

            this.name = name;
        }

        public void AddPoint(double x, double y, string name, int clusterIndex = 0)
        {
            var dataPoint = new DataPoint(x, y)
            {
                Label = name,
                Color = this.pointColors[clusterIndex]
            };
            this.chart.Series[0].Points.Add(dataPoint);
        }

        public void SaveAsFile()
        {
            string path = Application.StartupPath;
            if (!Directory.Exists(path + "\\charts\\"))
                Directory.CreateDirectory(path + "\\charts\\");

            this.chart.SaveImage($"{path}\\charts\\{this.name}.png", ChartImageFormat.Png);
        }

        public static void CreateChart(CountryStatistics[] countryStatistics, ClusterSet clusters, string name)
        {
            ConsoleWriter.WriteSystemMessage("Drawing diagrams for displaying cluster data...");
            var chartCreator = new ChartCreator("Sugar", "Death", name);

            for (int i = 0; i < clusters.NumberOfClusters; i++)
            {
                var clusterCountryStatistics = clusters.Cluster(i).Select(x => countryStatistics[x]);
                foreach (var countryStat in clusterCountryStatistics)
                    chartCreator.AddPoint(countryStat.SugarConsumption, countryStat.DeathLevel, countryStat.Country, i);
            }

            chartCreator.SaveAsFile();
            ConsoleWriter.WriteSystemMessage($"Saved at the app resources directory as '{name}.png'.");
        }

        public static void CreateChart(CountryStatistics[] countryStatistics, string name)
        {
            var chartCreator = new ChartCreator("Sugar", "Death", name);

            foreach (var countryStat in countryStatistics)
                chartCreator.AddPoint(countryStat.SugarConsumption, countryStat.DeathLevel, countryStat.Country);

            chartCreator.SaveAsFile();
        }
    }
}
