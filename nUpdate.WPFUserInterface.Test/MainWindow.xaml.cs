// MainWindow.xaml.cs, 10.06.2019
// Copyright (C) Dominic Beger 17.06.2019

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using nUpdate.Updating;
using System.Threading;
using System.Globalization;
using System.Windows.Data;

namespace nUpdate.WPFUserInterface.Test
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private UpdaterUI _updaterUI;
        private UpdateManager _manager;

        public MainWindow()
        {
            InitializeComponent();
            Services.ServiceInjector.InjectServices();

            ConditionValues =
                new ObservableCollection<KeyValueHelper>();

            DataContext = this;
        }


        public ObservableCollection<KeyValueHelper> ConditionValues { get; set; }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _manager = new UpdateManager(
                new Uri("http://spsonline.remotewebaccess.com/SPS/SPSSoftware/Downloads/updates.json"),
                "<RSAKeyValue><Modulus>odPC4iQ8XRXHSuikaF1AHdKwplw7tH4shZZahkUAyICdiOhK4ET8a5Ad7WXHTUbiUUEjFnAJLemM48ZfVnD8NAzAtOs4HqmKDeqhsM6kaM1GTHIrvSgWeOaBaELXdorqzfy0ih3Qfwi7E6VSZMddv3Fl8tBXpVaSXhreUUJPp01ifgKGRKUZWcIM5/0iNgI8u1k3uz8VGhkfNMyl3FyG7pBpp93gDqtJsp7jw+BtdnZ2w/WshCMACKve1d8eD4Z5uud/ybj42HYQnYYYIEVOOsi6dLM9fHeaf2Rp7i8clFislffFcXemgRlHyYAXq7l3yKT7PQtvCd1oL5wCtxbQKtI2QkPu4mHJz7lHoFHanmOCaYHaDoyLYZAUqw1Fyg3gYOnFD9yHFKIxk43ecY4KqFlpiKrJ9PUlUZLI8fyfjhStFwreh5Qh7Sgmo9pnNy3nWkfJZdiaZgtOd8QNyvtkUBE3IqtPcJJo+0/AkskMsJbnqZOpS2GxfRZuXpEk0IvZkGyU3OQk6dIASbhQHL0kaN3gQf+xVIrlrH1G+NHE9s/wzilRuo5cunlglL5W6cfIzzicX7AwsPlpR7QoMIl1xO9s4ldrnDJe1WxJHBue8Aq396MnepCsEUOq68k4xMOeoOsiccAVWnj9uv3aaz55jRYf0in2JXIrAgRwk95sqqVjpjHSY+2bFZ7WhXClzU3y2R8fqYU5JzVolN3eIUfp3TY4AfY80ysaNUhZUth7UfshWeLfZz8D8agUCKGx5UsUXfal6SmGyHxkulSMYVJNgb//cIiYUOztviKQLyrUAwLUUKaSyottLICO8RKQDHqJ6iHE3VVTZ7o47ppB4iAMKa6sIfH+LOHXPtnVkga3H6Px6Saf/TaP+gwOLegrfPvrvI1oHlpD1kcuUmU2v/SWTaPNu5zdgv0KN1gkatFzvdXPewESL7wqQptSAT1KxAq4WEe+HVNC2pF9t4Ir6pBPQR2Lo+losVz6JGhXvxkonLFxv6Epf9+pWJXLFdt1vO25Co7RvH9IYaYo8abMQZCVDVfzjw4oulCDJpz+jsmYnWucUlHFP8AmOQnDwhOHkM4SbarLR/yl0VD2DwmPy43qArQyKr9awpkqWN8WpeU2RkfKK6BljB7cu8Zngah1ERb4KupenY1A+RqpgQvGmJtsxX8BDjzY5giK0WgPgRlNGmBuZB1B6w4qxnpZN0ecycUeIiEA4SlKLzpx5RhGl0xHxvSeg82EJg0xtmTydiap/+2OgFsTlPIQQQ0jSMqIKznf4/QYjR26ELWMERc779x//M4qIMsneQpK2yfd1VssomwOnc7SDVZC4mfndgZxbCc5c4TwlpYoGxrTqfjl1sXm1Q==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>",
                new CultureInfo("de-AT"))
            {
                UseDynamicUpdateUri = true,
                CustomInstallerUiAssemblyPath =
                    "C:\\Workspace\\nUpdate\\nUpdate.WPFUpdateInstaller\\bin\\Debug\\nUpdate.WPFUpdateInstaller.dll",
                UseCustomInstallerUserInterface = true
            };

            _manager.Conditions = new List<KeyValuePair<string, string>>();
            foreach (KeyValueHelper pair in ConditionValues)
            {
                _manager.Conditions.Add(new KeyValuePair<string, string>(pair.Key, pair.Value));
            }

            _updaterUI = new UpdaterUI(_manager, SynchronizationContext.Current);


            if (chkHidden.IsChecked != null)
            {
                _updaterUI.UseHiddenSearch = chkHidden.IsChecked.Value;
            }


            _updaterUI.ShowUserInterface();
        }
    }


    public class KeyValueHelper
    {
        public KeyValueHelper()
        {
        }

        public string Key { get; set; }
        public string Value { get; set; }
    }
}