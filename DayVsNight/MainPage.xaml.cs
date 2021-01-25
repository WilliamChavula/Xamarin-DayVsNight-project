using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    }
}
