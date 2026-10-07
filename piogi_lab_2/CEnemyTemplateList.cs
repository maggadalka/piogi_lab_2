using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;

namespace piogi_lab_2
{
    public class CEnemyTemplateList
    {
        private List<CEnemyTemplate> enemies;

        public CEnemyTemplateList()
        {
            enemies = new List<CEnemyTemplate>();
        }

        public void AddEnemy(string name, string iconName, int baseLife, double lifeModifier,
                             int baseGold, double goldModifier, double spawnChance)
        {
            enemies.Add(new CEnemyTemplate(name, iconName, baseLife, lifeModifier,
                                            baseGold, goldModifier, spawnChance));
        }

        public CEnemyTemplate GetEnemyByName(string name)
        {
            return enemies.FirstOrDefault(e => e.Name == name);
        }

        public CEnemyTemplate GetEnemyByIndex(int id)
        {
            if (id < 0 || id >= enemies.Count) return null;
            return enemies[id];
        }

        public void DeleteEnemyByName(string name)
        {
            var enemy = GetEnemyByName(name);
            if (enemy != null) enemies.Remove(enemy);
        }

        public void DeleteEnemyByIndex(int id)
        {
            if (id >= 0 && id < enemies.Count)
                enemies.RemoveAt(id);
        }

        public List<string> GetListOfEnemyNames()
        {
            return enemies.Select(e => e.Name).ToList();
        }

        public int Count => enemies.Count;

        public void SaveToJson(string path)
        {
            string json = JsonSerializer.Serialize(enemies, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, json);
        }

        public void LoadFromJson(string path)
        {
            if (!File.Exists(path)) return; // защита от ошибки на первом запуске
            string json = File.ReadAllText(path);
            enemies = JsonSerializer.Deserialize<List<CEnemyTemplate>>(json) ?? new List<CEnemyTemplate>();
        }

        public void UpdateEnemy(string oldName, string newName, string iconName,
                        int baseLife, double lifeModifier,
                        int baseGold, double goldModifier, double spawnChance)
        {
            // Находим старого противника по имени
            var oldEnemy = GetEnemyByName(oldName);
            if (oldEnemy == null) return;

            // Удаляем его из списка
            enemies.Remove(oldEnemy);

            // Добавляем нового с обновлёнными данными
            enemies.Add(new CEnemyTemplate(newName, iconName, baseLife, lifeModifier,
                                            baseGold, goldModifier, spawnChance));
        }
    }

}
