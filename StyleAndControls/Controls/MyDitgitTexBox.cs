using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace StyleAndControls.Controls
{

    public class MyDitgitTexBox : TextBox
    {
        /// <summary>
        /// Контрол должен принимать десятичные значения или только целые
        /// </summary>
        public bool IsDouble { get; set; }
        public bool IsUseMinus { get; set; }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            if (String.IsNullOrEmpty(Text))
            {
                Text = "0";
            }
        }

        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            if (String.IsNullOrEmpty(e.Text))
                e.Handled = true;
            else
            {
                if (!Char.IsDigit(e.Text, 0))
                    e.Handled = true;

                if (IsDouble)
                    if ((e.Text.IndexOf('.') == 0 || e.Text.IndexOf(',') == 0 || e.Text.IndexOf('-') == 0) && Text.IndexOf('.') == -1 && Text.IndexOf(',') == -1)
                        e.Handled = false;

                if (IsUseMinus)
                    if (e.Text.IndexOf('-') == 0 && Text.IndexOf('-') == -1)
                        e.Handled = false;
            }


        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }

        //protected override void PreviewKeyDown
        private string _lastText = "";
        protected override void OnPreviewKeyUp(KeyEventArgs e)
        {
            if (!String.IsNullOrEmpty(Text))
            {
                Char pointChar =
                    Convert.ToChar(System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                if (pointChar == '.')
                {
                    if (Text.Contains(","))
                    {
                        Text = Text.Replace(',', '.');
                        SelectionStart = Text.Length;
                        //SetSelectionStart();
                    }
                }
                else if (Text.Contains("."))
                {
                    Text = Text.Replace('.', ',');
                    SelectionStart = Text.Length; //Убрал, т.к. если обновлять число в оптимизации, например поставить перед запятой цифру, то сразу в конец перекидывает
                    //SetSelectionStart();
                }

            }

        }

        private void SetSelectionStart()
        {

            if (String.IsNullOrEmpty(Text) || String.IsNullOrEmpty(_lastText))
            {
                
            }
            else
            {
                if (_lastText.Length > Text.Length)
                {
                    for (int i = 0; i < Text.Length; i++)
                    {
                        if (Text.Substring(i,1) != _lastText.Substring(i,1))
                        {
                            SelectionStart = i;
                            _lastText = Text;
                            return;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < _lastText.Length; i++)
                    {
                        if (Text.Substring(i,1) != _lastText.Substring(i,1))
                        {
                            SelectionStart = i+1;
                            _lastText = Text;
                            return;
                        }
                    }
                }

            }

            SelectionStart = Text.Length;
            _lastText = Text;

        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            try
            {
                if (Text != "")
                    if (Math.Abs(ConvertToDoubleMy(Text)) < 0.00000001)
                        Text = "";
            }
            catch
            {
                Text = "";
            }
        }

        /// <summary>
        /// Преобразование в double в зависимостии от знаказа разделителя
        /// </summary>
        /// <param name="tmp"></param>
        /// <returns></returns>
        public static double ConvertToDoubleMy(string tmp)
        {
            if (String.IsNullOrEmpty(tmp) ||
                tmp == "." || tmp == ",")
            {
                return 0;
            }

            Char pointChar =
                Convert.ToChar(System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
            if (pointChar == '.')
                return Convert.ToDouble(tmp.Replace(',', '.'));

            return Convert.ToDouble(tmp.Replace('.', ','));
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {

            if (String.IsNullOrEmpty(Text))
            {
                Text = "0";
            }
            base.OnLostFocus(e);
        }

        //public string Text
        //{
        //    get { return Text; }
        //    set { Text = value; }
        //}
    }
}
