using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Disasteroids
{
    public class DeathEffects
    {
        private List<Particle> particles;
        private List<PlayerFragment> fragments;
        private Random random;
        private Texture2D particleTexture;
        private Texture2D fragmentTexture;
        
        // Screen shake effect
        public Vector2 ScreenShake { get; private set; }
        private float shakeIntensity;
        private float shakeDuration;
        private float shakeTimer;
        
        // Flash effect
        public Color FlashColor { get; private set; }
        private float flashIntensity;
        private float flashTimer;
        
        // Death state
        public bool IsDeathEffectActive { get; private set; }
        private float deathEffectTimer;
        private const float DEATH_EFFECT_DURATION = 3.0f;
        
        public DeathEffects(Texture2D particleTexture, Texture2D fragmentTexture, Random randomGenerator)
        {
            this.particleTexture = particleTexture;
            this.fragmentTexture = fragmentTexture;
            this.random = randomGenerator;
            
            particles = new List<Particle>();
            fragments = new List<PlayerFragment>();
            
            // Create particle pool
            for (int i = 0; i < 50; i++)
            {
                particles.Add(new Particle(particleTexture));
            }
            
            // Create fragment pool
            for (int i = 0; i < 8; i++)
            {
                fragments.Add(new PlayerFragment(fragmentTexture));
            }
            
            Reset();
        }
        
        public void TriggerDeathEffect(Vector2 playerPosition, Vector2 playerVelocity)
        {
            IsDeathEffectActive = true;
            deathEffectTimer = DEATH_EFFECT_DURATION;
            
            // Create explosion particles
            CreateExplosionParticles(playerPosition, playerVelocity);
            
            // Create ship fragments
            CreateShipFragments(playerPosition, playerVelocity);
            
            // Start screen shake
            shakeIntensity = 15f;
            shakeDuration = 0.8f;
            shakeTimer = shakeDuration;
            
            // Start flash effect
            flashIntensity = 0.8f;
            flashTimer = 0.3f;
        }
        
        private void CreateExplosionParticles(Vector2 position, Vector2 baseVelocity)
        {
            int particleCount = 30;
            var availableParticles = particles.Where(p => !p.IsActive).Take(particleCount);
            
            foreach (var particle in availableParticles)
            {
                // Random direction
                float angle = (float)(random.NextDouble() * MathHelper.TwoPi);
                float speed = 50f + (float)(random.NextDouble() * 200f);
                
                Vector2 velocity = new Vector2(
                    (float)Math.Cos(angle) * speed,
                    (float)Math.Sin(angle) * speed
                ) + baseVelocity * 0.5f; // Add some of the player's momentum
                
                // Random color (fire colors)
                Color[] fireColors = { Color.Red, Color.Orange, Color.Yellow, Color.White };
                Color color = fireColors[random.Next(fireColors.Length)];
                
                float life = 0.5f + (float)(random.NextDouble() * 1.5f);
                float size = 0.3f + (float)(random.NextDouble() * 0.7f);
                float rotationSpeed = (float)(random.NextDouble() * 10f - 5f);
                
                particle.Initialize(position, velocity, color, life, size, rotationSpeed);
            }
        }
        
        private void CreateShipFragments(Vector2 position, Vector2 baseVelocity)
        {
            int fragmentCount = Math.Min(6, fragments.Count);
            var availableFragments = fragments.Where(f => !f.IsActive).Take(fragmentCount);
            
            foreach (var fragment in availableFragments)
            {
                // Random direction but less spread than particles
                float angle = (float)(random.NextDouble() * MathHelper.TwoPi);
                float speed = 30f + (float)(random.NextDouble() * 100f);
                
                Vector2 velocity = new Vector2(
                    (float)Math.Cos(angle) * speed,
                    (float)Math.Sin(angle) * speed
                ) + baseVelocity * 0.7f; // Inherit more of the player's momentum
                
                float life = 2f + (float)(random.NextDouble() * 2f);
                float rotationSpeed = (float)(random.NextDouble() * 8f - 4f);
                
                fragment.Initialize(position, velocity, life, rotationSpeed);
            }
        }
        
        public void Update(GameTime gameTime)
        {
            if (!IsDeathEffectActive) return;
            
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            // Update death effect timer
            deathEffectTimer -= deltaTime;
            if (deathEffectTimer <= 0)
            {
                Reset();
                return;
            }
            
            // Update particles
            foreach (var particle in particles)
            {
                particle.Update(gameTime);
            }
            
            // Update fragments
            foreach (var fragment in fragments)
            {
                fragment.Update(gameTime);
            }
            
            // Update screen shake
            if (shakeTimer > 0)
            {
                shakeTimer -= deltaTime;
                float shakeAmount = shakeIntensity * (shakeTimer / shakeDuration);
                
                ScreenShake = new Vector2(
                    (float)(random.NextDouble() * 2 - 1) * shakeAmount,
                    (float)(random.NextDouble() * 2 - 1) * shakeAmount
                );
            }
            else
            {
                ScreenShake = Vector2.Zero;
            }
            
            // Update flash effect
            if (flashTimer > 0)
            {
                flashTimer -= deltaTime;
                float alpha = flashIntensity * (flashTimer / 0.3f);
                FlashColor = Color.Red * alpha;
            }
            else
            {
                FlashColor = Color.Transparent;
            }
        }
        
        public void Draw(SpriteBatch spriteBatch)
        {
            if (!IsDeathEffectActive) return;
            
            // Draw particles
            foreach (var particle in particles)
            {
                particle.Draw(spriteBatch);
            }
            
            // Draw fragments
            foreach (var fragment in fragments)
            {
                fragment.Draw(spriteBatch);
            }
        }
        
        public void DrawFlashOverlay(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            if (FlashColor.A > 0)
            {
                // Create a full-screen rectangle for the flash effect
                Texture2D whitePixel = new Texture2D(graphicsDevice, 1, 1);
                whitePixel.SetData(new[] { Color.White });
                
                Rectangle screenRect = new Rectangle(0, 0, 
                    graphicsDevice.Viewport.Width, 
                    graphicsDevice.Viewport.Height);
                
                spriteBatch.Draw(whitePixel, screenRect, FlashColor);
            }
        }
        
        private void Reset()
        {
            IsDeathEffectActive = false;
            deathEffectTimer = 0f;
            ScreenShake = Vector2.Zero;
            FlashColor = Color.Transparent;
            shakeTimer = 0f;
            flashTimer = 0f;
            
            // Deactivate all particles and fragments
            foreach (var particle in particles)
            {
                particle.IsActive = false;
            }
            
            foreach (var fragment in fragments)
            {
                fragment.IsActive = false;
            }
        }
    }
    
    public class PlayerFragment
    {
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public float Rotation { get; set; }
        public float RotationSpeed { get; set; }
        public float Life { get; set; }
        public float MaxLife { get; set; }
        public bool IsActive { get; set; }
        
        private Texture2D texture;
        private Vector2 origin;
        
        public PlayerFragment(Texture2D fragmentTexture)
        {
            texture = fragmentTexture;
            origin = new Vector2(texture.Width / 2f, texture.Height / 2f);
            IsActive = false;
        }
        
        public void Initialize(Vector2 position, Vector2 velocity, float life, float rotationSpeed)
        {
            Position = position;
            Velocity = velocity;
            Life = life;
            MaxLife = life;
            RotationSpeed = rotationSpeed;
            Rotation = 0f;
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
            
            // Apply drag
            Velocity *= 0.95f;
            
            // Update life
            Life -= deltaTime;
            if (Life <= 0)
            {
                IsActive = false;
            }
        }
        
        public void Draw(SpriteBatch spriteBatch)
        {
            if (!IsActive) return;
            
            float alpha = Life / MaxLife;
            Color color = Color.White * alpha;
            
            spriteBatch.Draw(texture, Position, null, color, Rotation, origin, 0.5f, SpriteEffects.None, 0f);
        }
    }
}
