using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.IO;
using static System.Net.Mime.MediaTypeNames;
using Path = System.IO.Path;
using System.Reflection;

namespace WPFHW4
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool isPlaying = false;
        bool isMuted = false;
        double tmp;
        List<string> files = new List<string>();
        List<int> type = new List<int>();
        DispatcherTimer timer; 
        public MainWindow()
        {
            InitializeComponent();
            sound.ValueChanged += sound_ValueChanged;
            list.SelectionChanged += List_SelectionChanged;
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            seek.Value = medElement.Position.TotalSeconds;
            lblPos.Content = medElement.Position.TotalMinutes.ToString();
        }

        private void List_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            if (File.Exists(files[list.SelectedIndex]))
            {
                Open(true, files[list.SelectedIndex]);
                if (type[list.SelectedIndex] == 1)
                {
                    medElement.Visibility = Visibility.Collapsed;
                    img.Visibility = Visibility.Visible;
                    img.Source = new BitmapImage(new Uri("/bgImage.jpg", UriKind.Relative));
                }
                else if (type[list.SelectedIndex] == 2)
                {
                    medElement.Visibility = Visibility.Visible;
                    img.Visibility = Visibility.Collapsed;
                    img.Source = null;
                }
            }
            else
            {
                medElement.Stop();
                isPlaying = false;

                playBtn_Copy.Visibility = Visibility.Collapsed;
                playBtn.Visibility = Visibility.Visible;
                img.Visibility = Visibility.Collapsed;
                medElement.Visibility = Visibility.Collapsed;
                img.Source = null;
                medElement.Source = null;

                MessageBox.Show("Can't open the file!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Open(bool check, string fileName)
        {
            
            if (check)
            {
                medElement.Source = new Uri(fileName);
                medElement.Volume = sound.Value / 100;
                img.Source = null;

                medElement.LoadedBehavior = MediaState.Manual;
                medElement.UnloadedBehavior = MediaState.Manual;

                medElement.Play();
                isPlaying = true;

                playBtn_Copy.Visibility = Visibility.Visible;
                playBtn.Visibility = Visibility.Collapsed;
            }
        }

        private void openBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Audio files (*.mp3;*.wav;*.wma;*.mid;*.midi)|" +
                "*.mp3;*.wav;*.wma;*.mid;*.midi|" +
                "Video files (*.mp4;*.avi;*.mkv;*.wmv)|" +
                "*.mp4;*.avi;*.mkv;*.wmv";

            Open((bool)openFile.ShowDialog(), openFile.FileName);

            if (openFile.FilterIndex == 1)
            {
                medElement.Visibility = Visibility.Collapsed;
                img.Visibility = Visibility.Visible;
                img.Source = new BitmapImage(new Uri("/bgImage.jpg", UriKind.Relative));
            }
            else if (openFile.FilterIndex == 2)
            {
                medElement.Visibility = Visibility.Visible;
                img.Visibility = Visibility.Collapsed;
                img.Source = null;
            }
            type.Add(openFile.FilterIndex);
            files.Add(openFile.FileName);
            list.Items.Add(Path.GetFileName(openFile.FileName));
        }

        private void playBtn_Click(object sender, RoutedEventArgs e)
        {

            if (medElement.Source!=null)
            {
                if (isPlaying)
                {
                    playBtn_Copy.Visibility = Visibility.Collapsed;
                    playBtn.Visibility = Visibility.Visible;
                    medElement.Pause();
                    isPlaying = false;

                }
                else
                {
                    playBtn_Copy.Visibility = Visibility.Visible;
                    playBtn.Visibility = Visibility.Collapsed;
                    medElement.Play();
                    isPlaying = true;
                }
            }
        }

        private void fwdBtn_Click(object sender, RoutedEventArgs e)
        {
            medElement.Position = medElement.Position.Add(new TimeSpan(0, 0, 10));
        }

        private void bwdBtn_Click(object sender, RoutedEventArgs e)
        {
            medElement.Position = medElement.Position.Subtract(new TimeSpan(0, 0, 10));
        }

        private void sound_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            medElement.Volume = sound.Value / 100;
            lbl.Content = (int)sound.Value;
        }

        private void soundBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!isMuted)
            {
                tmp = sound.Value;
                sound.Value = 0;
                isMuted = true;
                soundBtn.Content = "Muted";
            }
            else 
            {
                sound.Value = tmp;
                isMuted = false;
                soundBtn.Content = "Unmuted";
            }
        }

        private void speedChecked(object sender, RoutedEventArgs e)
        {
            if ((bool)x05.IsChecked)
            {
                medElement.SpeedRatio = 0.5;
            }
            if ((bool)x1.IsChecked)
            {
                medElement.SpeedRatio = 1;
            }
            if ((bool)x125.IsChecked)
            {
                medElement.SpeedRatio = 1.25;
            }
            if ((bool)x15.IsChecked)
            {
                medElement.SpeedRatio = 1.5;
            }
            if ((bool)x175.IsChecked)
            {
                medElement.SpeedRatio = 1.75;
            }
            if ((bool)x2.IsChecked)
            {
                medElement.SpeedRatio = 2;
            }
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            medElement.Position = TimeSpan.FromMilliseconds(seek.Value);
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            string fileName = (string)((DataObject)e.Data).GetFileDropList()[0];
            Open(true, fileName);
        }

        private void medElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            TimeSpan timeSpan = medElement.NaturalDuration.TimeSpan;
            seek.Maximum = timeSpan.TotalSeconds;
            lblMax.Content = timeSpan.TotalMinutes.ToString();
            timer.Start();
        }
    }
}
