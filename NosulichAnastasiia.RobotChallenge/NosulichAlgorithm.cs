using System;
using System.Collections.Generic;
using Nosulich.Nastya.RobotChallenge;
using Robot.Common;

namespace NosulichAnastasiia.RobotChallenge
{
    public class NosulichAnastasiiaAlgorithm : IRobotAlgorithm
    {
        private Dictionary<int, bool> _hasCreatedRobot = new Dictionary<int, bool>();
        private List<int> _betterRobots = new List<int>();

        public string Author => "Nosulich Anastasiia";

        private int _robotNumber = 71;
        private int _myRobots = 10;


        private event Action NewRobot; 
        
        private void OnRobotCreating()
        {
            _myRobots++;
        }


        public RobotCommand DoStep(IList<Robot.Common.Robot> robots, int robotToMoveIndex, Map map)
        {
            NewRobot += OnRobotCreating;

            Robot.Common.Robot movingRobot = robots[robotToMoveIndex];

            if (IsRobotOnStation(movingRobot, map))
            {
                if (movingRobot.Energy >= DistanceHelper.FindDistance(movingRobot.Position, FindAppropriateStationPosition(movingRobot, map, robots)) && movingRobot.Energy >= 300)
                {
                    if (!_hasCreatedRobot.ContainsKey(robotToMoveIndex) && _myRobots < _robotNumber && CanCreateRobot(movingRobot, map, robots))
                    {
                        _hasCreatedRobot.Add(robotToMoveIndex, true);
                        NewRobot?.Invoke();
                        return new CreateNewRobotCommand() { NewRobotEnergy = 200};
                    }
                    else if (_hasCreatedRobot.ContainsKey(robotToMoveIndex) && !_hasCreatedRobot[robotToMoveIndex] && CanCreateRobot(movingRobot, map, robots) && _myRobots < _robotNumber)
                    {
                        _hasCreatedRobot[robotToMoveIndex] = true;
                        NewRobot?.Invoke();
                        return new CreateNewRobotCommand() { NewRobotEnergy = 200 };
                    }
                    else if(_hasCreatedRobot.ContainsKey(robotToMoveIndex) && _myRobots < _robotNumber)
                    {
                        _hasCreatedRobot[robotToMoveIndex] = true;
                        if (_betterRobots.Contains(robotToMoveIndex))
                        {
                            return new CollectEnergyCommand();
                        }
                        if(FindBetterStation(map, movingRobot) != null)
                        {
                            _betterRobots.Add(robotToMoveIndex);
                            return new MoveCommand() { NewPosition = FindBetterStation(map, movingRobot) };
                        }
                    }
                }
                return new CollectEnergyCommand();
            }
            return new MoveCommand() { NewPosition = FindAppropriateStationPosition(movingRobot, map, robots) };
        }

        public Position FindBetterStation(Map map, Robot.Common.Robot robot)
        {
            int maxNeighbors = 0; int h;
            EnergyStation station = null;
            foreach (var currentStation in map.Stations)
            {   
                
                if(robot.Energy >= DistanceHelper.FindDistance(robot.Position, currentStation.Position))
                {
                    var neignbors = GetStationsCount(currentStation, map, out h);
                    if (neignbors > maxNeighbors)
                    {
                        maxNeighbors = neignbors;
                        station = currentStation;
                    }
                    if(neignbors == maxNeighbors && DistanceHelper.FindDistance(currentStation.Position, robot.Position) < DistanceHelper.FindDistance(station.Position, robot.Position))
                    {
                        station = currentStation;
                    }
                }
                
            }
            return station?.Position;
        }

        public int GetStationsCount(EnergyStation station, Map map, out int energy)
        {
            int overallEnergy = 0;
            int stationCount = 0;
            var stationsDictionary = new Dictionary<Position, EnergyStation>();
            foreach (var currentStation in map.Stations)
            {
                if (!stationsDictionary.ContainsKey(currentStation.Position))
                {
                    stationsDictionary.Add(currentStation.Position, currentStation);
                }
            }
            int primaryX = station.Position.X - 2;
            int primaryY = station.Position.Y - 2;
            int x = primaryX;
            int y = primaryY;

            for (int i = 0; i < 5; ++i)
            {
                for (int g = 0; g < 5; ++g)
                {
                    var pos = new Position { X = x, Y = y };
                    if (stationsDictionary.ContainsKey(pos))
                    {
                        stationCount++;
                        overallEnergy += stationsDictionary[pos].Energy;
                    }
                    x++;
                }
                x = primaryX;
                y++;
            }
            energy = overallEnergy;
            return stationCount;
        }

        public bool CanCreateRobot(Robot.Common.Robot rob, Map map, IList<Robot.Common.Robot> robots)
        {
            var stationsDictionary = new Dictionary<Position, EnergyStation>();

            List<Robot.Common.Robot> myRobots = new List<Robot.Common.Robot>();
            foreach (var robot in  robots)
            {
                if(robot.OwnerName == "Nosulich Anastasiia")
                {
                    myRobots.Add(robot);
                }
            }

            bool j = true;

            foreach (var currentStation in map.Stations)
            {
                if (!stationsDictionary.ContainsKey(currentStation.Position))
                {
                    foreach (var robot in myRobots)
                    {
                        if(robot.Position == currentStation.Position)
                        {
                            j = false; break;
                        }
                    }
                    if (j)
                    {
                        stationsDictionary.Add(currentStation.Position, currentStation);
                    }
                }
                j = true;
            }
            foreach (var station in stationsDictionary.Values)
            {
                if (180 >= DistanceHelper.FindDistance(rob.Position, station.Position)) return true;
            }
            return false;
        }

        public Position FindAppropriateStationPosition(Robot.Common.Robot movingRobot, Map map, IList<Robot.Common.Robot> robots)
        {
            EnergyStation nearest = null;
            int minDistance = int.MaxValue;
            foreach (var station in map.Stations)
            {
                bool getOut = false;
                foreach (var robot in robots)
                {
                    if (robot.Position == station.Position && robot.OwnerName == "Nosulich Anastasiia")
                    {
                        getOut = true;
                    }
                }
                if (getOut)
                {
                    continue;
                }
                int d = DistanceHelper.FindDistance(station.Position, movingRobot.Position);
                if (d < minDistance)
                {
                    minDistance = d;
                    nearest = station;
                }
                if (d == minDistance)
                {
                    int stationEnergy, nearestEnergy;
                    int stationCount = GetStationsCount(station, map, out stationEnergy);
                    int nearestCount = GetStationsCount(nearest, map, out nearestEnergy);
                    if (stationCount > nearestCount)
                    {
                        nearest = station;
                    }
                    else if (nearestCount == stationCount)
                    {
                        if (stationEnergy > nearestEnergy)
                        {
                            nearest = station;
                        }
                    }
                }
            }
            return nearest?.Position;
        }

        public bool IsRobotOnStation(Robot.Common.Robot movingRobot, Map map)
        {
            foreach (var station in map.Stations)
            {
                if (movingRobot.Position == station.Position)
                {
                    return true;
                }
            }
            return false;
        }
    }
}

