using System;
using MindboxTest.Figures;
using NUnit.Framework;

namespace Figures.Tests
{
    [TestFixture]
    public class CircleTests
    {
        [Test]
        public void ShouldCreateCircleWithCorrectRadius()
        {
            Assert.DoesNotThrow(() => new Circle(1));
        }

        [TestCase(-1)]
        [TestCase(0)]
        public void CircleConstructorShouldThrowExceptionOnNonPositiveRadius(double radius)
        {
            Assert.Catch(typeof(ArgumentException),
                () => new Circle(radius), "Radius should be greater then 0");
        }
        
        [TestCase(2)]
        [TestCase(1)]
        public void ShouldCorrectlyCalculateArea(double radius)
        {
            Assert.AreEqual(radius * radius * Math.PI, new Circle(radius).CalculateArea());
        }
    }
}