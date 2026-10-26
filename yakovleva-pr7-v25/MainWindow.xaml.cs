using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace yakovleva_pr7_v25
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        HttpClient client = new HttpClient();
        EquipmentApi api;
        public ObservableCollection<Equipment> Equipments { get; set; } = new();
        private Equipment _originalEquipment;
        private Equipment _currentEquipment = new();
        public Equipment CurrentEquipment
        {
            get => _currentEquipment;
            set
            {
                _currentEquipment = value;
                AddEditPanel.DataContext = _currentEquipment;
            }
        }


        public MainWindow()
        {
            InitializeComponent();
            client.BaseAddress = new Uri("https://localhost:7263/");
            api = new EquipmentApi(client);

            DataContext = this;
            AddEditPanel.DataContext = CurrentEquipment;
        }
        private async void GetAllBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync("api/v1/Equipment");
                if (response.IsSuccessStatusCode)
                {
                    var list = await api.GetEquipments();
                    Equipments.Clear();
                    foreach (var item in list)
                        Equipments.Add(item);
                }
                else
                {
                    MessageBox.Show("Ошибка: " + response.StatusCode);
                }
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка");
            }
        }

        private async void CreateBtn_Click(object sender, RoutedEventArgs e)
        {
            try 
            { 
                var result = await api.CreateEquipment(new Equipment
                {
                    Name = CurrentEquipment.Name,
                    InventoryNumber = CurrentEquipment.InventoryNumber,
                    Price = CurrentEquipment.Price,
                    PurchaseDate = CurrentEquipment.PurchaseDate,
                    IsOperational = CurrentEquipment.IsOperational
                });
                if (result == null)
                {
                    MessageBox.Show("Успешно!");
                    GetAllBtn_Click(null, null);
                }
                else
                    MessageBox.Show(result, "Ошибка");
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка");
            }
}

        private async void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentEquipment.Id == 0)
            {
                MessageBox.Show("Выберите запись из списка");
                return;
            }

            var confirm = MessageBox.Show($"Вы уверены, что хотите удалить {CurrentEquipment.Name}?", "Подтверждение", MessageBoxButton.YesNo);
            if (confirm == MessageBoxResult.No)
                return;
            try 
            { 
                var result = await api.DeleteEquipment(CurrentEquipment.Id);
                if (result == null)
                {
                    MessageBox.Show("Успешно!");
                    CurrentEquipment = new();
                    GetAllBtn_Click(null, null);
                }
                else
                    MessageBox.Show(result, "Ошибка");
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка");
            }
}

        private async void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CurrentEquipment.Id == 0)
                {
                    CreateBtn_Click(null, null);
                    return;
                }

                var changes = new Dictionary<string, object>();

                if (CurrentEquipment.Name != _originalEquipment.Name)
                    changes["name"] = CurrentEquipment.Name;

                if (CurrentEquipment.InventoryNumber != _originalEquipment.InventoryNumber)
                    changes["inventoryNumber"] = CurrentEquipment.InventoryNumber;

                if (CurrentEquipment.Price != _originalEquipment.Price && CurrentEquipment.Price > 0)
                    changes["price"] = CurrentEquipment.Price;

                if (CurrentEquipment.PurchaseDate != _originalEquipment.PurchaseDate)
                    changes["purchaseDate"] = CurrentEquipment.PurchaseDate.ToString("yyyy-MM-dd");

                if (CurrentEquipment.IsOperational != _originalEquipment.IsOperational)
                    changes["isOperational"] = CurrentEquipment.IsOperational;

                if (changes.Count == 0)
                {
                    MessageBox.Show("Вы ничего не изменили");
                    return;
                }

                string result;

                if (changes.Count == 5)
                    result = await api.PutEquipment(CurrentEquipment.Id, CurrentEquipment);
                else
                    result = await api.PatchEquipment(CurrentEquipment.Id, changes);

                if (result == null)
                {
                    MessageBox.Show("Успешно сохранено");
                    GetAllBtn_Click(null, null);
                }
                else
                    MessageBox.Show(result, "Ошибка");
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка");
            }
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = (sender as ListView)?.SelectedItem as Equipment;
            if (selected != null)
            {
                CurrentEquipment = selected;
                _originalEquipment = new Equipment
                {
                    Id = selected.Id,
                    Name = selected.Name,
                    InventoryNumber = selected.InventoryNumber,
                    Price = selected.Price,
                    PurchaseDate = selected.PurchaseDate,
                    IsOperational = selected.IsOperational
                };
            }
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            CurrentEquipment = new();
            Equipments.Clear();
            FilterOperationalCb.SelectedIndex = 0;
            FilterMinPriceNup.Value = FilterMinPriceNup.Minimum;
        }

        private async Task Filters()
        {
            try 
            {
                decimal? minPrice = (decimal?)FilterMinPriceNup.Value;
                if (minPrice == 0)
                    minPrice = null;

                bool? operational = null;
                switch (FilterOperationalCb.SelectedIndex)
                {
                    case 1: operational = true; break;
                    case 2: operational = false; break;
                }

                if (api == null) 
                    return;

                var list = await api.GetEquipments(operational, minPrice);
                if (list == null)
                {
                    MessageBox.Show("Произошла ошибка");
                    return;
                }

                Equipments.Clear();
                foreach (var item in list)
                    Equipments.Add(item);
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка");
            }
        }

        private async void FilterOperationalCb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await Filters();
        }
        private async void FilterMinPriceNup_ValueChanged(object sender, RoutedPropertyChangedEventArgs<int> e)
        {
            await Filters();
        }
    }
}