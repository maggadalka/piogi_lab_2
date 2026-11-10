using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace piogi_lab_2
{
    // Абстрактный базовый класс всех бонусных сфер
    public abstract class CCollectable
    {
        protected Point position;     // позиция (левый верхний угол) в сцене
        protected Size size;          // размер
        protected double lifetime;    // оставшееся время жизни, сек
        double maxLifetime;           // начальное время жизни (для затухания)
        protected Ellipse sprite;     // визуальное отображение

        public CCollectable(Point position, double size, double lifetime)
        {
            this.position = position;
            this.size = new Size(size, size);
            this.lifetime = lifetime;
            maxLifetime = lifetime;

            sprite = new Ellipse();
            sprite.Fill = Brushes.BlueViolet;
            sprite.StrokeThickness = 2;
            sprite.Stroke = Brushes.Black;
            sprite.HorizontalAlignment = HorizontalAlignment.Left;
            sprite.VerticalAlignment = VerticalAlignment.Top;
            sprite.Width = this.size.Width;
            sprite.Height = this.size.Height;
            sprite.RenderTransform = new TranslateTransform(position.X, position.Y);
        }

        // получение формы объекта
        public Ellipse getSprite()
        {
            return sprite;
        }

        // попал ли курсор в круг
        public bool isMouseOnObject(Point mousePosition)
        {
            double r = size.Width / 2;
            double dx = mousePosition.X - (position.X + r);
            double dy = mousePosition.Y - (position.Y + r);
            return dx * dx + dy * dy <= r * r;
        }

        // уменьшение времени жизни; true — время истекло
        public bool updateLifetime(double dt)
        {
            lifetime -= dt;
            // сфера постепенно бледнеет перед исчезновением
            sprite.Opacity = Math.Max(0.3, lifetime / maxLifetime);
            return lifetime <= 0;
        }

        // обработка нажатия; true — сфера собрана и должна исчезнуть
        public abstract bool onClick(Player player, CController controller, Point mousePosition);
    }
}
