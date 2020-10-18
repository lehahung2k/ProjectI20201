using System;
using System.Collections.Generic;
using System.Linq;
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

namespace HungMp3
{
    /// <summary>
    /// Interaction logic for InterfaceApp.xaml
    /// </summary>
    public partial class InterfaceApp : UserControl
    {
        public InterfaceApp()
        {
            InitializeComponent();
        }

        //Tạo event click chuột
        private event EventHandler backToList;
        public event EventHandler BackToList
        {
            add { backToList += value; }
            remove { backToList -= value; }
        }
       

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (backToList != null) backToList(this, new EventArgs());
        }
    }
}
