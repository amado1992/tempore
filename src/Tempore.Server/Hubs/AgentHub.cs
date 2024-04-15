// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AgentHub.cs" company="Port Hope Investment S.A.">
// Copyright © 2023 - 2023 Port Hope Investment S.A. development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tempore.Server.Hubs
{
    /// <summary>
    /// The agent hub.
    /// </summary>
    [Microsoft.AspNetCore.Authorization.Authorize(Policy = Authorization.Roles.Roles.Agent.TemporeAgent)]
    public class AgentHub : Microsoft.AspNetCore.SignalR.Hub<Client.Services.Interfaces.IAgentReceiver>, Client.Services.Interfaces.IAgentHub
    {
        // private Dictionary<string, string> connections = new Dictionary<string, string>();

        /// <inheritdoc/>
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            // var feature = this.Context.Features.Get<IHttpConnectionFeature>();
            //// here you could get your client remote address
            // var remoteAddress = feature!.RemoteIpAddress?.ToString();
            // if (!string.IsNullOrEmpty(remoteAddress))
            // {
            //    this.connections.Remove(remoteAddress);
            // }
            await base.OnDisconnectedAsync(exception);
        }

        /// <inheritdoc/>
        public override async Task OnConnectedAsync()
        {
            // this.Context.
            // TODO: Check the session state on Keycloak?

            // var feature = Context.Features.Get<IHttpConnectionFeature>();
            // here you could get your client remote address
            // var remoteAddress = feature!.RemoteIpAddress?.ToString();
            // if (string.IsNullOrEmpty(remoteAddress) || this.connections.ContainsKey(remoteAddress))
            // {
            //    this.Context.Abort();
            //    return;
            // }
            await this.Clients.Caller.RegisterAsync(this.Context.ConnectionId);

            // this.connections[remoteAddress] = this.Context.ConnectionId;
            var connectionHeartbeatFeature = this.Context.Features.Get<Microsoft.AspNetCore.Connections.Features.IConnectionHeartbeatFeature>();
            if (connectionHeartbeatFeature is not null)
            {
                // TODO: Review this?
                // connectionHeartbeatFeature.OnHeartbeat(state =>
                //    {
                //        var context = (HubCallerContext)state;
                //        var expClaim = context.User?.FindFirst(claim => claim.Type == "exp");
                //        if (expClaim is not null)
                //        {
                //            var fromUnixTimeSeconds = DateTimeOffset.FromUnixTimeSeconds(int.Parse(expClaim.Value));
                //            if (fromUnixTimeSeconds < DateTimeOffset.UtcNow)
                //            {
                //                context.Abort();
                //            }
                //        }

                // }, this.Context);
            }

            await base.OnConnectedAsync();
        }
    }
}