using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Disasteroids
{
    public class Particle
    {
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public Color Color { get; set; }
        public float Life { get; set; }
        public float MaxLife { get; set; }
        public float Size { get; set; }
        public float Rotation { get; set; }
        public float RotationSpeed { get; set; }
        public bool IsActive { get; set; }
        
        private Texture2D texture;
        
        public Particle(Texture2D particleTexture)
        {
            texture = particleTexture;
            IsActive = false;
        }
        
        public void Initialize(Vector2 position, Vector2 velocity, Color color, float life, float size, float rotationSpeed)
        {
            Position = position;
            Velocity = velocity;
            Color = color;
            Life = life;
            MaxLife = life;
            Size = size;
            Rotation = 0f;
            RotationSpeed = rotationSpeed;
            IsActive = true;
        }
        
        public void Update(GameTime gameTime)
        {
            if (!IsActive) return;
            
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            // Update position
            Position += Velocity * deltaTime;
            
            // Update rotation
            Rotation += RotationSpeed * deltaTime;
            
            // Apply gravity/drag
            Velocity *= 0.98f;
            
            // Update life
            Life -= deltaTime;
            if (Life <= 0)
            {
                IsActive = false;
                return;
            }
            
            // Fade out over time
            float alpha = Life / MaxLife;
            Color = Color.Lerp(Color.Transparent, Color, alpha);
        }
        
        public void Draw(SpriteBatch spriteBatch)
        {
            if (!IsActive) return;
            
            float alpha = Life / MaxLife;
            Vector2 origin = new Vector2(texture.Width / 2f, texture.Height / 2f);
            
            spriteBatch.Draw(
                texture, 
                Position, 
                null, 
                Color * alpha, 
                Rotation, 
                origin, 
                Size, 
                SpriteEffects.None, 
                0f
            );
        }
    }
}
