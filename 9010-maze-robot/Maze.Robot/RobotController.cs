using Maze.Library;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Maze.Solver
{
    /// <summary>
    /// Moves a robot from its current position towards the exit of the maze
    /// </summary>
    public class RobotController
    {
        private readonly IRobot robot;

        /// <summary>
        /// Initializes a new instance of the <see cref="RobotController"/> class
        /// </summary>
        /// <param name="robot">Robot that is controlled</param>
        public RobotController(IRobot robot)
        {
            // Store robot for later use
            this.robot = robot;
        }

        /// <summary>
        /// Moves the robot to the exit
        /// </summary>
        /// <remarks>
        /// This function uses methods of the robot that was passed into this class'
        /// constructor. It has to move the robot until the robot's event
        /// <see cref="IRobot.ReachedExit"/> is fired. If the algorithm finds out that
        /// the exit is not reachable, it has to call <see cref="IRobot.HaltAndCatchFire"/>
        /// and exit.
        /// </remarks>
        public void MoveRobotToExit()
        {

            var reachedEnd = false;
            robot.ReachedExit += (_, __) => reachedEnd = true;

            // To select a random direction
            var randgen = new Random();

            // First move:
            // Where can we move?
            var possibleDirections = Enum.GetValues<Direction>().Where((d) => robot.CanIMove(d));
            if (!possibleDirections.Any())
            {
                // We can't move!!
                robot.HaltAndCatchFire();
                return;
            }
            // If we can move, select a random direction and save it into lastDirection
            Direction lastDirection = possibleDirections.ElementAt(randgen.Next(0, possibleDirections.Count()));
            robot.Move(lastDirection);

            // Once we did the first move
            while (!reachedEnd)
            {
                // Where can we move not going back?
                possibleDirections = Enum.GetValues<Direction>().Where((d) => d != Antagonist(lastDirection) && robot.CanIMove(d));

                if (possibleDirections.Any())
                {
                    // If we can move, select a random direction and save it into lastDirection
                    lastDirection = possibleDirections.ElementAt(randgen.Next(0, possibleDirections.Count()));
                }
                else
                {
                    // If we can't move, go back from where we came
                    lastDirection = Antagonist(lastDirection);
                }

                robot.Move(lastDirection);
            }

        }

        private static Direction Antagonist(Direction? direction) => direction switch
        {
            Direction.Up => Direction.Down,
            Direction.Right => Direction.Left,
            Direction.Down => Direction.Up,
            Direction.Left => Direction.Right,
            _ => throw new ArgumentOutOfRangeException(nameof(direction), $"Not expected direction value: {direction}"),
        };

    }

}
