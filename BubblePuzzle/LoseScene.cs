using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace BubblePuzzle
{
    public class LoseScene : Scene
    {
        SpriteFont font, headerFont;
        Texture2D background, cursorTexture;
        public Action OnReturnToMenu;
        int score, bestScore;
        float elapsedTime;

        public LoseScene(ContentManager content, GraphicsDevice graphicsDevice, int score, int bestScore, float elapsedTime, Action onReturnToMenu)
        {
            font = content.Load<SpriteFont>("Arial");
            headerFont = content.Load<SpriteFont>("ArialHeader");
            background = content.Load<Texture2D>("background");
            cursorTexture = content.Load<Texture2D>("cursor");
            OnReturnToMenu = onReturnToMenu;
            this.score = score;
            // For lose, do NOT update best score.
            this.bestScore = bestScore;
            this.elapsedTime = elapsedTime;
        }

        public override void Update(GameTime gameTime)
        {
            if (Microsoft.Xna.Framework.Input.Mouse.GetState().LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                OnReturnToMenu?.Invoke();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(background, new Rectangle(0, 0, 500, 800), Color.White);
            string loseText = "You Lose!";
            Vector2 loseSize = headerFont.MeasureString(loseText);
            Vector2 losePos = new Vector2((500 - loseSize.X) / 2, 220);
            spriteBatch.DrawString(headerFont, loseText, losePos, Color.Black);
            string scoreText = "Score: " + score;
            string bestText = "Best Score: " + bestScore;
            string timeText = "Time: " + Math.Round(elapsedTime, 1) + " s";
            Vector2 scoreSize = font.MeasureString(scoreText);
            Vector2 bestSize = font.MeasureString(bestText);
            Vector2 timeSize = font.MeasureString(timeText);
            Vector2 scorePos = new Vector2((500 - scoreSize.X) / 2, 300);
            Vector2 bestPos = new Vector2((500 - bestSize.X) / 2, 340);
            Vector2 timePos = new Vector2((500 - timeSize.X) / 2, 380);
            spriteBatch.DrawString(font, scoreText, scorePos, Color.Brown);
            spriteBatch.DrawString(font, bestText, bestPos, Color.Brown);
            spriteBatch.DrawString(font, timeText, timePos, Color.Brown);
            string returnText = "Click to return to Menu";
            Vector2 returnSize = font.MeasureString(returnText);
            Vector2 returnPos = new Vector2((500 - returnSize.X) / 2, 420);
            spriteBatch.DrawString(font, returnText, returnPos, Color.DarkGray);

            Vector2 mousePos = new Vector2(Microsoft.Xna.Framework.Input.Mouse.GetState().X, Microsoft.Xna.Framework.Input.Mouse.GetState().Y);
            spriteBatch.Draw(cursorTexture, mousePos, Color.White);
        }
    }
}




