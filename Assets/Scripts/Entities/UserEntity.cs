using System.Collections.Generic;

namespace FaxCap.Entity
{
    public class UserEntity
    {
        public string UserId { get; private set; }
        public string Username { get; private set; }
        public int Level { get; private set; }
        public int Experience { get; private set; }
        public int Score { get; private set; }
        public int HighestScore { get; private set; }
        public List<string> Achievements { get; private set; }
        public List<string> SeenQuestions { get; private set; }
        public int TotalQuestionsSolved { get; private set; }
        public int CorrectAnswers { get; private set; }
        public int WrongAnswers { get; private set; }
        public int HighestCombo { get; private set; }
        public int HighestScoreInOneRun { get; private set; }
        public int LongestLogin { get; private set; }
        public float TotalTimePlayed { get; private set; }

        private UserEntity(string userId, string username)
        {
            UserId = userId;
            Username = username;
            Achievements = new List<string>();
            SeenQuestions = new List<string>();
        }

        public static UserEntity Create(
            string userId,
            string username,
            int level,
            int experience,
            int score,
            int highestScore,
            List<string> achievements,
            List<string> seenQuestions,
            int totalQuestionsSolved,
            int correctAnswers,
            int wrongAnswers,
            int highestCombo,
            int highestScoreInOneRun,
            int longestLogin,
            float totalTimePlayed)
        {
            var userEntity = new UserEntity(userId, username)
            {
                Level = level,
                Experience = experience,
                Score = score,
                HighestScore = highestScore,
                TotalQuestionsSolved = totalQuestionsSolved,
                CorrectAnswers = correctAnswers,
                WrongAnswers = wrongAnswers,
                HighestCombo = highestCombo,
                HighestScoreInOneRun = highestScoreInOneRun,
                LongestLogin = longestLogin,
                TotalTimePlayed = totalTimePlayed
            };

            userEntity.Achievements.AddRange(achievements);
            userEntity.SeenQuestions.AddRange(seenQuestions);

            return userEntity;
        }

        public void UpdateUserId(string userId)
        {
            UserId = userId;
        }

        public void UpdateUsername(string username)
        {
            Username = username;
        }

        public void UpdateLevel(int level)
        {
            Level = level;
        }

        public void UpdateExperience(int experience)
        {
            Experience = experience;
        }

        public void UpdateScore(int score)
        {
            Score = score;
        }

        public void UpdateHighestScore(int highestScore)
        {
            HighestScore = highestScore;
        }

        public void UpdateAchievements(List<string> achievements)
        {
            Achievements = achievements;
        }

        public void UpdateSeenQuestions(List<string> seenQuestions)
        {
            SeenQuestions = seenQuestions;
        }

        public void UpdateTotalQuestionsSolved(int totalQuestionsSolved)
        {
            TotalQuestionsSolved = totalQuestionsSolved;
        }

        public void UpdateCorrectAnswers(int correctAnswers)
        {
            CorrectAnswers = correctAnswers;
        }

        public void UpdateWrongAnswers(int wrongAnswers)
        {
            WrongAnswers = wrongAnswers;
        }

        public void UpdateHighestCombo(int highestCombo)
        {
            HighestCombo = highestCombo;
        }

        public void UpdateHighestScoreInOneRun(int highestScoreInOneRun)
        {
            HighestScoreInOneRun = highestScoreInOneRun;
        }

        public void UpdateLongestLogin(int longestLogin)
        {
            LongestLogin = longestLogin;
        }

        public void UpdateTotalTimePlayed(float totalTimePlayed)
        {
            TotalTimePlayed = totalTimePlayed;
        }
    }
}
