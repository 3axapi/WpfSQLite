using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

namespace WpfSqlite
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private const string ConnectionString = "Data Source=NaszaBaz.db;Version=3;";

        public App ()
        {
            createTable();
        }

        private void createTable ()
        {
            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open ();
                string query = "CREATE TABLE IF NOT EXISTS Students (id INTEGER PRIMARY KEY AUTOINCREMENT, imie TEXT, nazwisko TEXT)";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
