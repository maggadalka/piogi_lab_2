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

namespace piogi_lab_2
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {

        private CEnemyTemplateList enemyList = new CEnemyTemplateList();
        private List<EnemyIcon> enemyIcons = new List<EnemyIcon>();

        private string currentIconsPath = "";


        // события кнопок
        public MainWindow()
        {
            InitializeComponent();

            LoadIconsBtn.Click += LoadIconsBtn_Click;
            IconsListBox.SelectionChanged += IconsListBox_SelectionChanged;

            LoadIconsFromFolder(".\\Icons");

            AddBtn.Click += AddBtn_Click;
            DeleteBtn.Click += DeleteBtn_Click;
            SaveBtn.Click += SaveBtn_Click;
            LoadBtn.Click += LoadBtn_Click;
            EditBtn.Click += EditBtn_Click;
            EnemiesListBox.SelectionChanged += EnemiesListBox_SelectionChanged;
            StartGameBtn.Click += StartGameBtn_Click;
        }

        private void LoadIconsBtn_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    LoadIconsFromFolder(dialog.SelectedPath);
                }
            }
        }

        // метод загрузки иконок
        private void LoadIconsFromFolder(string path)
        {
            if (!System.IO.Directory.Exists(path))
            {
                MessageBox.Show("Папка не найдена! Выберите другую папку с иконками.");
                return;
            }

            enemyIcons.Clear();
            IconsListBox.Items.Clear();

            currentIconsPath = path;

            string[] files = System.IO.Directory.GetFiles(path, "*.png");
            foreach (string file in files)
            {
                enemyIcons.Add(new EnemyIcon
                {
                    Name = System.IO.Path.GetFileName(file),
                    ImagePath = file
                });
            }

            foreach (EnemyIcon icon in enemyIcons)
            {
                Image image = new Image()
                {
                    Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(icon.ImagePath)),
                    Height = 64
                };
                IconsListBox.Items.Add(image);
            }
        }


        // обработчик выбора иконки
        private void IconsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IconsListBox.SelectedItem is Image selectedImage)
            {
                string iconName = System.IO.Path.GetFileName(selectedImage.Source.ToString());
                MainEnemyIcon.Source = selectedImage.Source;
                // сохранение имкни выбранной иконки
                this.Tag = iconName;
            }
        }

        // обновление списка противников в интерфейсе
        private void RefreshEnemiesList()
        {
            EnemiesListBox.Items.Clear();
            foreach (string name in enemyList.GetListOfEnemyNames())
            {
                EnemiesListBox.Items.Add(name);
            }
        }

        // добавление противника
        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameBox.Text))
            {
                MessageBox.Show("Введите имя противника!");
                return;
            }

            if (this.Tag == null)
            {
                MessageBox.Show("Выберите иконку!");
                return;
            }

            if (!int.TryParse(BaseLifeBox.Text, out int baseLife) ||
                !double.TryParse(LifeModBox.Text, out double lifeMod) ||
                !int.TryParse(BaseGoldBox.Text, out int baseGold) ||
                !double.TryParse(GoldModBox.Text, out double goldMod) ||
                !double.TryParse(SpawnChanceBox.Text, out double spawnChance))
            {
                MessageBox.Show("Проверьте числовые поля!");
                return;
            }

            enemyList.AddEnemy(NameBox.Text, this.Tag.ToString(), baseLife, lifeMod,
                               baseGold, goldMod, spawnChance);

            RefreshEnemiesList();
        }

        // удаление противника
        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (EnemiesListBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите противника!");
                return;
            }

            enemyList.DeleteEnemyByName(EnemiesListBox.SelectedItem.ToString());
            RefreshEnemiesList();
        }

        // сохранение в JSON
        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.DefaultExt = "json";
            dlg.Filter = "JSON files (*.json)|*.json";

            if (dlg.ShowDialog() == true)
            {
                enemyList.SaveToJson(dlg.FileName);
                MessageBox.Show("Сохранено!");
            }
        }

        // редактирование противника из списка
        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, выбран ли противник
            if (EnemiesListBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите противника из списка!");
                return;
            }

            // Запоминаем старое имя (по нему найдём противника)
            string oldName = EnemiesListBox.SelectedItem.ToString();

            // Проверяем поля
            if (string.IsNullOrWhiteSpace(NameBox.Text))
            {
                MessageBox.Show("Введите имя противника!");
                return;
            }

            if (this.Tag == null)
            {
                MessageBox.Show("Выберите иконку!");
                return;
            }

            if (!int.TryParse(BaseLifeBox.Text, out int baseLife) ||
                !double.TryParse(LifeModBox.Text, out double lifeMod) ||
                !int.TryParse(BaseGoldBox.Text, out int baseGold) ||
                !double.TryParse(GoldModBox.Text, out double goldMod) ||
                !double.TryParse(SpawnChanceBox.Text, out double spawnChance))
            {
                MessageBox.Show("Проверьте числовые поля!");
                return;
            }

            // Обновляем противника
            enemyList.UpdateEnemy(oldName, NameBox.Text, this.Tag.ToString(),
                                  baseLife, lifeMod, baseGold, goldMod, spawnChance);

            // Обновляем список на экране
            RefreshEnemiesList();
            MessageBox.Show("Противник обновлён!");
        }

        // загрузка из JSON
        private void LoadBtn_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = "json";
            dlg.Filter = "JSON files (*.json)|*.json";

            if (dlg.ShowDialog() == true)
            {
                enemyList.LoadFromJson(dlg.FileName);
                RefreshEnemiesList();
                MessageBox.Show("Загружено!");
            }
        }

        // выбор противника из списка
        private void EnemiesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EnemiesListBox.SelectedItem == null) return;

            var enemy = enemyList.GetEnemyByName(EnemiesListBox.SelectedItem.ToString());
            if (enemy == null) return;

            NameBox.Text = enemy.Name;
            BaseLifeBox.Text = enemy.BaseLife.ToString();
            LifeModBox.Text = enemy.LifeModifier.ToString();
            BaseGoldBox.Text = enemy.BaseGold.ToString();
            GoldModBox.Text = enemy.GoldModifier.ToString();
            SpawnChanceBox.Text = enemy.SpawnChance.ToString();
        }

        // переход к игровому окну
        private void StartGameBtn_Click(object sender, RoutedEventArgs e)
        {
            if (enemyList.Count == 0)
            {
                MessageBox.Show("Сначала добавьте хотя бы одного противника в редакторе!");
                return;
            }

            if (string.IsNullOrEmpty(currentIconsPath))
            {
                MessageBox.Show("Сначала загрузите иконки в редакторе (нажмите 'Загрузить иконки')!");
                return;
            }

            // Берем сохраненный путь и передаем его в игру
            var allTemplates = enemyList.GetAllTemplates();
            GameWindow game = new GameWindow(allTemplates, currentIconsPath);
            game.Show();
        }
     }
}


