using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BubblePuzzle
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SceneManager sceneManager;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 500;
            graphics.PreferredBackBufferHeight = 800;
            Content.RootDirectory = "Content";
            IsMouseVisible = false;
        }

        protected override void Initialize()
        {
            sceneManager = new SceneManager();
            // Start with MenuScene.
            MenuScene menu = new MenuScene(Content, GraphicsDevice)
            {
                OnStartGame = OnStartGameHandler,
                OnQuitGame = () => Exit()
            };
            sceneManager.ChangeScene(menu);
            base.Initialize();
        }

        private void OnStartGameHandler()
        {
            GameScene gameScene = new GameScene(Content, GraphicsDevice);
            gameScene.OnWin = () =>
            {
                // Update BestScore on win.
                Singleton.Instance.BestScore = (Singleton.Instance.BestScore == 0) ? Singleton.Instance.Score :
                                               System.Math.Max(Singleton.Instance.BestScore, Singleton.Instance.Score);
                sceneManager.ChangeScene(new WinScene(Content, GraphicsDevice, Singleton.Instance.Score, Singleton.Instance.BestScore, gameScene.ElapsedTime, () =>
                {
                    MenuScene menu = new MenuScene(Content, GraphicsDevice)
                    {
                        OnStartGame = OnStartGameHandler,
                        OnQuitGame = () => Exit()
                    };
                    sceneManager.ChangeScene(menu);
                }));
            };
            gameScene.OnLose = () =>
            {
                // Do not update BestScore on lose.
                sceneManager.ChangeScene(new LoseScene(Content, GraphicsDevice, Singleton.Instance.Score, Singleton.Instance.BestScore, gameScene.ElapsedTime, () =>
                {
                    MenuScene menu = new MenuScene(Content, GraphicsDevice)
                    {
                        OnStartGame = OnStartGameHandler,
                        OnQuitGame = () => Exit()
                    };
                    sceneManager.ChangeScene(menu);
                }));
            };
            sceneManager.ChangeScene(gameScene);
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            sceneManager.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            sceneManager.Draw(spriteBatch);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}



























