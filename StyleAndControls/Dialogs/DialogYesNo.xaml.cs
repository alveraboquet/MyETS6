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

namespace ClassControlsAndStyle.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для DialogYesNo.xaml
    /// </summary>
    public partial class DialogYesNo 
    {
        public MessageBoxResult Result;

        #region Constructors

        public DialogYesNo()
        {
            InitializeComponent();

            this.ShowDialog();
        }

        public DialogYesNo(object message, String title)
        {
            InitializeComponent();
            this.Title = title;
            this.MsgBox.Text = message.ToString();

            this.ShowDialog();
        }

        #endregion

        private void BtnNo_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.No;
            Close();
        }

        private void BtnYes_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.Yes;
            Close();
        }
    }
}
