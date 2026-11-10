using System.Windows;
using System.Windows.Media;

namespace piogi_lab_2
{
    // Сфера: временное увеличение урона
    public class CDamageSphere : CCollectable
    {
        public CDamageSphere(Point position, double size, double lifetime)
            : base(position, size, lifetime)
        {
            sprite.Fill = Brushes.Crimson;
        }

        public override bool onClick(Player player, CController controller, Point mousePosition)
        {
            if (isMouseOnObject(mousePosition) == false)
                return false;

            controller.addActiveBonus(new CDamageBoost(), player);
            return true;
        }
    }
}
