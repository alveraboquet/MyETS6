using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Win32;

namespace TerminalRbkm.Config
{
    public class Cfg
    {
        public static string MainTerminalsSetting = "Терминалы";
        public static string MainSettings = "Основные";
        public static string MainStrategies = "Стратегии";
        public static string MainStorageHistory = "Хранилище данных";
        public static string MainDataSource = "Источники данных";
        public static string MainManageRobots = "Управление роботами";
        public static string MainManagePortfels = "Портфели роботов";
        public static string MainManagePortfelsStrategies = "Портфели стратегий";

        #region Типы продуктов

        public static string TypeProductMain = "Main";
        public static string TypeProductMainProduct = "MainProduct";
        public static string TypeProductAutoStops = "AutoStops";
        public static string TypeProductRm = "Rm";
        public static string TypeProductGrid = "Grid";
        public static string TypeProductLevelCandle = "LevelCandle";
        public static string TypeProductPrivodDu = "PrivodDu";
        public static string TypeProductArbitrage = "Arbitrage";
        public static string TypeProductRebalance = "Rebalance";
        public static string TypeProductCryptoArbitrage = "CryptoArbitrage";


        public static string TypeProductMainOriginal = "ETS";
        public static string TypeProductAutoStopsOriginal = "Автостопы";
        public static string TypeProductRmOriginal = "Риск менеджмент";
        public static string TypeProductGridOriginal = "Сетка заявок";
        public static string TypeProductLevelCandleOriginal = "Уровневые стратегии";
        public static string TypeProductPrivodOriginal = "Инструменты управляющего";
        public static string TypeProductArbitrageOriginal = "Арбитраж";
        public static string TypeProductRebalanceOriginal = "Ребалансировка портфеля";
        public static string TypeProductCryptoArbitrageOriginal = "КриптоАрбитраж";

        #endregion

        #region MyRegion

        public static string BtnContentActive = "Активировать";
        public static string BtnContentBuy = "Приобрести";
        public static string BtnContentActived = "Активна";
        public static string BtnContentEnd = "Закончилась";

        #endregion



        public static string LogTypeRobot = "Робот";
        public static string LogTypeStop = "Стоп-заявка";
        public static string LogTypeOrder = "Заявка";
        public static string LogTypeDeal = "Сделка";

        public static string UpdateDebug = "Debug";
        public static string UpdateRelease = "Release";



        public static void SetLog(string log)
        {
            MainWindow.Log.Info(log);
        }




        #region Получение списка всех квиков


        static void RecursGetPath(DirectoryInfo predDirec, string findFile, List<string> pathfile)
        {

            FileInfo[] fi;
            try
            {
                fi = predDirec.GetFiles();//Сюда попадут все файлы этой папки
            }
            catch
            {
                return;
            }
            for (int i = 0; i < fi.Length; i++)
            {
                if (fi[i].Name == findFile)
                {
                    pathfile.Add((fi[i].DirectoryName));

                }
            }
            DirectoryInfo[] di = predDirec.GetDirectories();
            for (int i = 0; i < di.Length; i++)
            {
                RecursGetPath(di[i], findFile, pathfile);
            }
        }

        #endregion
    }
}
