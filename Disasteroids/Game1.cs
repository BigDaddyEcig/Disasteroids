using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Disasteroids
{
    /// <summary>
    /// Main game class - inherits from MonoGame's Game class
    /// This demonstrates the core game loop pattern: Initialize -> LoadContent -> Update/Draw loop
    /// </summary>
    public class Game1 : Game
    {
        // MonoGame framework objects - required for graphics and rendering
        private GraphicsDeviceManager _graphics;  // Manages graphics device and settings
        private SpriteBatch _spriteBatch;          // Used for 2D rendering (drawing textures)
        
        // Game entity collections - demonstrates object-oriented design
        // Using composition: Game1 "has-a" player, bullets, etc.
        private Player player;                     // Single player instance
        private List<Bullet> bullets;              // Collection for object pooling pattern
        private List<Asteroid> asteroids;          // Dynamic collection that grows/shrinks
        private List<PowerUp> powerUps;            // Limited pool of power-ups
        private DeathEffects deathEffects;         // Particle system for visual effects
        
        // Textures
        private Texture2D playerTexture;
        private Texture2D bulletTexture;
        private Texture2D asteroidTexture;
        private Texture2D particleTexture;
        private Texture2D fragmentTexture;
        private Texture2D powerUpTexture;
        
        // Game state
        private Random random;
        private KeyboardState previousKeyboardState;
        private int score;
        private int lastLifeScore; // Track score for free life awards
        private bool playerWasAlive;
        private GameState currentGameState;
        private SimpleTextRenderer textRenderer;
        private int currentLevel; // Track current level for asteroid scaling
        
        // Sound and high score systems
        private SoundManager soundManager;
        private HighScoreManager highScoreManager;
        private SoundEffectInstance thrustSoundInstance;
        
        // High score entry
        private string playerNameInput = "";
        private bool isEnteringName = false;
        
        // Life loss effect
        private bool lifeLossEffectActive;
        private float lifeLossTimer;
        private const float LIFE_LOSS_PAUSE_DURATION = 1.5f;
        private DeathEffects lifeLossEffects;
        
        // Constants
        private const int MAX_BULLETS = 25;
        private const int INITIAL_ASTEROIDS = 4;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            
            // Set window size
            _graphics.PreferredBackBufferWidth = 1024;
            _graphics.PreferredBackBufferHeight = 768;
        }

        protected override void Initialize()
        {
            random = new Random();
            bullets = new List<Bullet>();
            asteroids = new List<Asteroid>();
            powerUps = new List<PowerUp>();
            score = 0;
            lastLifeScore = 0;
            currentLevel = 1;
            
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            
            // Create simple colored textures since we don't have image files
            CreateTextures();
            
            // Initialize player
            Vector2 playerStart = new Vector2(
                GraphicsDevice.Viewport.Width / 2f,
                GraphicsDevice.Viewport.Height / 2f
            );
            player = new Player(playerTexture, playerStart);
            
            // Create bullet pool
            for (int i = 0; i < MAX_BULLETS; i++)
            {
                bullets.Add(new Bullet(bulletTexture));
            }
            
            // Create initial asteroids
            CreateAsteroids(INITIAL_ASTEROIDS);
            
            // Create power-up pool
            for (int i = 0; i < 5; i++)
            {
                powerUps.Add(new PowerUp(powerUpTexture, random));
            }
            
            // Initialize death effects
            deathEffects = new DeathEffects(particleTexture, fragmentTexture, random);
            lifeLossEffects = new DeathEffects(particleTexture, fragmentTexture, random);
            playerWasAlive = true;
            lifeLossEffectActive = false;
            lifeLossTimer = 0f;
            
            // Initialize text renderer
            textRenderer = new SimpleTextRenderer(GraphicsDevice);
            
            // Initialize sound and high score systems
            soundManager = new SoundManager();
            soundManager.LoadContent(Content);
            highScoreManager = new HighScoreManager();
            
            // Create thrust sound instance for looping
            thrustSoundInstance = soundManager.CreateLoopingSound("thrust", 0.3f);
            
            currentGameState = GameState.MainMenu;
        }
        
        private void CreateTextures()
        {
            // Create player texture (elongated triangle shape)
            playerTexture = new Texture2D(GraphicsDevice, 24, 30);
            Color[] playerData = new Color[24 * 30];
            for (int i = 0; i < playerData.Length; i++)
            {
                int x = i % 24;
                int y = i / 24;
                
                // Create an elongated triangle shape pointing upward
                // Triangle tip at top center, base at bottom
                int centerX = 12;
                int tipY = 2;
                int baseY = 27;
                
                // Calculate if point is inside triangle
                bool insideTriangle = false;
                
                if (y >= tipY && y <= baseY)
                {
                    // Calculate triangle width at current y position
                    float progress = (float)(y - tipY) / (baseY - tipY);
                    int halfWidth = (int)(progress * 10); // Max half-width of 10 pixels
                    
                    if (x >= centerX - halfWidth && x <= centerX + halfWidth)
                    {
                        insideTriangle = true;
                    }
                }
                
                playerData[i] = insideTriangle ? Color.White : Color.Transparent;
            }
            playerTexture.SetData(playerData);
            
            // Create bullet texture (small white square)
            bulletTexture = new Texture2D(GraphicsDevice, 4, 4);
            Color[] bulletData = new Color[4 * 4];
            for (int i = 0; i < bulletData.Length; i++)
            {
                bulletData[i] = Color.Cyan;
            }
            bulletTexture.SetData(bulletData);
            
            // Create asteroid texture (irregular rocky shape)
            asteroidTexture = new Texture2D(GraphicsDevice, 40, 40);
            Color[] asteroidData = new Color[40 * 40];
            Vector2 center = new Vector2(20, 20);
            Random asteroidRandom = new Random(42); // Fixed seed for consistent asteroid shape
            
            for (int i = 0; i < asteroidData.Length; i++)
            {
                int x = i % 40;
                int y = i / 40;
                Vector2 pos = new Vector2(x, y);
                
                float distance = Vector2.Distance(pos, center);
                
                // Create irregular asteroid shape with noise
                float angle = (float)Math.Atan2(y - center.Y, x - center.X);
                float noiseValue = (float)(Math.Sin(angle * 6) * 0.3 + Math.Sin(angle * 4) * 0.2 + Math.Sin(angle * 8) * 0.1);
                float irregularRadius = 15 + noiseValue * 4; // Base radius with irregular bumps
                
                if (distance <= irregularRadius)
                {
                    // Create different shades for depth effect
                    float normalizedDistance = distance / irregularRadius;
                    if (normalizedDistance < 0.3f)
                    {
                        asteroidData[i] = Color.LightGray; // Inner lighter area
                    }
                    else if (normalizedDistance < 0.7f)
                    {
                        asteroidData[i] = Color.Gray; // Middle area
                    }
                    else
                    {
                        asteroidData[i] = Color.DarkGray; // Outer darker edge
                    }
                    
                    // Add some random rocky texture
                    if (asteroidRandom.NextDouble() < 0.1)
                    {
                        asteroidData[i] = Color.DimGray; // Random dark spots for texture
                    }
                }
                else
                {
                    asteroidData[i] = Color.Transparent;
                }
            }
            asteroidTexture.SetData(asteroidData);
            
            // Create particle texture (small square for explosion particles)
            particleTexture = new Texture2D(GraphicsDevice, 6, 6);
            Color[] particleData = new Color[6 * 6];
            for (int i = 0; i < particleData.Length; i++)
            {
                particleData[i] = Color.White;
            }
            particleTexture.SetData(particleData);
            
            // Create fragment texture (small triangle pieces)
            fragmentTexture = new Texture2D(GraphicsDevice, 8, 8);
            Color[] fragmentData = new Color[8 * 8];
            for (int i = 0; i < fragmentData.Length; i++)
            {
                int x = i % 8;
                int y = i / 8;
                
                // Create small triangle fragments
                if ((x >= 3 && x <= 5 && y >= 1 && y <= 6) || 
                    (y >= 6 && y <= 7 && x >= 4 && x <= 8))
                {
                    fragmentData[i] = Color.LightGray;
                }
                else
                {
                    fragmentData[i] = Color.Transparent;
                }
            }
            fragmentTexture.SetData(fragmentData);
            
            // Create power-up texture (diamond shape)
            powerUpTexture = new Texture2D(GraphicsDevice, 16, 16);
            Color[] powerUpData = new Color[16 * 16];
            for (int i = 0; i < powerUpData.Length; i++)
            {
                int x = i % 16;
                int y = i / 16;
                
                // Create a diamond shape
                int centerX = 8, centerY = 8;
                int distance = Math.Abs(x - centerX) + Math.Abs(y - centerY);
                if (distance <= 6)
                {
                    powerUpData[i] = Color.White;
                }
                else
                {
                    powerUpData[i] = Color.Transparent;
                }
            }
            powerUpTexture.SetData(powerUpData);
        }
        
        private void CreateAsteroids(int count)
        {
            for (int i = 0; i < count; i++)
            {
                Asteroid asteroid = new Asteroid(asteroidTexture, AsteroidSize.Large, random);
                
                // Random position (avoid spawning too close to player)
                Vector2 position;
                do
                {
                    position = new Vector2(
                        random.Next(0, GraphicsDevice.Viewport.Width),
                        random.Next(0, GraphicsDevice.Viewport.Height)
                    );
                } while (Vector2.Distance(position, player.Position) < 150);
                
                // Random velocity
                float angle = (float)(random.NextDouble() * MathHelper.TwoPi);
                float speed = 30f + (float)(random.NextDouble() * 50f);
                Vector2 velocity = new Vector2(
                    (float)Math.Cos(angle) * speed,
                    (float)Math.Sin(angle) * speed
                );
                
                asteroid.Initialize(position, velocity);
                asteroids.Add(asteroid);
            }
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || 
                keyboardState.IsKeyDown(Keys.Escape))
                Exit();
            
            // Handle main menu input
            if (currentGameState == GameState.MainMenu)
            {
                if (keyboardState.IsKeyDown(Keys.S) && !previousKeyboardState.IsKeyDown(Keys.S))
                {
                    StartNewGame();
                }
                else if (keyboardState.IsKeyDown(Keys.H) && !previousKeyboardState.IsKeyDown(Keys.H))
                {
                    currentGameState = GameState.HighScoreDisplay;
                }
                else if (keyboardState.IsKeyDown(Keys.Q) && !previousKeyboardState.IsKeyDown(Keys.Q))
                {
                    Exit();
                }
            }
            
            // Handle pause toggle
            if (keyboardState.IsKeyDown(Keys.P) && !previousKeyboardState.IsKeyDown(Keys.P))
            {
                if (currentGameState == GameState.Playing)
                {
                    currentGameState = GameState.Paused;
                }
                else if (currentGameState == GameState.Paused)
                {
                    currentGameState = GameState.Playing;
                }
            }
            
            if (currentGameState == GameState.Playing)
            {
                // Check if player just died to trigger death effects
                if (playerWasAlive && !player.IsAlive)
                {
                    deathEffects.TriggerDeathEffect(player.Position, player.Velocity);
                    playerWasAlive = false;
                }
                
                // Transition to game over after death effects finish
                if (!player.IsAlive && !deathEffects.IsDeathEffectActive)
                {
                    // Check if this is a high score
                    if (highScoreManager.IsHighScore(score))
                    {
                        currentGameState = GameState.HighScoreEntry;
                        isEnteringName = true;
                        playerNameInput = "";
                    }
                    else
                    {
                        currentGameState = GameState.GameOver;
                    }
                }
            }
            else if (currentGameState == GameState.GameOver)
            {
                // Handle Y/N input for restart
                if (keyboardState.IsKeyDown(Keys.Y) && !previousKeyboardState.IsKeyDown(Keys.Y))
                {
                    RestartGame();
                }
                else if (keyboardState.IsKeyDown(Keys.N) && !previousKeyboardState.IsKeyDown(Keys.N))
                {
                    Exit();
                }
                else if (keyboardState.IsKeyDown(Keys.H) && !previousKeyboardState.IsKeyDown(Keys.H))
                {
                    currentGameState = GameState.HighScoreDisplay;
                }
            }
            else if (currentGameState == GameState.HighScoreEntry)
            {
                HandleHighScoreEntry(keyboardState);
            }
            else if (currentGameState == GameState.HighScoreDisplay)
            {
                // Handle input to return to previous screen
                if (keyboardState.IsKeyDown(Keys.Enter) && !previousKeyboardState.IsKeyDown(Keys.Enter))
                {
                    // Return to main menu if we came from there, otherwise game over
                    currentGameState = GameState.MainMenu;
                }
            }
            
            // Update player
            player.Update(gameTime, GraphicsDevice);
            
            // Update death effects
            deathEffects.Update(gameTime);
            
            // Update life-loss effects and timer
            if (lifeLossEffectActive)
            {
                lifeLossEffects.Update(gameTime);
                lifeLossTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                
                if (lifeLossTimer <= 0)
                {
                    lifeLossEffectActive = false;
                }
            }
            
            
            // Handle shooting (only if player is alive and not in life-loss pause)
            if (player.IsAlive && !lifeLossEffectActive && keyboardState.IsKeyDown(Keys.Space) && !previousKeyboardState.IsKeyDown(Keys.Space))
            {
                FireBullet();
            }
            
            // Update bullets
            foreach (var bullet in bullets)
            {
                bullet.Update(gameTime, GraphicsDevice);
            }
            
            // Update asteroids
            foreach (var asteroid in asteroids.ToList())
            {
                asteroid.Update(gameTime, GraphicsDevice);
            }
            
            // Update power-ups
            foreach (var powerUp in powerUps)
            {
                powerUp.Update(gameTime, GraphicsDevice);
            }
            
            // Randomly spawn power-ups
            if (random.NextDouble() < 0.001) // 0.1% chance per frame
            {
                SpawnPowerUp();
            }
            
            // Check collisions
            CheckCollisions();
            
            // Clean up inactive objects
            asteroids.RemoveAll(a => !a.IsActive);
            
            // Spawn new asteroids if all are destroyed
            if (asteroids.Count == 0)
            {
                currentLevel++;
                // Increase large asteroids by 1 every 4 levels
                int additionalAsteroids = (currentLevel - 1) / 4;
                CreateAsteroids(INITIAL_ASTEROIDS + additionalAsteroids);
            }
            
            previousKeyboardState = keyboardState;
            base.Update(gameTime);
        }
        
        private void FireBullet()
        {
            Vector2 gunPosition = player.GetGunPosition();
            Vector2 gunDirection = player.GetGunDirection();
            
            switch (player.CurrentWeapon)
            {
                case WeaponType.Single:
                    FireSingleBullet(gunPosition, gunDirection);
                    break;
                case WeaponType.Double:
                    FireDoubleBullet(gunPosition, gunDirection);
                    break;
                case WeaponType.Triple:
                    FireTripleBullet(gunPosition, gunDirection);
                    break;
                case WeaponType.Spread:
                    FireSpreadBullet(gunPosition, gunDirection);
                    break;
            }
        }
        
        private void FireSingleBullet(Vector2 position, Vector2 direction)
        {
            Bullet availableBullet = bullets.FirstOrDefault(b => !b.IsActive);
            if (availableBullet != null)
            {
                availableBullet.Fire(position, direction);
                soundManager?.PlaySound("shoot", 0.4f);
            }
        }
        
        private void FireDoubleBullet(Vector2 position, Vector2 direction)
        {
            // Fire two bullets side by side
            Vector2 perpendicular = new Vector2(-direction.Y, direction.X) * 8f;
            
            var bullet1 = bullets.FirstOrDefault(b => !b.IsActive);
            if (bullet1 != null)
            {
                bullet1.Fire(position + perpendicular, direction);
                
                var bullet2 = bullets.Where(b => !b.IsActive && b != bullet1).FirstOrDefault();
                if (bullet2 != null)
                {
                    bullet2.Fire(position - perpendicular, direction);
                }
                
                // Play sound effect
                soundManager?.PlaySound("shoot", 0.4f);
            }
        }
        
        private void FireTripleBullet(Vector2 position, Vector2 direction)
        {
            // Fire three bullets: center, left, right
            var availableBullets = bullets.Where(b => !b.IsActive).Take(3).ToList();
            
            if (availableBullets.Count >= 1)
            {
                // Center bullet
                availableBullets[0].Fire(position, direction);
                
                // Play sound effect
                soundManager?.PlaySound("shoot", 0.4f);
            }
            
            if (availableBullets.Count >= 2)
            {
                // Left bullet (15 degrees)
                float leftAngle = (float)Math.Atan2(direction.Y, direction.X) - 0.26f;
                Vector2 leftDirection = new Vector2((float)Math.Cos(leftAngle), (float)Math.Sin(leftAngle));
                availableBullets[1].Fire(position, leftDirection);
            }
            
            if (availableBullets.Count >= 3)
            {
                // Right bullet (15 degrees)
                float rightAngle = (float)Math.Atan2(direction.Y, direction.X) + 0.26f;
                Vector2 rightDirection = new Vector2((float)Math.Cos(rightAngle), (float)Math.Sin(rightAngle));
                availableBullets[2].Fire(position, rightDirection);
            }
        }
        
        private void FireSpreadBullet(Vector2 position, Vector2 direction)
        {
            // Fire 5 bullets in a spread pattern
            var availableBullets = bullets.Where(b => !b.IsActive).Take(5).ToList();
            float baseAngle = (float)Math.Atan2(direction.Y, direction.X);
            
            if (availableBullets.Count > 0)
            {
                // Play sound effect
                soundManager?.PlaySound("shoot", 0.4f);
                
                for (int i = 0; i < availableBullets.Count; i++)
                {
                    float spreadAngle = baseAngle + (i - 2) * 0.3f; // 30 degree spread
                    Vector2 spreadDirection = new Vector2((float)Math.Cos(spreadAngle), (float)Math.Sin(spreadAngle));
                    availableBullets[i].Fire(position, spreadDirection);
                }
            }
        }
        
        private void SpawnPowerUp()
        {
            PowerUp availablePowerUp = powerUps.FirstOrDefault(p => !p.IsActive);
            if (availablePowerUp != null)
            {
                // Random position
                Vector2 position = new Vector2(
                    random.Next(50, GraphicsDevice.Viewport.Width - 50),
                    random.Next(50, GraphicsDevice.Viewport.Height - 50)
                );
                
                // Random power-up type (removed ExtraLife)
                PowerUpType[] types = { PowerUpType.DoubleShot, PowerUpType.TripleShot, PowerUpType.SpreadShot };
                PowerUpType randomType = types[random.Next(types.Length)];
                
                availablePowerUp.Initialize(position, randomType);
            }
        }
        
        private void HandleHighScoreEntry(KeyboardState keyboardState)
        {
            // Handle text input for player name
            Keys[] pressedKeys = keyboardState.GetPressedKeys();
            
            foreach (Keys key in pressedKeys)
            {
                if (!previousKeyboardState.IsKeyDown(key))
                {
                    if (key == Keys.Enter && playerNameInput.Length > 0)
                    {
                        // Submit high score
                        highScoreManager.AddHighScore(playerNameInput, score, currentLevel);
                        isEnteringName = false;
                        currentGameState = GameState.HighScoreDisplay;
                        return;
                    }
                    else if (key == Keys.Back && playerNameInput.Length > 0)
                    {
                        // Remove last character
                        playerNameInput = playerNameInput.Substring(0, playerNameInput.Length - 1);
                    }
                    else if (playerNameInput.Length < 10) // Limit name length
                    {
                        // Add character if it's a letter
                        if (key >= Keys.A && key <= Keys.Z)
                        {
                            playerNameInput += key.ToString();
                        }
                        else if (key >= Keys.D0 && key <= Keys.D9)
                        {
                            playerNameInput += ((int)(key - Keys.D0)).ToString();
                        }
                        else if (key == Keys.Space)
                        {
                            playerNameInput += " ";
                        }
                    }
                }
            }
        }
        
        private void StartNewGame()
        {
            // Reset game state
            currentGameState = GameState.Playing;
            score = 0;
            lastLifeScore = 0;
            currentLevel = 1;
            
            // Reset player
            player.Position = new Vector2(
                GraphicsDevice.Viewport.Width / 2f,
                GraphicsDevice.Viewport.Height / 2f
            );
            player.Velocity = Vector2.Zero;
            player.Rotation = 0f;
            player.IsAlive = true;
            player.Lives = 3;
            player.CurrentWeapon = WeaponType.Single;
            player.WeaponTimer = 0f;
            playerWasAlive = true;
            
            // Reset life loss effects
            lifeLossEffectActive = false;
            lifeLossTimer = 0f;
            
            // Clear all bullets
            foreach (var bullet in bullets)
            {
                bullet.Deactivate();
            }
            
            // Clear all power-ups
            foreach (var powerUp in powerUps)
            {
                powerUp.IsActive = false;
            }
            
            // Clear and recreate asteroids
            asteroids.Clear();
            CreateAsteroids(INITIAL_ASTEROIDS);
            
            // Reset death effects
            deathEffects = new DeathEffects(particleTexture, fragmentTexture, random);
            lifeLossEffects = new DeathEffects(particleTexture, fragmentTexture, random);
        }
        
        private void RestartGame()
        {
            StartNewGame(); // Use the same logic as starting a new game
        }
        
        private void CheckCollisions()
        {
            // Bullet vs Asteroid collisions
            foreach (var bullet in bullets.Where(b => b.IsActive))
            {
                foreach (var asteroid in asteroids.Where(a => a.IsActive).ToList())
                {
                    if (asteroid.CollidesWith(bullet.GetBounds()))
                    {
                        bullet.Deactivate();
                        
                        // Play asteroid hit sound
                        soundManager?.PlaySound("asteroidHit", 0.6f);
                        
                        // Split asteroid
                        Asteroid[] fragments = asteroid.Split();
                        asteroid.Destroy();
                        
                        // Add fragments
                        asteroids.AddRange(fragments);
                        
                        // Update score
                        switch (asteroid.Size)
                        {
                            case AsteroidSize.Large: score += 20; break;
                            case AsteroidSize.Medium: score += 50; break;
                            case AsteroidSize.Small: score += 100; break;
                        }
                        
                        // Check for free life every 10,000 points
                        if (score / 10000 > lastLifeScore / 10000)
                        {
                            player.Lives++;
                            lastLifeScore = score;
                        }
                        
                        break;
                    }
                }
            }
            
            // Player vs Asteroid collisions
            if (player.IsAlive && !lifeLossEffectActive)
            {
                foreach (var asteroid in asteroids.Where(a => a.IsActive))
                {
                    if (asteroid.CollidesWith(player.GetBounds()))
                    {
                        player.Lives--;
                        
                        // Play player hit sound
                        soundManager?.PlaySound("playerHit", 0.8f);
                        
                        // Trigger life-loss effect
                        lifeLossEffects.TriggerDeathEffect(player.Position, player.Velocity);
                        lifeLossEffectActive = true;
                        lifeLossTimer = LIFE_LOSS_PAUSE_DURATION;
                        
                        if (player.Lives <= 0)
                        {
                            player.IsAlive = false;
                            lifeLossEffectActive = false; // Let main death effect take over
                        }
                        else
                        {
                            // Respawn player at center but keep them paused
                            player.Position = new Vector2(
                                GraphicsDevice.Viewport.Width / 2f,
                                GraphicsDevice.Viewport.Height / 2f
                            );
                            player.Velocity = Vector2.Zero;
                        }
                        break;
                    }
                }
            }
            
            // Bullet vs Power-up collisions
            foreach (var bullet in bullets.Where(b => b.IsActive))
            {
                foreach (var powerUp in powerUps.Where(p => p.IsActive))
                {
                    if (powerUp.CollidesWith(bullet.GetBounds()))
                    {
                        bullet.Deactivate();
                        
                        // Apply power-up effect
                        switch (powerUp.Type)
                        {
                            case PowerUpType.DoubleShot:
                                player.CurrentWeapon = WeaponType.Double;
                                player.WeaponTimer = 10f;
                                break;
                            case PowerUpType.TripleShot:
                                player.CurrentWeapon = WeaponType.Triple;
                                player.WeaponTimer = 10f;
                                break;
                            case PowerUpType.SpreadShot:
                                player.CurrentWeapon = WeaponType.Spread;
                                player.WeaponTimer = 10f;
                                break;
                            case PowerUpType.ExtraLife:
                                player.Lives++;
                                break;
                        }
                        
                        // Play power-up sound
                        soundManager?.PlaySound("powerUp", 0.7f);
                        
                        powerUp.Collect();
                        break;
                    }
                }
            }
            
            // Player vs Power-up collisions (still allow direct contact collection)
            if (player.IsAlive)
            {
                foreach (var powerUp in powerUps.Where(p => p.IsActive))
                {
                    if (powerUp.CollidesWith(player.GetBounds()))
                    {
                        // Apply power-up effect
                        switch (powerUp.Type)
                        {
                            case PowerUpType.DoubleShot:
                                player.CurrentWeapon = WeaponType.Double;
                                player.WeaponTimer = 10f;
                                break;
                            case PowerUpType.TripleShot:
                                player.CurrentWeapon = WeaponType.Triple;
                                player.WeaponTimer = 10f;
                                break;
                            case PowerUpType.SpreadShot:
                                player.CurrentWeapon = WeaponType.Spread;
                                player.WeaponTimer = 10f;
                                break;
                            case PowerUpType.ExtraLife:
                                player.Lives++;
                                break;
                        }
                        
                        // Play power-up sound
                        soundManager?.PlaySound("powerUp", 0.7f);
                        
                        powerUp.Collect();
                        break;
                    }
                }
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            
            if (currentGameState == GameState.MainMenu)
            {
                // Draw main menu
                _spriteBatch.Begin();
                
                Vector2 screenCenter = new Vector2(GraphicsDevice.Viewport.Width / 2f, GraphicsDevice.Viewport.Height / 2f);
                
                string titleText = "DISASTEROIDS";
                Vector2 titleSize = textRenderer.MeasureText(titleText, 5f);
                Vector2 titlePos = screenCenter - titleSize / 2f - new Vector2(0, 200);
                textRenderer.DrawText(_spriteBatch, titleText, titlePos, Color.Cyan, 5f);
                
                string startText = "S - START GAME";
                Vector2 startSize = textRenderer.MeasureText(startText, 2.5f);
                Vector2 startPos = screenCenter - startSize / 2f - new Vector2(0, 50);
                textRenderer.DrawText(_spriteBatch, startText, startPos, Color.White, 2.5f);
                
                string highScoreText = "H - HIGH SCORES";
                Vector2 highScoreSize = textRenderer.MeasureText(highScoreText, 2.5f);
                Vector2 highScorePos = screenCenter - highScoreSize / 2f;
                textRenderer.DrawText(_spriteBatch, highScoreText, highScorePos, Color.White, 2.5f);
                
                string quitText = "Q - QUIT";
                Vector2 quitSize = textRenderer.MeasureText(quitText, 2.5f);
                Vector2 quitPos = screenCenter - quitSize / 2f + new Vector2(0, 50);
                textRenderer.DrawText(_spriteBatch, quitText, quitPos, Color.White, 2.5f);
                
                // Show controls
                string controlsText = "CONTROLS: WASD/ARROWS - MOVE, SPACE - SHOOT, V - QUICK TURN, P - PAUSE";
                Vector2 controlsSize = textRenderer.MeasureText(controlsText, 1.2f);
                Vector2 controlsPos = new Vector2((GraphicsDevice.Viewport.Width - controlsSize.X) / 2f, GraphicsDevice.Viewport.Height - 30);
                textRenderer.DrawText(_spriteBatch, controlsText, controlsPos, Color.Yellow, 1.2f);
                
                _spriteBatch.End();
            }
            else if (currentGameState == GameState.Playing)
            {
                // Apply screen shake by transforming the sprite batch
                Matrix transform = Matrix.CreateTranslation(deathEffects.ScreenShake.X, deathEffects.ScreenShake.Y, 0);
                _spriteBatch.Begin(transformMatrix: transform);
                
                // Draw player
                player.Draw(_spriteBatch);
                
                // Draw bullets
                foreach (var bullet in bullets)
                {
                    bullet.Draw(_spriteBatch);
                }
                
                // Draw asteroids
                foreach (var asteroid in asteroids)
                {
                    asteroid.Draw(_spriteBatch);
                }
                
                // Draw power-ups
                foreach (var powerUp in powerUps)
                {
                    powerUp.Draw(_spriteBatch);
                }
                
                // Draw death effects (particles and fragments)
                deathEffects.Draw(_spriteBatch);
                
                // Draw life-loss effects if active
                if (lifeLossEffectActive)
                {
                    lifeLossEffects.Draw(_spriteBatch);
                }
                
                _spriteBatch.End();
                
                // Draw flash overlay (without screen shake)
                _spriteBatch.Begin();
                deathEffects.DrawFlashOverlay(_spriteBatch, GraphicsDevice);
                
                // Draw life-loss flash overlay if active
                if (lifeLossEffectActive)
                {
                    lifeLossEffects.DrawFlashOverlay(_spriteBatch, GraphicsDevice);
                }
                _spriteBatch.End();
                
                // Draw UI (score, lives, weapon) without screen shake
                _spriteBatch.Begin();
                textRenderer.DrawText(_spriteBatch, $"SCORE: {score}", new Vector2(10, 10), Color.White, 2f);
                textRenderer.DrawText(_spriteBatch, $"LIVES: {player.Lives}", new Vector2(10, 40), Color.Green, 2f);
                
                string weaponText = $"WEAPON: {player.CurrentWeapon}";
                if (player.WeaponTimer > 0)
                {
                    weaponText += $" ({player.WeaponTimer:F1}s)";
                }
                textRenderer.DrawText(_spriteBatch, weaponText, new Vector2(10, 70), Color.Yellow, 2f);
                _spriteBatch.End();
            }
            else if (currentGameState == GameState.GameOver)
            {
                // Draw game over screen
                _spriteBatch.Begin();
                
                // Center the game over text
                Vector2 screenCenter = new Vector2(GraphicsDevice.Viewport.Width / 2f, GraphicsDevice.Viewport.Height / 2f);
                
                string gameOverText = "GAME OVER";
                Vector2 gameOverSize = textRenderer.MeasureText(gameOverText, 4f);
                Vector2 gameOverPos = screenCenter - gameOverSize / 2f - new Vector2(0, 100);
                textRenderer.DrawText(_spriteBatch, gameOverText, gameOverPos, Color.Red, 4f);
                
                string finalScoreText = $"FINAL SCORE: {score}";
                Vector2 finalScoreSize = textRenderer.MeasureText(finalScoreText, 2f);
                Vector2 finalScorePos = screenCenter - finalScoreSize / 2f - new Vector2(0, 20);
                textRenderer.DrawText(_spriteBatch, finalScoreText, finalScorePos, Color.White, 2f);
                
                string playAgainText = "PLAY AGAIN?";
                Vector2 playAgainSize = textRenderer.MeasureText(playAgainText, 2f);
                Vector2 playAgainPos = screenCenter - playAgainSize / 2f + new Vector2(0, 40);
                textRenderer.DrawText(_spriteBatch, playAgainText, playAgainPos, Color.Yellow, 2f);
                
                string yesNoText = "Y - YES    N - NO    H - HIGH SCORES";
                Vector2 yesNoSize = textRenderer.MeasureText(yesNoText, 2f);
                Vector2 yesNoPos = screenCenter - yesNoSize / 2f + new Vector2(0, 80);
                textRenderer.DrawText(_spriteBatch, yesNoText, yesNoPos, Color.Green, 2f);
                
                _spriteBatch.End();
            }
            else if (currentGameState == GameState.Paused)
            {
                // Draw paused game in background (dimmed)
                Matrix transform = Matrix.CreateTranslation(deathEffects.ScreenShake.X, deathEffects.ScreenShake.Y, 0);
                _spriteBatch.Begin(transformMatrix: transform);
                
                // Draw all game objects with reduced alpha
                Color dimmedColor = Color.White * 0.3f;
                
                // Draw player (dimmed)
                if (player.IsAlive)
                {
                    _spriteBatch.Draw(playerTexture, player.Position, null, dimmedColor, player.Rotation, 
                        new Vector2(playerTexture.Width / 2f, playerTexture.Height / 2f), 1f, SpriteEffects.None, 0f);
                }
                
                // Draw bullets (dimmed)
                foreach (var bullet in bullets.Where(b => b.IsActive))
                {
                    _spriteBatch.Draw(bulletTexture, bullet.Position, null, Color.Cyan * 0.3f, 0f, 
                        new Vector2(bulletTexture.Width / 2f, bulletTexture.Height / 2f), 1f, SpriteEffects.None, 0f);
                }
                
                // Draw asteroids (dimmed)
                foreach (var asteroid in asteroids.Where(a => a.IsActive))
                {
                    _spriteBatch.Draw(asteroidTexture, asteroid.Position, null, dimmedColor, asteroid.Rotation,
                        new Vector2(asteroidTexture.Width / 2f, asteroidTexture.Height / 2f), asteroid.GetRadius() / 20f, SpriteEffects.None, 0f);
                }
                
                // Draw power-ups (dimmed)
                foreach (var powerUp in powerUps.Where(p => p.IsActive))
                {
                    _spriteBatch.Draw(powerUpTexture, powerUp.Position, null, dimmedColor, powerUp.Rotation,
                        new Vector2(powerUpTexture.Width / 2f, powerUpTexture.Height / 2f), 0.8f, SpriteEffects.None, 0f);
                }
                
                _spriteBatch.End();
                
                // Draw pause overlay
                _spriteBatch.Begin();
                
                Vector2 screenCenter = new Vector2(GraphicsDevice.Viewport.Width / 2f, GraphicsDevice.Viewport.Height / 2f);
                
                string pausedText = "PAUSED";
                Vector2 pausedSize = textRenderer.MeasureText(pausedText, 4f);
                Vector2 pausedPos = screenCenter - pausedSize / 2f - new Vector2(0, 50);
                textRenderer.DrawText(_spriteBatch, pausedText, pausedPos, Color.Yellow, 4f);
                
                string resumeText = "PRESS P TO RESUME";
                Vector2 resumeSize = textRenderer.MeasureText(resumeText, 2f);
                Vector2 resumePos = screenCenter - resumeSize / 2f + new Vector2(0, 20);
                textRenderer.DrawText(_spriteBatch, resumeText, resumePos, Color.White, 2f);
                
                _spriteBatch.End();
            }
            else if (currentGameState == GameState.HighScoreEntry)
            {
                // Draw high score entry screen
                _spriteBatch.Begin();
                
                Vector2 screenCenter = new Vector2(GraphicsDevice.Viewport.Width / 2f, GraphicsDevice.Viewport.Height / 2f);
                
                string newHighScoreText = "NEW HIGH SCORE!";
                Vector2 newHighScoreSize = textRenderer.MeasureText(newHighScoreText, 4f);
                Vector2 newHighScorePos = screenCenter - newHighScoreSize / 2f - new Vector2(0, 120);
                textRenderer.DrawText(_spriteBatch, newHighScoreText, newHighScorePos, Color.Gold, 4f);
                
                string scoreText = $"SCORE: {score}";
                Vector2 scoreSize = textRenderer.MeasureText(scoreText, 2f);
                Vector2 scorePos = screenCenter - scoreSize / 2f - new Vector2(0, 60);
                textRenderer.DrawText(_spriteBatch, scoreText, scorePos, Color.White, 2f);
                
                string rankText = $"RANK: #{highScoreManager.GetHighScoreRank(score)}";
                Vector2 rankSize = textRenderer.MeasureText(rankText, 2f);
                Vector2 rankPos = screenCenter - rankSize / 2f - new Vector2(0, 20);
                textRenderer.DrawText(_spriteBatch, rankText, rankPos, Color.Cyan, 2f);
                
                string enterNameText = "ENTER YOUR NAME:";
                Vector2 enterNameSize = textRenderer.MeasureText(enterNameText, 2f);
                Vector2 enterNamePos = screenCenter - enterNameSize / 2f + new Vector2(0, 20);
                textRenderer.DrawText(_spriteBatch, enterNameText, enterNamePos, Color.Yellow, 2f);
                
                string nameText = playerNameInput + "_";
                Vector2 nameSize = textRenderer.MeasureText(nameText, 3f);
                Vector2 namePos = screenCenter - nameSize / 2f + new Vector2(0, 60);
                textRenderer.DrawText(_spriteBatch, nameText, namePos, Color.White, 3f);
                
                string instructionText = "PRESS ENTER TO SUBMIT";
                Vector2 instructionSize = textRenderer.MeasureText(instructionText, 1.5f);
                Vector2 instructionPos = screenCenter - instructionSize / 2f + new Vector2(0, 120);
                textRenderer.DrawText(_spriteBatch, instructionText, instructionPos, Color.Green, 1.5f);
                
                _spriteBatch.End();
            }
            else if (currentGameState == GameState.HighScoreDisplay)
            {
                // Draw high score display screen
                _spriteBatch.Begin();
                
                Vector2 screenCenter = new Vector2(GraphicsDevice.Viewport.Width / 2f, GraphicsDevice.Viewport.Height / 2f);
                
                string titleText = "HIGH SCORES";
                Vector2 titleSize = textRenderer.MeasureText(titleText, 4f);
                Vector2 titlePos = screenCenter - titleSize / 2f - new Vector2(0, 300);
                textRenderer.DrawText(_spriteBatch, titleText, titlePos, Color.Gold, 4f);
                
                var highScores = highScoreManager.GetHighScores();
                for (int i = 0; i < highScores.Count; i++)
                {
                    var entry = highScores[i];
                    string rankText = $"{i + 1:D2}.";
                    string nameText = entry.PlayerName.PadRight(10);
                    string scoreText = entry.Score.ToString().PadLeft(8);
                    string levelText = $"LV{entry.Level}";
                    
                    string fullText = $"{rankText} {nameText} {scoreText} {levelText}";
                    
                    Vector2 entryPos = screenCenter - new Vector2(200, 200 - (i * 30));
                    Color entryColor = i == 0 ? Color.Gold : (i < 3 ? Color.Silver : Color.White);
                    
                    textRenderer.DrawText(_spriteBatch, fullText, entryPos, entryColor, 1.8f);
                }
                
                string returnText = "PRESS ENTER TO RETURN";
                Vector2 returnSize = textRenderer.MeasureText(returnText, 2f);
                Vector2 returnPos = screenCenter - returnSize / 2f + new Vector2(0, 250);
                textRenderer.DrawText(_spriteBatch, returnText, returnPos, Color.Green, 2f);
                
                _spriteBatch.End();
            }
            
            base.Draw(gameTime);
        }
    }
}
