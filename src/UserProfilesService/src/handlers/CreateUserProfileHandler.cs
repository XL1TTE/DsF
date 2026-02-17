using Messages;
using Wolverine;

namespace Handlers;

public static class CreateUserProfileHandler{
    
    public static async Task<OutgoingMessages> Consume(CreateUserProfile message)
    {
        var result = new UserProfileCreated(Guid.NewGuid().ToString());
        
        var outgoing = new OutgoingMessages{result};
    
        outgoing.RespondToSender(result); 
        
        return outgoing;
    }   
}
