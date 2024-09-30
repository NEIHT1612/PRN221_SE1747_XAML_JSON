using DemoXML_JSON.Model;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace DemoXML_JSON
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnLoadXML_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog()
            {
                Filter = "XML Files (*.xml)|*.xml|All Files (*.*)|*.*"
            };

            var result = ofd.ShowDialog();

            XmlSerializer serializer = new XmlSerializer(typeof(catalog), new XmlRootAttribute("catalog")
            {
                Namespace = "http://www.w3.org/2001/XMLSchema"
            });

            if (result == false) return;

            try
            {
                using (StreamReader reader = new StreamReader(ofd.FileName))
                {
                    var catalog = (catalog)serializer.Deserialize(reader);
                    lvBook.ItemsSource = catalog.book;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnLoadJson_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog()
            {
                Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*"

            };

            var result = ofd.ShowDialog();

            if (result == false) return;

            try
            {
                using (StreamReader reader = new StreamReader(ofd.FileName))
                {
                    var catalogString = reader.ReadToEnd();
                    var catalog = JsonSerializer.Deserialize<CatalogWrapper>(catalogString);

                    if (catalog != null)
                    {
                        lvBook.ItemsSource = catalog?.Catalog?.book;
                    }
                    else
                    {
                        MessageBox.Show("No Data");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnLoadTxt_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog()
            {
                Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*"
            };

            var result = ofd.ShowDialog();

            if (result == false) return;

            try
            {
                using (StreamReader reader = new StreamReader(ofd.FileName))
                {
                    var lines = reader.ReadToEnd().Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

                    ObservableCollection<catalogBook> books = new ObservableCollection<catalogBook>();

                    foreach (var line in lines)
                    {
                        string pattern = "\"([^\"]*)\"|([^,\"\\s]+)";
                        var matches = Regex.Matches(line, pattern);

                        if (matches.Count == 0) continue;

                        var parts = new List<string>();

                        foreach (Match match in matches)
                        {
                            if (match.Groups[1].Success)
                            {
                                parts.Add(match.Groups[1].Value);
                            }
                            else if (match.Groups[2].Success)
                            {
                                parts.Add(match.Groups[2].Value);
                            }
                        }

                        if (parts.Count == 6 || parts.Count == 7)
                        {
                            books.Add(new catalogBook
                            {
                                title = parts[0].Trim(),
                                author = parts[1].Trim(),
                                genre = parts[2].Trim(),
                                price = decimal.Parse(parts[3].Trim()),
                                publish_date = DateTime.Parse(parts[4].Trim()),
                                description = parts[5].Trim(),
                                // id = parts[6].Trim() 
                            });
                        }
                    }

                    lvBook.ItemsSource = books;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}