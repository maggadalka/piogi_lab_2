using System.Windows;
using System.Windows.Media;

namespace piogi_lab_2
{
    // Сфера: временное уменьшение перезарядки
    public class CCooldownSphere : CCollectable
    {
        public CCooldownSphere(Point position, double size, double lifetime)
            : base(position, size, lifetime)
        {
            sprite.Fill = Brushes.DeepSkyBlue;
        }

        public override bool onClick(Player player, CController controller, Point mousePosition)
        {
            if (isMouseOnObject(mousePosition) == false)
                return false;

            controller.addActiveBonus(new CCooldownBoost(), player);
            return true;
        }
    }
}
