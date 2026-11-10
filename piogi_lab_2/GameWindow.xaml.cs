using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

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

        // Таймер для перезарядки (и позже для бонусов)
        private DispatcherTimer timer;
        private const double TickSeconds = 0.1;

        public GameWindow(List<CEnemyTemplate> loadedTemplates, string iconsPath)
        {
            InitializeComponent();

            templates = loadedTemplates;
            iconsFolderPath = iconsPath;

            player = new Player();

            UpgradeBtn.Click += UpgradeBtn_Click;
            CooldownUpgradeBtn.Click += CooldownUpgradeBtn_Click;

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(TickSeconds);
            timer.Tick += Timer_Tick;
            timer.Start();
            Closed += (s, e) => timer.Stop();

            SpawnNewEnemy();

            UpdateUI();
        }

        // тик таймера: обновляем перезарядку
        private void Timer_Tick(object sender, EventArgs e)
        {
            player.update(TickSeconds);
            UpdateTimedUI();
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

            // перезарядка ещё идёт — клик игнорируется
            if (!player.Cooldown.IsReady)
                return;

            string healthBefore = currentEnemy.CurrentHealth.ToString();

            // урон с учётом активных бонусов
            currentEnemy.TakeDamage(player.EffectiveDamage);

            // после удара запускаем перезарядку заново
            player.Cooldown.Reset();

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

        // покупка улучшения перезарядки
        private void CooldownUpgradeBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!player.Cooldown.CanReduce)
            {
                MessageBox.Show("Перезарядка уже минимальна!");
                return;
            }

            if (player.TryUpgradeCooldown())
            {
                UpdateUI();
            }
            else
            {
                MessageBox.Show("Недостаточно золота!");
            }
        }


        // то, что меняется каждый тик таймера
        private void UpdateTimedUI()
        {
            CooldownBar.Value = player.Cooldown.Progress;
            CooldownText.Text = $"Перезарядка: {player.Cooldown.Duration:0.00} с";
        }


        // обновление интерфейса
        private void UpdateUI()
        {
            GoldText.Text = player.Gold.ToString();
            DamageText.Text = player.EffectiveDamage.ToString();

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

            if (player.Cooldown.CanReduce)
                CooldownUpgradeBtn.Content = $"Быстрее клики (Цена: {player.CooldownUpgradeCost})";
            else
                CooldownUpgradeBtn.Content = "Перезарядка минимальна";

            UpdateTimedUI();
        }

    }
}
