using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Disasteroids
{
    public enum AsteroidSize
    {
        Large,
        Medium,
        Small
    }
    
    public class Asteroid
    {
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public float Rotation { get; set; }
        public float RotationSpeed { get; set; }
        public bool IsActive { get; set; }
        public AsteroidSize Size { get; set; }
        
        private Texture2D texture;
        private Vector2 origin;
        private float scale;
        private Random random;
        
        public Asteroid(Texture2D asteroidTexture, AsteroidSize size, Random randomGenerator)
        {
            texture = asteroidTexture;
            Size = size;
            random = randomGenerator;
            IsActive = true;
            
            // Set scale based on size (50% larger than original)
            switch (size)
            {
                case AsteroidSize.Large:
                    scale = 1.5f;
                    break;
                case AsteroidSize.Medium:
                    scale = 0.9f;
                    break;
                case AsteroidSize.Small:
                    scale = 0.45f;
                    break;
            }
            
            origin = new Vector2(texture.Width / 2f, texture.Height / 2f);
            
            // Random rotation speed
            RotationSpeed = (float)(random.NextDouble() * 4 - 2); // -2 to 2 radians per second
        }
        
        public void Initialize(Vector2 startPosition, Vector2 velocity)
        {
            Position = startPosition;
            Velocity = velocity;
            Rotation = (float)(random.NextDouble() * MathHelper.TwoPi);
        }
        
        public void Update(GameTime gameTime, GraphicsDevice graphicsDevice)
        {
            if (!IsActive) return;
            
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            // Update rotation
            Rotation += RotationSpeed * deltaTime;
            
            // Update position
            Position += Velocity * deltaTime;
            
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
            
            spriteBatch.Draw(texture, Position, null, Color.White, Rotation, origin, scale, SpriteEffects.None, 0f);
        }
        
        public Rectangle GetBounds()
        {
            if (!IsActive) return Rectangle.Empty;
            
            float scaledWidth = texture.Width * scale;
            float scaledHeight = texture.Height * scale;
            
            return new Rectangle(
                (int)(Position.X - scaledWidth / 2),
                (int)(Position.Y - scaledHeight / 2),
                (int)scaledWidth,
                (int)scaledHeight
            );
        }
        
        public float GetRadius()
        {
            return (texture.Width * scale) / 2f;
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
        
        public Asteroid[] Split()
        {
            if (Size == AsteroidSize.Small)
                return new Asteroid[0]; // Small asteroids don't split
            
            AsteroidSize newSize = Size == AsteroidSize.Large ? AsteroidSize.Medium : AsteroidSize.Small;
            Asteroid[] fragments = new Asteroid[2];
            
            for (int i = 0; i < 2; i++)
            {
                fragments[i] = new Asteroid(texture, newSize, random);
                
                // Random velocity for fragments
                float angle = (float)(random.NextDouble() * MathHelper.TwoPi);
                float speed = 50f + (float)(random.NextDouble() * 100f);
                Vector2 velocity = new Vector2(
                    (float)Math.Cos(angle) * speed,
                    (float)Math.Sin(angle) * speed
                );
                
                fragments[i].Initialize(Position, velocity);
            }
            
            return fragments;
        }
        
        public void Destroy()
        {
            IsActive = false;
        }
    }
}
