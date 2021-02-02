using Foundation;
using ListDesign;
using ListDesign.iOS.CustomRenderers;
using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

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