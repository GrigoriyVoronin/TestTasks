using System;

namespace MindboxTest.Figures
{
    public class Circle : AbstractFigure
    {
        public Circle(double radius)
        {
            Radius = radius;
            VerifyParameters();
        }

        public double Radius { get; }

        public override double CalculateArea()
        {
            return Math.PI * Radius * Radius;
        }

        protected sealed override void VerifyParameters()
        {
            if (Radius <= 0)
            {
                throw new ArgumentException($"{nameof(Radius)} should be greater then 0");
            }
        }
    }
}