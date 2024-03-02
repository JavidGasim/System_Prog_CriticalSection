using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows;

namespace System_Prog_CriticalSection;

public partial class MainWindow : Window, INotifyPropertyChanged
{
    private ObservableCollection<Car> cars;
    public ObservableCollection<Car> Cars { 
        get { return cars; }

        set {  
            cars = value;
            OnPropertyChanged();
        }    
    }

    private string milliSecond;
    public string MilliSecond
    {
        get { return milliSecond; }
        set
        {
            milliSecond = value;
            OnPropertyChanged();
        }
    }

    private string second;
    public string Second
    {
        get { return second; }
        set
        {
            second = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    public void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public MainWindow()
    {
        InitializeComponent();

        MilliSecond = "00";
        Second = "00";

        DataContext = this;
    }

    private async void start_btn_Click(object sender, RoutedEventArgs e)
    {
        if (Cars != null)
        {
            Application.Current.Dispatcher.Invoke(() => { Cars.Clear(); });
        }

        if (single.IsChecked == true)
        {
            MilliSecond = "00";
            Second = "00";

            Thread thread = new Thread(() =>
            {
                var watch = new Stopwatch();
                watch.Start();
                ThreadSingle();
                watch.Stop();
                MilliSecond = watch.Elapsed.Milliseconds.ToString();
                Second = watch.Elapsed.Seconds.ToString();
            });
            thread.Start();
        }

        else
        {
            MilliSecond = "00";
            Second = "00";

            Thread thread = new Thread(() =>
            {
                var watch = new Stopwatch();
                watch.Start();
                ThreadMulti();
                watch.Stop();
                MilliSecond = watch.Elapsed.Milliseconds.ToString();
                Second = watch.Elapsed.Seconds.ToString();
            });
            thread.Start();
        }

        await Task.Delay(1000);
    }

    public async void ThreadSingle()
    {
        Thread thread1 = new Thread(() =>
        {
            object _lock = new object();
            lock (_lock)
            {
                var AllCarss = new List<Car>();
                for (int i = 1; i < 10; i++)
                {
                    string jsonName = "Cars" + i.ToString() + ".json";
                    if (File.Exists(jsonName))
                    {
                        var jsonTxt = File.ReadAllText(jsonName);
                        var c = JsonSerializer.Deserialize<List<Car>>(jsonTxt);
                        AllCarss.AddRange(c);
                    }
                }
                Application.Current.Dispatcher.Invoke(() => { Cars = new ObservableCollection<Car>(AllCarss); });
            }
        });
        thread1.Start();
        await Task.Delay(1000);
    }

    public async void ThreadMulti()
    {
        var AllCars = new List<Car>();
        object _lock = new object();
        for (int i = 1; i < 10; i++)
        {
            string jsonName = "Cars" + i.ToString() + ".json";
            if (File.Exists(jsonName))
            {
                Thread thread2 = new Thread(() =>
                {
                    lock (_lock)
                    {
                        var jsonTxt = File.ReadAllText(jsonName);
                        var c = JsonSerializer.Deserialize<List<Car>>(jsonTxt);
                        AllCars.AddRange(c);
                        Application.Current.Dispatcher.Invoke(() => { Cars = new ObservableCollection<Car>(AllCars); });
                    }
                });
                thread2.Start();
            }
        }

        await Task.Delay(1000);
    }
}