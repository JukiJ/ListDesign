# List design with shadow and floating button to skip to today

In this article I will show you how to make a simple shadow on top of the list along with a floating button to scroll to certain list items. In this case the floating button is used to scroll to items under the today's date, and it is shown only when no today items are visible. In a scenario I am covering here we have a list view containing many items which represent some events that have a set date on when they will happen. These events are grouped based on a date when they will happen. By imaginary design request we need to show a shadow all the time the user has navigated anywhere for beggining of the list. It is not shown only when user is right on the start of the list. As for the button it is shown when no items with today or any nearest day in future are shown. Clicking on it takes you to those items.

### Step 1: Create a control in a shared project

Your control in a shared project should look something like this:

``` csharp
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
```

You may have noticed the ListGroupItem class being used so you need to add that too.

``` csharp
    public class ListGroupItem : List<string>
    {
        public DateTime ItemDate { get; set; }

        public List<string> Items => this;
    }
```

This class is just a UI model for grouped item in list and contains the date when these items will be executed. As for the MyListView class the most important part is under method <i>OnPropertyChanged</i>. There we check if there are any tasks today or in the future and if there is we assign it to the variable <i>nextTask</i>. If there isn't one, then we just use the last item from the list. If there is no items whatsoever, we set all three variables to -1. If there is an item then we have different logic for iOS and Android. For iOS we just take IndexOf the <i>nextItem</i> from the list, but for Android we need to do some calculations to get the index of first and last item inside <i>nextItem</i>. Bear in mind that Android also calculates header and footer as items inside the list even if there isnt any so we needed to include them in this list. After all the calculations we just call <i>ScrollTo</i> method to scroll the list down to the needed item.

### Step 2: Usage in page

To use this control in page we need to add our custom list along with the floating button and the shadow separately. This can be also combined together in one control to be reusable across pages, but for the simplicity of this post I will not be doing it. Xaml for the whole page is shown below and is very simple so I wont be explaining it.

``` xml
<Grid RowSpacing="0">
    <Grid.RowDefinitions>
      <RowDefinition Height="200" />
      <RowDefinition Height="*" />
    </Grid.RowDefinitions>

    <StackLayout
      Grid.Row="0"
      BackgroundColor="Gray"
      HorizontalOptions="FillAndExpand"
      VerticalOptions="FillAndExpand">

      <Label
        HorizontalOptions="CenterAndExpand"
        Text="This is the bar above"
        TextColor="White"
        VerticalOptions="CenterAndExpand" />

    </StackLayout>

    <Grid Grid.Row="1" VerticalOptions="FillAndExpand">

      <listdesign:MyListView
        x:Name="mainList"
        HeightRequest="1000000"
        IsGroupingEnabled="True"
        HasUnevenRows="True"
        ItemsSource="{Binding Items}"
        SeparatorVisibility="None">

        <ListView.GroupHeaderTemplate>
          <DataTemplate>
            <ViewCell>
              <StackLayout
                BackgroundColor="LightGray"
                HeightRequest="40"
                HorizontalOptions="FillAndExpand"
                VerticalOptions="FillAndExpand">
                <Label
                  Text="{Binding ItemDate}"
                  TextColor="White"
                  VerticalOptions="CenterAndExpand" />
              </StackLayout>
            </ViewCell>
          </DataTemplate>
        </ListView.GroupHeaderTemplate>

        <ListView.ItemTemplate>
          <DataTemplate>
            <ViewCell>
              <StackLayout
                BackgroundColor="LightGray"
                HeightRequest="48"
                HorizontalOptions="FillAndExpand">
                <Label
                  HorizontalOptions="CenterAndExpand"
                  Text="{Binding .}"
                  TextColor="White"
                  VerticalOptions="CenterAndExpand" />
              </StackLayout>
            </ViewCell>
          </DataTemplate>
        </ListView.ItemTemplate>
      </listdesign:MyListView>

      <skia:SKCanvasView
        HeightRequest="10"
        HorizontalOptions="FillAndExpand"
        IsVisible="{Binding ShowShadow, Source={x:Reference mainList}}"
        PaintSurface="DrawShadow"
        VerticalOptions="StartAndExpand" />

      <Button
        Margin="0,0,0,16"
        Command="{Binding ShowTodayCommand, Source={x:Reference mainList}}"
        HorizontalOptions="CenterAndExpand"
        IsVisible="{Binding ShowTodayButtonVisible, Source={x:Reference mainList}}"
        Text="Show today"
        VerticalOptions="EndAndExpand" />

    </Grid>

  </Grid>
``` 

### Step 3: ViewModel

In view model we only need to create a list property to populate our list. Here we used dummy data just for presentation purposes.

``` csharp
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
```

### Step 4: Creating a custom renderer for Android

Below is shown whole custom renderer for Android. Under <i>OnElementChanged</i> method wee need to set our custom scroll listener to the control. Our custom scroll listener we implemented just below and named it <i>MyDatesListScrollListener</i>. In it we added private field to contain reference to our shared list view control which we assign through constructor. Inside our scroll listener on each scroll we check if the offset to the top is less than zero (values are negative when scrolling down, and 0 is the starting position) and if it is, we set <i>ShowShadow</i> property through reference to our custom control to true. After that we check first and last visible item of the list and if it does not fall inbetween our previously set <i>TodayFirst</i> and <i>TodayLast</i> from custom control we set <i>ShowTodayButtonVisible</i> property on our custom list to true, otherwise to false.

``` csharp
[assembly: ExportRenderer(typeof(MyListView), typeof(MyListViewRenderer))]
namespace ListDesign.Droid.CustomRenderer
{
    public class MyListViewRenderer : ListViewRenderer
    {
        public MyListViewRenderer(Context context) : base(context)
        {

        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.ListView> e)
        {
            base.OnElementChanged(e);

            if (Control != null && Element is MyListView list)
            {
                Control.SetOnScrollListener(new MyDatesListScrollListener(list));
            }
        }
    }

    public class MyDatesListScrollListener : Java.Lang.Object, IOnScrollListener
    {
        private MyListView _customListView;

        public MyDatesListScrollListener(MyListView customList)
        {
            _customListView = customList;
        }

        public void OnScroll(AbsListView view, int firstVisibleItem, int visibleItemCount, int totalItemCount)
        {
            var headerView = view.GetChildAt(0);

            if (headerView != null)
            {
                _customListView.ShowShadow = headerView.Top < 0;

                int lastVisibleItem = firstVisibleItem + visibleItemCount;

                if (_customListView.TodaysFirst != -1 && _customListView.TodaysLast != -1 &&
                    (firstVisibleItem > _customListView.TodaysFirst || lastVisibleItem < _customListView.TodaysFirst) &&
                    (firstVisibleItem > _customListView.TodaysLast || lastVisibleItem < _customListView.TodaysLast))
                {
                    _customListView.ShowTodayButtonVisible = true;
                }
                else
                {
                    _customListView.ShowTodayButtonVisible = false;
                }
            }
        }

        public void OnScrollStateChanged(AbsListView view, [GeneratedEnum] ScrollState scrollState)
        {

        }
    }
}
```

### Step 5: Creating a custom renderer for iOS

Now we move on to iOS renderer. Renderer for iOS is actually much simpler than Android one as we do not need to have any special listeners, we just add observer to the control like you can see on line 304. Because we added that observer we now get <i>ScrollInvoked</i> method called on every scroll. Inside <i>ScrollInvoked</i> method we first check is ContentOffset on Y axis bigger than 0, because if it is it means that a list was scrolled down and we need to show the Shadow and therefore we set our control's <i>ShowShadow</i> property to true. After that we check do we even have any items in our list, if we dont we set <i>ShowTodayButtonVisible</i> to false and exit the method. If we are still in the method we then check is any of currently visible list rows matching the one we set as the "today" one. If there is a match it means we already show today's items and we hide the button, otherwise we show the button.

``` csharp
[assembly: ExportRenderer(typeof(MyListView), typeof(MyListViewRenderer))]
namespace ListDesign.iOS.CustomRenderers
{
    public class MyListViewRenderer : ListViewRenderer
    {
        MyListView _listView;

        private IDisposable _offsetObserver;

        protected override void OnElementChanged(ElementChangedEventArgs<ListView> e)
        {
            base.OnElementChanged(e);

            if (Control != null && Element is MyListView list)
            {
                _listView = list;

                _offsetObserver = Control.AddObserver("contentOffset", Foundation.NSKeyValueObservingOptions.New, HandleAction);
            }
        }

        private void HandleAction(NSObservedChange obj)
        {
            ScrollInvoked();
        }

        public void ScrollInvoked()
        {
            if (Control == null)
                return;

            _listView.ShowShadow = Control.ContentOffset.Y > 0;

            if (_listView.TodayGroupNumber < 0 || !Control.IndexPathsForVisibleRows?.Any() == true)
            {
                _listView.ShowTodayButtonVisible = false;
                return;
            }

            if (Control.IndexPathsForVisibleRows?.Any(x => x.Section == _listView.TodayGroupNumber) == true)
                _listView.ShowTodayButtonVisible = false;
            else
                _listView.ShowTodayButtonVisible = true;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing && _offsetObserver != null)
            {
                _offsetObserver.Dispose();
                _offsetObserver = null;
            }
        }
    }
}
```