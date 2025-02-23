using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace BubblePuzzle
{
    public abstract class Scene
    {
        public virtual void LoadContent(ContentManager content) { }
        public virtual void UnloadContent() { }
        public abstract void Update(GameTime gameTime);
        public abstract void Draw(SpriteBatch spriteBatch);
    }

    public class SceneManager
    {
        private Scene currentScene;

        public void ChangeScene(Scene newScene)
        {
            currentScene = newScene;
        }

        public void Update(GameTime gameTime)
        {
            currentScene?.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            currentScene?.Draw(spriteBatch);
        }
    }
}










