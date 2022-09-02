using CommonDataContract;

namespace SourceEts.Models
{
    public class UserChartPropModel
    {
        /// <summary>
        /// Цвет линнии
        /// </summary>
        public System.Drawing.Color ColorLine { get; set; }
        /// <summary>
        /// Толщина линии
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Прозрачность
        /// </summary>
        public byte Transparency { get; set; }

        /// <summary>
        /// линия, гистограмма, точки
        /// </summary>
        public CfgSourceEts.EnumTypeLine TypeLine { get; set; }


        /// <summary>
        /// Отрисовка коллекции на графике
        /// </summary>
        public bool IsNotPlotSeries { get; set; }
        /// <summary>
        /// Номер панели, на которой рисуется
        /// </summary>
        public int NumberPanel { get; set; }

        /// <summary>
        /// Глубина перерисовки индикатора при добавлении новой свечи
        /// </summary>
        public int DeepUpdate { get; set; }

        /// <summary>
        /// По умолчанию график скрыт
        /// </summary>
        public bool IsDefaultCollapse { get; set; }

        /// <summary>
        /// Название дополнительной панели, если не указано, то выводятся данные на главную панель
        /// </summary>
        public string NameAddPanel { get; set; }

    }
}
