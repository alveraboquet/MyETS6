using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace StyleAndControls.Controls
{
    /// <summary>
    /// Текстбокс с фильтром для названия файлов, исключающий ненужные символы
    /// </summary>
    public class TextBoxFiltrName : TextBox
    {

        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            e.Handled = " =%$#&^!?|*<>:;\"-+@".IndexOf(e.Text) > 0; 
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Oem5 || e.Key == Key.Divide
                || e.Key == Key.Multiply)
                e.Handled = true;
        }

    }
}
