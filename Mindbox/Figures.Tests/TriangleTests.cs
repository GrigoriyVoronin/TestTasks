using System;
using MindboxTest.Figures;
using NUnit.Framework;

namespace Figures.Tests
{
    [TestFixture]
    public class TriangleTests
    {
        [Test]
        public void ShouldCreateTriangleWithCorrectSides()
        {
            Assert.DoesNotThrow(() => new Triangle(1, 2, 2));
        }

        [TestCase(0, 1, 1, "FirstSide")]
        [TestCase(1, -1, 1, "SecondSide")]
        [TestCase(1, 1, -2, "ThirdSide")]
        public void TriangleConstructorShouldThrowExceptionOnNonPositiveSides(double firstSide,
            double secondSide, double thirdSide, string parameterName)
        {
            Assert.Catch(typeof(ArgumentException),
                () => new Triangle(firstSide, secondSide, thirdSide), $"{parameterName} should be greater then 0");
        }

        [TestCase(1, 1, 2)]
        [TestCase(1, 5, 1)]
        [TestCase(3, 1, 1)]
        public void TriangleConstructorShouldThrowExceptionWithIncorrectSidesSize(double firstSide,
            double secondSide, double thirdSide)
        {
            const string message = "The triangle rule is not observed. " +
                                   "The sum of the lengths of any two sides must be greater than the length of the third.";
            Assert.Catch(typeof(ArgumentException),
                () => new Triangle(firstSide, secondSide, thirdSide), message);
        }

        [TestCase(3, 4, 5, true)]
        [TestCase(3, 5, 4, true)]
        [TestCase(5, 4, 3, true)]
        [TestCase(3, 3, 5, false)]
        public void TriangleShouldCorrectlyComputeRectangularTriangle(double firstSide,
            double secondSide, double thirdSide, bool isRectangularTriangle)
        {
            Assert.AreEqual(isRectangularTriangle,
                new Triangle(firstSide, secondSide, thirdSide).IsRectangularTriangle());
        }

        [TestCase(1, 2, 2)]
        public void TrianglePerimeterShouldBeTheSumOfAllSides(double firstSide,
            double secondSide, double thirdSide)
        {
            Assert.AreEqual(firstSide+ secondSide+ thirdSide, new Triangle(firstSide,secondSide, thirdSide).Perimeter);
        }


        [TestCase(1, 2, 2, 0.968_245_8)]
        [TestCase(3, 4, 5, 6)]
        public void TriangleShouldCorrectlyCalculateTriangleArea(double firstSide,
            double secondSide, double thirdSide, double area)
        {
            Assert.AreEqual(area, new Triangle(firstSide, secondSide, thirdSide).CalculateArea(), 0.000_000_1);
        }
    }
}