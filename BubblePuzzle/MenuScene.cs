using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BubblePuzzle
{
    public class MenuScene : Scene
    {
        SpriteFont headerFont, font;
        Texture2D background, pixelTexture, cursorTexture;
        Rectangle startButtonRect, quitButtonRect;
        MouseState previousMouseState;
        public System.Action OnStartGame;
        public System.Action OnQuitGame;

        public MenuScene(ContentManager content, GraphicsDevice graphicsDevice)
        {
            headerFont = content.Load<SpriteFont>("ArialHeader");
            font = content.Load<SpriteFont>("Arial");
            background = content.Load<Texture2D>("background");
            cursorTexture = content.Load<Texture2D>("cursor");
            pixelTexture = new Texture2D(graphicsDevice, 1, 1);
            pixelTexture.SetData(new[] { Color.White });

            startButtonRect = new Rectangle(175, 300, 150, 40);
            quitButtonRect = new Rectangle(175, 350, 150, 40);
        }

        public override void Update(GameTime gameTime)
        {
            MouseState mouse = Mouse.GetState();
            if (mouse.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Released)
            {
                Point pos = new Point(mouse.X, mouse.Y);
                if (startButtonRect.Contains(pos))
                    OnStartGame?.Invoke();
                else if (quitButtonRect.Contains(pos))
                    OnQuitGame?.Invoke();
            }
            previousMouseState = mouse;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(background, new Rectangle(0, 0, 500, 800), Color.White);

            string title = "Desert POP";
            Vector2 titleSize = headerFont.MeasureString(title);
            Vector2 titlePos = new Vector2((500 - titleSize.X) / 2, 100);
            spriteBatch.DrawString(headerFont, title, titlePos, Color.Black);

            spriteBatch.Draw(pixelTexture, startButtonRect, Color.DarkBlue);
            Vector2 startTextSize = font.MeasureString("Start");
            Vector2 startTextPos = new Vector2(startButtonRect.X + (startButtonRect.Width - startTextSize.X) / 2,
                                                startButtonRect.Y + (startButtonRect.Height - startTextSize.Y) / 2);
            spriteBatch.DrawString(font, "Start", startTextPos, Color.White);

            spriteBatch.Draw(pixelTexture, quitButtonRect, Color.DarkBlue);
            Vector2 quitTextSize = font.MeasureString("Quit");
            Vector2 quitTextPos = new Vector2(quitButtonRect.X + (quitButtonRect.Width - quitTextSize.X) / 2,
                                               quitButtonRect.Y + (quitButtonRect.Height - quitTextSize.Y) / 2);
            spriteBatch.DrawString(font, "Quit", quitTextPos, Color.White);

            Vector2 mousePos = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            spriteBatch.Draw(cursorTexture, mousePos, Color.White);
        }
    }
}






