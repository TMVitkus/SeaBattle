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

namespace SeaBattle
{
    /// <summary>
    /// Логика взаимодействия для GameResult.xaml
    /// </summary>
    public partial class GameResult : Window
    {
        public GameResult()
        {
            InitializeComponent();
            // Добавляем try/catch во избежание вылета приложения
            // Если с базой данных будут какие-то неполадки
            try
            {
                InformationOutput();
            }
            catch 
            {
                MessageBox.Show("Приносим свои извинения.\nФункция временно недоступна.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }        
        }
        /// <summary>
        /// Метод, который выводит результат игры
        /// </summary>
        public void InformationOutput()
        {
            labelCountShots.Content += SeaBattleDB.GameResultOutput("NumberOfShots");
            labelCountHits.Content += SeaBattleDB.GameResultOutput("NumberOfHits");
            labelTimer.Content += SeaBattleDB.GameResultOutput("Timer");
        }
    }
}