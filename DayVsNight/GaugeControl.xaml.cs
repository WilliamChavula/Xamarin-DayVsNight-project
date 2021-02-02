using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using DayVsNight.Resources.Themes;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace DayVsNight
{
    public partial class GaugeControl : ContentView
    {
        public GaugeControl()
        {
            InitializeComponent();
            Percent = 50;
            MessagingCenter.Subscribe<ThemeMessage>(this, ThemeMessage.ThemeChanged, (themeMessage) => UpdateTheme());
        }

        private void UpdateTheme()
        {
            TempGaugeCanvas.InvalidateSurface();
        }

        private double percent;
        public double Percent
        {
            get => percent;
            set
            {
                percent = value;
                TempGaugeCanvas.InvalidateSurface();
            }
        }

        const float bottomPadding = 100f;

        readonly SKPath clipPath = SKPath.ParseSvgPathData("M.021 28.481a25.933 25.933 0 0 0 8.824-2.112 27.72 27.72 0 0 0 7.391-5.581l19.08-17.045S39.879.5 44.516.5s9.352 3.243 9.352 3.243l20.74 18.628a30.266 30.266 0 0 0 4.525 3.545c3.318 2.263 11.011 2.564 11.011 2.564z");

        SKPaint redBrush = new SKPaint()
        {
            Style = SKPaintStyle.Stroke,
            Color = Color.Red.ToSKColor(),
            StrokeWidth = 3
        };

        // Background Brush
        SKPaint backgroundBrush = new SKPaint()
        {
            Style = SKPaintStyle.Fill,
            Color = Color.Red.ToSKColor(),
        };

        SKPaint tickBrush = new SKPaint()
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 2

        };

        private void TempGaugeCanvas_PaintSurface(object sender, SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs e)
        {
            SKImageInfo info = e.Info;
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.Clear();

            // calculate densities for screens instead of working with pixels
            float density = info.Size.Width / (float)this.Width;


            // Get bounds (dimension/size) of clipPath so that we can scale it
            var scaledClipPath = new SKPath(clipPath);
            scaledClipPath.Transform(SKMatrix.CreateScale(density, density));
            scaledClipPath.GetTightBounds(out var tightBounds);

            // position clipPath
            float xPos = info.Width * ((float)percent / 100);

            // clamp the slider min and max values
            xPos = Math.Min(Math.Max(xPos, 150), info.Width - 150);

            float translateX = (xPos - tightBounds.MidX);
            float translateY = info.Height - (tightBounds.Height + bottomPadding);

            using (new SKAutoCanvasRestore(canvas))
            {
                // apply translations
                canvas.Translate(translateX, translateY);
                canvas.ClipPath(scaledClipPath, SKClipOperation.Difference, antialias: true);
                canvas.Translate(-translateX, -translateY);


                // gradient background

                BackgroundCanvasGradient(info, canvas);

                // Draw tick mark
                DrawTickMarks(info, canvas);


            }

            // draw thumb
            using (Stream stream = GetType().Assembly.GetManifestResourceStream("DayVsNight.Resources.Images.Thumb.png"))
            {
                SKBitmap bitmap = SKBitmap.Decode(stream);

                float imageHeight = (float)(bitmap.Height * 0.75);
                float imageWidth = (float)(bitmap.Width * 0.9);

                SKRect thumbRect = new SKRect(0, 0, imageWidth, imageHeight);
                thumbRect.Location = new SKPoint(xPos - (imageWidth / 2), info.Height - (imageHeight + 50f));

                //SKPoint location = new SKPoint(xPos - (bitmap.Width / 2), info.Height - bitmap.Height);



                canvas.DrawBitmap(bitmap, thumbRect);
            }


        }

        private void DrawSVGAtPoint(SKCanvas canvas, SKPoint sKPoint, float size, string svgName)
        {
            // Load SVG
            using (Stream stream = GetType().Assembly.GetManifestResourceStream(svgName))
            {
                SkiaSharp.Extended.Svg.SKSvg svg = new SkiaSharp.Extended.Svg.SKSvg();
                svg.Load(stream);

                using (new SKAutoCanvasRestore(canvas))
                {
                    SKRect bounds = svg.ViewBox;

                    // work out how big we want to be
                    float xRatio = size / bounds.Width;
                    float yRatio = size / bounds.Height;

                    float ratio = Math.Min(xRatio, yRatio);

                    // transalate to location

                    canvas.Translate(sKPoint.X - bounds.MidX * ratio, sKPoint.Y - bounds.MidY * ratio);

                    SKMatrix matrix = SKMatrix.CreateScale(ratio, ratio);

                    // render the image
                    canvas.DrawPicture(svg.Picture, ref matrix);
                }

            }
        }

        private void BackgroundCanvasGradient(SKImageInfo info, SKCanvas canvas)
        {
            // get the theme
            SKColor gaugeStartColor = ((Color)Application.Current.Resources["GaugeGradientStartColor"]).ToSKColor();
            SKColor gaugeEndColor = ((Color)Application.Current.Resources["GaugeGradientEndColor"]).ToSKColor();

            backgroundBrush.Shader = SKShader.CreateLinearGradient(new SKPoint(0, 0),
                                new SKPoint(info.Width, info.Height),
                                new SKColor[] { gaugeStartColor, gaugeEndColor },
                                new float[] { 0, 1 },
                                SKShaderTileMode.Clamp);

            SKRect backgroundBounds = new SKRect(0, 0, info.Width, (info.Height - bottomPadding));
            canvas.DrawRoundRect(backgroundBounds, 20, 20, backgroundBrush);
        }

        private void DrawTickMarks(SKImageInfo info, SKCanvas canvas)
        {
            int numTicks = 20;
            int tickHeight = 100;
            int distanceBetweenTicks = info.Width / numTicks;

            for (int i = 1; i < numTicks; i++)
            {
                SKPoint start = new SKPoint(i * distanceBetweenTicks, (info.Height - bottomPadding));
                SKPoint end = new SKPoint(i * distanceBetweenTicks, info.Height - (tickHeight + bottomPadding));
                tickBrush.Shader = SKShader.CreateLinearGradient(start, end, new SKColor[] { new SKColor(255, 255, 255, 200), SKColors.Transparent }, new float[] { 0, 1 }, SKShaderTileMode.Clamp);

                canvas.DrawLine(start, end, tickBrush);
            }

            DrawSVGAtPoint(canvas, new SKPoint(100, info.Height - (float)(bottomPadding + info.Height * 0.3)), 100f, "DayVsNight.Resources.Images.Snowflake.svg");
            DrawSVGAtPoint(canvas, new SKPoint(info.Width - 100, info.Height - (float)(bottomPadding + info.Height * 0.3)), 100f, "DayVsNight.Resources.Images.Heat.svg");
        }

        void TouchEffect_TouchAction(System.Object sender, DayVsNight.Effects.TouchActionEventArgs args)
        {
            Percent = (args.Location.X / TempGaugeCanvas.Width) * 100;
        }
    }
}
