using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace ListDesign
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            BindingContext = new MainPageViewModel();
        }

        private void DrawShadow(object sender, SKPaintSurfaceEventArgs args)
        {
            var canvas = args.Surface.Canvas;
            int surfaceWidth = args.Info.Width;
            int surfaceHeight = args.Info.Height;

            canvas.Clear();

            using (SKPaint paint = new SKPaint())
            {
                SKRect rect = new SKRect(0, 0, surfaceWidth, surfaceHeight);

                paint.Shader = SKShader.CreateLinearGradient(
                                    new SKPoint(rect.Left, rect.Top),
                                    new SKPoint(rect.Left, rect.Bottom),
                                    new SKColor[] { Color.Gray.ToSKColor(), Color.FromHex("55D3D3D3").ToSKColor() },
                                    new float[] { 0, 1 },
                                    SKShaderTileMode.Repeat);

                canvas.DrawRect(rect, paint);
            }
        }
    }
}
