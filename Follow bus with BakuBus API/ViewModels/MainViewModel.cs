using Follow_bus_with_BakuBus_API.Command;
using Follow_bus_with_BakuBus_API.Models;
using Microsoft.Maps.MapControl.WPF;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;
using System.Xml.Serialization;

namespace Follow_bus_with_BakuBus_API.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {



        public List<Bus> _GetBusList { get; set; }
        public List<Bus> GetBusList { get { return _GetBusList; } set { _GetBusList = value; OnpropertyChanged(); } }
        public List<Bus> _SetBusList { get; set; }
        public List<Bus> SetBusList { get { return _SetBusList; } set { _SetBusList = value; OnpropertyChanged(); } }

        private Bus _bus;

        public Bus Buses { get { return _bus; } set { _bus = value; OnpropertyChanged(); } }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnpropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public RelayCommand FindCommand { get; set; }



        public MainWindow MainWindows { get; set; }


        public ApplicationIdCredentialsProvider Provider { get; set; }
        string APIKey = ConfigurationManager.AppSettings["BingKey"];
        public HttpClient Client { get; set; } = new HttpClient();
        HttpResponseMessage responseMessage = new HttpResponseMessage();
        public string apiLink = @"https://www.bakubus.az/az/ajax/apiNew1";

        public dynamic Data { get; set; }


        public MainViewModel()
        {


            Provider = new ApplicationIdCredentialsProvider(APIKey);


            responseMessage = Client.GetAsync($@"{apiLink}").Result;
            var str = responseMessage.Content.ReadAsStringAsync().Result;

            Data = JsonConvert.DeserializeObject(str);


            GetBusList = new List<Bus>();
            SetBusList = new List<Bus>();

            foreach (var item in Data.BUS)
            {
                dynamic busatribute = item["@attributes"];


        


                GetBusList.Add(new Bus
                    {
                        BUS_ID = busatribute["BUS_ID"],
                        PLATE = busatribute["PLATE"],
                        CURRENT_STOP = busatribute["CURRENT_STOP"],
                        PREV_STOP = busatribute["PREV_STOP"],
                        SPEED = busatribute["SPEED"],
                        BUS_MODEL = busatribute["BUS_MODEL"],
                        LATITUDE = busatribute["LATITUDE"],
                        LONGITUDE = busatribute["LONGITUDE"],
                        ROUTE_NAME = busatribute["ROUTE_NAME"],
                        LAST_UPDATE_TIME = busatribute["LAST_UPDATE_TIME"],
                        DISPLAY_ROUTE_CODE = busatribute["DISPLAY_ROUTE_CODE"],
                        SVCOUNT = busatribute["SVCOUNT"],
                        ImagePath = "../Images/Bus vehicle 1.jpg",

                    });


                var xml = new XmlSerializer(typeof(List<Bus>));
                using (var fs = new FileStream("../file.xml", FileMode.OpenOrCreate))
                {
                    xml.Serialize(fs, GetBusList);

                }



            }




     


            FindCommand = new RelayCommand((e) =>
            {




                if (MainWindows.BusNumberCombobox.SelectedIndex != -1)
                {
                    int index = 0;



                    index = MainWindows.BusNumberCombobox.SelectedIndex;

                    var f = GetBusList[index].BusInfo;



                    if (f == String.Empty)
                    {

                        SetBusList = GetBusList;

            
                    }

                    else
                    {

                        Binding bindings = new Binding();
                        bindings.Path = new PropertyPath("SetBusList");
                        bindings.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                        MainWindows.MapItem.SetBinding(MapItemsControl.ItemsSourceProperty, bindings);



                        SetBusList = new List<Bus>(GetBusList.Where(x => x.BusInfo == f));

                        SetBusList.ForEach(y => MessageBox.Show(y.BusInfo));
                    }

 
                }

        

            });


        }




 
    }
}
