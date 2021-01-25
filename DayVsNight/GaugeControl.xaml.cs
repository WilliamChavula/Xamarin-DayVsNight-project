using System;
using System.Collections.Generic;
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
        }

        // Background Brush
        SKPaint backgroundBrush = new SKPaint()
        {
            Style = SKPaintStyle.Fill,
            Color = Color.Red.ToSKColor(),
        };

        private void TempGaugeCanvas_PaintSurface(object sender, SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs e)
        {
            SKImageInfo info = e.Info;
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;

            // Draw a background
            SKRect backgroundBounds = new SKRect(0, 0, info.Width, info.Height);
            canvas.DrawRoundRect(backgroundBounds, 20, 20, backgroundBrush);
        }
    }
}
