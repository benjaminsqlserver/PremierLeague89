using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class PremierLeagueChampionAtLeast80
{
    static Random random = new Random();

    // List of teams
    static List<string> teams = new List<string>
    {
        "ARS", "ASV", "BRN", "BRI", "BOU",
        "BUR", "CHE", "CRY", "EVE", "FUL",
        "LIV", "LUT", "MNC", "MNU",
        "NWC", "FOR", "SHU",
        "TOT", "WHU", "WOL"
    };

    static List<Fixture> fixtures = new List<Fixture>();
    static Dictionary<string, TeamStats> leagueTable = new Dictionary<string, TeamStats>();

    static void Main(string[] args)
    {
        // Initialize league table
        foreach (var team in teams)
        {
            leagueTable[team] = new TeamStats { TeamName = team };
        }

        // Generate fixtures
        int fixtureID = 1;
        for (int homeIndex = 0; homeIndex < teams.Count; homeIndex++)
        {
            for (int awayIndex = 0; awayIndex < teams.Count; awayIndex++)
            {
                if (homeIndex != awayIndex)
                {
                    Fixture fixture = new Fixture
                    {
                        FixtureID = fixtureID++,
                        HomeTeam = teams[homeIndex],
                        AwayTeam = teams[awayIndex]
                    };
                    PlayMatch(fixture);
                    fixtures.Add(fixture);
                }
            }
        }

        // Ensure the league champion has at least 89 points
        while (leagueTable.Values.Max(t => t.Points) < 89)
        {
            fixtures.Clear();
            foreach (var team in leagueTable.Keys.ToList())
            {
                leagueTable[team] = new TeamStats { TeamName = team };
            }
            fixtureID = 1;
            for (int homeIndex = 0; homeIndex < teams.Count; homeIndex++)
            {
                for (int awayIndex = 0; awayIndex < teams.Count; awayIndex++)
                {
                    if (homeIndex != awayIndex)
                    {
                        Fixture fixture = new Fixture
                        {
                            FixtureID = fixtureID++,
                            HomeTeam = teams[homeIndex],
                            AwayTeam = teams[awayIndex]
                        };
                        PlayMatch(fixture);
                        fixtures.Add(fixture);
                    }
                }
            }
        }

        // Output fixtures to CSV
        using (var writer = new StreamWriter("PremierLeagueFixtures.csv"))
        {
            writer.WriteLine("FixtureID,HomeTeam,HomeScore,AwayTeam,AwayScore");
            foreach (var fixture in fixtures)
            {
                writer.WriteLine($"{fixture.FixtureID},{fixture.HomeTeam},{fixture.HomeScore},{fixture.AwayTeam},{fixture.AwayScore}");
            }
        }

        // Generate league table
        var sortedLeagueTable = leagueTable.Values.OrderByDescending(t => t.Points)
                                                  .ThenByDescending(t => t.GoalDifference)
                                                  .ThenByDescending(t => t.GoalsFavour)
                                                  .ToList();
        using (var writer = new StreamWriter("PremierLeagueTable.csv"))
        {
            writer.WriteLine("Position,TeamName,NumberOfMatchesPlayed,Won,Drawn,Lost,GoalsFavour,GoalsAgainst,GoalsDifference,Points");
            for (int i = 0; i < sortedLeagueTable.Count; i++)
            {
                var teamStats = sortedLeagueTable[i];
                writer.WriteLine($"{i + 1},{teamStats.TeamName},{teamStats.NumberOfMatchesPlayed},{teamStats.Won},{teamStats.Drawn},{teamStats.Lost},{teamStats.GoalsFavour},{teamStats.GoalsAgainst},{teamStats.GoalDifference},{teamStats.Points}");
            }
        }

        Console.WriteLine("Fixtures and league table have been generated.");
    }

    static void PlayMatch(Fixture fixture)
    {
        int totalGoals = random.Next(0, 7); // Total goals scored in the match (0-6)
        if(totalGoals==0)
        {
            fixture.HomeScore = 0;
            fixture.AwayScore = 0;
        }
        else
        {
            int homeGoals = random.Next(0, totalGoals + 1); // Goals scored by home team
            int awayGoals = totalGoals - homeGoals; // Goals scored by away team

            fixture.HomeScore = homeGoals;
            fixture.AwayScore = awayGoals;
        }
       

        // Update league table
        UpdateTeamStats(fixture.HomeTeam, fixture.AwayTeam, fixture.HomeScore, fixture.AwayScore);
    }

    static void UpdateTeamStats(string homeTeam, string awayTeam, int homeGoals, int awayGoals)
    {
        var homeStats = leagueTable[homeTeam];
        var awayStats = leagueTable[awayTeam];

        homeStats.NumberOfMatchesPlayed++;
        awayStats.NumberOfMatchesPlayed++;

        homeStats.GoalsFavour += homeGoals;
        homeStats.GoalsAgainst += awayGoals;

        awayStats.GoalsFavour += awayGoals;
        awayStats.GoalsAgainst += homeGoals;

        homeStats.GoalDifference = homeStats.GoalsFavour - homeStats.GoalsAgainst;
        awayStats.GoalDifference = awayStats.GoalsFavour - awayStats.GoalsAgainst;

        if (homeGoals > awayGoals)
        {
            homeStats.Won++;
            awayStats.Lost++;
            homeStats.Points += 3;
        }
        else if (homeGoals < awayGoals)
        {
            awayStats.Won++;
            homeStats.Lost++;
            awayStats.Points += 3;
        }
        else
        {
            homeStats.Drawn++;
            awayStats.Drawn++;
            homeStats.Points++;
            awayStats.Points++;
        }
    }
}

class Fixture
{
    public int FixtureID { get; set; }
    public string HomeTeam { get; set; }
    public int HomeScore { get; set; }
    public string AwayTeam { get; set; }
    public int AwayScore { get; set; }
}

class TeamStats
{
    public string TeamName { get; set; }
    public int NumberOfMatchesPlayed { get; set; }
    public int Won { get; set; }
    public int Drawn { get; set; }
    public int Lost { get; set; }
    public int GoalsFavour { get; set; }
    public int GoalsAgainst { get; set; }
    public int GoalDifference { get; set; }
    public int Points { get; set; }
}
