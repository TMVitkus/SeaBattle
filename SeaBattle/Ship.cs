using static SeaBattle.MainWindow.Ships;

namespace SeaBattle
{
    public partial class MainWindow
    {
        /// <summary>
        /// Класс, который хранит в себе тип и количество кораблей
        /// </summary>
        public class Ship
        {
            // Тип корабля
            public TypeShip Type { get; set; }
            // Количество кораблей
            public int CountShip { get; set; }
        }
    }
}