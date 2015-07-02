using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms.DataVisualization.Charting;
using Akka.Actor;

namespace ChartApp.Actors
{
    public class ChartingActor : ReceiveActor
    {
        /// <summary>
        /// Maximum number of points we will allow in a series
        /// </summary>
        public const int MaxPoints = 250;

        /// <summary>
        /// Incrementing counter we use to plot along the X-axis
        /// </summary>
        public int xPosCounter = 0;

        #region Messages

        public class InitializeChart
        {
            public InitializeChart(Dictionary<string, Series> initialSeries)
            {
                InitialSeries = initialSeries;
            }

            public Dictionary<string, Series> InitialSeries { get; private set; }
        }

        /// <summary>
        /// Add a new <see cref="Series"/> to the chart
        /// </summary>
        public class AddSeries
        {
            public AddSeries(Series series)
            {
                Series = series;
            }

            public Series Series { get; private set; }
        }

        /// <summary>
        /// Remove a <see cref="Series"/> to the chart
        /// </summary>
        public class RemoveSeries
        {
            public RemoveSeries(string seriesName)
            {
                SeriesName = seriesName;
            }

            public string SeriesName { get; private set; }
        }

        #endregion

        private readonly Chart chart;
        private Dictionary<string, Series> seriesIndex;

        public ChartingActor(Chart chart) : this(chart, new Dictionary<string, Series>())
        {
        }

        public ChartingActor(Chart chart, Dictionary<string, Series> seriesIndex)
        {
            this.chart = chart;
            this.seriesIndex = seriesIndex;

            Receive<InitializeChart>(ic => HandleInitialize(ic));
            Receive<AddSeries>(addSeries => HandleAddSeries(addSeries));
            Receive<RemoveSeries>(removeSeries => HandleRemoveSeries(removeSeries));
            Receive<Metric>(metric => HandleMetrics(metric));
        }

        #region Individual Message Type Handlers

        private void HandleInitialize(InitializeChart ic)
        {
            if (ic.InitialSeries != null)
            {
                //swap the two series out
                seriesIndex = ic.InitialSeries;
            }

            //delete any existing series
            chart.Series.Clear();

            // set te axes up
            var area = chart.ChartAreas[0];
            area.AxisX.IntervalType = DateTimeIntervalType.Number;
            area.AxisY.IntervalType = DateTimeIntervalType.Number;

            SetChartBoundaries();

            //attempt to render the initial chart
            if (seriesIndex.Any())
            {
                foreach (var series in seriesIndex)
                {
                    //force both the chart and the internal index to use the same names
                    series.Value.Name = series.Key;
                    chart.Series.Add(series.Value);
                }
            }

            SetChartBoundaries();
        }

        private void HandleAddSeries(AddSeries series)
        {
            if (!string.IsNullOrEmpty(series.Series.Name) && !seriesIndex.ContainsKey(series.Series.Name))
            {
                seriesIndex.Add(series.Series.Name, series.Series);
                chart.Series.Add(series.Series);

                SetChartBoundaries();
            }
        }

        private void HandleRemoveSeries(RemoveSeries series)
        {
            if (!string.IsNullOrEmpty(series.SeriesName) && seriesIndex.ContainsKey(series.SeriesName))
            {
                var seriesToRemove = seriesIndex[series.SeriesName];

                seriesIndex.Remove(series.SeriesName);
                chart.Series.Remove(seriesToRemove);

                SetChartBoundaries();
            }
        }

        private void HandleMetrics(Metric metric)
        {
            if (!string.IsNullOrEmpty(metric.Series) && seriesIndex.ContainsKey(metric.Series))
            {
                var series = seriesIndex[metric.Series];
                series.Points.AddXY(xPosCounter++, metric.CounterValue);

                while (series.Points.Count > MaxPoints)
                    series.Points.RemoveAt(0);

                SetChartBoundaries();
            }
        }

        #endregion

        private void SetChartBoundaries()
        {
            double maxAxisX, maxAxisY, minAxisX, minAxisY = 0;

            var allPoints = seriesIndex.Values.SelectMany(series => series.Points).ToList();
            var yValues = allPoints.SelectMany(point => point.YValues).ToList();

            maxAxisX = xPosCounter;
            minAxisX = xPosCounter - MaxPoints;
            maxAxisY = yValues.Count > 0 ? Math.Ceiling(yValues.Max()) : 1.0d;
            minAxisY = yValues.Count > 0 ? Math.Floor(yValues.Min()) : 0.0d;

            if (allPoints.Count > 2)
            {
                var area = chart.ChartAreas[0];

                area.AxisX.Minimum = minAxisX;
                area.AxisX.Maximum = maxAxisX;
                area.AxisY.Minimum = minAxisY;
                area.AxisY.Maximum = maxAxisY;
            }
        }
    }
}
