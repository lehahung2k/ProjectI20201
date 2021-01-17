using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
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

namespace HungMp3
{
    /// <summary>
    /// Interaction logic for InterfaceApp.xaml
    /// </summary>
    public partial class InterfaceApp : UserControl, INotifyPropertyChanged
    {
        //Ảnh
        public string imageAudio { get; } = @"~\..\Resources\audio.png";
        public string imagePre { get; } = @"~\..\Resources\pre.png";
        public string imageNext { get; } = @"~\..\Resources\next.png";
        public string imagePlay
        {
            get
            {
                if (IsPlaying) return @"~\..\Resources\pause.png";
                else return @"~\..\Resources\play.png";
            }
        }

        private bool shuffle;
        public bool Shuffle
        {
            get { return shuffle; }
            set
            {
                shuffle = value;
                ShuffleToggled?.Invoke(shuffle);
            }
        }

        public delegate void CheckBoxToggled(bool isChecked);
        public event CheckBoxToggled ShuffleToggled;

        private Song infor;
        public Song Infor 
        { 
            get { return infor; } 
            set 
            { 
                infor = value; 
                DownloadSong(Infor);
                this.DataContext = Infor;
                OnPropertyChanged("Infor");
                
            } 
        }

        //Đếm thời gian:
        DispatcherTimer timer;

        public InterfaceApp()
        {
            InitializeComponent();
            cbxShuffle.DataContext = this;
            gridControl.DataContext = this;
            this.DataContext = Infor;
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            Infor.Position++;
            audioDuration.Value = Infor.Position;
        }

        //Tạo event click chuột
        private event EventHandler backToList;
        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler BackToList
        {
            add { backToList += value; }
            remove { backToList -= value; }
        }

        void DownloadSong(Song infor)
        {
            string songName = Infor.SavePath;
            //Kiểm tra xem đã có file hay chưa?
            if (!File.Exists(songName))
            {
                WebClient wb = new WebClient();
                wb.DownloadFile(Infor.DownloadUrl, songName);
            }
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (backToList != null) backToList(this, new EventArgs());
        }

        //Binding
        protected virtual void OnPropertyChanged(string newName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(newName));
            }
        }

        private void audio_MediaOpened(object sender, RoutedEventArgs e)
        {
            audio.MediaEnded += Audio_MediaEnded;
            IsPlaying = true;
            Infor.Duration = audio.NaturalDuration.TimeSpan.TotalSeconds;
            //Hiển thị thời gian bài hát: phút:giây
            timeEnd.Text = new TimeSpan(0, (int)Infor.Duration / 60, (int)Infor.Duration % 60).ToString(@"mm\:ss");
            audioDuration.Maximum = Infor.Duration;
            Infor.Position = 0;
        }

        //Kéo thả theo thời gian audio phát:
        //Kiểm tra xem có kéo hay ko:
        bool isKeo = false;
        private void audioDuration_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (isKeo)
            {
                Infor.Position = audioDuration.Value;
                audio.Position = new TimeSpan(0, 0, (int)Infor.Position);
            }
            //Lấy thời gian đang chạy
            timeDuration.Text = new TimeSpan(0, (int)Infor.Position / 60, (int)Infor.Position % 60).ToString(@"mm\:ss");
        }

        private void audioDuration_MouseDown(object sender, MouseButtonEventArgs e)
        {
            isKeo = true;
        }

        private void audioDuration_MouseUp(object sender, MouseButtonEventArgs e)
        {
            isKeo = false;
        }

        //Xử lý sự kiện click nút play:
        public bool IsPlaying
        {
            get { return isPlaying; }
            set
            {
                isPlaying = value;
                if (isPlaying)
                {
                    audio.Play();
                    timer.Start();
                    //btnPlay.Content = new Uri("D:\\20201\\Project I\\ProjectI20201\\HưngMP3\\HungMp3\\Resources\\play.png");
                }
                else 
                { 
                    audio.Pause();
                    timer.Stop();
                }
            }
        }

        private void Audio_MediaEnded(object sender, RoutedEventArgs e)
        {
            IsPlaying = false;
            if (cbxLoop.IsChecked == true)
            {
                audio.Position = new TimeSpan(0);
                infor.Position = 0;
                IsPlaying = true;
            }
            else nextClick?.Invoke(this, new EventArgs());
        }

        private bool isPlaying;
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            IsPlaying = !IsPlaying;
            
        }

        //Xử lý nút previous
        private event EventHandler previousClick;
        public event EventHandler PreviousClick
        {
            add { previousClick += value; }
            remove { previousClick -= value; }
        }
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (previousClick != null) previousClick(this, new EventArgs());
        }
        //xử lý nút next:
        private event EventHandler nextClick;
        public event EventHandler NextClick
        {
            add { nextClick += value; }
            remove { nextClick -= value; }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            if (nextClick != null) nextClick(this, new EventArgs());
        }
        //Volume
        private void ChangeMediaVolume(object sender, RoutedPropertyChangedEventArgs<double> args)
        {
            audio.Volume = (double)volumeSlider.Value;
        }

    }
}