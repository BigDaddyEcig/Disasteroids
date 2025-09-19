using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Disasteroids
{
    /// <summary>
    /// Enum demonstrating type safety - restricts weapon types to valid values
    /// This is better than using magic numbers or strings
    /// </summary>
    public enum WeaponType
    {
        Single,   // Default weapon
        Double,   // Fires two bullets side by side
        Triple,   // Fires three bullets in a spread
        Spread    // Fires five bullets in a wide spread
    }
    
    /// <summary>
    /// Player class demonstrates key OOP concepts:
    /// - Encapsulation: Private fields with public properties
    /// - Composition: Player "has-a" texture, position, etc.
    /// - Single Responsibility: Handles only player-related logic
    /// </summary>
    public class Player
    {
        // Properties demonstrate encapsulation - controlled access to internal state
        // Auto-properties provide get/set without explicit backing fields
        public Vector2 Position { get; set; }      // Current world position
        public Vector2 Velocity { get; set; }      // Current movement vector
        public float Rotation { get; set; }        // Current rotation in radians
        public bool IsAlive { get; set; }          // State flag for game logic
        public WeaponType CurrentWeapon { get; set; } // Current weapon type (enum usage)
        public float WeaponTimer { get; set; }     // Time remaining for special weapons
        public int Lives { get; set; }             // Remaining lives
        
        // Constants demonstrate good practice - named values instead of magic numbers
        private const float THRUST_POWER = 200f;   // Acceleration force
        private const float ROTATION_SPEED = 5f;   // Radians per second
        private const float MAX_SPEED = 300f;      // Maximum velocity magnitude
        private const float DRAG = 0.98f;          // Velocity reduction factor (0-1)
        
        // Private fields demonstrate encapsulation - internal implementation details
        private Texture2D texture;                 // Visual representation
        private Vector2 origin;                    // Center point for rotation
        private KeyboardState previousKeyboardState; // For input edge detection
        
        public Player(Texture2D playerTexture, Vector2 startPosition)
        {
            texture = playerTexture;
            Position = startPosition;
            Velocity = Vector2.Zero;
            Rotation = 0f;
            IsAlive = true;
            CurrentWeapon = WeaponType.Single;
            WeaponTimer = 0f;
            Lives = 3;
            origin = new Vector2(texture.Width / 2f, texture.Height / 2f);
        }
        
        public void Update(GameTime gameTime, GraphicsDevice graphicsDevice)
        {
            if (!IsAlive) return;
            
            KeyboardState keyboardState = Keyboard.GetState();
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            // Rotation
            if (keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.A))
                Rotation -= ROTATION_SPEED * deltaTime;
            if (keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.D))
                Rotation += ROTATION_SPEED * deltaTime;
            
            // Quick 180-degree turn with V key
            if (keyboardState.IsKeyDown(Keys.V) && !previousKeyboardState.IsKeyDown(Keys.V))
            {
                Rotation += MathHelper.Pi; // Add 180 degrees (π radians)
            }
            
            // Thrust
            if (keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.W))
            {
                Vector2 thrustDirection = new Vector2(
                    (float)Math.Cos(Rotation - MathHelper.PiOver2),
                    (float)Math.Sin(Rotation - MathHelper.PiOver2)
                );
                Velocity += thrustDirection * THRUST_POWER * deltaTime;
            }
            
            // Apply drag
            Velocity *= DRAG;
            
            // Limit max speed
            if (Velocity.Length() > MAX_SPEED)
            {
                Velocity.Normalize();
                Velocity *= MAX_SPEED;
            }
            
            // Update position
            Position += Velocity * deltaTime;
            
            // Screen wrapping
            if (Position.X < 0) Position = new Vector2(graphicsDevice.Viewport.Width, Position.Y);
            if (Position.X > graphicsDevice.Viewport.Width) Position = new Vector2(0, Position.Y);
            if (Position.Y < 0) Position = new Vector2(Position.X, graphicsDevice.Viewport.Height);
            if (Position.Y > graphicsDevice.Viewport.Height) Position = new Vector2(Position.X, 0);
            
            // Update weapon timer
            if (WeaponTimer > 0)
            {
                WeaponTimer -= deltaTime;
                if (WeaponTimer <= 0)
                {
                    CurrentWeapon = WeaponType.Single;
                }
            }
            
            // Update previous keyboard state for next frame
            previousKeyboardState = keyboardState;
        }
        
        public void Draw(SpriteBatch spriteBatch)
        {
            if (!IsAlive) return;
            
            spriteBatch.Draw(texture, Position, null, Color.White, Rotation, origin, 1f, SpriteEffects.None, 0f);
        }
        
        public Rectangle GetBounds()
        {
            return new Rectangle(
                (int)(Position.X - origin.X),
                (int)(Position.Y - origin.Y),
                texture.Width,
                texture.Height
            );
        }
        
        public Vector2 GetGunPosition()
        {
            Vector2 gunOffset = new Vector2(
                (float)Math.Cos(Rotation - MathHelper.PiOver2),
                (float)Math.Sin(Rotation - MathHelper.PiOver2)
            ) * (texture.Height / 2f);
            
            return Position + gunOffset;
        }
        
        public Vector2 GetGunDirection()
        {
            return new Vector2(
                (float)Math.Cos(Rotation - MathHelper.PiOver2),
                (float)Math.Sin(Rotation - MathHelper.PiOver2)
            );
        }
    }
}
