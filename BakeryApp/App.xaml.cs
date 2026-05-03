// App.xaml.cs
using System.Windows;
using BakeryApp.Data;

namespace BakeryApp
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Инициализируем БД при старте; если не получилось — завершаем приложение
            if (!DatabaseHelper.EnsureDatabase())
            {
                Shutdown(1);
            }
        }
    }
}
