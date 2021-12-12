using System;

namespace MindboxTest.Figures
{
    public class Triangle : AbstractFigure
    {
        public Triangle(double firstSide, double secondSide, double thirdSide)
        {
            FirstSide = firstSide;
            SecondSide = secondSide;
            ThirdSide = thirdSide;
            VerifyParameters();
        }

        public double FirstSide { get; }
        public double SecondSide { get; }
        public double ThirdSide { get; }

        public double Perimeter => FirstSide + SecondSide + ThirdSide;

        public override double CalculateArea()
        {
            if (IsRectangularTriangle(out var area))
            {
                return area;
            }
            var halfPerimeter = Perimeter / 2;
            return Math.Sqrt(halfPerimeter
                             * (halfPerimeter - FirstSide)
                             * (halfPerimeter - SecondSide)
                             * (halfPerimeter - ThirdSide));
        }

        protected sealed override void VerifyParameters()
        {
            VerifyParameterGreaterThenZero(FirstSide, nameof(FirstSide));
            VerifyParameterGreaterThenZero(SecondSide, nameof(SecondSide));
            VerifyParameterGreaterThenZero(ThirdSide, nameof(ThirdSide));
            if (FirstSide + SecondSide <= ThirdSide
                || FirstSide + ThirdSide <= SecondSide
                || ThirdSide + SecondSide <= FirstSide)
            {
                throw new ArgumentException("The triangle rule is not observed. " +
                                            "The sum of the lengths of any two sides must be greater than the length of the third.");
            }
        }

        private static void VerifyParameterGreaterThenZero(double parameter, string parameterName)
        {
            if (parameter <= 0)
            {
                throw new ArgumentException($"{parameterName} should be greater then 0");
            }
        }

        public bool IsRectangularTriangle()
        {
            return IsRectangularTriangle(out _);
        }

        private bool IsRectangularTriangle(out double area)
        {
            return CheckPythagoreanTheorem(FirstSide, SecondSide, ThirdSide, out area)
                   || CheckPythagoreanTheorem(FirstSide, ThirdSide, SecondSide, out area)
                   || CheckPythagoreanTheorem(SecondSide, ThirdSide, FirstSide, out area);
        }

        private static bool CheckPythagoreanTheorem(double a, double b, double c, out double area)
        {
            area = double.NaN;
            var isRectangularTriangle = Math.Abs(a * a + b * b - c * c) < 0.000_000_1;
            if (isRectangularTriangle)
            {
                area = a * b * 0.5;
            }

            return isRectangularTriangle;
        }
    }
}