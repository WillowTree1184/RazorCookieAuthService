using System.Diagnostics.CodeAnalysis;

namespace CookieAuthService
{
    /// <summary>
    /// A service to manage the token dictionary
    /// </summary>
    public static class TokenDictionaryService
    {
        static Dictionary<string, string> tokenDictionary = new Dictionary<string, string>();  //Dictionary<tokenId, token>

        /// <summary>
        /// Occurs when token has been registed just now
        /// </summary>
        public static event EventHandler<string>? TokenRegisted;

        /// <summary>
        /// Regist the token and generate a tokenId corresponding to the token
        /// </summary
        /// <param name="token">The token which will be registed</param>
        /// <returns>A complex and unique string as tokenId</returns>
        public static string Regist(string token)
        {
            string tokenId = Sha256Calculator.GetSha256Value($"{DateTime.Now.Ticks}{Guid.NewGuid()}TokenId{Random.Shared.Next()}{Sha256Calculator.GetSha256Value($"{Guid.NewGuid()}{token}{DateTime.Now}{Random.Shared.Next()}")}{Random.Shared.Next()}{Random.Shared.Next()}");   //Use complex and unique strings to generate tokenId
            tokenDictionary.Add(tokenId, token);
            TokenRegisted?.Invoke(token, tokenId);
            return tokenId;
        }

        /// <summary>
        /// Try to get the token which corresponding to the tokenId and delete the corresponding record
        /// </summary>
        /// <param name="tokenId">The tokenId</param>
        /// <param name="token">The corresponding token</param>
        /// <returns>When the tokenId has ben registed, return true; Otherwise, it returns false</returns>
        public static bool TryPopToken(string tokenId, [MaybeNullWhen(false)] out string token) => tokenDictionary.TryGetValue(tokenId, out token);
    }
}
