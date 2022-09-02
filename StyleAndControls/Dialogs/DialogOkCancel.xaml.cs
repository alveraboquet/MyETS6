using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Логика взаимодействия для DialogOkCancel.xaml
    /// </summary>
    public partial class DialogOkCancel
    {
        public MessageBoxResult Result;

        #region Constructors

        public DialogOkCancel()
        {
            InitializeComponent();

            this.ShowDialog();
        }

        public DialogOkCancel(object message, string title)
        {
            InitializeComponent();
            
            this.Title = title;
          
            this.MessageBoxOkCancel.Text = message.ToString();

            this.ShowDialog();
        }

        #endregion

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.OK;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.Cancel;
            Close();
        }

        private void DialogOkCancel_OnClosing(object sender, CancelEventArgs e)
        {
            if (Result == MessageBoxResult.None)
                Result = MessageBoxResult.Cancel;
        }
    }
}
