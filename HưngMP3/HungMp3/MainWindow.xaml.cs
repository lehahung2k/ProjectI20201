using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        //Danh sách ảnh
        public string imageVN { get; } = @"~\..\Resources\nhacvn.jpg";
        public string imageUS { get; } = @"~\..\Resources\nhacusuk.jpg";
        public string imageYeuThich { get; } = @"~\..\Resources\nhacyeuthich.jpg";
        public string imageNgheNhieu { get; } = @"~\..\Resources\Love.png";

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
                isListVN = value; isListUS = false; isListYeuThich = false; isListNgheNhieu = false;
                curList = ListVN;
                listMenu.ItemsSource = curList;
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
                curList = ListYeuThich;
                listMenu.ItemsSource = curList;
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
                curList = ListNgheNhieu;
                listMenu.ItemsSource = curList;
                OnPropertyChanged("IsListVN");
                OnPropertyChanged("IsListUS");
                OnPropertyChanged("IsListYeuThich");
                OnPropertyChanged("IsListNgheNhieu");
            } 
        }
        public bool IsListUS { 
            get => isListUS; 
            set { 
                isListUS = value; isListVN = false; isListYeuThich = false; isListNgheNhieu = false;
                curList = ListUS;
                listMenu.ItemsSource = curList;
                OnPropertyChanged("IsListVN");
                OnPropertyChanged("IsListUS");
                OnPropertyChanged("IsListYeuThich");
                OnPropertyChanged("IsListNgheNhieu");
            } 
        }
        //danh sách
        private ObservableCollection<Song> listVN;
        private ObservableCollection<Song> listUS;
        private ObservableCollection<Song> listYeuThich;
        private ObservableCollection<Song> listNgheNhieu;
        private List<Song> listShuffle = new List<Song>(); //List các bài đã play khi bật chế độ shuffle --> tránh phát lại các bài có trong list này
        private ObservableCollection<Song> curList; //list hiện tại đang chọn

        private Song currentSong;

        public ObservableCollection<Song> ListVN { get => listVN; set => listVN = value; }
        public ObservableCollection<Song> ListUS { get => listUS; set => listUS = value; }
        public ObservableCollection<Song> ListYeuThich { get => listYeuThich; set => listYeuThich = value; }
        public ObservableCollection<Song> ListNgheNhieu { get => listNgheNhieu; set => listNgheNhieu = value; }

        //crawl data:
        void CrawlData()
        {
            HttpRequest http = new HttpRequest();

            string htmlData = http.Get(@"https://www.nhaccuatui.com/bai-hat/top-20.nhac-viet.html").ToString();
            string htmlPattern = @"<div class=""box_resource_slide"">(.*?)</ul>";
            var listBXH = Regex.Matches(htmlData, htmlPattern, RegexOptions.Singleline);

            string bxhVN = listBXH[0].ToString();

            AddSongsToList(ListVN, bxhVN);

            //US=UK:
            string htmlData0 = http.Get(@"https://www.nhaccuatui.com/bai-hat/top-20.au-my.html").ToString();
            string htmlPattern0 = @"<div class=""box_resource_slide"">(.*?)</ul>";
            var listBXH1 = Regex.Matches(htmlData0, htmlPattern0, RegexOptions.Singleline);

            string bxhUS = listBXH1[0].ToString();
            AddSongsToList(ListUS, bxhUS);
        }

        //thêm bài hát vào danh sách
        void AddSongsToList(ObservableCollection<Song> listSong, string html)
        {
            var listSongHtml = Regex.Matches(html, @"<li>(.*?)</li>", RegexOptions.Singleline);
            for (int i = 0; i < listSongHtml.Count; i++)
            {
                var song = Regex.Matches(listSongHtml[i].ToString(), @"<a\shref=""https://(.*?)\stitle=""(.*?)""", RegexOptions.Singleline);

                string songString = song[0].ToString();
                int indexSong = songString.IndexOf("title=\"");
                //Lấy tên bài hát và ca sĩ
                string songName = songString.Substring(songString.IndexOf("title=\""), songString.Length - indexSong - 1).Replace("title=\"", "");

                //lấy link bài hát
                int indexUrl = songString.IndexOf("https");
                string urlSong = songString.Substring(indexUrl, indexSong - indexUrl - 2);

                //Lấy ảnh bài hát:
                string imgUrl = Regex.Match(listSongHtml[i].ToString(), @"<img\ssrc=\""(.*?)""", RegexOptions.Singleline).Value;
                string imageUrl = imgUrl.Replace("<img src=\"", "").Replace("\"", "");
                //imageUrl = imageUrl.Replace("<img src=\"", "").Replace("\"", "");


                //Lấy lyrics:
                HttpRequest http = new HttpRequest();
                string htmlSong = http.Get(urlSong).ToString();
                var lyrics = Regex.Matches(htmlSong, @"<p id=""divLyric"" class=""pd_lyric trans""(.*?)</p>", RegexOptions.Singleline);
                string tempLyric = "Chưa có";
                if (lyrics.Count > 0)
                {
                    tempLyric = lyrics[0]
                                    .ToString()
                                    .Replace("<p id=\"divLyric\" class=\"pd_lyric trans\" style=\"height:auto;max-height:255px;overflow:hidden;\">", "");
                    tempLyric = tempLyric.Replace("<br />", "").Replace("</p>", "");
                }
                //Lấy url để download
                string dlUrl = Regex.Match(htmlSong.ToString(), @"<iframe\ssrc=""https://www.n(.*?)""", RegexOptions.Singleline).Value;
                string downloadUrl = dlUrl.Replace("<iframe src=\"", "").Replace("\"", "");
               
                //Lấy đường dẫn trên máy:
                string savePath = AppDomain.CurrentDomain.BaseDirectory + "Song\\" + songName + ".mp3";

                listSong.Add(new Song() {SongName = songName, SongUrl = urlSong, 
                                        STT = i + 1, Lyrics = tempLyric, DownloadUrl = downloadUrl, 
                                        SavePath = savePath, ImageUrl = imageUrl });
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
            Song song = (sender as Grid).DataContext as Song;
            //MessageBox.Show(song.Lyrics);
            currentSong = song;
            playList.Visibility = Visibility.Hidden;
            songControl.Visibility = Visibility.Visible;
            songControl.Infor = song;
        }

        //Thêm 1 bài hát vào danh sách yêu thích
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ObservableCollection<Song> curList = null;
            if (IsListVN) curList = ListVN;
            else if (IsListUS) curList = ListUS;

            if (curList == null) return;

            Song song = (((sender as Button).Parent as Grid).Parent as Grid).DataContext as Song;
            if (!ListYeuThich.Contains(song)) ListYeuThich.Add(song);
            else MessageBox.Show(Globals.textListContained);
        }

        //Binding từ giao diện sang code
        protected virtual void OnPropertyChanged(string newName)
        {
            //Clear danh sách đã play
            listShuffle.Clear();

            //Tạo event mà không thấy dùng ở đâu cả?
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(newName));
            }       
        }

        public MainWindow()
        {
            InitializeComponent();
            songControl.BackToList += SongControl_BackToList; //Hàm quay về playlist
            songControl.ShuffleToggled += songControl_ShuffleToggled; //Thêm hàm xử lý cho event shuffle toggled
            this.DataContext = this;

            ListVN = new ObservableCollection<Song>();
            ListUS = new ObservableCollection<Song>();
            ListYeuThich = new ObservableCollection<Song>();
            ListNgheNhieu = new ObservableCollection<Song>();

            CrawlData();

            IsListVN = true;

        }

        //Xử lý bài hát trước sau:
        void ChangeSong(ObservableCollection<Song> listSong, int position, int add)
        {
            int index = listSong.IndexOf(currentSong);
            
            if (songControl.Shuffle)
            {
                //Nếu là đang shuffle
                Random gen = new Random();
                if (listShuffle.Count >= listSong.Count)
                    listShuffle.Clear();
                do
                {
                    currentSong = listSong[gen.Next(0, listSong.Count)];
                } while (listShuffle.Contains(currentSong));
                listShuffle.Add(currentSong);
                songControl.Infor = currentSong;
            }
            else
            {
                //Nếu bài đầu hoặc cuối danh sách thì nhấn nút nó ko làm gì
                //=> sửa thành thì nó lặp lại danh sách
                if (index == 0 && add == -1) currentSong = listSong.Last();
                else if (index == listSong.Count - 1 && add == 1) currentSong = listSong.First();
                else currentSong = listSong[index + add];
                songControl.Infor = currentSong;
            }
        }
        //Khi nhấn nút pre sẽ nhảy về bài trc
        private void songControl_PreviousClick(object sender, EventArgs e)
        {
            ChangeSong(curList, 0, -1);
        }

        //Khi nhấn nút next sẽ nhảy về bài sau
        private void songControl_NextClick(object sender, EventArgs e)
        {
            ChangeSong(curList, 19, 1);
        }

        //Xử lý khi check box shuffle thay đổi value
        private void songControl_ShuffleToggled(bool shuffle)
        {
            //Nếu shuffle chuyển từ true sang false thì clear list đã play (listShuffle)
            if (!shuffle)
            {
                listShuffle.Clear();
            }
        }
    }
}
