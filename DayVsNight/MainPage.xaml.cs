using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using DayVsNight.Resources.Themes;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace DayVsNight
{
    public partial class MainPage : ContentPage
    {
        public ObservableCollection<Room> Rooms { get; set; }
        public MainPage()
        {
            InitializeComponent();
            Rooms = new ObservableCollection<Room>();
            loadRooms();
            BindingContext = this;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            MessagingCenter.Subscribe<ThemeMessage>(this, ThemeMessage.ThemeChanged, (themeMessage) => UpdateTheme());
        }

        private void UpdateTheme()
        {
            BackgroundGradient.InvalidateSurface();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            MessagingCenter.Unsubscribe<ThemeMessage>(this, ThemeMessage.ThemeChanged);
        }

        private void loadRooms()
        {
            Rooms.Clear();
            Rooms.Add(new Room
            {
                ImageLabel = "Room1",
                RoomLabel = "Zone 1"

            });
            Rooms.Add(new Room
            {
                ImageLabel = "Room2",
                RoomLabel = "Zone 2"

            });
            Rooms.Add(new Room
            {
                ImageLabel = "Room3",
                RoomLabel = "Zone 3"

            });
        }

        string themeName = "light";

        void ProfileImage_Tapped(System.Object sender, System.EventArgs e)
        {
            if (themeName == "light")
                themeName = "dark";
            else
                themeName = "light";

            ThemeHelper.ChangeTheme(themeName);
        }

        SKPaint backgroundBrush = new SKPaint()
        {
            Style = SKPaintStyle.Fill,
            Color = Color.Red.ToSKColor()
        };

        void BackgroundGradient_PaintSurface(System.Object sender, SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs e)
        {
            SKImageInfo info = e.Info;
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.Clear();

            //Debug.WriteLine($"Info: {info}");
            //Debug.WriteLine($"Surface: {surface}");
            //Debug.WriteLine($"Canvas: {canvas}");

            // get brush based on theme
            SKColor gradientStart = ((Color)Application.Current.Resources["BackgroundGradientStartColor"]).ToSKColor();
            SKColor gradientMid = ((Color)Application.Current.Resources["BackgroundGradientMidColor"]).ToSKColor();
            SKColor gradientEnd = ((Color)Application.Current.Resources["BackgroundGradientEndColor"]).ToSKColor();

            // Create radial gradient
            backgroundBrush.Shader = SKShader.CreateRadialGradient
                (new SKPoint(0, info.Height * .8f),
                info.Height * .8f,
                new SKColor[] { gradientStart, gradientMid, gradientEnd },
                new float[] { 0, .5f, 1 },
                SKShaderTileMode.Clamp);


            // Create linear gradient
            //backgroundBrush.Shader = SKShader.CreateLinearGradient(
            //        new SKPoint(0, 0),
            //        new SKPoint(info.Width, info.Height),
            //        new SKColor[]
            //        {
            //            gradientStart,
            //            gradientMid,
            //            gradientEnd
            //        },
            //        new float[] { 0, 0.5f, 1 },
            //        SKShaderTileMode.Clamp);
            SKRect backgroundBounds = new SKRect(0, 0, info.Width, info.Height);
            canvas.DrawRect(backgroundBounds, backgroundBrush);
        }
    }
}
