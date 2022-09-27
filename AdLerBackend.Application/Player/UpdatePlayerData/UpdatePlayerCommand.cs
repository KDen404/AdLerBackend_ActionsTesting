using AdLerBackend.Application.Common;
using AdLerBackend.Application.Common.Responses.Player;
using AdLerBackend.Domain.Entities.PlayerData;
using Microsoft.AspNetCore.JsonPatch;

namespace AdLerBackend.Application.Player.UpdatePlayerData;

public record UpdatePlayerCommand : CommandWithToken<PlayerDataResponse>
{
    public JsonPatchDocument<PlayerData> PatchDocument { get; init; }
}