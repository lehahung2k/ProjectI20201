using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
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
                isListVN = value; isListUS = false; isListYeuThich = false; isListNgheNhieu = false;
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
                isListUS = value; isListVN = false; isListYeuThich = false; isListNgheNhieu = false;
                OnPropertyChanged("IsListVN");
                OnPropertyChanged("IsListUS");
                OnPropertyChanged("IsListYeuThich");
                OnPropertyChanged("IsListNgheNhieu");
            } 
        }

        public MainWindow()
        {
            InitializeComponent();
            songControl.BackToList += SongControl_BackToList;
            listMenu.ItemsSource = new List<String>() { "", "", "", "", "", "", "", "", "", "" };
            this.DataContext = this;

            IsListVN = true;
            
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
    }
}
