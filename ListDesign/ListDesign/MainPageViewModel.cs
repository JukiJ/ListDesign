using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ListDesign
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private List<ListGroupItem> _items;
        public List<ListGroupItem> Items
        {
            get => _items;
            set
            {
                if (value == _items)
                    return;

                _items = value;
                RaisePropertyChanged(nameof(Items));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public MainPageViewModel()
        {
            var list = new List<ListGroupItem>();

            for (int j = 10; j >= 0; j--)
            {
                var listGroupItem = new ListGroupItem()
                {
                    ItemDate = DateTime.UtcNow.Date.AddDays(-j)
                };
                for (int i = 0; i < 5; i++)
                {
                    listGroupItem.Items.Add($"Item {i}");
                }
                list.Add(listGroupItem);
            }

            Items = list;
        }

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
