using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Disasteroids
{
    public enum PowerUpType
    {
        DoubleShot,
        TripleShot,
        SpreadShot,
        ExtraLife
    }
    
    public class PowerUp
    {
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public float Rotation { get; set; }
        public float RotationSpeed { get; set; }
        public bool IsActive { get; set; }
        public PowerUpType Type { get; set; }
        public float Life { get; set; }
        public float MaxLife { get; set; }
        
        private Texture2D texture;
        private Vector2 origin;
        private Color color;
        private Random random;
        
        public PowerUp(Texture2D powerUpTexture, Random randomGenerator)
        {
            texture = powerUpTexture;
            random = randomGenerator;
            origin = new Vector2(texture.Width / 2f, texture.Height / 2f);
            IsActive = false;
        }
        
        public void Initialize(Vector2 position, PowerUpType type)
        {
            Position = position;
            Type = type;
            IsActive = true;
            Life = 20f; // Power-up lasts 20 seconds
            MaxLife = Life;
            
            // Random velocity like asteroids
            float angle = (float)(random.NextDouble() * MathHelper.TwoPi);
            float speed = 40f + (float)(random.NextDouble() * 60f); // Faster like asteroids
            Velocity = new Vector2(
                (float)Math.Cos(angle) * speed,
                (float)Math.Sin(angle) * speed
            );
            
            // Random rotation
            Rotation = (float)(random.NextDouble() * MathHelper.TwoPi);
            RotationSpeed = (float)(random.NextDouble() * 6f - 3f); // Faster rotation
            
            // Set color based on type
            switch (type)
            {
                case PowerUpType.DoubleShot:
                    color = Color.Yellow;
                    break;
                case PowerUpType.TripleShot:
                    color = Color.Orange;
                    break;
                case PowerUpType.SpreadShot:
                    color = Color.Red;
                    break;
                case PowerUpType.ExtraLife:
                    color = Color.Green;
                    break;
            }
        }
        
        public void Update(GameTime gameTime, GraphicsDevice graphicsDevice)
        {
            if (!IsActive) return;
            
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            // Update position
            Position += Velocity * deltaTime;
            
            // Update rotation
            Rotation += RotationSpeed * deltaTime;
            
            // Keep powerups moving continuously (no drag applied)
            
            // Update life
            Life -= deltaTime;
            if (Life <= 0)
            {
                IsActive = false;
                return;
            }
            
            // Screen wrapping
            float radius = GetRadius();
            if (Position.X < -radius) Position = new Vector2(graphicsDevice.Viewport.Width + radius, Position.Y);
            if (Position.X > graphicsDevice.Viewport.Width + radius) Position = new Vector2(-radius, Position.Y);
            if (Position.Y < -radius) Position = new Vector2(Position.X, graphicsDevice.Viewport.Height + radius);
            if (Position.Y > graphicsDevice.Viewport.Height + radius) Position = new Vector2(Position.X, -radius);
        }
        
        public void Draw(SpriteBatch spriteBatch)
        {
            if (!IsActive) return;
            
            // Pulse effect based on remaining life
            float alpha = 0.7f + 0.3f * (float)Math.Sin(Life * 8f);
            if (Life < 3f) // Flash when about to expire
            {
                alpha *= (Life < 1f) ? 0.5f + 0.5f * (float)Math.Sin(Life * 20f) : 1f;
            }
            
            Color drawColor = color * alpha;
            
            spriteBatch.Draw(texture, Position, null, drawColor, Rotation, origin, 0.8f, SpriteEffects.None, 0f);
        }
        
        public Rectangle GetBounds()
        {
            if (!IsActive) return Rectangle.Empty;
            
            float scaledWidth = texture.Width * 0.8f;
            float scaledHeight = texture.Height * 0.8f;
            
            return new Rectangle(
                (int)(Position.X - scaledWidth / 2),
                (int)(Position.Y - scaledHeight / 2),
                (int)scaledWidth,
                (int)scaledHeight
            );
        }
        
        public float GetRadius()
        {
            return (texture.Width * 0.8f) / 2f;
        }
        
        public bool CollidesWith(Rectangle otherBounds)
        {
            if (!IsActive) return false;
            
            // Simple circular collision detection
            Vector2 otherCenter = new Vector2(
                otherBounds.X + otherBounds.Width / 2f,
                otherBounds.Y + otherBounds.Height / 2f
            );
            
            float distance = Vector2.Distance(Position, otherCenter);
            float combinedRadius = GetRadius() + Math.Min(otherBounds.Width, otherBounds.Height) / 2f;
            
            return distance < combinedRadius;
        }
        
        public void Collect()
        {
            IsActive = false;
        }
    }
}
