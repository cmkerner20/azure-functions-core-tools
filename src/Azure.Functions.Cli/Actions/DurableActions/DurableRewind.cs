﻿using System;
using System.Threading.Tasks;
using Fclp;
using Azure.Functions.Cli.Interfaces;
using Azure.Functions.Cli.Common;


namespace Azure.Functions.Cli.Actions.DurableActions
{
    [Action(Name = "rewind", Context = Context.Durable, HelpText = "Rewinds a specified orchestration instance")]
    class DurableRewind : BaseAction
    {
        public string Version { get; set; }

        public string Instance { get; set; }

        public string Reason { get; set; }

        private readonly ISecretsManager _secretsManager;
        private readonly DurableManager durableManager;

        public DurableRewind(ISecretsManager secretsManager)
        {
            _secretsManager = secretsManager;
            durableManager = new DurableManager(secretsManager);
        }


        public override ICommandLineParserResult ParseArgs(string[] args)
        {
            var parser = new FluentCommandLineParser();
            parser
                .Setup<string>("version")
                .WithDescription("This shows up in the help next to the version option.")
                .SetDefault(string.Empty)
                .Callback(v => Version = v);

            parser
                .Setup<string>("instance")
                .WithDescription("This specifies the id of a failed orchestration to rewind")
                .SetDefault(null)
                .Callback(i => Instance = i);

            parser
                 .Setup<string>("reason")
                 .WithDescription("This specifies the reason for rewinding the orchestration")
                 .SetDefault("Instance termination.")
                 .Callback(r => Reason = r);

            return parser.Parse(args);
        }

        public override async Task RunAsync()
        {
            await durableManager.Rewind(Instance, Reason);
        }
    }
}