using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;

namespace BubblePuzzle
{
    public class GameScene : Scene
    {
        GraphicsDevice graphicsDevice;
        ContentManager content;

        SpriteFont font, arialHeader;
        Texture2D bubbleTexture, pixelTexture, background, cursorTexture;
        SoundEffect destroySound, collisionSound;
        Song bgMusic;

        BubbleManager bubbleManager;
        Bubble currentBubble;
        Bubble readyBall;
        bool shotInProgress = false;
        Vector2 launcherPosition;
        float aimAngle;
        const float minAimAngle = -MathHelper.Pi / 3;
        const float maxAimAngle = MathHelper.Pi / 3;
        float elapsedTime = 0f;
        public float ElapsedTime => elapsedTime;

        float gridStartX = 40f;
        float gridStartY = 40f;
        float spacing = 45f;
        int gridRows = 15;
        int gridCols = 10;
        int initialRows = 7;

        MouseState previousMouseState;
        Random random;

        // Win/Lose events.
        public Action OnWin;
        public Action OnLose;

        public GameScene(ContentManager content, GraphicsDevice graphicsDevice)
        {
            this.content = content;
            this.graphicsDevice = graphicsDevice;
            font = content.Load<SpriteFont>("Arial");
            arialHeader = content.Load<SpriteFont>("ArialHeader");
            bubbleTexture = CreateCircleTexture(2 * Bubble.Radius, Color.White);
            pixelTexture = new Texture2D(graphicsDevice, 1, 1);
            pixelTexture.SetData(new[] { Color.White });
            background = content.Load<Texture2D>("background");
            cursorTexture = content.Load<Texture2D>("cursor");
            destroySound = content.Load<SoundEffect>("destroySound");
            collisionSound = content.Load<SoundEffect>("collisionSound");
            bgMusic = content.Load<Song>("bgMusic");

            if (bgMusic != null)
            {
                MediaPlayer.IsRepeating = true;
                MediaPlayer.Volume = 0.5f;
                MediaPlayer.Play(bgMusic);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Error: bgMusic is null. Check Content Pipeline.");
            }

            random = new Random();
            launcherPosition = new Vector2(500 / 2, 800 - Bubble.Radius - 10);
            aimAngle = 0f;
            bubbleManager = new BubbleManager(gridStartX, gridStartY, spacing, gridRows, gridCols);
            bubbleManager.InitializeBubbles(initialRows);
            readyBall = new Bubble(launcherPosition, GetRandomBubbleColor());
            Singleton.Instance.ResetScore();
            elapsedTime = 0f;
        }

        Texture2D CreateCircleTexture(int diameter, Color color)
        {
            Texture2D texture = new Texture2D(graphicsDevice, diameter, diameter);
            Color[] colorData = new Color[diameter * diameter];
            float radius = diameter / 2f;
            Vector2 center = new Vector2(radius, radius);
            for (int y = 0; y < diameter; y++)
            {
                for (int x = 0; x < diameter; x++)
                {
                    Vector2 diff = new Vector2(x, y) - center;
                    colorData[y * diameter + x] = diff.Length() <= radius ? color : Color.Transparent;
                }
            }
            texture.SetData(colorData);
            return texture;
        }

        public override void Update(GameTime gameTime)
        {
            elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            MouseState mouse = Mouse.GetState();
            Vector2 mousePos = new Vector2(mouse.X, mouse.Y);

            if (!shotInProgress)
            {
                if (mousePos.Y < launcherPosition.Y)
                {
                    float newAngle = (float)Math.Atan2(mousePos.X - launcherPosition.X, -(mousePos.Y - launcherPosition.Y));
                    aimAngle = MathHelper.Clamp(newAngle, minAimAngle, maxAimAngle);
                }
                if (mouse.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Released)
                {
                    currentBubble = readyBall;
                    float speed = 500f;
                    Vector2 direction = new Vector2((float)Math.Sin(aimAngle), -(float)Math.Cos(aimAngle));
                    currentBubble.Velocity = direction * speed;
                    currentBubble.IsMoving = true;
                    shotInProgress = true;
                    readyBall = new Bubble(launcherPosition, GetRandomBubbleColor());
                }
            }
            previousMouseState = mouse;

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (currentBubble != null && currentBubble.IsMoving)
            {
                currentBubble.Position += currentBubble.Velocity * dt;
                if (currentBubble.Position.X <= Bubble.Radius)
                {
                    currentBubble.Position.X = Bubble.Radius;
                    currentBubble.Velocity.X *= -1;
                }
                else if (currentBubble.Position.X >= 500 - Bubble.Radius)
                {
                    currentBubble.Position.X = 500 - Bubble.Radius;
                    currentBubble.Velocity.X *= -1;
                }
                if (currentBubble.Position.Y <= Bubble.Radius)
                {
                    currentBubble.Position.Y = Bubble.Radius;
                    currentBubble.IsMoving = false;
                    SnapAndHandleBubble(currentBubble);
                    shotInProgress = false;
                }
                else
                {
                    foreach (var bubble in bubbleManager.Grid.Values)
                    {
                        if (Vector2.Distance(currentBubble.Position, bubble.Position) <= Bubble.Radius * 2)
                        {
                            currentBubble.IsMoving = false;
                            SnapAndHandleBubble(currentBubble);
                            shotInProgress = false;
                            break;
                        }
                    }
                }
            }

            RemoveFloatingBubbles();

            // Check win condition.
            if (bubbleManager.Grid.Count == 0)
                OnWin?.Invoke();

            // Check lose condition.
            foreach (var bubble in bubbleManager.Grid.Values)
            {
                if (bubble.Row >= gridRows - 1)
                {
                    OnLose?.Invoke();
                    break;
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(background, new Rectangle(0, 0, 500, 800), Color.White);
            foreach (var bubble in bubbleManager.Grid.Values)
                spriteBatch.Draw(bubbleTexture, bubble.Position - new Vector2(Bubble.Radius, Bubble.Radius), GetColor(bubble.Color));
            if (currentBubble != null && currentBubble.IsMoving)
                spriteBatch.Draw(bubbleTexture, currentBubble.Position - new Vector2(Bubble.Radius, Bubble.Radius), GetColor(currentBubble.Color));
            spriteBatch.Draw(bubbleTexture, launcherPosition - new Vector2(Bubble.Radius, Bubble.Radius), GetColor(readyBall.Color));
            float arrowLength = 50f;
            Vector2 arrowDirection = new Vector2((float)Math.Sin(aimAngle), -(float)Math.Cos(aimAngle));
            Vector2 arrowEnd = launcherPosition + arrowDirection * arrowLength;
            DrawLine(spriteBatch, launcherPosition, arrowEnd, Color.Yellow, 3);
            spriteBatch.DrawString(font, "Score: " + Singleton.Instance.Score, new Vector2(10, 800 - 30), Color.Brown);
            spriteBatch.DrawString(font, "Time: " + Math.Round(elapsedTime, 1) + " s", new Vector2(10, 800 - 60), Color.Brown);
            Vector2 mousePos = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            spriteBatch.Draw(cursorTexture, mousePos, Color.White);
        }

        private void DrawLine(SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color, int thickness)
        {
            Vector2 edge = end - start;
            float angle = (float)Math.Atan2(edge.Y, edge.X);
            spriteBatch.Draw(pixelTexture,
                new Rectangle((int)start.X, (int)start.Y, (int)edge.Length(), thickness),
                null, color, angle, Vector2.Zero, SpriteEffects.None, 0);
        }

        private Color GetColor(BubbleColor bubbleColor)
        {
            switch (bubbleColor)
            {
                case BubbleColor.Red: return Color.Red;
                case BubbleColor.Yellow: return Color.Yellow;
                case BubbleColor.Blue: return Color.Blue;
                case BubbleColor.Green: return Color.Green;
                case BubbleColor.Purple: return Color.Purple;
                default: return Color.White;
            }
        }

        void SnapAndHandleBubble(Bubble bubble)
        {
            int row = (int)Math.Round((bubble.Position.Y - bubbleManager.StartY) / bubbleManager.Spacing);
            row = MathHelper.Clamp(row, 0, gridRows - 1);
            int col;
            if (row % 2 == 0)
                col = (int)Math.Round((bubble.Position.X - bubbleManager.StartX) / bubbleManager.Spacing);
            else
                col = (int)Math.Round((bubble.Position.X - (bubbleManager.StartX + bubbleManager.Spacing / 2)) / bubbleManager.Spacing);
            int maxCol = (row % 2 == 0) ? gridCols - 1 : gridCols - 2;
            col = MathHelper.Clamp(col, 0, maxCol);
            bubble.Position = bubbleManager.GetCellCenter(row, col);
            bubble.Row = row;
            bubble.Col = col;
            bubbleManager.AddBubble(bubble, row, col);
            int removedCount = HandleCluster(new Point(col, row), bubble.Color);
            if (removedCount > 0)
                RemoveFloatingBubbles();
        }

        int HandleCluster(Point startCell, BubbleColor color)
        {
            Queue<Point> toCheck = new Queue<Point>();
            HashSet<Point> visited = new HashSet<Point>();
            List<Point> cluster = new List<Point>();
            toCheck.Enqueue(startCell);
            while (toCheck.Count > 0)
            {
                Point cell = toCheck.Dequeue();
                if (visited.Contains(cell))
                    continue;
                visited.Add(cell);
                if (bubbleManager.Grid.TryGetValue(cell, out Bubble b) && b.Color == color)
                {
                    cluster.Add(cell);
                    foreach (Point neighbor in bubbleManager.GetNeighbors(cell))
                    {
                        if (neighbor.Y >= 0 && neighbor.Y < gridRows && neighbor.X >= 0)
                            toCheck.Enqueue(neighbor);
                    }
                }
            }
            if (cluster.Count >= 3)
            {
                foreach (Point cell in cluster)
                    bubbleManager.RemoveBubble(cell);
                destroySound.Play();
                Singleton.Instance.AddScore(cluster.Count * 100);
                return cluster.Count;
            }
            else
            {
                collisionSound.Play();
                Singleton.Instance.AddScore(-100);
                return 0;
            }
        }

        void RemoveFloatingBubbles()
        {
            HashSet<Point> connected = new HashSet<Point>();
            Queue<Point> queue = new Queue<Point>();
            foreach (var kvp in bubbleManager.Grid)
            {
                if (kvp.Value.Row == 0)
                {
                    if (!connected.Contains(kvp.Key))
                    {
                        connected.Add(kvp.Key);
                        queue.Enqueue(kvp.Key);
                    }
                }
            }
            while (queue.Count > 0)
            {
                Point current = queue.Dequeue();
                foreach (Point neighbor in bubbleManager.GetNeighbors(current))
                {
                    if (neighbor.Y < 0 || neighbor.Y >= gridRows || neighbor.X < 0)
                        continue;
                    if (bubbleManager.Grid.ContainsKey(neighbor) && !connected.Contains(neighbor))
                    {
                        connected.Add(neighbor);
                        queue.Enqueue(neighbor);
                    }
                }
            }
            List<Point> floatingCells = new List<Point>();
            foreach (var kvp in bubbleManager.Grid)
            {
                if (!connected.Contains(kvp.Key))
                    floatingCells.Add(kvp.Key);
            }
            if (floatingCells.Count > 0)
            {
                foreach (Point cell in floatingCells)
                    bubbleManager.RemoveBubble(cell);
                destroySound.Play();
                Singleton.Instance.AddScore(floatingCells.Count * 100);
            }
        }

        public override void UnloadContent()
        {
        }

        private BubbleColor GetRandomBubbleColor()
        {
            HashSet<BubbleColor> availableColors = new HashSet<BubbleColor>();
            foreach (var bubble in bubbleManager.Grid.Values)
                availableColors.Add(bubble.Color);
            if (availableColors.Count == 0)
                return (BubbleColor)random.Next(0, 5);
            else
            {
                List<BubbleColor> colorList = new List<BubbleColor>(availableColors);
                int index = random.Next(0, colorList.Count);
                return colorList[index];
            }
        }
    }
}









