using NosulichAnastasiia.RobotChallenge;
using NUnit.Framework;
using Robot.Common;

namespace RobotChallenge.UnitTests
{
    [TestFixture]
    class DistanceHelperTests
    {
        [Test]
        public void FindDistance_WhenCalled_ReturnsRightDistance()
        {
            var p1 = new Position(1, 1);
            var p2 = new Position(2, 4);
            Assert.AreEqual(10, DistanceHelper.FindDistance(p1, p2));
        }
    }
}
