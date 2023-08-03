/*
   Arke ARI Framework
   Automatically generated file @ 8/2/2023 11:01:41 AM
*/
using System;
using System.Collections.Generic;
using Arke.ARI.Models;
using Arke.ARI;
using System.Threading.Tasks;

namespace Arke.ARI.Actions
{

    public interface IRecordingsActions
    {
        /// <summary>
        /// List recordings that are complete.. 
        /// </summary>
        Task<List<StoredRecording>> ListStoredAsync();
        /// <summary>
        /// Get a stored recording's details.. 
        /// </summary>
        /// <param name="recordingName">The name of the recording</param>
        Task<StoredRecording> GetStoredAsync(string recordingName);
        /// <summary>
        /// Delete a stored recording.. 
        /// </summary>
        /// <param name="recordingName">The name of the recording</param>
        Task DeleteStoredAsync(string recordingName);
        /// <summary>
        /// Get the file associated with the stored recording.. 
        /// </summary>
        /// <param name="recordingName">The name of the recording</param>
        Task<byte[]> GetStoredFileAsync(string recordingName);
        /// <summary>
        /// Copy a stored recording.. 
        /// </summary>
        /// <param name="recordingName">The name of the recording to copy</param>
        /// <param name="destinationRecordingName">The destination name of the recording</param>
        Task<StoredRecording> CopyStoredAsync(string recordingName, string destinationRecordingName);
        /// <summary>
        /// List live recordings.. 
        /// </summary>
        /// <param name="recordingName">The name of the recording</param>
        Task<LiveRecording> GetLiveAsync(string recordingName);
        /// <summary>
        /// Stop a live recording and discard it.. 
        /// </summary>
        /// <param name="recordingName">The name of the recording</param>
        Task CancelAsync(string recordingName);
        /// <summary>
        /// Stop a live recording and store it.. 
        /// </summary>
        /// <param name="recordingName">The name of the recording</param>
        Task StopAsync(string recordingName);
        /// <summary>
        /// Pause a live recording.. Pausing a recording suspends silence detection, which will be restarted when the recording is unpaused. Paused time is not included in the accounting for maxDurationSeconds.
        /// </summary>
        /// <param name="recordingName">The name of the recording</param>
        Task PauseAsync(string recordingName);
        /// <summary>
        /// Unpause a live recording.. 
        /// </summary>
        /// <param name="recordingName">The name of the recording</param>
        Task UnpauseAsync(string recordingName);
        /// <summary>
        /// Mute a live recording.. Muting a recording suspends silence detection, which will be restarted when the recording is unmuted.
        /// </summary>
        /// <param name="recordingName">The name of the recording</param>
        Task MuteAsync(string recordingName);
        /// <summary>
        /// Unmute a live recording.. 
        /// </summary>
        /// <param name="recordingName">The name of the recording</param>
        Task UnmuteAsync(string recordingName);
    }
}
