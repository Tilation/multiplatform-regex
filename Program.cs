using CommandLine;
using Octokit;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;

namespace updater
{
    partial class Program
    {
        static void Main(string[] args)
        {
            string? outputFile = Environment.GetEnvironmentVariable("GITHUB_OUTPUT");
            string? _github = Environment.GetEnvironmentVariable("_github");
            string? _regex = Environment.GetEnvironmentVariable("_regex");
            string? _ref = Environment.GetEnvironmentVariable("_ref");
            string? _token = Environment.GetEnvironmentVariable("_token");
            string? _file = Environment.GetEnvironmentVariable("_file");
            string? _debug = Environment.GetEnvironmentVariable("_debug");

            if (new[] { _github, _regex, _ref, _token, _file }.Any(x=>string.IsNullOrWhiteSpace(x)))
            {
                throw new Exception("Missing parameter");
            }

            Octokit.GitHubClient client = new GitHubClient(new ProductHeaderValue("regex-on-repo"));
            client.Credentials = new Credentials(_token);

            string _owner = _github.Split('/')[0];
            string _repo = _github.Split('/')[1];

            Repository repo = Task.Run(() => client.Repository.Get(_owner, _repo)).Result;

            Regex reg = new Regex(_regex, RegexOptions.Compiled);

            var content = Task.Run(() => client.Repository.Content.GetAllContentsByRef(repo.Id, _file, _ref)).Result[0];

            var matches = reg.Matches(content.Content);

            var sb = new StringBuilder();
            sb.AppendLine($"matches={matches.Count}");
            for(int i = 0; i < matches.Count; i++)
            {
                var match = matches[i];
                sb.AppendLine($"match_{i}_groups={match.Groups.Count}");
                for (int j = 0; j < match.Groups.Count; j++)
                {
                    var group = match.Groups[j];
                    sb.AppendLine($"match_{i}_group_{j}={group.Value}");
                }
            }
            if (string.Equals(_debug, "true", StringComparison.InvariantCultureIgnoreCase))
            {
                Console.WriteLine("DEBUG OUTPUT START");
                Console.WriteLine(sb.ToString());
                Console.WriteLine("DEBUG OUTPUT END");
            }
            File.WriteAllText(outputFile, sb.ToString());
        }
    }
}
