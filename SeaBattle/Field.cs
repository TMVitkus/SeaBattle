using System.Windows.Controls;
using System.Windows.Media;

namespace SeaBattle
{
    public partial class MainWindow
    {
        public class Field
        {
            // Название корабля
            public string NameShip { get; set; }
            // Клетка, которую выбрали
            internal Button PushButton { get; set; }
            // Цвет коробля
            private Color color;
            internal Color Color
            {
                // Возвращаем цвет
                get { return color; }
                // Присваиваем цвет и окрашиваем кнопку
                set
                {
                    color = value;
                    PushButton.Background = new SolidColorBrush(color);
                }
            }
            // Для проверки, занята ли кнопка
            public bool Busy { get; set; } = false;
            // Для проверки, стреляли ли в эту кнопку
            public bool Shooting { get; set; } = false;
            // Конструкотры класса
            public Field(Button PushButton, Color Color)
            {
                this.PushButton = PushButton;
                this.Color = Color;
            }
            public Field(Button PushButton)
            {
                this.PushButton = PushButton;
            }
            public Field()
            {
            }
            /// <summary>
            /// Метод, если попали в корабль
            /// </summary>
            public void Hitting()
            {
                PushButton.Background = Brushes.Maroon;
                Shooting = true;
            }
            /// <summary>
            /// Метод, если не попали в корабль
            /// </summary>
            public void Miss()
            {
                PushButton.Background = Brushes.Black;
                Shooting = true;
            }
        }
    }
}