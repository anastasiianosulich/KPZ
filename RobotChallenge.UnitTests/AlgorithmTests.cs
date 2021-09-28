using NUnit.Framework;
using NosulichAnastasiia.RobotChallenge;
using Robot.Common;
using System.Collections.Generic;
using Assert = NUnit.Framework.Assert;

namespace RobotChallenge.UnitTests
{
    [TestFixture]
    public class AlgorithmTests
    {
        [Test]
        public void DoStep_RobotIsOnStation_CollectEnergyCommandIsReturned()
        {
            var algorithm = new NosulichAnastasiiaAlgorithm();
            var map = new Map();
            var stationPosition = new Position(1, 2);
            map.Stations.Add(new EnergyStation { Energy = 100, Position = stationPosition });
            var robots = new List<Robot.Common.Robot>() { new Robot.Common.Robot { Position = stationPosition, Energy = 0 } };

            var command = algorithm.DoStep(robots, 0, map);

            Assert.That(command, Is.InstanceOf<CollectEnergyCommand>());
        }

        [Test]
        public void DoStep_RobotIsOnDifferentDistancesToTwoStations_RobotMovesToNearerStation()
        {
            var algorithm = new NosulichAnastasiiaAlgorithm();
            var map = new Map();
            var stationNearer = new EnergyStation { Position = new Position { X = 0, Y = 7 } };
            var stationFarer = new EnergyStation { Position = new Position { X = 0, Y = 14 } };
            var robots = new List<Robot.Common.Robot>() { new Robot.Common.Robot { Position = new Position { X = 0, Y = 10 }, Energy = 100 } };
            map.Stations.Add(stationNearer);
            map.Stations.Add(stationFarer);


            var command = algorithm.DoStep(robots, 0, map);

            Assert.That(command, Is.InstanceOf<MoveCommand>());
            Assert.AreEqual(((MoveCommand)command).NewPosition, stationNearer.Position);
        }

        [Test]
        public void DoStep_RobotIsOnEqualDistancesToTwoStationsWithDifferentEnergies_RobotMovesToStationWhithMoreEnergy()
        {

            var algorithm = new NosulichAnastasiiaAlgorithm();
            var map = new Map();
            var stationWithLessEnergy = new EnergyStation { Position = new Position { X = 0, Y = 7 }, Energy = 150 };
            var stationWithMoreEnergy = new EnergyStation { Position = new Position { X = 0, Y = 13 }, Energy = 200 };
            var robots = new List<Robot.Common.Robot>() { new Robot.Common.Robot { Position = new Position { X = 0, Y = 10 } } };
            map.Stations.Add(stationWithLessEnergy);
            map.Stations.Add(stationWithMoreEnergy);


            var command = algorithm.DoStep(robots, 0, map);

            Assert.That(command, Is.InstanceOf<MoveCommand>());
            Assert.AreEqual(((MoveCommand)command).NewPosition, stationWithMoreEnergy.Position);
        }

        [Test]
        public void DoStep_RobotIsOnEqualDistanceBetweenStations_RobotMovesToStationThatHasMoreOtherStationsNextToIt()
        {
            var algorithm = new NosulichAnastasiiaAlgorithm();
            var map = new Map();
            var leftstation1 = new EnergyStation { Position = new Position(1, 1) };
            var leftstation2 = new EnergyStation { Position = new Position(3, 2) };
            var rightstation1 = new EnergyStation { Position = new Position(8, 0) };
            var rightstation2 = new EnergyStation { Position = new Position(7, 2) };
            var rightstation3 = new EnergyStation { Position = new Position(9, 3) };

            map.Stations.Add(leftstation1);
            map.Stations.Add(leftstation2);
            map.Stations.Add(rightstation1);
            map.Stations.Add(rightstation2);
            map.Stations.Add(rightstation3);

            var robots = new List<Robot.Common.Robot>() { new Robot.Common.Robot { Position = new Position(5, 2) } };

            var command = algorithm.DoStep(robots, 0, map);

            Assert.That(command, Is.InstanceOf<MoveCommand>());
            Assert.AreEqual(((MoveCommand)command).NewPosition, rightstation2);
        }

        [Test]
        public void DoStep_RobotIsOnDifferentDistanceBetweenStationsButFarerHasMoreNeighbors_RobotMovesToFarerWithMoreStationsNextToIt()
        {
            var algorithm = new NosulichAnastasiiaAlgorithm();
            var map = new Map();
            var leftstation1 = new EnergyStation { Position = new Position(1, 2) };
            var leftstation2 = new EnergyStation { Position = new Position(0, 4) };
            var rightstation1 = new EnergyStation { Position = new Position(8, 2) };

            map.Stations.Add(leftstation1);
            map.Stations.Add(leftstation2);
            map.Stations.Add(rightstation1);

            var robots = new List<Robot.Common.Robot>() { new Robot.Common.Robot { Position = new Position(6, 2), Energy = 20 } };

            var command = algorithm.DoStep(robots, 0, map);

            Assert.That(command, Is.InstanceOf<MoveCommand>());
            Assert.AreEqual(((MoveCommand)command).NewPosition, leftstation1);
        }

        [Test]
        public void DoStep_RobotIsOnDifferentDistanceBetweenStationsButFarerHasMoreNeighborsButRobotDoesNotHaveEnoughEnergy_RobotMovesToNearer()
        {
            var algorithm = new NosulichAnastasiiaAlgorithm();
            var map = new Map();
            var leftstation1 = new EnergyStation { Position = new Position(1, 2) };
            var leftstation2 = new EnergyStation { Position = new Position(0, 4) };
            var rightstation1 = new EnergyStation { Position = new Position(8, 2) };

            map.Stations.Add(leftstation1);
            map.Stations.Add(leftstation2);
            map.Stations.Add(rightstation1);

            var robots = new List<Robot.Common.Robot>() { new Robot.Common.Robot { Position = new Position(6, 2), Energy = 15 } };

            var command = algorithm.DoStep(robots, 0, map);

            Assert.That(command, Is.InstanceOf<MoveCommand>());
            Assert.AreEqual(((MoveCommand)command).NewPosition, rightstation1);
        }

        [Test]
        public void DoStep_ExistsOneStationWithAlienRobotOnIt_RobotPushesAlienRobot()
        {
            var algorithm = new NosulichAnastasiiaAlgorithm();
            var map = new Map();
            var station = new EnergyStation { Position = new Position(2, 0) };
            var myRobot = new Robot.Common.Robot { Position = new Position(0, 1), OwnerName = "Nosulich Anastasiia" };
            var alienRobot = new Robot.Common.Robot { Position = station.Position, OwnerName = "Someone alien" };
            map.Stations.Add(station);
            var robots = new List<Robot.Common.Robot>() { myRobot, alienRobot };

            var command = algorithm.DoStep(robots, 0, map);

            Assert.That(command, Is.InstanceOf<MoveCommand>());
            Assert.AreEqual(((MoveCommand)command).NewPosition, station.Position);
        }

        [Test]
        [TestCase(3, 0, 0, 4)]
        [TestCase(4, 0, 7, 0)]
        public void DoStep_OneStationWithAlienRobotIsNearerAndSecondPLainStationIsFarer_RobotMovesOptimally(int x, int y, int resX, int resY)
        {
            var algorithm = new NosulichAnastasiiaAlgorithm();
            var map = new Map();
            var plainStation = new EnergyStation { Position = new Position(0, 4) };
            var stationWithAlienRobot = new EnergyStation { Position = new Position(7, 0) };
            var myRobot = new Robot.Common.Robot { Position = new Position(x, y), OwnerName = "Nosulich Anastasiia" };
            var alienRobot = new Robot.Common.Robot { Position = stationWithAlienRobot.Position, OwnerName = "Someone alien" };
            map.Stations.Add(plainStation);
            map.Stations.Add(stationWithAlienRobot);
            var robots = new List<Robot.Common.Robot>() { myRobot, alienRobot };

            var command = algorithm.DoStep(robots, 0, map);

            Assert.That(command, Is.InstanceOf<MoveCommand>());
            Assert.AreEqual(((MoveCommand)command).NewPosition, new Position(resX, resY));
        }
    }
}
