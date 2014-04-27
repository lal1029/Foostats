using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Foostats2Contracts
{
    [ServiceContract]
    public interface IFoostats
    {
        [OperationContract(AsyncPattern = true)]
        [WebInvoke(Method = "GET", UriTemplate = "PlayerApi/ListAll")]
        IAsyncResult BeginListAllPlayers(AsyncCallback callback, object state);

        IEnumerable<ExtendedPlayer> EndListAllPlayers(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [WebInvoke(Method = "POST", UriTemplate = "PlayerApi/List")]
        IAsyncResult BeginListPlayers(ListAllInput input, AsyncCallback callback, object state);

        IEnumerable<ExtendedPlayer> EndListPlayers(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [WebInvoke(Method = "GET", UriTemplate = "PlayerApi/GetPlayerByAlias?alias={alias}")]
        IAsyncResult BeginGetPlayerByAlias(string alias, AsyncCallback callback, object state);

        ExtendedPlayer EndGetPlayerByAlias(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [WebInvoke(Method = "GET", UriTemplate = "PlayerApi/GetPlayerByDisplayName?displayName={displayName}")]
        IAsyncResult BeginGetPlayerByDisplayName(string displayName, AsyncCallback callback, object state);

        IEnumerable<ExtendedPlayer> EndGetPlayerByDisplayName(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [WebInvoke(Method = "GET", UriTemplate = "MatchApi/ListAll")]
        IAsyncResult BeginListAllMatches(AsyncCallback callback, object state);

        IEnumerable<Match> EndListAllMatches(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [WebInvoke(Method = "POST", UriTemplate = "MatchApi/List")]
        IAsyncResult BeginListMatches(ListAllInput input, AsyncCallback callback, object state);

        IEnumerable<Match> EndListMatches(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [WebInvoke(Method = "POST", UriTemplate = "MatchApi/AddMatch")]
        IAsyncResult BeginAddMatch(MatchInput input, AsyncCallback callback, object state);

        void EndAddMatch(IAsyncResult asyncResult);
    }
}
