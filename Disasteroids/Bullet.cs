using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Disasteroids
{
    /// <summary>
    /// Bullet class demonstrates the Object Pooling pattern
    /// Instead of creating/destroying bullets constantly (expensive),
    /// we reuse a fixed pool of bullet objects (memory efficient)
    /// 
    /// Key concepts demonstrated:
    /// - Object lifecycle management (Active/Inactive states)
    /// - Performance optimization through object reuse
    /// - Composition over inheritance (has-a texture, not is-a texture)
    /// </summary>
    public class Bullet
    {
        // Properties for external access - demonstrates encapsulation
        public Vector2 Position { get; set; }      // World position
        public Vector2 Velocity { get; set; }      // Movement vector (pixels/second)
        public bool IsActive { get; set; }         // Pool management flag
        public float TimeToLive { get; set; }      // Automatic cleanup timer
        
        // Constants - better than magic numbers scattered in code
        private const float BULLET_SPEED = 400f;   // Pixels per second
        private const float MAX_LIFETIME = 2f;     // Seconds before auto-cleanup
        
        // Private fields - internal implementation details
        private Texture2D texture;                 // Visual representation
        private Vector2 origin;                    // Center point for drawing
        
        public Bullet(Texture2D bulletTexture)
        {
            texture = bulletTexture;
            IsActive = false;
            origin = new Vector2(texture.Width / 2f, texture.Height / 2f);
        }
        
        public void Fire(Vector2 startPosition, Vector2 direction)
        {
            Position = startPosition;
            Velocity = direction * BULLET_SPEED;
            IsActive = true;
            TimeToLive = MAX_LIFETIME;
        }
        
        public void Update(GameTime gameTime, GraphicsDevice graphicsDevice)
        {
            if (!IsActive) return;
            
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            // Update position
            Position += Velocity * deltaTime;
            
            // Update lifetime
            TimeToLive -= deltaTime;
            if (TimeToLive <= 0)
            {
                IsActive = false;
                return;
            }
            
            // Screen wrapping
            if (Position.X < 0) Position = new Vector2(graphicsDevice.Viewport.Width, Position.Y);
            if (Position.X > graphicsDevice.Viewport.Width) Position = new Vector2(0, Position.Y);
            if (Position.Y < 0) Position = new Vector2(Position.X, graphicsDevice.Viewport.Height);
            if (Position.Y > graphicsDevice.Viewport.Height) Position = new Vector2(Position.X, 0);
        }
        
        public void Draw(SpriteBatch spriteBatch)
        {
            if (!IsActive) return;
            
            spriteBatch.Draw(texture, Position, null, Color.Yellow, 0f, origin, 1f, SpriteEffects.None, 0f);
        }
        
        public Rectangle GetBounds()
        {
            if (!IsActive) return Rectangle.Empty;
            
            return new Rectangle(
                (int)(Position.X - origin.X),
                (int)(Position.Y - origin.Y),
                texture.Width,
                texture.Height
            );
        }
        
        public void Deactivate()
        {
            IsActive = false;
        }
    }
}
