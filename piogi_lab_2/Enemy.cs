using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace piogi_lab_2
{
    public class Enemy
    {
        // Здоровье и награда скрыты от прямого изменения.
        public string Name { get; private set; }
        public BigNumber MaxHealth { get; private set; }
        public BigNumber CurrentHealth { get; private set; }
        public BigNumber RewardGold { get; private set; }

        public Enemy(CEnemyTemplate template, int wave)
        {
            Name = template.Name;

            // Базовое здоровье из редактора
            long baseHealth = template.BaseLife;

            // Прирост: за каждую волну добавляем 20% от БАЗОВОГО здоровья
            // (если база 100, то на 3-й волне будет 100 + 20 + 20 = 140)
            long healthBonus = (long)(baseHealth * 0.2 * (wave - 1));

            long finalHealth = baseHealth + healthBonus;
            if (finalHealth < 1) finalHealth = 1; // Защита, чтобы враг не был с 0 хп

            MaxHealth = new BigNumber(finalHealth);
            CurrentHealth = new BigNumber(finalHealth);

            // То же самое для золота
            long baseGold = template.BaseGold;
            long goldBonus = (long)(baseGold * 0.3 * (wave - 1)); // Золото растет на 30% от базы
            long finalGold = baseGold + goldBonus;

            RewardGold = new BigNumber(finalGold);
        }

        public void TakeDamage(BigNumber damage)
        {
            CurrentHealth = CurrentHealth - damage;

            if (CurrentHealth < new BigNumber(0))
            {
                CurrentHealth = new BigNumber(0); 
            }
        }

        public bool IsDead()
        {
            BigNumber zero = new BigNumber(0);

            return CurrentHealth <= zero;
        }
    }

}
