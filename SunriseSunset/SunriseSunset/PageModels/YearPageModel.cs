using FreshMvvm;
using Innovative.SolarCalculator;
using Plugin.Geolocator;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunriseSunset.PageModels
{
    [ImplementPropertyChanged]
    public class YearPageModel : FreshBasePageModel
    {
        public ObservableCollection<SolarInfo1> SolarInfos { get; set; } = new ObservableCollection<SolarInfo1>();

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
                var si = new SolarInfo1
                {
                    Date = date,
                    Sunrise = TimeZoneInfo.ConvertTime(solarTimes.Sunrise, TimeZoneInfo.Local),
                    Sunset = TimeZoneInfo.ConvertTime(solarTimes.Sunset, TimeZoneInfo.Local)
                };
               SolarInfos.Add(si);
            }

        }
    }

    public class SolarInfo1
    {
        public DateTime Date { get; set; }
        public DateTime Sunrise { get; set; }
        public DateTime Sunset { get; set; }

        public string SunriseToSunset { get { return $"{Sunrise.ToString("h:mm tt")} - {Sunset.ToString("h:mm tt")}"; } }
        public TimeSpan DayLength { get { return Sunset - Sunrise; } }
    }
}
