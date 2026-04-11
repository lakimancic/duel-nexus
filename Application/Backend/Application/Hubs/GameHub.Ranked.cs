using Backend.Utils.WebApi;
using Microsoft.AspNetCore.SignalR;

public partial class GameHub
{
    [HubMethodName("ranked:queue:join")]
    public async Task JoinRankedQueue()
    {
        try
        {
            var userId = GetUserId();
            await RankedMatchmaking.JoinQueue(userId);
        }
        catch (BadRequestException exception)
        {
            throw new HubException(exception.Message);
        }
        catch (ObjectNotFoundException exception)
        {
            throw new HubException(exception.Message);
        }
    }

    [HubMethodName("ranked:queue:leave")]
    public async Task LeaveRankedQueue()
    {
        var userId = GetUserId();
        await RankedMatchmaking.LeaveQueue(userId);
    }
}
