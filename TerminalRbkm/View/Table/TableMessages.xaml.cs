using ScriptSolution.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace TerminalRbkm.View.Table
{
    /// <summary>
    /// Логика взаимодействия для TableMessages.xaml
    /// </summary>
    public partial class TableMessages : UserControl
    {
        public TableMessages(ObservableCollection<PersonalLog> col)
        {
            InitializeComponent();
            //DataContext = this;
            ListBoxLogs.ItemsSource = col;
        }
    }
}
