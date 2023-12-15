using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfSqlite.Modules;

namespace WpfSqlite
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private const string ConnectionString = "Data Source=NaszaBaz.db;Version=3;";

        public MainWindow()
        {
            InitializeComponent();
        }


        private void SaveData (object sender, RoutedEventArgs e)
        {
            string name = textboxName.Text;
            string surname = textboxSurname.Text;

            if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(surname))
            {
                using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO Students (imie, nazwisko) VALUES (@Name, @Surname)";

                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Name", name);
                        command.Parameters.AddWithValue("@Surname", surname);
                        command.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Dane zostały zapisane");
            }
            else MessageBox.Show("Wprowadź poprawne dane");

        }

        private void LoadData (object sender, RoutedEventArgs e)
        {
            string query = "SELECT * FROM Students";
            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                connection.Open();

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    List<Person> studentsList = new List<Person>();

                    while (reader.Read())
                    {
                        studentsList.Add (new Person
                            {
                                Id = Convert.ToInt32(reader["id"]),
                                Name = reader["imie"].ToString(),
                                Surname = reader["nazwisko"].ToString()
                            }
                        );
                        datGrid.ItemsSource = studentsList;
                    }
                }
            }
        }

        private void SaveChanges(object sender, RoutedEventArgs e)
        {
            List<Person>? newStudentList = datGrid.ItemsSource as List<Person>;

            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                try
                {
                    if (newStudentList != null && newStudentList.Count > 0)
                    {
                        foreach (Person student in newStudentList)
                        {
                            if (student != null)
                            {
                                string insertQuery = "INSERT INTO Students (imie, nazwisko) VALUES (@Name, @Surname)";
                                SQLiteCommand insertCommand = new SQLiteCommand(insertQuery, connection);
                                insertCommand.Parameters.AddWithValue("@Name", student.Name);
                                insertCommand.Parameters.AddWithValue("@Surname", student.Surname);
                                insertCommand.ExecuteNonQuery();
                                MessageBox.Show("Dodałeś nowego ucznia");
                            }
                            else
                            {
                                string updateStudents = "UPDATE Students SET imie = @Name, nazwisko = @Surname WHERE id = @Id";
                                SQLiteCommand updateCommand = new SQLiteCommand(updateStudents, connection);
                                updateCommand.Parameters.AddWithValue("@Name", student.Name);
                                updateCommand.Parameters.AddWithValue("@Surname", student.Surname);
                                updateCommand.Parameters.AddWithValue("@Id", student.Id);
                                updateCommand.ExecuteNonQuery();
                            }
                        }
                        MessageBox.Show("Zmiane zostały zapisane");
                    }
                    else
                    {
                        MessageBox.Show("Nie wolno wprowadzać zmian w pustej datagrid");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void ClaerDataGrid(object sender, RoutedEventArgs e)
        {
            datGrid.ItemsSource = null;
        }
    }
}
