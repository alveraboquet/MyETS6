using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Adapter.Config;
using CommonDataContract;

namespace Adapter.Convertors
{
    [ValueConversion(typeof(DateTime), typeof(String))]
    public class ConvertoDate : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime date = (DateTime)value;
            if (date.Year == 1)
                return "";
            return date.ToShortDateString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string strValue = value as string;
            DateTime resultDateTime;
            if (DateTime.TryParse(strValue, out resultDateTime))
            {

                return resultDateTime;
            }
            return DependencyProperty.UnsetValue;
        }
    }

    [ValueConversion(typeof(DateTime), typeof(String))]
    public class ConvertoTime : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime date = (DateTime)value;
            return date.ToLongTimeString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //необходимо править
            string strValue = value as string;
            DateTime resultDateTime;
            if (DateTime.TryParse(strValue, out resultDateTime))
            {
                return resultDateTime;
            }
            return DependencyProperty.UnsetValue;
        }
    }

    /// <summary>
    /// Конвертор string, если значение 0, то возвращаем значение ""
    /// </summary>
    [ValueConversion(typeof(DateTime), typeof(String))]
    public class ConvertorString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            String tmp = System.Convert.ToString(value);
            if (tmp == "0" || tmp == "0.0" || tmp == "0,0")
                return "";

            return tmp;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string strValue = value as string;
            String resultDateTime = strValue;

            return resultDateTime;

            // return DependencyProperty.UnsetValue;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ConvertorOperBuySell : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //bool tmp = System.Convert.ToBoolean(value);
            if ((bool)value)
                return "Купля";

            return "Продажа";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string strValue = value as string;
            String resultDateTime = strValue;

            return resultDateTime;

            // return DependencyProperty.UnsetValue;
        }
    }

   

    /// <summary>
    /// Конвертор string
    /// </summary>
    [ValueConversion(typeof(DateTime), typeof(String))]
    public class ConvertorStringWithZero : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            String tmp = System.Convert.ToString(value);
            if (tmp == "0" || tmp == "0.0" || tmp == "0,0")
                return "0";

            return tmp;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string strValue = value as string;
            String resultDateTime = strValue;

            return resultDateTime;

            // return DependencyProperty.UnsetValue;
        }
    }


    /// <summary>
    /// Конвертор string
    /// </summary>
    [ValueConversion(typeof(DateTime), typeof(String))]
    public class ConvertorStringWithZeroAndPercent : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            String tmp = System.Convert.ToString(value);
            if (tmp == "0" || tmp == "0.0" || tmp == "0,0")
                return "0 %";

            return tmp + " %";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string strValue = value as string;
            String resultDateTime = strValue;

            return resultDateTime;

            // return DependencyProperty.UnsetValue;
        }
    }

    [ValueConversion(typeof(Double), typeof(String))]
    public class ConvertorDouble : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Double tmp = (Double)value;
            if (Math.Abs(tmp) < CfgSourceEts.MyEpsilon)
                return "";

            return tmp;
            //            return date.ToShortDateString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string strValue = value as string;
            Double resultDateTime;
            if (Double.TryParse(strValue, out resultDateTime))
            {
                return resultDateTime;
            }
            return DependencyProperty.UnsetValue;
        }
    }

    [ValueConversion(typeof(Int32), typeof(String))]
    public class ConverToInt : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Int32 tmp = (Int32)ConfigTermins.ConvertToDoubleMy(value.ToString());
            if (Math.Abs(tmp) < CfgSourceEts.MyEpsilon)
                return "";

            return tmp;
            //            return date.ToShortDateString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {

            return value;
        }
    }

    [ValueConversion(typeof(Int32), typeof(String))]
    public class ConverStakanToDouble : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var tmp = ConfigTermins.ConvertToDoubleMy(value.ToString());
            if (Math.Abs(tmp) < CfgSourceEts.MyEpsilon)
                return "";

            return tmp;
            //            return date.ToShortDateString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {

            return value;
        }
    }

    [ValueConversion(typeof(Double), typeof(String))]
    public class ConvertorDoubleWithSeparatorThousand : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Double tmp = System.Convert.ToDouble(value);
            if (Math.Abs(tmp) < CfgSourceEts.MyEpsilon)
                return "";

            return tmp.ToString("### ### ### ###.### ###");
            //            return date.ToShortDateString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string strValue = value as string;
            Double resultDateTime;
            if (Double.TryParse(strValue, out resultDateTime))
            {
                return resultDateTime;
            }
            return DependencyProperty.UnsetValue;
        }
    }

    [ValueConversion(typeof(String), typeof(SolidColorBrush))]
    public class ColorConverter : IValueConverter
    {
        private static SolidColorBrush RED_BRUSH = new SolidColorBrush(Colors.DarkOrange);
        private static SolidColorBrush GREEN_BRUSH = new SolidColorBrush(Colors.LawnGreen);
        private static SolidColorBrush WHITE_BRUSH = new SolidColorBrush(Colors.White);
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!String.IsNullOrEmpty(System.Convert.ToString(value)))
            {
                Double obj = ConfigTermins.ConvertToDoubleMy(System.Convert.ToString(value));
                if (Math.Abs(obj) > CfgSourceEts.MyEpsilon)
                    return (obj < 0 ? RED_BRUSH : GREEN_BRUSH);
            }
            return WHITE_BRUSH;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    [ValueConversion(typeof(String), typeof(SolidColorBrush))]
    public class ColorConverterIceberg : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (System.Convert.ToBoolean(value))
            {
                return new SolidColorBrush(Colors.DodgerBlue);
            }
            return new SolidColorBrush(Colors.White);
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    [ValueConversion(typeof(String), typeof(SolidColorBrush))]
    public class ColorConverterStatusConnection : IValueConverter
    {
        private static SolidColorBrush GREEN_BRUSH = new SolidColorBrush(Colors.LawnGreen);
        private static SolidColorBrush YELLOW_BRUSH = new SolidColorBrush(Colors.Yellow);
        private static SolidColorBrush WHITE_BRUSH = new SolidColorBrush(Colors.White);
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!String.IsNullOrEmpty(System.Convert.ToString(value)))
            {
                string val = System.Convert.ToString(value);
                if (val == ConfigTermins.ColorStatusConnectionYellow)
                    return YELLOW_BRUSH;
                if (val == ConfigTermins.ColorStatusConnectionGreen)
                    return GREEN_BRUSH;
            }
            return WHITE_BRUSH;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    /// <summary>
    /// Подкраска зявки в зависимости от ее статуса
    /// </summary>
    [ValueConversion(typeof(String), typeof(SolidColorBrush))]
    public class ColorConverterStatusOrder : IValueConverter
    {
        private static SolidColorBrush RED_BRUSH = new SolidColorBrush(Colors.DarkOrange); //активна
        private static SolidColorBrush BLUE_BRUSH = new SolidColorBrush(Colors.LightYellow); //исполненна
        private static SolidColorBrush WHITE_BRUSH = new SolidColorBrush(Colors.White); //снята
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!String.IsNullOrEmpty(System.Convert.ToString(value)))
            {
                String obj = System.Convert.ToString(value);

                if (obj == ConfigTermins.Performed)
                    return BLUE_BRUSH;

                if (obj == ConfigTermins.Active)
                    return RED_BRUSH;
            }
            return WHITE_BRUSH;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }



    /// <summary>
    /// Покараска счета в цвет, в случае его не нахождении среди активных счетов
    /// </summary>
    [ValueConversion(typeof(String), typeof(SolidColorBrush))]
    public class ColorConverterAccount : IValueConverter
    {
        private static SolidColorBrush RED_BRUSH = new SolidColorBrush(Colors.DarkOrange);
        private static SolidColorBrush WHITE_BRUSH = new SolidColorBrush(Colors.White);
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!String.IsNullOrEmpty(System.Convert.ToString(value)))
            {
                bool obj = System.Convert.ToBoolean(value);
                if (obj)
                    return RED_BRUSH;
            }
            return WHITE_BRUSH;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    [ValueConversion(typeof(String), typeof(SolidColorBrush))]
    public class ColorConverterStatusRebalance : IValueConverter
    {
        private static SolidColorBrush RED_BRUSH = new SolidColorBrush(Colors.DarkOrange);
        private static SolidColorBrush GREEN_BRUSH = new SolidColorBrush(Colors.LawnGreen);
        private static SolidColorBrush BLUE_BRUSH = new SolidColorBrush(Colors.SkyBlue);
        private static SolidColorBrush WHITE_BRUSH = new SolidColorBrush(Colors.White);
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!String.IsNullOrEmpty(System.Convert.ToString(value)))
            {
                string obj = System.Convert.ToString(value);
                if (obj == "Ошибка")
                    return RED_BRUSH;
                if (obj == "Исполнена")
                    return GREEN_BRUSH;
                if (obj == "Отменена")
                    return BLUE_BRUSH;
            }
            return WHITE_BRUSH;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }


    /// <summary>
    /// Подкрашиваем строки стакана в зависимости от значений объемов
    /// </summary>
    [ValueConversion(typeof(String), typeof(SolidColorBrush))]
    public class GlassColorConvertor : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (ConfigTermins.ConvertToDoubleMy(value.ToString()) > 0)
                return new SolidColorBrush(Colors.MistyRose);
            else
                return new SolidColorBrush(Colors.LightGreen);

           // return new SolidColorBrush(Colors.White);
        }



        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// Подкрашиваем строки стакана в зависимости от значений объемов
    /// </summary>
    [ValueConversion(typeof(String), typeof(SolidColorBrush))]
    public class MultiGlassColorConvertor : IMultiValueConverter
    {
        private static SolidColorBrush RED_BRUSH = new SolidColorBrush(Colors.DarkOrange);
        private static SolidColorBrush GREEN_BRUSH = new SolidColorBrush(Colors.LawnGreen);
        private static SolidColorBrush WHITE_BRUSH = new SolidColorBrush(Colors.White);
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.Length>1)
            {
                if (ConfigTermins.ConvertToDoubleMy(values[0].ToString()) > 0)
                    return new SolidColorBrush(Colors.MistyRose);
                if (ConfigTermins.ConvertToDoubleMy(values[1].ToString()) > 0)
                    return new SolidColorBrush(Colors.LightGreen);
            }
            return WHITE_BRUSH;
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Подкрашивает бэккграунд ячейки в поле исполнено, если сделка была на истории
    /// </summary>
    public class HistoricalDealConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((bool)value)
                return Brushes.Silver;

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    /// <summary>
    /// Если сделка была на истории то вы водится вирт., иначе реал.
    /// </summary>
    public class HistoricalDealConverterString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((bool)value)
                return "Истор.";

            return "Реал.";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    /// <summary>
    /// Подкраска ячеек позиций по роботам, чтоб можно было какая сделка по робота ведется реальная или виртуальная
    /// </summary>
    public class ColorConverterTypeOpenPos : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!String.IsNullOrEmpty(System.Convert.ToString(value)))
            {
                if (value.ToString() == ConfigTermins.TypeOpenPosIsVirtual)
                    return Brushes.Silver;

                if (value.ToString() == ConfigTermins.TypeOpenPosIsRealAndVirtual)
                    return Brushes.LightSalmon;

            }
            return null;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }


    /// <summary>
    /// Подкраска ячеек позиций по роботам, чтоб можно было какая сделка по робота ведется реальная или виртуальная
    /// </summary>
    public class ColorConverterBackGround : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!String.IsNullOrEmpty(System.Convert.ToString(value)))
            {

                var color = (Color)System.Windows.Media.ColorConverter.ConvertFromString(System.Convert.ToString(value));
                return new SolidColorBrush(color);
            }
            return null;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }


    /// <summary>
    /// Подкраска ячеек позиций по роботам, чтоб можно было какая сделка по робота ведется реальная или виртуальная
    /// </summary>
    public class ColorConverterBackGroundFromColorDrawing : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((System.Drawing.Color)value != null)
            {
                System.Windows.Media.SolidColorBrush brush = new SolidColorBrush();
                brush.Color = new Color
                {
                    A = ((System.Drawing.Color)value).A,
                    B = ((System.Drawing.Color)value).B,
                    R = ((System.Drawing.Color)value).R,
                    G = ((System.Drawing.Color)value).G,
                };

                return brush;
            }
            return null;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    /// <summary>
    /// Показывать или нет
    /// </summary>
    public class VisibilityModulConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // если это модуль, то не показывать
            if ((bool)value)
                return Visibility.Hidden;

            return Visibility.Visible;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
            //throw new Exception("The method or operation is not implemented.");
        }
    }


    [ValueConversion(typeof(String), typeof(SolidColorBrush))]
    public class MultiColorConverterExceptionJob : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values!=null && values[1]!=null && values[0].ToString() == "True" && !String.IsNullOrEmpty(values[1].ToString()))
            {
                return new SolidColorBrush(Colors.OrangeRed);
            }

            return new SolidColorBrush(Colors.White);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }


        //public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        //{
        //    if (!String.IsNullOrEmpty(System.Convert.ToString(value)))
        //    {
        //        Double obj = ConfigTermins.ConvertToDoubleMy(System.Convert.ToString(value));
        //        if (Math.Abs(obj) > CfgSourceEts.MyEpsilon)
        //            return (obj < 0 ? RED_BRUSH : GREEN_BRUSH);
        //    }
        //    return WHITE_BRUSH;
        //}
        //public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        //{
        //    return value;
        //}
    }

    public class LinkConverters : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            //values - ваши свойства
            string domainName = (string)parameter;
            string uriString = String.Join(string.Empty, values); //1-ый аргумент метода - разделитель ваших свойств, можно заменить на "/"

            string link = string.Concat(domainName, uriString);
            return new Uri(link);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ComicVineConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null && value.ToString().Length > 0)
            {
                string comicVine = "ComicVine";
                return comicVine;
            }
            string empty = "";
            return empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Uri comicVine = new Uri((string)value);
            return comicVine;
        }
    }


    public class BoolConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool result = true;
            foreach (bool value in values)
                if (result)
                    result = value;

            return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Заменяем запятую на точку при вводе десятичных чисел в DataGrid.
    /// </summary>
    public class DecimalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
                //return ConfigTermins.ConvertToDoubleMy(value.ToString());
                return value.ToString().Replace(",", ".");
            else return null;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
                //return ConfigTermins.ConvertToDoubleMy(value.ToString());
                return value.ToString().Replace(",", ".");
            else return null;
        }
    }
}
