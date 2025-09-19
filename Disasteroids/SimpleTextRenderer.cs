using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Disasteroids
{
    public class SimpleTextRenderer
    {
        private Texture2D fontTexture;
        private Dictionary<char, Rectangle> charMap;
        private int charWidth = 10;
        private int charHeight = 14;
        
        public SimpleTextRenderer(GraphicsDevice graphicsDevice)
        {
            CreateFontTexture(graphicsDevice);
            CreateCharacterMap();
        }
        
        private void CreateFontTexture(GraphicsDevice graphicsDevice)
        {
            // Create a simple bitmap font texture with larger, thicker characters
            int textureWidth = 16 * charWidth; // 16 characters per row
            int textureHeight = 6 * charHeight; // 6 rows
            
            fontTexture = new Texture2D(graphicsDevice, textureWidth, textureHeight);
            Color[] fontData = new Color[textureWidth * textureHeight];
            
            // Initialize with transparent
            for (int i = 0; i < fontData.Length; i++)
            {
                fontData[i] = Color.Transparent;
            }
            
            // Draw simple characters (numbers, letters, and basic symbols)
            DrawCharacter(fontData, textureWidth, '0', 0, 0);
            DrawCharacter(fontData, textureWidth, '1', 1, 0);
            DrawCharacter(fontData, textureWidth, '2', 2, 0);
            DrawCharacter(fontData, textureWidth, '3', 3, 0);
            DrawCharacter(fontData, textureWidth, '4', 4, 0);
            DrawCharacter(fontData, textureWidth, '5', 5, 0);
            DrawCharacter(fontData, textureWidth, '6', 6, 0);
            DrawCharacter(fontData, textureWidth, '7', 7, 0);
            DrawCharacter(fontData, textureWidth, '8', 8, 0);
            DrawCharacter(fontData, textureWidth, '9', 9, 0);
            
            DrawCharacter(fontData, textureWidth, 'A', 10, 0);
            DrawCharacter(fontData, textureWidth, 'B', 11, 0);
            DrawCharacter(fontData, textureWidth, 'C', 12, 0);
            DrawCharacter(fontData, textureWidth, 'D', 13, 0);
            DrawCharacter(fontData, textureWidth, 'E', 14, 0);
            DrawCharacter(fontData, textureWidth, 'F', 15, 0);
            
            DrawCharacter(fontData, textureWidth, 'G', 0, 1);
            DrawCharacter(fontData, textureWidth, 'H', 1, 1);
            DrawCharacter(fontData, textureWidth, 'I', 2, 1);
            DrawCharacter(fontData, textureWidth, 'J', 3, 1);
            DrawCharacter(fontData, textureWidth, 'K', 4, 1);
            DrawCharacter(fontData, textureWidth, 'L', 5, 1);
            DrawCharacter(fontData, textureWidth, 'M', 6, 1);
            DrawCharacter(fontData, textureWidth, 'N', 7, 1);
            DrawCharacter(fontData, textureWidth, 'O', 8, 1);
            DrawCharacter(fontData, textureWidth, 'P', 9, 1);
            DrawCharacter(fontData, textureWidth, 'Q', 10, 1);
            DrawCharacter(fontData, textureWidth, 'R', 11, 1);
            DrawCharacter(fontData, textureWidth, 'S', 12, 1);
            DrawCharacter(fontData, textureWidth, 'T', 13, 1);
            DrawCharacter(fontData, textureWidth, 'U', 14, 1);
            DrawCharacter(fontData, textureWidth, 'V', 15, 1);
            
            DrawCharacter(fontData, textureWidth, 'W', 0, 2);
            DrawCharacter(fontData, textureWidth, 'X', 1, 2);
            DrawCharacter(fontData, textureWidth, 'Y', 2, 2);
            DrawCharacter(fontData, textureWidth, 'Z', 3, 2);
            DrawCharacter(fontData, textureWidth, ' ', 4, 2);
            DrawCharacter(fontData, textureWidth, ':', 5, 2);
            DrawCharacter(fontData, textureWidth, '?', 6, 2);
            DrawCharacter(fontData, textureWidth, '!', 7, 2);
            DrawCharacter(fontData, textureWidth, '-', 8, 2);
            DrawCharacter(fontData, textureWidth, '/', 9, 2);
            DrawCharacter(fontData, textureWidth, '(', 10, 2);
            DrawCharacter(fontData, textureWidth, ')', 11, 2);
            DrawCharacter(fontData, textureWidth, '.', 12, 2);
            DrawCharacter(fontData, textureWidth, '\\', 13, 2);
            DrawCharacter(fontData, textureWidth, '#', 14, 2);
            DrawCharacter(fontData, textureWidth, '_', 15, 2);
            
            fontTexture.SetData(fontData);
        }
        
        private void DrawCharacter(Color[] fontData, int textureWidth, char character, int gridX, int gridY)
        {
            int startX = gridX * charWidth;
            int startY = gridY * charHeight;
            
            // Simple pixel patterns for each character
            bool[,] pattern = GetCharacterPattern(character);
            
            for (int y = 0; y < charHeight; y++)
            {
                for (int x = 0; x < charWidth; x++)
                {
                    if (pattern[x, y])
                    {
                        int index = (startY + y) * textureWidth + (startX + x);
                        if (index >= 0 && index < fontData.Length)
                        {
                            fontData[index] = Color.White;
                        }
                    }
                }
            }
        }
        
        private bool[,] GetCharacterPattern(char character)
        {
            bool[,] pattern = new bool[charWidth, charHeight];
            
            switch (character)
            {
                case '0':
                    // Thick outline rectangle
                    DrawRect(pattern, 2, 2, 6, 10);
                    ClearRect(pattern, 4, 4, 2, 6);
                    break;
                case '1':
                    // Thick vertical line with base
                    DrawRect(pattern, 4, 2, 2, 10);
                    DrawRect(pattern, 3, 3, 1, 1);
                    DrawRect(pattern, 2, 11, 6, 1);
                    break;
                case '2':
                    // Thick S-curve
                    DrawRect(pattern, 2, 2, 6, 2);
                    DrawRect(pattern, 6, 4, 2, 2);
                    DrawRect(pattern, 2, 6, 6, 2);
                    DrawRect(pattern, 2, 8, 2, 2);
                    DrawRect(pattern, 2, 10, 6, 2);
                    break;
                case '3':
                    // Thick E-like shape
                    DrawRect(pattern, 2, 2, 6, 2);
                    DrawRect(pattern, 6, 4, 2, 8);
                    DrawRect(pattern, 2, 6, 4, 2);
                    DrawRect(pattern, 2, 10, 6, 2);
                    break;
                case '4':
                    // Thick 4 shape
                    DrawRect(pattern, 2, 2, 2, 6);
                    DrawRect(pattern, 6, 2, 2, 10);
                    DrawRect(pattern, 2, 6, 6, 2);
                    break;
                case '5':
                    // Thick reverse S
                    DrawRect(pattern, 2, 2, 6, 2);
                    DrawRect(pattern, 2, 4, 2, 2);
                    DrawRect(pattern, 2, 6, 6, 2);
                    DrawRect(pattern, 6, 8, 2, 2);
                    DrawRect(pattern, 2, 10, 6, 2);
                    break;
                case '6':
                    // Thick 6
                    DrawRect(pattern, 2, 2, 6, 2);
                    DrawRect(pattern, 2, 4, 2, 8);
                    DrawRect(pattern, 2, 6, 6, 2);
                    DrawRect(pattern, 6, 8, 2, 2);
                    DrawRect(pattern, 2, 10, 6, 2);
                    break;
                case '7':
                    // Thick 7
                    DrawRect(pattern, 2, 2, 6, 2);
                    DrawRect(pattern, 6, 4, 2, 8);
                    break;
                case '8':
                    // Thick 8
                    DrawRect(pattern, 2, 2, 6, 2);
                    DrawRect(pattern, 2, 4, 2, 8);
                    DrawRect(pattern, 6, 4, 2, 8);
                    DrawRect(pattern, 2, 6, 6, 2);
                    DrawRect(pattern, 2, 10, 6, 2);
                    break;
                case '9':
                    // Thick 9
                    DrawRect(pattern, 2, 2, 6, 2);
                    DrawRect(pattern, 2, 4, 2, 2);
                    DrawRect(pattern, 6, 4, 2, 8);
                    DrawRect(pattern, 2, 6, 6, 2);
                    DrawRect(pattern, 2, 10, 6, 2);
                    break;
                case 'A':
                    // Thick A
                    DrawRect(pattern, 2, 2, 6, 2);
                    DrawRect(pattern, 2, 4, 2, 8);
                    DrawRect(pattern, 6, 4, 2, 8);
                    DrawRect(pattern, 2, 7, 6, 2);
                    break;
                case 'B':
                    // Thick B
                    DrawRect(pattern, 2, 2, 2, 10);
                    DrawRect(pattern, 2, 2, 5, 2);
                    DrawRect(pattern, 5, 4, 2, 1);
                    DrawRect(pattern, 2, 6, 5, 2);
                    DrawRect(pattern, 5, 9, 2, 1);
                    DrawRect(pattern, 2, 10, 5, 2);
                    break;
                case 'C':
                    // Thick C
                    DrawRect(pattern, 2, 2, 6, 2);
                    DrawRect(pattern, 2, 4, 2, 6);
                    DrawRect(pattern, 2, 10, 6, 2);
                    break;
                case 'D':
                    // Thick D
                    DrawRect(pattern, 2, 2, 2, 10);
                    DrawRect(pattern, 2, 2, 5, 2);
                    DrawRect(pattern, 6, 4, 2, 6);
                    DrawRect(pattern, 2, 10, 5, 2);
                    break;
                case 'E':
                    // Thick E
                    DrawRect(pattern, 2, 2, 2, 10);
                    DrawRect(pattern, 2, 2, 6, 2);
                    DrawRect(pattern, 2, 6, 5, 2);
                    DrawRect(pattern, 2, 10, 6, 2);
                    break;
                case 'F':
                    // Thick F
                    DrawRect(pattern, 2, 2, 2, 10);
                    DrawRect(pattern, 2, 2, 6, 2);
                    DrawRect(pattern, 2, 6, 5, 2);
                    break;
                case 'G':
                    // Thick G
                    DrawRect(pattern, 2, 2, 6, 2);
                    DrawRect(pattern, 2, 4, 2, 6);
                    DrawRect(pattern, 4, 7, 4, 2);
                    DrawRect(pattern, 6, 8, 2, 2);
                    DrawRect(pattern, 2, 10, 6, 2);
                    break;
                case 'H':
                    // Thick H
                    DrawRect(pattern, 2, 2, 2, 10);
                    DrawRect(pattern, 6, 2, 2, 10);
                    DrawRect(pattern, 2, 6, 6, 2);
                    break;
                case 'I':
                    // Thick I
                    DrawRect(pattern, 2, 2, 6, 2);
                    DrawRect(pattern, 4, 4, 2, 6);
                    DrawRect(pattern, 2, 10, 6, 2);
                    break;
                case 'J':
                    // Thick J
                    DrawRect(pattern, 2, 2, 6, 2);
                    DrawRect(pattern, 6, 4, 2, 6);
                    DrawRect(pattern, 2, 8, 2, 2);
                    DrawRect(pattern, 2, 10, 6, 2);
                    break;
                case 'K':
                    // Thick K
                    DrawRect(pattern, 2, 2, 2, 10);
                    DrawRect(pattern, 4, 6, 2, 2);
                    DrawRect(pattern, 6, 4, 2, 2);
                    DrawRect(pattern, 6, 8, 2, 2);
                    break;
                case 'L':
                    // Thick L
                    DrawRect(pattern, 2, 2, 2, 10);
                    DrawRect(pattern, 2, 10, 6, 2);
                    break;
                case 'M':
                    // Thick M
                    DrawRect(pattern, 2, 2, 2, 10);
                    DrawRect(pattern, 6, 2, 2, 10);
                    DrawRect(pattern, 3, 4, 1, 3);
                    DrawRect(pattern, 5, 4, 1, 3);
                    DrawRect(pattern, 4, 5, 2, 1);
                    break;
                case 'N':
                    // Thick N
                    DrawRect(pattern, 2, 2, 2, 10);
                    DrawRect(pattern, 6, 2, 2, 10);
                    DrawRect(pattern, 3, 5, 1, 1);
                    DrawRect(pattern, 4, 6, 1, 1);
                    DrawRect(pattern, 5, 7, 1, 1);
                    break;
                case 'O':
                    // Thick O
                    DrawRect(pattern, 2, 2, 6, 2);
                    DrawRect(pattern, 2, 4, 2, 6);
                    DrawRect(pattern, 6, 4, 2, 6);
                    DrawRect(pattern, 2, 10, 6, 2);
                    break;
                case 'P':
                    // Thick P
                    DrawRect(pattern, 2, 2, 2, 10);
                    DrawRect(pattern, 2, 2, 5, 2);
                    DrawRect(pattern, 6, 4, 2, 2);
                    DrawRect(pattern, 2, 6, 5, 2);
                    break;
                case 'Q':
                    // Thick Q
                    DrawRect(pattern, 2, 2, 6, 2);
                    DrawRect(pattern, 2, 4, 2, 6);
                    DrawRect(pattern, 6, 4, 2, 6);
                    DrawRect(pattern, 2, 10, 6, 2);
                    DrawRect(pattern, 5, 8, 2, 2);
                    break;
                case 'R':
                    // Thick R
                    DrawRect(pattern, 2, 2, 2, 10);
                    DrawRect(pattern, 2, 2, 5, 2);
                    DrawRect(pattern, 6, 4, 2, 2);
                    DrawRect(pattern, 2, 6, 5, 2);
                    DrawRect(pattern, 5, 8, 1, 1);
                    DrawRect(pattern, 6, 9, 2, 3);
                    break;
                case 'S':
                    // Thick S
                    DrawRect(pattern, 2, 2, 6, 2);
                    DrawRect(pattern, 2, 4, 2, 2);
                    DrawRect(pattern, 2, 6, 6, 2);
                    DrawRect(pattern, 6, 8, 2, 2);
                    DrawRect(pattern, 2, 10, 6, 2);
                    break;
                case 'T':
                    // Thick T
                    DrawRect(pattern, 2, 2, 6, 2);
                    DrawRect(pattern, 4, 4, 2, 8);
                    break;
                case 'U':
                    // Thick U
                    DrawRect(pattern, 2, 2, 2, 8);
                    DrawRect(pattern, 6, 2, 2, 8);
                    DrawRect(pattern, 2, 10, 6, 2);
                    break;
                case 'V':
                    // Thick V
                    DrawRect(pattern, 2, 2, 2, 6);
                    DrawRect(pattern, 6, 2, 2, 6);
                    DrawRect(pattern, 3, 8, 1, 2);
                    DrawRect(pattern, 5, 8, 1, 2);
                    DrawRect(pattern, 4, 10, 2, 2);
                    break;
                case 'W':
                    // Thick W - much more distinct from O
                    DrawRect(pattern, 1, 2, 2, 8);
                    DrawRect(pattern, 7, 2, 2, 8);
                    DrawRect(pattern, 2, 10, 1, 2);
                    DrawRect(pattern, 4, 8, 2, 2);
                    DrawRect(pattern, 7, 10, 1, 2);
                    break;
                case 'X':
                    // Thick X
                    DrawRect(pattern, 2, 2, 2, 2);
                    DrawRect(pattern, 6, 2, 2, 2);
                    DrawRect(pattern, 3, 4, 2, 2);
                    DrawRect(pattern, 4, 6, 2, 2);
                    DrawRect(pattern, 3, 8, 2, 2);
                    DrawRect(pattern, 2, 10, 2, 2);
                    DrawRect(pattern, 6, 10, 2, 2);
                    break;
                case 'Y':
                    // Thick Y
                    DrawRect(pattern, 2, 2, 2, 4);
                    DrawRect(pattern, 6, 2, 2, 4);
                    DrawRect(pattern, 3, 6, 1, 1);
                    DrawRect(pattern, 5, 6, 1, 1);
                    DrawRect(pattern, 4, 7, 2, 5);
                    break;
                case 'Z':
                    // Thick Z
                    DrawRect(pattern, 2, 2, 6, 2);
                    DrawRect(pattern, 6, 4, 2, 2);
                    DrawRect(pattern, 4, 6, 2, 2);
                    DrawRect(pattern, 2, 8, 2, 2);
                    DrawRect(pattern, 2, 10, 6, 2);
                    break;
                case ':':
                    // Thick colon
                    DrawRect(pattern, 4, 4, 2, 2);
                    DrawRect(pattern, 4, 8, 2, 2);
                    break;
                case '?':
                    // Thick question mark
                    DrawRect(pattern, 2, 2, 5, 2);
                    DrawRect(pattern, 6, 4, 2, 2);
                    DrawRect(pattern, 4, 6, 2, 2);
                    DrawRect(pattern, 4, 10, 2, 2);
                    break;
                case '!':
                    // Thick exclamation
                    DrawRect(pattern, 4, 2, 2, 6);
                    DrawRect(pattern, 4, 10, 2, 2);
                    break;
                case '-':
                    // Thick dash
                    DrawRect(pattern, 2, 6, 6, 2);
                    break;
                case '.':
                    // Thick period
                    DrawRect(pattern, 4, 10, 2, 2);
                    break;
                case '(':
                    // Thick left paren
                    DrawRect(pattern, 4, 2, 2, 2);
                    DrawRect(pattern, 2, 4, 2, 6);
                    DrawRect(pattern, 4, 10, 2, 2);
                    break;
                case ')':
                    // Thick right paren
                    DrawRect(pattern, 4, 2, 2, 2);
                    DrawRect(pattern, 6, 4, 2, 6);
                    DrawRect(pattern, 4, 10, 2, 2);
                    break;
                case '/':
                    // Thick slash
                    DrawRect(pattern, 6, 2, 2, 2);
                    DrawRect(pattern, 5, 4, 2, 2);
                    DrawRect(pattern, 4, 6, 2, 2);
                    DrawRect(pattern, 3, 8, 2, 2);
                    DrawRect(pattern, 2, 10, 2, 2);
                    break;
                case '\\':
                    // Thick backslash
                    DrawRect(pattern, 2, 2, 2, 2);
                    DrawRect(pattern, 3, 4, 2, 2);
                    DrawRect(pattern, 4, 6, 2, 2);
                    DrawRect(pattern, 5, 8, 2, 2);
                    DrawRect(pattern, 6, 10, 2, 2);
                    break;
                case '#':
                    // Thick hash/pound
                    DrawRect(pattern, 3, 2, 1, 10);
                    DrawRect(pattern, 6, 2, 1, 10);
                    DrawRect(pattern, 1, 4, 8, 1);
                    DrawRect(pattern, 1, 8, 8, 1);
                    break;
                case '_':
                    // Thick underscore
                    DrawRect(pattern, 1, 11, 8, 1);
                    break;
                case ' ':
                    // Space - no pixels
                    break;
                default:
                    // Unknown character - draw a thick box
                    DrawRect(pattern, 2, 2, 6, 10);
                    ClearRect(pattern, 4, 4, 2, 6);
                    break;
            }
            
            return pattern;
        }
        
        private void DrawRect(bool[,] pattern, int x, int y, int width, int height)
        {
            for (int dy = 0; dy < height; dy++)
            {
                for (int dx = 0; dx < width; dx++)
                {
                    int px = x + dx;
                    int py = y + dy;
                    if (px >= 0 && px < charWidth && py >= 0 && py < charHeight)
                    {
                        pattern[px, py] = true;
                    }
                }
            }
        }
        
        private void ClearRect(bool[,] pattern, int x, int y, int width, int height)
        {
            for (int dy = 0; dy < height; dy++)
            {
                for (int dx = 0; dx < width; dx++)
                {
                    int px = x + dx;
                    int py = y + dy;
                    if (px >= 0 && px < charWidth && py >= 0 && py < charHeight)
                    {
                        pattern[px, py] = false;
                    }
                }
            }
        }
        
        private void CreateCharacterMap()
        {
            charMap = new Dictionary<char, Rectangle>();
            
            // Numbers
            for (int i = 0; i < 10; i++)
            {
                char c = (char)('0' + i);
                charMap[c] = new Rectangle(i * charWidth, 0, charWidth, charHeight);
            }
            
            // Letters A-F (row 0)
            for (int i = 0; i < 6; i++)
            {
                char c = (char)('A' + i);
                charMap[c] = new Rectangle((10 + i) * charWidth, 0, charWidth, charHeight);
            }
            
            // Letters G-V (row 1)
            for (int i = 0; i < 16; i++)
            {
                char c = (char)('G' + i);
                charMap[c] = new Rectangle(i * charWidth, charHeight, charWidth, charHeight);
            }
            
            // Letters W-Z and symbols (row 2)
            charMap['W'] = new Rectangle(0 * charWidth, 2 * charHeight, charWidth, charHeight);
            charMap['X'] = new Rectangle(1 * charWidth, 2 * charHeight, charWidth, charHeight);
            charMap['Y'] = new Rectangle(2 * charWidth, 2 * charHeight, charWidth, charHeight);
            charMap['Z'] = new Rectangle(3 * charWidth, 2 * charHeight, charWidth, charHeight);
            charMap[' '] = new Rectangle(4 * charWidth, 2 * charHeight, charWidth, charHeight);
            charMap[':'] = new Rectangle(5 * charWidth, 2 * charHeight, charWidth, charHeight);
            charMap['?'] = new Rectangle(6 * charWidth, 2 * charHeight, charWidth, charHeight);
            charMap['!'] = new Rectangle(7 * charWidth, 2 * charHeight, charWidth, charHeight);
            charMap['-'] = new Rectangle(8 * charWidth, 2 * charHeight, charWidth, charHeight);
            charMap['/'] = new Rectangle(9 * charWidth, 2 * charHeight, charWidth, charHeight);
            charMap['('] = new Rectangle(10 * charWidth, 2 * charHeight, charWidth, charHeight);
            charMap[')'] = new Rectangle(11 * charWidth, 2 * charHeight, charWidth, charHeight);
            charMap['.'] = new Rectangle(12 * charWidth, 2 * charHeight, charWidth, charHeight);
            charMap['\\'] = new Rectangle(13 * charWidth, 2 * charHeight, charWidth, charHeight);
            charMap['#'] = new Rectangle(14 * charWidth, 2 * charHeight, charWidth, charHeight);
            charMap['_'] = new Rectangle(15 * charWidth, 2 * charHeight, charWidth, charHeight);
        }
        
        public void DrawText(SpriteBatch spriteBatch, string text, Vector2 position, Color color, float scale = 1f)
        {
            Vector2 currentPos = position;
            
            foreach (char c in text.ToUpper())
            {
                if (charMap.ContainsKey(c))
                {
                    Rectangle sourceRect = charMap[c];
                    spriteBatch.Draw(fontTexture, currentPos, sourceRect, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                }
                
                currentPos.X += charWidth * scale;
            }
        }
        
        public Vector2 MeasureText(string text, float scale = 1f)
        {
            return new Vector2(text.Length * charWidth * scale, charHeight * scale);
        }
    }
}
