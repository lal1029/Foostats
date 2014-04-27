using System.Net;
using Foostats2Contracts;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Threading.Tasks;

namespace Foostats2ApiSampleClient
{
    public class Foostats2ApiClient
    {
        private readonly WebHttpBinding _binding = null;
        private readonly Uri _endpoint = null;
        private readonly ChannelFactory<IFoostats> _channelFactory = null;

        private static WebHttpBinding CreateWebHttpBinding()
        {
            WebHttpBinding binding = null;
            binding = new WebHttpBinding(WebHttpSecurityMode.None);
            return binding;
        }

        public Foostats2ApiClient(Uri endpoint)
        {
            _binding = CreateWebHttpBinding();
            _endpoint = endpoint;
            var endpointAddress = new EndpointAddress(_endpoint.ToString());
            _channelFactory = new ChannelFactory<IFoostats>(_binding, endpointAddress);
            _channelFactory.Endpoint.Behaviors.Add(new WebHttpBehavior());
        }

        public async Task<IEnumerable<Player>> ListAllPlayersAsync()
        {
            var foostatsChannel = _channelFactory.CreateChannel();
            var task = await Task<IEnumerable<Player>>.Factory.FromAsync(
                                                                foostatsChannel.BeginListAllPlayers, 
                                                                foostatsChannel.EndListAllPlayers, 
                                                                foostatsChannel);
            return task;
        }

        public IEnumerable<Player> ListAllPlayers()
        {
            var foostatsChannel = _channelFactory.CreateChannel();
            var async = foostatsChannel.BeginListAllPlayers(null, null);
            return foostatsChannel.EndListAllPlayers(async);
        }

        public async Task<IEnumerable<ExtendedPlayer>> ListPlayersAsync(ListAllInput input)
        {
            var foostatsChannel = _channelFactory.CreateChannel();
            var task = await Task<IEnumerable<ExtendedPlayer>>.Factory.FromAsync(
                                                                        foostatsChannel.BeginListPlayers, 
                                                                        foostatsChannel.EndListPlayers, 
                                                                        input, 
                                                                        foostatsChannel);
            return task;
        }

        public IEnumerable<ExtendedPlayer> ListPlayers(ListAllInput input)
        {
            var foostatsChannel = _channelFactory.CreateChannel();
            var async = foostatsChannel.BeginListPlayers(input, null, null);
            return foostatsChannel.EndListPlayers(async);
        }

        public async Task<ExtendedPlayer> GetPlayerByAliasAsync(string alias)
        {
            var foostatsChannel = _channelFactory.CreateChannel();
            var task = await Task<ExtendedPlayer>.Factory.FromAsync(
                                                            foostatsChannel.BeginGetPlayerByAlias,
                                                            foostatsChannel.EndGetPlayerByAlias,
                                                            alias,
                                                            foostatsChannel);
            return task;
        }

        public ExtendedPlayer GetPlayerByAlias(string alias)
        {
            var foostatsChannel = _channelFactory.CreateChannel();
            var async = foostatsChannel.BeginGetPlayerByAlias(alias, null, null);
            return foostatsChannel.EndGetPlayerByAlias(async);
        }

        public async Task<IEnumerable<ExtendedPlayer>> GetPlayersByDisplayNameAsync(string displayName)
        {
            var foostatsChannel = _channelFactory.CreateChannel();
            var task = await Task<IEnumerable<ExtendedPlayer>>.Factory.FromAsync(
                                                            foostatsChannel.BeginGetPlayerByDisplayName,
                                                            foostatsChannel.EndGetPlayerByDisplayName,
                                                            displayName,
                                                            foostatsChannel);
            return task;
        }

        public IEnumerable<ExtendedPlayer> GetPlayersByDisplayName(string displayName)
        {
            var foostatsChannel = _channelFactory.CreateChannel();
            var async = foostatsChannel.BeginGetPlayerByDisplayName(displayName, null, null);
            return foostatsChannel.EndGetPlayerByDisplayName(async);
        }

        public IEnumerable<Match> ListAllMatches()
        {
            var foostatsChannel = _channelFactory.CreateChannel();
            var async = foostatsChannel.BeginListAllMatches(null, null);
            return foostatsChannel.EndListAllMatches(async);
        }

        public async Task<IEnumerable<Match>> ListAllMatchesAsync()
        {
            var foostatsChannel = _channelFactory.CreateChannel();
            var task = await Task<IEnumerable<Match>>.Factory.FromAsync(
                                                                    foostatsChannel.BeginListAllMatches,
                                                                    foostatsChannel.EndListAllMatches,
                                                                    foostatsChannel);
            return task;
        }

        public IEnumerable<Match> ListMatches(ListAllInput input)
        {
            var foostatsChannel = _channelFactory.CreateChannel();
            var async = foostatsChannel.BeginListMatches(input, null, null);
            return foostatsChannel.EndListMatches(async);
        }

        public async Task<IEnumerable<Match>> ListMatchesAsync(ListAllInput input)
        {
            var foostatsChannel = _channelFactory.CreateChannel();
            var task = await Task<IEnumerable<Match>>.Factory.FromAsync(
                                                                    foostatsChannel.BeginListMatches,
                                                                    foostatsChannel.EndListMatches,
                                                                    input,
                                                                    foostatsChannel);
            return task;
        }

        public void AddMatch(MatchInput input)
        {
            var foostatsChannel = _channelFactory.CreateChannel();
            var async = foostatsChannel.BeginAddMatch(input, null, null);
            foostatsChannel.EndAddMatch(async);
        }

        public async Task AddMatchAsnyc(MatchInput input)
        {
            var foostatsChannel = _channelFactory.CreateChannel();
            await Task.Factory.FromAsync(
                                foostatsChannel.BeginAddMatch,
                                foostatsChannel.EndAddMatch,
                                input,
                                foostatsChannel);
        }
    }
}
