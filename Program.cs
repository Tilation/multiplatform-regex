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
        class Options
        {
            [Option('g', "github",
                Required = true,
                HelpText = "The owner and repository name combination, example \"the-owner/and-the-repository\"")]
            public string Repo { get; set; }

            [Option('r', "ref",
                Required = true,
                HelpText = "The reference to use for getting the file, can be a commit sha, branch name, or tag.")]
            public string Ref { get; set; }

            [Option('t', "token",
                Required = true,
                HelpText = "A github token that has read access to the repository.")]
            public string GithubToken { get; set; }

            [Option('x', "regex",
                Required = true,
                HelpText = "The regex match string")]
            public string RegexString { get; set; }

            [Option('f', "file",
                Required = true,
                HelpText = "The relative directory to the file to read")]
            public string FilePath { get; set; }

            [Option('d', "debug",
                Required = false,
                HelpText = "Debug output toggle")]
            public bool Debug { get; set; }
        }

        static void Main(string[] args)
        {
            string outputFile = Environment.GetEnvironmentVariable("GITHUB_OUTPUT");

            Options options = null;
            var options_result = CommandLine.Parser.Default.ParseArguments<Options>(args);
            if (options_result.Errors.Any()) throw new Exception("There are errors on the parameters.");
            else if (options_result.Value != null) options = options_result.Value;

            Octokit.GitHubClient client = new GitHubClient(new ProductHeaderValue("regex-on-repo"));
            client.Credentials = new Credentials(options.GithubToken);

            string _owner = options.Repo.Split('/')[0];
            string _repo = options.Repo.Split('/')[1];

            Repository repo = Task.Run(() => client.Repository.Get(_owner, _repo)).Result;

            Regex reg = new Regex(options.RegexString, RegexOptions.Compiled);

            var content = Task.Run(() => client.Repository.Content.GetAllContentsByRef(repo.Id, options.FilePath, options.Ref)).Result[0];

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
            if (options.Debug)
            {
                Console.WriteLine("DEBUG OUTPUT START");
                Console.WriteLine(sb.ToString());
                Console.WriteLine("DEBUG OUTPUT END");
            }
            File.WriteAllText(outputFile, sb.ToString());
        }
    }
}
