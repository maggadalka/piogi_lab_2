using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace piogi_lab_2
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // 👇 ДОБАВЬ ВОТ ЭТУ СТРОКУ
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
        }

        // 👇 И ДОБАВЬ ВОТ ЭТОТ МЕТОД
        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("ОШИБКА: " + e.Exception.Message + "\n\n" + e.Exception.StackTrace, "Критическая ошибка");
            e.Handled = true; // Чтобы игра не закрывалась сразу
        }
    }

}
