using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Media.Imaging;
using Waves.Api.Models.Communitys.DataCenter;

namespace WutheringWavesTool.Models.Wrapper
{
    public partial class DataCenterExploreItem : ObservableObject
    {
        [ObservableProperty]
        public partial ObservableCollection<DataCenterExploreCountryItem> Country { get; set; }

        [ObservableProperty]
        public partial string CountryName { get; set; }

        [ObservableProperty]
        public partial double CountryProgress { get; set; }

        [ObservableProperty]
        public partial BitmapImage Icon { get; set; }

        public DataCenterExploreItem(ExploreList item)
        {
            this.Country = new ObservableCollection<DataCenterExploreCountryItem>();
            foreach (var areainfo in item.AreaInfoList)
            {
                this.Country.Add(new DataCenterExploreCountryItem(areainfo));
            }
            this.CountryName = item.Country.CountryName;
            this.CountryProgress = Convert.ToDouble(item.CountryProgress);
            this.Icon = new(new(item.Country.HomePageIcon));
        }
    }
}
