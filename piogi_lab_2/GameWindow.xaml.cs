using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace piogi_lab_2
{
    public partial class GameWindow : Window
    {
        private Player player;
        private Enemy currentEnemy;
        private List<CEnemyTemplate> templates; 
        private string iconsFolderPath;
        private int currentWave = 1;
        private Random rnd = new Random();

        public GameWindow(List<CEnemyTemplate> loadedTemplates, string iconsPath)
        {
            InitializeComponent();

            templates = loadedTemplates;
            iconsFolderPath = iconsPath;

            player = new Player();

            UpgradeBtn.Click += UpgradeBtn_Click;

            SpawnNewEnemy();

            UpdateUI();
        }

        private void SpawnNewEnemy()
        {
            if (templates == null || templates.Count == 0)
            {
                MessageBox.Show("Список врагов пуст! Загрузите шаблоны в редакторе.");
                this.Close();
                return;
            }

            // Считаем общую сумму шансов 
            double totalChance = 0;
            foreach (var t in templates)
            {
                if (t.SpawnChance > 0) totalChance += t.SpawnChance;
            }

            if (totalChance <= 0)
            {
                CEnemyTemplate fallback = templates[rnd.Next(templates.Count)];
                currentEnemy = new Enemy(fallback, currentWave);
                UpdateEnemyVisuals(fallback);
                return;
            }

            //  Выбираем случайное число от 0 до общей суммы
            double randomValue = rnd.NextDouble() * totalChance;
            double currentSum = 0;
            CEnemyTemplate selectedTemplate = templates[0];

            foreach (var template in templates)
            {
                if (template.SpawnChance <= 0) continue; 

                currentSum += template.SpawnChance;
                if (randomValue <= currentSum)
                {
                    selectedTemplate = template;
                    break;
                }
            }

            //  Создаем врага и обновляем картинку
            currentEnemy = new Enemy(selectedTemplate, currentWave);
            UpdateEnemyVisuals(selectedTemplate);
        }

        private void UpdateEnemyVisuals(CEnemyTemplate template)
        {
            EnemyNameText.Text = currentEnemy.Name;
            
            try
            {
                if (!string.IsNullOrEmpty(iconsFolderPath))
                {
                    string imagePath = System.IO.Path.Combine(iconsFolderPath, template.IconName);
                    EnemyImage.Source = new BitmapImage(new Uri(imagePath));
                }
            }
            catch
            {
                EnemyImage.Source = null; 
            }
        }


        // клик по врагу
        private void EnemyImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (currentEnemy == null)
            {
                MessageBox.Show("Враг равен null!");
                return;
            }

            string healthBefore = currentEnemy.CurrentHealth.ToString();

            currentEnemy.TakeDamage(player.Damage);

            string healthAfter = currentEnemy.CurrentHealth.ToString();

            if (currentEnemy.IsDead())
            {
                player.AddGold(currentEnemy.RewardGold);
                currentWave++;
                SpawnNewEnemy();
            }

            UpdateUI();
        }


        // покупка улучгения
        private void UpgradeBtn_Click(object sender, RoutedEventArgs e)
        {
            if (player.TryUpgrade())
            {
                UpdateUI();
            }
            else
            {
                MessageBox.Show("Недостаточно золота!");
            }
        }


        // обновление интерфейса
        private void UpdateUI()
        {
            GoldText.Text = player.Gold.ToString();
            DamageText.Text = player.Damage.ToString();

            if (currentEnemy != null)
            {
                HealthText.Text = $"{currentEnemy.CurrentHealth} / {currentEnemy.MaxHealth}";

                HealthBar.Minimum = 0;
                HealthBar.Maximum = 100;

                try
                {
                    if (double.TryParse(currentEnemy.CurrentHealth.ToString(), out double currentHP) &&
                        double.TryParse(currentEnemy.MaxHealth.ToString(), out double maxHP))
                    {
                        if (maxHP > 0)
                        {
                            double percent = (currentHP / maxHP) * 100;
                            HealthBar.Value = percent;
                        }
                    }
                }
                catch
                {
                    HealthBar.Value = 0; 
                }
            }

            BigNumber nextCost = player.Damage * 10;
            UpgradeBtn.Content = $"Улучшить урон (Цена: {nextCost})";
        }

    }
}