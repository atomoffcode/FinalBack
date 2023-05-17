namespace RazerFinal.Helpers
{
    public class PasswordGenerator
    {
        public static string GenerateRandomPassword(int length = 12)
        {
            const string uppercaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lowercaseChars = "abcdefghijklmnopqrstuvwxyz";
            const string numberChars = "0123456789";
            const string specialChars = "!@#$%^&*()_+-=";

            var random = new Random();
            var passwordChars = new char[length];
            var charGroups = new[]
            {
                uppercaseChars,
                lowercaseChars,
                numberChars,
                specialChars
            };

            // Add at least one character from each group
            for (int i = 0; i < charGroups.Length; i++)
            {
                passwordChars[i] = charGroups[i][random.Next(charGroups[i].Length)];
            }

            // Add remaining characters randomly
            for (int i = charGroups.Length; i < length; i++)
            {
                var randomGroup = charGroups[random.Next(charGroups.Length)];
                passwordChars[i] = randomGroup[random.Next(randomGroup.Length)];
            }

            // Shuffle the password characters
            for (int i = 0; i < length; i++)
            {
                int randomIndex = random.Next(length);
                (passwordChars[i], passwordChars[randomIndex]) = (passwordChars[randomIndex], passwordChars[i]);
            }

            return new string(passwordChars);
        }
    }
}
