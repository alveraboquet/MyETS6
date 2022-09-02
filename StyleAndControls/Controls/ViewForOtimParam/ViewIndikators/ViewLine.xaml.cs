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
using System.Windows.Shapes;

namespace StyleAndControls.Controls.ViewIndikators
{
    /// <summary>
    /// Interaction logic for ViewLine.xaml
    /// </summary>
    public partial class ViewLine 
    {
        public ViewLine()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Title SourcesForTestModelbox
        /// </summary>
        public object GpbxHeader
        {
            get { return GpbxItem.Header; }
            set { GpbxItem.Header = value; }
        }

        /// <summary>
        /// Добавляем в стекпанель настраевыемые параметры индикатора
        /// </summary>
        public UIElement StplChildren
        {
            get { return null; }
            set { StplViewLine.Children.Add(value); }
        }

        /// <summary>
        /// Получаем список элементов
        /// </summary>
        /// <returns></returns>
        public UIElementCollection GetShildrenStpl()
        {
            return StplViewLine.Children;
        }
    }
}
