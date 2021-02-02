using Android.Content;
using Android.Runtime;
using Android.Widget;
using ListDesign;
using ListDesign.Droid.CustomRenderer;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using static Android.Widget.AbsListView;

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
                Control.SetOnScrollListener(new MyListScrollListener(list));
            }
        }
    }

    public class MyListScrollListener : Java.Lang.Object, IOnScrollListener
    {
        private MyListView _customListView;

        public MyListScrollListener(MyListView customList)
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