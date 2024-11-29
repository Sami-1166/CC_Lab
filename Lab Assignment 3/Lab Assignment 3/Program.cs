using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace GalaxyShooterGame
{
    internal class Program
    {
        // Game Constants
        static int screenWidth = 30;
        static int screenHeight = 10;
        static int playerX = screenWidth / 2;
        static int playerY = screenHeight - 1;
        static List<int> bullets = new List<int>(); // Bullet positions (Y positions)
        static List<int> enemyX = new List<int>();   // Enemy X positions
        static List<int> enemyY = new List<int>();   // Enemy Y positions
        static List<int> enemyDirection = new List<int>();  // Direction of enemies (-1 for left, 1 for right)
        static bool gameOver = false;

        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            Console.SetWindowSize(screenWidth, screenHeight);
            Console.SetBufferSize(screenWidth, screenHeight);

            InitializeEnemies();

            // Game Loop
            while (!gameOver)
            {
                Update();
                Render();
                Thread.Sleep(100);
            }

            // Show Game Over message
            Console.SetCursorPosition(0, screenHeight);
            Console.WriteLine("Game Over! Press any key to exit...");
            Console.ReadKey();
        }

        // Initialize enemies at random positions
        static void InitializeEnemies()
        {
            Console.WriteLine("Enter the number of Enemies to Kill: ");
            int value = int.TryParse(Console.ReadLine(), out value) ? value : throw new FormatException("Invalid input. Please enter a valid integer.");
            Random rand = new Random();
            for (int i = 0; i < value; i++)
            {
                int enemyXPos = rand.Next(0, screenWidth);  // Random X position for enemy
                int enemyYPos = rand.Next(0, screenHeight / 2); // Random Y position for enemy (upper half of screen)
                enemyX.Add(enemyXPos);
                enemyY.Add(enemyYPos);
                enemyDirection.Add(rand.Next(0, 2) * 2 - 1);  // Random direction (1 for right, -1 for left)
            }
        }

        // Handle user input for player movement and shooting
        static void HandleInput()
        {
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.LeftArrow && playerX > 0)
                    playerX--;
                else if (key == ConsoleKey.RightArrow && playerX < screenWidth - 1)
                    playerX++;
                else if (key == ConsoleKey.Spacebar)
                    bullets.Add(playerY - 1); // Add a new bullet starting from the player's position
            }
        }

        // Move bullets upwards
        static void MoveBullets()
        {
            for (int i = 0; i < bullets.Count; i++)
            {
                bullets[i]--; // Move the bullet upwards (decrease Y position)
                if (bullets[i] < 0) bullets.RemoveAt(i--); // Remove bullet if it goes off-screen
            }
        }

        // Move enemies horizontally
        static void MoveEnemies()
        {
            for (int i = 0; i < enemyX.Count; i++)
            {
                // Move the enemy horizontally
                enemyX[i] += enemyDirection[i];

                // If enemy reaches the edge of the screen, reverse direction
                if (enemyX[i] >= screenWidth - 1 || enemyX[i] <= 0)
                {
                    enemyDirection[i] *= -1;  // Reverse direction
                }
            }
        }

        // Check for collisions between bullets and enemies, and player and enemies
        static void CheckCollisions()
        {
            for (int i = 0; i < bullets.Count; i++)
            {
                int bulletY = bullets[i];
                for (int j = 0; j < enemyX.Count; j++)
                {
                    // Bullet hits enemy if it has the same X and Y position
                    if (bulletY == enemyY[j] && enemyX[j] == playerX)
                    {
                        // Remove the enemy and bullet on collision
                        enemyX.RemoveAt(j);
                        enemyY.RemoveAt(j);
                        bullets.RemoveAt(i--); // Remove bullet
                        break; // Bullet can only hit one enemy, so break out of inner loop
                    }
                }
            }

            // Check if any enemy reaches the player's position (game over condition)
            for (int i = 0; i < enemyX.Count; i++)
            {
                if (enemyY[i] == playerY && enemyX[i] == playerX) // Player position collision
                {
                    gameOver = true; // Game over if an enemy reaches the player's position
                    break;
                }
            }
        }

        // Update game state: input, movement, and collisions
        static void Update()
        {
            if (!gameOver)
            {
                HandleInput();
                MoveBullets();
                MoveEnemies();
                CheckCollisions();
            }
        }

        // Render the game state to the console
        static void Render()
        {
            Console.Clear();

            // Draw player
            Console.SetCursorPosition(playerX, playerY);
            Console.Write('@');

            // Draw bullets
            foreach (var bulletY in bullets)
            {
                Console.SetCursorPosition(playerX, bulletY);
                Console.Write('|');
            }

            // Draw enemies
            for (int i = 0; i < enemyX.Count; i++)
            {
                Console.SetCursorPosition(enemyX[i], enemyY[i]);
                Console.Write('X');
            }
        }
    }
}