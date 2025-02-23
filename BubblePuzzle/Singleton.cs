namespace BubblePuzzle
{
    public sealed class Singleton
    {
        private static Singleton instance = null;
        private static readonly object padlock = new object();

        public int Score { get; private set; }
        public int BestScore { get; set; }

        private Singleton()
        {
            Score = 0;
            BestScore = 0;
        }

        public static Singleton Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                        instance = new Singleton();
                    return instance;
                }
            }
        }

        public void AddScore(int points)
        {
            Score += points;
        }

        public void ResetScore()
        {
            Score = 0;
        }
    }
}






















