using BLL;

namespace WebApp;

public static class GameCache
{
    // Stores runtime copies of games by their GameId
    public static readonly Dictionary<string, GameConfiguration> RuntimeGames = new();
}
