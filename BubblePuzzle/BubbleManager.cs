using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace BubblePuzzle
{
    public class BubbleManager
    {
        public Dictionary<Point, Bubble> Grid { get; private set; }

        public float StartX { get; private set; }
        public float StartY { get; private set; }
        public float Spacing { get; private set; }
        public int GridRows { get; private set; }
        public int GridCols { get; private set; }

        public BubbleManager(float startX, float startY, float spacing, int gridRows, int gridCols)
        {
            Grid = new Dictionary<Point, Bubble>();
            StartX = startX;
            StartY = startY;
            Spacing = spacing;
            GridRows = gridRows;
            GridCols = gridCols;
        }

        public Vector2 GetCellCenter(int row, int col)
        {
            float x = StartX;
            if (row % 2 == 1)
                x += Spacing / 2;
            x += col * Spacing;
            float y = StartY + row * Spacing;
            return new Vector2(x, y);
        }

        public void InitializeBubbles(int initialRows)
        {
            Random rand = new Random();
            for (int row = 0; row < initialRows; row++)
            {
                int colsInRow = (row % 2 == 1) ? GridCols - 1 : GridCols;
                for (int col = 0; col < colsInRow; col++)
                {
                    BubbleColor color = (BubbleColor)rand.Next(0, 5);
                    Vector2 pos = GetCellCenter(row, col);
                    Bubble bubble = new Bubble(pos, color);
                    bubble.Row = row;
                    bubble.Col = col;
                    Grid[new Point(col, row)] = bubble;
                }
            }
        }

        public void AddBubble(Bubble bubble, int row, int col)
        {
            bubble.Row = row;
            bubble.Col = col;
            bubble.Position = GetCellCenter(row, col);
            Grid[new Point(col, row)] = bubble;
        }

        public void RemoveBubble(Point cell)
        {
            if (Grid.ContainsKey(cell))
                Grid.Remove(cell);
        }

        public List<Point> GetNeighbors(Point cell)
        {
            List<Point> neighbors = new List<Point>();
            int row = cell.Y;
            int col = cell.X;
            neighbors.Add(new Point(col - 1, row));
            neighbors.Add(new Point(col + 1, row));

            if (row % 2 == 0)
            {
                neighbors.Add(new Point(col - 1, row - 1));
                neighbors.Add(new Point(col, row - 1));
                neighbors.Add(new Point(col - 1, row + 1));
                neighbors.Add(new Point(col, row + 1));
            }
            else
            {
                neighbors.Add(new Point(col, row - 1));
                neighbors.Add(new Point(col + 1, row - 1));
                neighbors.Add(new Point(col, row + 1));
                neighbors.Add(new Point(col + 1, row + 1));
            }
            return neighbors;
        }
    }
}























