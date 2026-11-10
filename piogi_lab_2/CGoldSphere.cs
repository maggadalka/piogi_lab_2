using System.Windows;
using System.Windows.Media;

namespace piogi_lab_2
{
    // Сфера: мгновенное золото (эффект разовый, поэтому в список активных бонусов не попадает)
    public class CGoldSphere : CCollectable
    {
        public CGoldSphere(Point position, double size, double lifetime)
            : base(position, size, lifetime)
        {
            sprite.Fill = Brushes.Gold;
        }

        public override bool onClick(Player player, CController controller, Point mousePosition)
        {
            if (isMouseOnObject(mousePosition) == false)
                return false;

            player.AddGold(controller.getGoldBonus());
            return true;
        }
    }
}
