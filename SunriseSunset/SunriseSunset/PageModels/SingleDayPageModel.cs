using FreshMvvm;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Plugin.Geolocator;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Innovative.SolarCalculator;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace SunriseSunset.PageModels
{
    [ImplementPropertyChanged]
    class SingleDayPageModel : FreshBasePageModel
    {
        public string SunriseTime { get; set; }
        public string SunsetTime { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Model { get; set; }
        public string DeviceId { get; set; }

        public PlotModel Graph { get; set; }

        public ObservableCollection<SolarInfo> SolarInfos { get; set; } = new ObservableCollection<SolarInfo>();

        public ICommand ShowYearCommand {  get { return new Command(() => CoreMethods.PushPageModel<YearPageModel>()); } }

        public override async void Init(object initData)
        {
            base.Init(initData);

            var locator = CrossGeolocator.Current;
            locator.DesiredAccuracy = 50;

            var position = await locator.GetPositionAsync(10000);

            int daysInYear = DateTime.IsLeapYear(DateTime.Now.Year) ? 366 : 365;
            var startDate = new DateTime(DateTime.Now.Year, 1, 1);

            for (int i = 0; i < daysInYear; i++)
            {
                DateTime date = startDate.AddDays(i);
                var solarTimes = new SolarTimes(date, position.Latitude, position.Longitude);
                SolarInfos.Add(new SolarInfo
                {
                    Date = date,
                    Sunrise = TimeZoneInfo.ConvertTime(solarTimes.Sunrise, TimeZoneInfo.Local),
                    Sunset = TimeZoneInfo.ConvertTime(solarTimes.Sunset, TimeZoneInfo.Local)
                });
            }

            var todaysInfo = SolarInfos[DateTime.Now.DayOfYear - 1];
            SunriseTime = todaysInfo.Sunrise.ToString("h:mm");
            SunsetTime = todaysInfo.Sunset.ToString("h:mm");

            Graph = new PlotModel
            {
                Title = "Sunrise and Sunset",
                Subtitle = TimeZoneInfo.Local.StandardName,
                Background = OxyColors.White
            };

            Graph.Axes.Add(new DateTimeAxis
            {
                IntervalType = DateTimeIntervalType.Months,
                MajorGridlineStyle = LineStyle.Solid,
                StringFormat = "MMM"
            });

            Graph.Axes.Add(new TimeSpanAxis
            {
                MajorGridlineStyle = LineStyle.Solid,
                Maximum = 86400,
                Minimum = 0,
                StringFormat = "h:mm"
            });

            var series = new AreaSeries
            {
                DataFieldX = "Day",
                DataFieldY = "Sunrise",
                Fill = OxyColor.FromArgb(128, 255, 255, 0),
                Color = OxyColors.Black,
                DataFieldX2 = "Day",
                DataFieldY2 = "Sunset"
            };
            Graph.Series.Add(series);

            foreach (var si in SolarInfos)
            {
                series.Points.Add(new DataPoint(DateTimeAxis.ToDouble(si.Date), TimeSpanAxis.ToDouble(si.Sunrise - si.Date)));
                series.Points2.Add(new DataPoint(DateTimeAxis.ToDouble(si.Date), TimeSpanAxis.ToDouble(si.Sunset - si.Date)));
            }

        }

        public class SolarInfo
        {
            public DateTime Date { get; set; }
            public DateTime Sunrise { get; set; }
            public DateTime Sunset { get; set; }
        }
    }
}
