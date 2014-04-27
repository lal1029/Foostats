# Foostats API
## Description
The foostats api allows you to interact with the foostats database through
RESTful calls to the foostats api controllers. Through the foostats api you can

* List all players
* Get a player by alias, including associated win/loss and trueskill data
* Get a player by their display name
* Retrieve a list of players, ordered by trueskill or name
* List all matches
* Retrieve a list of matches ordered by most recently played
* Retrieve a list of matches a particular player played in

## Player
### GET /api/PlayerApi/ListAll
Retrieves an array of all the players. See List below for example of the Player
object being returned.

### POST /api/PlayerApi/List
#### Description
Retrieves an array of players. An example player object is shown below

	{
	  "Trueskill":{
		 "Id":2,
		 "Player":(null),
		 "StandardDeviation":7.1714758070092195,
		 "Mean":20.604168307008482,
		 "ConservativeRating":-0.91025911401917625
	  },
	  "WinLoss":{
		 "Id":2,
		 "Player":(null),
		 "Win":0,
		 "Loss":1
	  },
	  "Id":2,
	  "DisplayName":"REDMOND\\japayne",
	  "MutableDisplayName":null,
	  "Password":null,
	  "Salt":null
	}

#### Sample Request

	POST http://localhost:25027/Api/PlayerApi/List HTTP/1.1
	Host: localhost:25027
	Content-Type: application/json

	{ OrderBy: "Alias", Desc: true, Limit: 5, StartIndex: 2, IncludeExtendedData: true }

The input contains an OrderBy column, the OrderBy values supported are 
DisplayName, Alias, Id. If IncludeExtendedData is true then you can also use the
values Trueskill and WinLoss. This Id is an internal Id that does not correspond 
to any external value. Sorting is done in either Descending or Ascending order,
determined by the Desc bool value. If ommited the default value is false and
the results will be in ascending order. If no OrderBy field is passed then 
Desc will have no effect. Limit will limit the result set to that many results.
Passing a StartIndex of a positive non-zero integer will result in the processor
skipping over StartIndex-1 results (the result set will start at that index).
For the purpose of pagination an example of usage would be setting the limit to
5 and on the second request using a StartIndex of 6 and incrementing the start
index each time. Note this is 1-indexed. IncludeExtendedData includes the 
trueskill and winloss data for a player.

### GET /api/PlayerApi/GetPlayerByAlias?alias=&lt;alias&gt;
#### Description
Retrieves a players data. Looks up a player based on Alias. If the player could
not be found then returns 404 with the reason "Player '{0}' could not be found".

### GET /api/GetPlayerByDisplayName?displayName=&lt;displayName&gt;
#### Description
Retrieves a list of players data based on the players display names. If no
players could be found returns an empty list.

## Matches
### GET /api/MatchApi/ListAll
Retrieves an array of all matches in the database, up to 1000 records. If over
1000 records would be returned, use /List with StartIndex and Limit to paginate
the data, or set a larger Limit (not recommended).

### POST /api/MatchApi/List
#### Description
Retrieves a list of matches that meet the critera, ordered in a particular 
fashion specified in the input. An example of a match object is shown below.
Note extended player data such as trueskill and win loss ratios are not shown.
To retrieve this data make a separate request to the PlayerApi.

	[
	   {
		  "Id":1,
		  "Team1":{
			 "Id":1,
			 "Player1":{
				"Id":1,
				"DisplayName":"REDMOND\\amayoub",
				"MutableDisplayName":null,
				"Password":null,
				"Salt":null
			 },
			 "Player2":null,
			 "DisplayName":"REDMOND\\amayoub"
		  },
		  "Team2":{
			 "Id":2,
			 "Player1":{
				"Id":2,
				"DisplayName":"REDMOND\\japayne",
				"MutableDisplayName":null,
				"Password":null,
				"Salt":null
			 },
			 "Player2":null,
			 "DisplayName":"REDMOND\\japayne"
		  },
		  "Score1":8,
		  "Score2":2,
		  "Team1Validated":"2013-11-08T23:59:13.68",
		  "Team2Validated":null
	   }
	]

#### Sample Request

	POST http://localhost:25027/Api/MatchApi/List HTTP/1.1
	Host: localhost:25027
	Content-Type: application/json

	{ OrderBy: "Date", Desc: true, Limit: 5, SearchKey: "Alias", SearchTerm: "REDMOND\\amayoub"}

There is only one valid OrderBy column so far which is Date, anything else
will result in an error. You can also omit date for now and the default
will be sorted by date. Desc does similar to players and will sort the matches
in descending order if true. Limit will limit the number of results and 
StartIndex will start the results at a specific index. The functionality of 
pagination is the same as Player /List. Readers are recommended to look over
the documentation for that for more details. So far the only supported search
keys are "Alias".

### POST /api/MatchApi/AddMatch
#### Description
Adds a match to the database, if the player doesn't exist yet (determined by
player alias) then the player is added to the database. This is the only way
to create a player.

#### Sample Request

	POST http://localhost:25027/Api/MatchApi/AddMatch HTTP/1.1
	Host: localhost:25027
	Content-Type: application/json

	{ 
		Team1Player1Alias: "REDMOND\\amayoub", 
		Team1Player2Alias: "REDMOND\\kasun",
		Team2Player1Alias: "REDMOND\\japayne",
		Team2Player2Alias: "REDMOND\\anhiggin",
		Team1Score: 2,
		Team2Score: 8
	}
