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

            // Считаем общую сумму шансов (игнорируем нулевые и отрицательные)
            double totalChance = 0;
            foreach (var t in templates)
            {
                if (t.SpawnChance > 0) totalChance += t.SpawnChance;
            }

            // Если у всех врагов шанс 0 (или пусто), выбираем случайного
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
            CEnemyTemplate selectedTemplate = templates[0]; // По умолчанию первый

            // Ищем, в какой промежуток попало наше случайное число
            foreach (var template in templates)
            {
                if (template.SpawnChance <= 0) continue; // Пропускаем нулевые

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

        // Вынес обновление визуала в отдельный метод, чтобы не дублировать код
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
                EnemyImage.Source = null; // Если картинки нет, не падаем
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

            // Запоминаем здоровье ДО удара
            string healthBefore = currentEnemy.CurrentHealth.ToString();

            // Наносим урон
            currentEnemy.TakeDamage(player.Damage);

            // Запоминаем здоровье ПОСЛЕ удара
            string healthAfter = currentEnemy.CurrentHealth.ToString();

            if (currentEnemy.IsDead())
            {
                player.AddGold(currentEnemy.RewardGold);
                currentWave++;
                SpawnNewEnemy();
            }

            UpdateUI();
        }


        // ПОКУПКА УЛУЧШЕНИЯ
        private void UpgradeBtn_Click(object sender, RoutedEventArgs e)
        {
            // Цена улучшения = текущий урон * 10 (например, чтобы росла экспоненциально)
            BigNumber upgradeCost = player.Damage * 10;

            if (player.TryUpgrade(upgradeCost))
            {
                // Если купили, обновляем интерфейс
                UpdateUI();
            }
            else
            {
                MessageBox.Show("Недостаточно золота!");
            }
        }

        // ОБНОВЛЕНИЕ ЦИФР НА ЭКРАНЕ
        private void UpdateUI()
        {
            GoldText.Text = player.Gold.ToString();
            DamageText.Text = player.Damage.ToString();

            if (currentEnemy != null)
            {
                // Показываем точные цифры (даже если там миллиарды)
                HealthText.Text = $"{currentEnemy.CurrentHealth} / {currentEnemy.MaxHealth}";

                // Настраиваем шкалу так, чтобы она всегда была от 0 до 100 (проценты)
                HealthBar.Minimum = 0;
                HealthBar.Maximum = 100;

                try
                {
                    // Переводим наши BigNumber в обычные дробные числа для расчета процентов
                    if (double.TryParse(currentEnemy.CurrentHealth.ToString(), out double currentHP) &&
                        double.TryParse(currentEnemy.MaxHealth.ToString(), out double maxHP))
                    {
                        if (maxHP > 0)
                        {
                            // Высчитываем процент оставшегося здоровья
                            double percent = (currentHP / maxHP) * 100;
                            HealthBar.Value = percent;
                        }
                    }
                }
                catch
                {
                    HealthBar.Value = 0; // Если что-то пошло не так, просто обнуляем шкалу
                }
            }

            // Обновляем текст на кнопке улучшения
            BigNumber nextCost = player.Damage * 10;
            UpgradeBtn.Content = $"Улучшить урон (Цена: {nextCost})";
        }

    }
}