using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace ListDesign
{
    public class MyListView : ListView
    {
        private ListGroupItem _nextTask;

        public MyListView()
        {

        }

        public static BindableProperty ShowShadowProperty = BindableProperty.Create(nameof(ShowShadow), typeof(bool), typeof(MyListView), false, BindingMode.TwoWay);

        public bool ShowShadow
        {
            get => (bool)GetValue(ShowShadowProperty);
            set => SetValue(ShowShadowProperty, value);
        }

        public int TodaysFirst { get; set; } = -1;

        public int TodaysLast { get; set; } = -1;

        public int TodayGroupNumber { get; set; } = -1;

        public static BindableProperty ShowTodayButtonVisibleProperty = BindableProperty.Create(nameof(ShowTodayButtonVisible), typeof(bool), typeof(MyListView), false, BindingMode.TwoWay);

        public bool ShowTodayButtonVisible
        {
            get => (bool)GetValue(ShowTodayButtonVisibleProperty);
            set => SetValue(ShowTodayButtonVisibleProperty, value);
        }

        public static BindableProperty ShowTodayCommandProperty = BindableProperty.Create(nameof(ShowTodayCommand), typeof(ICommand), typeof(MyListView), defaultBindingMode: BindingMode.TwoWay);

        public ICommand ShowTodayCommand => new Command(() => Device.BeginInvokeOnMainThread(() => ScrollTo(_nextTask?.Items.FirstOrDefault(), _nextTask, ScrollToPosition.Center, true)));

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == ItemsSourceProperty.PropertyName)
            {
                if (ItemsSource is IEnumerable<ListGroupItem> list && list.Count() > 0)
                {
                    var today = DateTime.UtcNow;

                    _nextTask = list.FirstOrDefault(x => x.ItemDate >= today);

                    if (_nextTask == null)
                        _nextTask = list.LastOrDefault();

                    if (_nextTask != null)
                    {
                        if (Device.RuntimePlatform == Device.iOS)
                            TodayGroupNumber = list.IndexOf(_nextTask);

                        if (Device.RuntimePlatform == Device.Android)
                        {
                            var elementsUpToToday = list.Where(x => x.ItemDate < _nextTask.ItemDate).Sum(x => x.Items.Count + 1);
                            var totalSum = list.Sum(x => x.Items.Count + 1);
                            TodaysFirst = elementsUpToToday + 2;
                            TodaysLast = elementsUpToToday + _nextTask.Items.Count +1;
                        }
                    }
                    else
                    {
                        TodayGroupNumber = -1;
                        TodaysFirst = -1;
                        TodaysLast = -1;
                    }

                    Device.BeginInvokeOnMainThread(() => ScrollTo(_nextTask?.Items.FirstOrDefault(), _nextTask, ScrollToPosition.Center, true));
                }
            }
        }
    }
}
