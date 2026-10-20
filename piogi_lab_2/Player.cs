using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace piogi_lab_2
{
    public class Player
    {
        // ИНКАПСУЛЯЦИЯ: Снаружи можно только ЧИТАТЬ (get), 
        // но нельзя изменить (private set). Изменить можно только через методы ниже.
        public BigNumber Gold { get; private set; }
        public BigNumber Damage { get; private set; }

        // Конструктор: задаем начальные значения
        public Player()
        {
            Gold = new BigNumber(0);
            Damage = new BigNumber(1);
        }

        // Метод для получения золота
        public void AddGold(BigNumber amount)
        {
            Gold = Gold + amount;
        }

        // Метод покупки улучшения
        public bool TryUpgrade(BigNumber cost)
        {
            if (Gold >= cost)
            {
                Gold = Gold - cost; 
                Damage = Damage + new BigNumber(1); 
                return true; 
            }

            return false; 
        }
    }



}
