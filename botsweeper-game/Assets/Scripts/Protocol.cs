using System.Collections.Generic;

namespace Protocol
{
	public enum BaseMessageType
	{
		ListGamesRequest = 1,
		ListGamesResponse,

		CreateGameRequest,
		CreateGameResponse,

		RemoveGameRequest,
		RemoveGameResponse,

		EndpointMessage,
		EndpointMessageError,
	}

	public enum MessageType
	{
		JoinGameRequest = 1,
		JoinGameResponse,

		LeaveGameRequest,
		LeaveGameResponse,

		VoteAvailable,
		VoteResult,
		Vote,

		MineSweepResult,
	}

	//public class Message
	//{
	//	public string from_id { get; set; }
	//	public string to_id { get; set; }
	//	public Dictionary<object, object> message { get; set; }
 //   }

 //   public class BaseMessage
 //   {
 //       public BaseMessage()
 //       {
 //           type = BaseMessageType.EndpointMessage;
 //       }

 //       public BaseMessageType type { get; set; }

 //       public string name { get; set; }
 //       public Message message { get; set; }
 //   }

    public class Message : Dictionary<object, object> { }
}