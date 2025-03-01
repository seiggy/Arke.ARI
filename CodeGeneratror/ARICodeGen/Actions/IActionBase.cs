namespace Arke.ARI.Actions
{
    /// <summary>
    /// Base interface for all ARI action classes
    /// </summary>
    public interface IActionBase
    {
    }

    /// <summary>
    /// Interface for Asterisk-related actions
    /// </summary>
    public interface IAsteriskActions : IActionBase
    {
    }

    /// <summary>
    /// Interface for Application-related actions
    /// </summary>
    public interface IApplicationsActions : IActionBase
    {
    }

    /// <summary>
    /// Interface for Bridge-related actions
    /// </summary>
    public interface IBridgesActions : IActionBase
    {
    }

    /// <summary>
    /// Interface for Channel-related actions
    /// </summary>
    public interface IChannelsActions : IActionBase
    {
    }

    /// <summary>
    /// Interface for DeviceState-related actions
    /// </summary>
    public interface IDeviceStatesActions : IActionBase
    {
    }

    /// <summary>
    /// Interface for Endpoint-related actions
    /// </summary>
    public interface IEndpointsActions : IActionBase
    {
    }

    /// <summary>
    /// Interface for Event-related actions
    /// </summary>
    public interface IEventsActions : IActionBase
    {
    }

    /// <summary>
    /// Interface for Mailbox-related actions
    /// </summary>
    public interface IMailboxesActions : IActionBase
    {
    }

    /// <summary>
    /// Interface for Playback-related actions
    /// </summary>
    public interface IPlaybacksActions : IActionBase
    {
    }

    /// <summary>
    /// Interface for Recording-related actions
    /// </summary>
    public interface IRecordingsActions : IActionBase
    {
    }

    /// <summary>
    /// Interface for Sound-related actions
    /// </summary>
    public interface ISoundsActions : IActionBase
    {
    }
} 