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
using System.Windows.Navigation;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;
using System.IO;

namespace SQL_CS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string kapcsolatLeiro = "datasource=127.0.0.1;port=3306;username=root;password=;database=hw;";
        MySqlConnection SQLkapcsolat;

        List<Termek> termekek = new List<Termek>();
        public MainWindow()
        {
            InitializeComponent();
            AdatbazisMegnyitas();
            KategoriaBetoltese();
            GyartokBetoltese();
            TermekBetoltesListaba();
            AdatbazisLezarasa();
        }
        private void AdatbazisMegnyitas()
        {
            try
            {
                SQLkapcsolat = new MySqlConnection(kapcsolatLeiro);
                SQLkapcsolat.Open();
            }
            catch (Exception)
            {
                MessageBox.Show("Error!");
            }
        }
        private void AdatbazisLezarasa()
        {
            SQLkapcsolat.Close();
            SQLkapcsolat.Dispose();
        }
        private void TermekBetoltesListaba()
        {
            string SQLOsszesTermek = "SELECT * FROM termékek;";
            MySqlCommand SQLParancs = new MySqlCommand(SQLOsszesTermek, SQLkapcsolat);
            MySqlDataReader eredmenyOlvaso = SQLParancs.ExecuteReader();

            while (eredmenyOlvaso.Read())
            {
                Termek uj = new Termek(
                    eredmenyOlvaso.GetString("Kategória"),
                    eredmenyOlvaso.GetString("Gyártó"),
                    eredmenyOlvaso.GetString("Név"),
                    eredmenyOlvaso.GetInt32("Ár"),
                    eredmenyOlvaso.GetInt32("Garidő"));
                termekek.Add(uj);
            }
            eredmenyOlvaso.Close();
            dgTermekek.ItemsSource = termekek;
        }
        private void btnMentes_Click(object sender, RoutedEventArgs e)
        {
            StreamWriter sw = new StreamWriter("save.csv");
            foreach (var item in termekek)
            {
                sw.WriteLine(item.ToString());
            }
            sw.Close();
        }
        private void GyartokBetoltese()
        {
            string SQLGyartokRendezve = "SELECT DISTINCT gyártó FROM termékek ORDER BY gyártó;";
            MySqlCommand SQLparancs = new MySqlCommand(SQLGyartokRendezve, SQLkapcsolat);
            MySqlDataReader eredmenyOlvaso = SQLparancs.ExecuteReader();

            cbGyarto.Items.Add(" - Nincs megadva - ");
            while (eredmenyOlvaso.Read())
            {
                cbGyarto.Items.Add(eredmenyOlvaso.GetString("Gyártó"));
            }
            eredmenyOlvaso.Close();
            cbGyarto.SelectedIndex = 0;
        }
        private void KategoriaBetoltese()
        {
            string SQLGKategoriakRendezve = "SELECT DISTINCT kategória FROM termékek ORDER BY kategória;";
            MySqlCommand SQLparancs = new MySqlCommand(SQLGKategoriakRendezve, SQLkapcsolat);
            MySqlDataReader eredmenyOlvaso = SQLparancs.ExecuteReader();

            cbGyarto.Items.Add(" - Nincs megadva - ");
            while (eredmenyOlvaso.Read())
            {
                cbGyarto.Items.Add(eredmenyOlvaso.GetString("kategória"));
            }
            eredmenyOlvaso.Close();
            cbGyarto.SelectedIndex = 0;
        }

        private void btnSzukit_Click(object sender, RoutedEventArgs e)
        {
            termekek.Clear();
            string SQLSzukitettLista = SzukitoLekerdezesEloallitasa();

            MySqlCommand SQLparancs = new MySqlCommand(SQLSzukitettLista, SQLkapcsolat);
            MySqlDataReader eredmenyOlvaso = SQLparancs.ExecuteReader();
            while (eredmenyOlvaso.Read())
            {
                Termek uj = new Termek(
                    eredmenyOlvaso.GetString("Kategória"),
                    eredmenyOlvaso.GetString("Gyártó"),
                    eredmenyOlvaso.GetString("Név"),
                    eredmenyOlvaso.GetInt32("Ár"),
                    eredmenyOlvaso.GetInt32("Garidő"));
                termekek.Add(uj);
            }
            eredmenyOlvaso.Close();
            dgTermekek.Items.Refresh();

        }
        private string SzukitoLekerdezesEloallitasa()
        {
            bool vanMarFeltetel = false;
            string SQLSzukitettLista = "SELECT * FROM termékek ";
            if (cbGyarto.SelectedIndex > 0 || cbKategoria.SelectedIndex > 0 ||txtTermek.Text!= "")
            {
                SQLSzukitettLista += "WHERE ";
            }
            if (cbGyarto.SelectedIndex > 0)
            {
                SQLSzukitettLista += $"gyártó='{cbGyarto.SelectedItem}'";
                vanMarFeltetel = true;
            }
            if (cbKategoria.SelectedIndex > 0)
            {
                if (vanMarFeltetel)
                {
                    SQLSzukitettLista += " AND ";
                }
                SQLSzukitettLista += $"kategória='{cbKategoria.SelectedItem}'";
                vanMarFeltetel = true;
            }
            if (txtTermek.Text != "")
            {
                if (vanMarFeltetel)
                {
                    SQLSzukitettLista += " AND ";
                }
                SQLSzukitettLista += $"név LIKE '%{txtTermek.Text}%'";
            }
            return SQLSzukitettLista;
        }
    }
}

