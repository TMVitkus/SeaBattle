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
    /// Логика взаимодействия для Statistic.xaml
    /// </summary>
    public partial class Statistic : Window
    {
        public Statistic()
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
        /// Метод, который выводит статистику игр
        /// </summary>
        public void InformationOutput()
        {
            labelCountWins.Content += SeaBattleDB.StatisticsOutput("NumberOfWins");
            labelCountGame.Content += SeaBattleDB.StatisticsOutput("NumberOfGames");
            labelCountShots.Content += SeaBattleDB.StatisticsOutput("NumberOfShots");
            labelCountHits.Content += SeaBattleDB.StatisticsOutput("NumberOfHits");
        }
    }
}