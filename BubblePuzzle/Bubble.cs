using Microsoft.Xna.Framework;

namespace BubblePuzzle
{
    public enum BubbleColor { Red, Yellow, Blue, Green, Purple }

    public class Bubble
    {
        public Vector2 Position;
        public BubbleColor Color;
        public bool IsMoving = false;
        public Vector2 Velocity;
        public int Row, Col; // Grid position.
        public static int Radius = 20;

        public Bubble(Vector2 position, BubbleColor color)
        {
            Position = position;
            Color = color;
        }
    }
}






















