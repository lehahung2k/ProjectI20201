using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using xNet;

namespace HungMp3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        //Kiểm tra danh sách
        private bool isListVN;
        private bool isListUS;
        private bool isListYeuThich;
        private bool isListNgheNhieu;

        public event PropertyChangedEventHandler PropertyChanged;

        //Chọn playlist để phát:
        public bool IsListVN { 
            get => isListVN; 
            set { 
                isListVN = value; listMenu.ItemsSource = ListVN; isListUS = false; isListYeuThich = false; isListNgheNhieu = false;
                OnPropertyChanged("IsListVN");   //Xác định sự kiện để cập nhật trên màn hình
                OnPropertyChanged("IsListUS");
                OnPropertyChanged("IsListYeuThich");
                OnPropertyChanged("IsListNgheNhieu");
            } 
        }
        public bool IsListYeuThich { 
            get => isListYeuThich; 
            set { 
                isListYeuThich = value; isListVN = false; isListUS = false; isListNgheNhieu = false;
                OnPropertyChanged("IsListVN");
                OnPropertyChanged("IsListUS");
                OnPropertyChanged("IsListYeuThich");
                OnPropertyChanged("IsListNgheNhieu");
            } 
        }
        public bool IsListNgheNhieu { 
            get => isListNgheNhieu; 
            set { 
                isListNgheNhieu = value; isListVN = false; isListUS = false; isListYeuThich = false;
                OnPropertyChanged("IsListVN");
                OnPropertyChanged("IsListUS");
                OnPropertyChanged("IsListYeuThich");
                OnPropertyChanged("IsListNgheNhieu");
            } 
        }
        public bool IsListUS { 
            get => isListUS; 
            set { 
                isListUS = value; listMenu.ItemsSource = ListUS; isListVN = false; isListYeuThich = false; isListNgheNhieu = false;
                OnPropertyChanged("IsListVN");
                OnPropertyChanged("IsListUS");
                OnPropertyChanged("IsListYeuThich");
                OnPropertyChanged("IsListNgheNhieu");
            } 
        }
        //danh sách
        private List<Song> listVN;
        private List<Song> listUS;
        private List<Song> listYeuThich;
        private List<Song> listNgheNhieu;

        public List<Song> ListVN { get => listVN; set => listVN = value; }
        public List<Song> ListUS { get => listUS; set => listUS = value; }
        public List<Song> ListYeuThich { get => listYeuThich; set => listYeuThich = value; }
        public List<Song> ListNgheNhieu { get => listNgheNhieu; set => listNgheNhieu = value; }

        //crawl data:
        void CrawlData()
        {
            HttpRequest http = new HttpRequest();

            string htmlData = http.Get(@"https://www.nhaccuatui.com/bai-hat/top-20.nhac-viet.html").ToString();
            string htmlPattern = @"<div class=""box_resource_slide"">(.*?)</div>";
            var listBXH = Regex.Matches(htmlData, htmlPattern, RegexOptions.Singleline);

            string bxhVN = listBXH[0].ToString();
            AddSongToList(ListVN, bxhVN);

            string bxhUS = listBXH[0].ToString();
            AddSongToList(ListUS, bxhUS);
        }

        void AddSongToList(List<Song> listSong, string html)
        {
            var listSongHtml = Regex.Matches(html, @"<li>(.*?)</li>", RegexOptions.Singleline);
            for (int i = 0; i < listSongHtml.Count; i++)
            {
                var song = Regex.Matches(listSongHtml[i].ToString(), @"<a\s\S*\slp=""\s\S*\s\S*\shref=""(.*?)""", RegexOptions.Singleline);
                var singer = Regex.Matches(listSongHtml[i].ToString(), @"", RegexOptions.Singleline);

                string songString = song[0].ToString();
                int indexSong = songString.IndexOf("title=\"");

                string songName = songString.Substring(songString.IndexOf("title=\""), songString.Length - indexSong - 1).Replace("title=\"", "");

                int indexUrl = songString.IndexOf("href=\"");
                string urlSong = songString.Substring(indexUrl, indexSong - indexUrl - 2).Replace("href=\"", "");

                listSong.Add(new Song { SingerName = "", SongName = songName, SongUrl = urlSong, STT = i + 1 });
            }
        }

        //Từ trình phát nhạc back về playlist
        private void SongControl_BackToList(object sender, EventArgs e)
        {
            playList.Visibility = Visibility.Visible;
            songControl.Visibility = Visibility.Hidden;
        }

        //Từ playlist chuyển sang trình phát nhạc
        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            playList.Visibility = Visibility.Hidden;
            songControl.Visibility = Visibility.Visible;
        }
        //Binding 
        protected virtual void OnPropertyChanged(string newName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(newName));
        }

        public MainWindow()
        {
            InitializeComponent();
            songControl.BackToList += SongControl_BackToList;
            this.DataContext = this;

            IsListVN = true;

            ListVN = new List<Song>();
            ListUS = new List<Song>();
            ListYeuThich = new List<Song>();
            ListNgheNhieu = new List<Song>();

            CrawlData();

        }
    }
}
