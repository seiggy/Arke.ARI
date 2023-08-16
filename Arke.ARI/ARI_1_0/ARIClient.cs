 /*
	Arke ARI Framework
	Automatically generated file @ 8/16/2023 10:25:28 AM
*/
using System;
using System.Collections.Generic;
using Arke.ARI.Models;
using System.Threading.Tasks;

namespace Arke.ARI
{
	public delegate Task DeviceStateChangedEventHandler(IAriClient sender, DeviceStateChangedEvent e);
	public delegate Task PlaybackStartedEventHandler(IAriClient sender, PlaybackStartedEvent e);
	public delegate Task PlaybackContinuingEventHandler(IAriClient sender, PlaybackContinuingEvent e);
	public delegate Task PlaybackFinishedEventHandler(IAriClient sender, PlaybackFinishedEvent e);
	public delegate Task RecordingStartedEventHandler(IAriClient sender, RecordingStartedEvent e);
	public delegate Task RecordingFinishedEventHandler(IAriClient sender, RecordingFinishedEvent e);
	public delegate Task RecordingFailedEventHandler(IAriClient sender, RecordingFailedEvent e);
	public delegate Task ApplicationMoveFailedEventHandler(IAriClient sender, ApplicationMoveFailedEvent e);
	public delegate Task ApplicationReplacedEventHandler(IAriClient sender, ApplicationReplacedEvent e);
	public delegate Task BridgeCreatedEventHandler(IAriClient sender, BridgeCreatedEvent e);
	public delegate Task BridgeDestroyedEventHandler(IAriClient sender, BridgeDestroyedEvent e);
	public delegate Task BridgeMergedEventHandler(IAriClient sender, BridgeMergedEvent e);
	public delegate Task BridgeVideoSourceChangedEventHandler(IAriClient sender, BridgeVideoSourceChangedEvent e);
	public delegate Task BridgeBlindTransferEventHandler(IAriClient sender, BridgeBlindTransferEvent e);
	public delegate Task BridgeAttendedTransferEventHandler(IAriClient sender, BridgeAttendedTransferEvent e);
	public delegate Task ChannelCreatedEventHandler(IAriClient sender, ChannelCreatedEvent e);
	public delegate Task ChannelDestroyedEventHandler(IAriClient sender, ChannelDestroyedEvent e);
	public delegate Task ChannelEnteredBridgeEventHandler(IAriClient sender, ChannelEnteredBridgeEvent e);
	public delegate Task ChannelLeftBridgeEventHandler(IAriClient sender, ChannelLeftBridgeEvent e);
	public delegate Task ChannelStateChangeEventHandler(IAriClient sender, ChannelStateChangeEvent e);
	public delegate Task ChannelDtmfReceivedEventHandler(IAriClient sender, ChannelDtmfReceivedEvent e);
	public delegate Task ChannelDialplanEventHandler(IAriClient sender, ChannelDialplanEvent e);
	public delegate Task ChannelCallerIdEventHandler(IAriClient sender, ChannelCallerIdEvent e);
	public delegate Task ChannelUsereventEventHandler(IAriClient sender, ChannelUsereventEvent e);
	public delegate Task ChannelHangupRequestEventHandler(IAriClient sender, ChannelHangupRequestEvent e);
	public delegate Task ChannelVarsetEventHandler(IAriClient sender, ChannelVarsetEvent e);
	public delegate Task ChannelHoldEventHandler(IAriClient sender, ChannelHoldEvent e);
	public delegate Task ChannelUnholdEventHandler(IAriClient sender, ChannelUnholdEvent e);
	public delegate Task ChannelTalkingStartedEventHandler(IAriClient sender, ChannelTalkingStartedEvent e);
	public delegate Task ChannelTalkingFinishedEventHandler(IAriClient sender, ChannelTalkingFinishedEvent e);
	public delegate Task ContactStatusChangeEventHandler(IAriClient sender, ContactStatusChangeEvent e);
	public delegate Task PeerStatusChangeEventHandler(IAriClient sender, PeerStatusChangeEvent e);
	public delegate Task EndpointStateChangeEventHandler(IAriClient sender, EndpointStateChangeEvent e);
	public delegate Task DialEventHandler(IAriClient sender, DialEvent e);
	public delegate Task StasisEndEventHandler(IAriClient sender, StasisEndEvent e);
	public delegate Task StasisStartEventHandler(IAriClient sender, StasisStartEvent e);
	public delegate Task TextMessageReceivedEventHandler(IAriClient sender, TextMessageReceivedEvent e);
	public delegate Task ChannelConnectedLineEventHandler(IAriClient sender, ChannelConnectedLineEvent e);
	public delegate Task UnhandledEventHandler(object sender, Arke.ARI.Models.Event eventMessage);


	public interface IAriEventClient
	{
		event DeviceStateChangedEventHandler OnDeviceStateChangedEvent;
		event PlaybackStartedEventHandler OnPlaybackStartedEvent;
		event PlaybackContinuingEventHandler OnPlaybackContinuingEvent;
		event PlaybackFinishedEventHandler OnPlaybackFinishedEvent;
		event RecordingStartedEventHandler OnRecordingStartedEvent;
		event RecordingFinishedEventHandler OnRecordingFinishedEvent;
		event RecordingFailedEventHandler OnRecordingFailedEvent;
		event ApplicationMoveFailedEventHandler OnApplicationMoveFailedEvent;
		event ApplicationReplacedEventHandler OnApplicationReplacedEvent;
		event BridgeCreatedEventHandler OnBridgeCreatedEvent;
		event BridgeDestroyedEventHandler OnBridgeDestroyedEvent;
		event BridgeMergedEventHandler OnBridgeMergedEvent;
		event BridgeVideoSourceChangedEventHandler OnBridgeVideoSourceChangedEvent;
		event BridgeBlindTransferEventHandler OnBridgeBlindTransferEvent;
		event BridgeAttendedTransferEventHandler OnBridgeAttendedTransferEvent;
		event ChannelCreatedEventHandler OnChannelCreatedEvent;
		event ChannelDestroyedEventHandler OnChannelDestroyedEvent;
		event ChannelEnteredBridgeEventHandler OnChannelEnteredBridgeEvent;
		event ChannelLeftBridgeEventHandler OnChannelLeftBridgeEvent;
		event ChannelStateChangeEventHandler OnChannelStateChangeEvent;
		event ChannelDtmfReceivedEventHandler OnChannelDtmfReceivedEvent;
		event ChannelDialplanEventHandler OnChannelDialplanEvent;
		event ChannelCallerIdEventHandler OnChannelCallerIdEvent;
		event ChannelUsereventEventHandler OnChannelUsereventEvent;
		event ChannelHangupRequestEventHandler OnChannelHangupRequestEvent;
		event ChannelVarsetEventHandler OnChannelVarsetEvent;
		event ChannelHoldEventHandler OnChannelHoldEvent;
		event ChannelUnholdEventHandler OnChannelUnholdEvent;
		event ChannelTalkingStartedEventHandler OnChannelTalkingStartedEvent;
		event ChannelTalkingFinishedEventHandler OnChannelTalkingFinishedEvent;
		event ContactStatusChangeEventHandler OnContactStatusChangeEvent;
		event PeerStatusChangeEventHandler OnPeerStatusChangeEvent;
		event EndpointStateChangeEventHandler OnEndpointStateChangeEvent;
		event DialEventHandler OnDialEvent;
		event StasisEndEventHandler OnStasisEndEvent;
		event StasisStartEventHandler OnStasisStartEvent;
		event TextMessageReceivedEventHandler OnTextMessageReceivedEvent;
		event ChannelConnectedLineEventHandler OnChannelConnectedLineEvent;
		event UnhandledEventHandler OnUnhandledEvent;
	}


	/// <summary>
	/// 
	/// </summary>
	public class BaseAriClient : IAriEventClient
	{

		#region Events
					public virtual event DeviceStateChangedEventHandler OnDeviceStateChangedEvent;
			public virtual event PlaybackStartedEventHandler OnPlaybackStartedEvent;
			public virtual event PlaybackContinuingEventHandler OnPlaybackContinuingEvent;
			public virtual event PlaybackFinishedEventHandler OnPlaybackFinishedEvent;
			public virtual event RecordingStartedEventHandler OnRecordingStartedEvent;
			public virtual event RecordingFinishedEventHandler OnRecordingFinishedEvent;
			public virtual event RecordingFailedEventHandler OnRecordingFailedEvent;
			public virtual event ApplicationMoveFailedEventHandler OnApplicationMoveFailedEvent;
			public virtual event ApplicationReplacedEventHandler OnApplicationReplacedEvent;
			public virtual event BridgeCreatedEventHandler OnBridgeCreatedEvent;
			public virtual event BridgeDestroyedEventHandler OnBridgeDestroyedEvent;
			public virtual event BridgeMergedEventHandler OnBridgeMergedEvent;
			public virtual event BridgeVideoSourceChangedEventHandler OnBridgeVideoSourceChangedEvent;
			public virtual event BridgeBlindTransferEventHandler OnBridgeBlindTransferEvent;
			public virtual event BridgeAttendedTransferEventHandler OnBridgeAttendedTransferEvent;
			public virtual event ChannelCreatedEventHandler OnChannelCreatedEvent;
			public virtual event ChannelDestroyedEventHandler OnChannelDestroyedEvent;
			public virtual event ChannelEnteredBridgeEventHandler OnChannelEnteredBridgeEvent;
			public virtual event ChannelLeftBridgeEventHandler OnChannelLeftBridgeEvent;
			public virtual event ChannelStateChangeEventHandler OnChannelStateChangeEvent;
			public virtual event ChannelDtmfReceivedEventHandler OnChannelDtmfReceivedEvent;
			public virtual event ChannelDialplanEventHandler OnChannelDialplanEvent;
			public virtual event ChannelCallerIdEventHandler OnChannelCallerIdEvent;
			public virtual event ChannelUsereventEventHandler OnChannelUsereventEvent;
			public virtual event ChannelHangupRequestEventHandler OnChannelHangupRequestEvent;
			public virtual event ChannelVarsetEventHandler OnChannelVarsetEvent;
			public virtual event ChannelHoldEventHandler OnChannelHoldEvent;
			public virtual event ChannelUnholdEventHandler OnChannelUnholdEvent;
			public virtual event ChannelTalkingStartedEventHandler OnChannelTalkingStartedEvent;
			public virtual event ChannelTalkingFinishedEventHandler OnChannelTalkingFinishedEvent;
			public virtual event ContactStatusChangeEventHandler OnContactStatusChangeEvent;
			public virtual event PeerStatusChangeEventHandler OnPeerStatusChangeEvent;
			public virtual event EndpointStateChangeEventHandler OnEndpointStateChangeEvent;
			public virtual event DialEventHandler OnDialEvent;
			public virtual event StasisEndEventHandler OnStasisEndEvent;
			public virtual event StasisStartEventHandler OnStasisStartEvent;
			public virtual event TextMessageReceivedEventHandler OnTextMessageReceivedEvent;
			public virtual event ChannelConnectedLineEventHandler OnChannelConnectedLineEvent;
			public virtual event UnhandledEventHandler OnUnhandledEvent; 
			public virtual event UnhandledExceptionEventHandler OnUnhandledException;
		#endregion

		protected bool UnhandledException(object sender, Exception exception)
		{
			if (OnUnhandledException != null)
			{
				OnUnhandledException(sender, new UnhandledExceptionEventArgs(exception, false));
				return true;
			}
			return false;
		}
		
		protected void FireEvent(string eventName, object eventArgs, IAriClient sender)
		{
		
			switch(eventName) 
			{
			
			
				case "DeviceStateChanged":
					if(OnDeviceStateChangedEvent != null)
						OnDeviceStateChangedEvent(sender, (DeviceStateChangedEvent)eventArgs);
					else if (OnUnhandledEvent != null) OnUnhandledEvent(sender, (Event) eventArgs);
					break;
			
			
				case "PlaybackStarted":
					if(OnPlaybackStartedEvent != null)
						OnPlaybackStartedEvent(sender, (PlaybackStartedEvent)eventArgs);
					else if (OnUnhandledEvent != null) OnUnhandledEvent(sender, (Event) eventArgs);
					break;
			
			
				case "PlaybackContinuing":
					if(OnPlaybackContinuingEvent != null)
						OnPlaybackContinuingEvent(sender, (PlaybackContinuingEvent)eventArgs);
					else if (OnUnhandledEvent != null) OnUnhandledEvent(sender, (Event) eventArgs);
					break;
			
			
				case "PlaybackFinished":
					if(OnPlaybackFinishedEvent != null)
						OnPlaybackFinishedEvent(sender, (PlaybackFinishedEvent)eventArgs);
					else if (OnUnhandledEvent != null) OnUnhandledEvent(sender, (Event) eventArgs);
					break;
			
			
				case "RecordingStarted":
					if(OnRecordingStartedEvent != null)
						OnRecordingStartedEvent(sender, (RecordingStartedEvent)eventArgs);
					else if (OnUnhandledEvent != null) OnUnhandledEvent(sender, (Event) eventArgs);
					break;
			
			
				case "RecordingFinished":
					if(OnRecordingFinishedEvent != null)
						OnRecordingFinishedEvent(sender, (RecordingFinishedEvent)eventArgs);
					else if (OnUnhandledEvent != null) OnUnhandledEvent(sender, (Event) eventArgs);
					break;
			
			
				case "RecordingFailed":
					if(OnRecordingFailedEvent != null)
						OnRecordingFailedEvent(sender, (RecordingFailedEvent)eventArgs);
					else if (OnUnhandledEvent != null) OnUnhandledEvent(sender, (Event) eventArgs);
					break;
			
			
				case "ApplicationMoveFailed":
					if(OnApplicationMoveFailedEvent != null)
						OnApplicationMoveFailedEvent(sender, (ApplicationMoveFailedEvent)eventArgs);
					else if (OnUnhandledEvent != null) OnUnhandledEvent(sender, (Event) eventArgs);
					break;
			
			
				case "ApplicationReplaced":
					if(OnApplicationReplacedEvent != null)
						OnApplicationReplacedEvent(sender, (ApplicationReplacedEvent)eventArgs);
					else if (OnUnhandledEvent != null) OnUnhandledEvent(sender, (Event) eventArgs);
					break;
			
			
				case "BridgeCreated":
					if(OnBridgeCreatedEvent != null)
						OnBridgeCreatedEvent(sender, (BridgeCreatedEvent)eventArgs);
					else if (OnUnhandledEvent != null) OnUnhandledEvent(sender, (Event) eventArgs);
					break;
			
			
				case "BridgeDestroyed":
					if(OnBridgeDestroyedEvent != null)
						OnBridgeDestroyedEvent(sender, (BridgeDestroyedEvent)eventArgs);
					else if (OnUnhandledEvent != null) OnUnhandledEvent(sender, (Event) eventArgs);
					break;
			
			
				case "BridgeMerged":
					if(OnBridgeMergedEvent != null)
						OnBridgeMergedEvent(sender, (BridgeMergedEvent)eventArgs);
					else if (OnUnhandledEvent != null) OnUnhandledEvent(sender, (Event) eventArgs);
					break;
			
			
				case "BridgeVideoSourceChanged":
					if(OnBridgeVideoSourceChangedEvent != null)
						OnBridgeVideoSourceChangedEvent(sender, (BridgeVideoSourceChangedEvent)eventArgs);
					else if (OnUnhandledEvent != null) OnUnhandledEvent(sender, (Event) eventArgs);
					break;
			
			
				case "BridgeBlindTransfer":
					if(OnBridgeBlindTransferEvent != null)
						OnBridgeBlindTransferEvent(sender, (BridgeBlindTransferEvent)eventArgs);
					else if (OnUnhandledEvent != null) OnUnhandledEvent(sender, (Event) eventArgs);
					break;
			
			
				case "BridgeAttendedTransfer":
					if(OnBridgeAttendedTransferEvent != null)
						OnBridgeAttendedTransferEvent(sender, (BridgeAttendedTransferEvent)eventArgs);
					else if (OnUnhandledEvent != null) OnUnhandledEvent(sender, (Event) eventArgs);
					break;
			
			
				case "ChannelCreated":
					if(OnChannelCreatedEvent != null)
						OnChannelCreatedEvent(sender, (ChannelCreatedEvent)eventArgs);
					else if (OnUnhandledEvent != null) OnUnhandledEvent(sender, (Event) eventArgs);
					break;
			
			
				case "ChannelDestroyed":
					if(OnChannelDestroyedEvent != null)
						OnChannelDestroyedEvent(sender, (ChannelDestroyedEvent)eventArgs);
					else if (OnUnhandledEvent != null) OnUnhandledEvent(sender, (Event) eventArgs);
					break;
			
			
				case "ChannelEnteredBridge":
					if(OnChannelEnteredBridgeEvent != null)
						OnChannelEnteredBridgeEvent(sender, (ChannelEnteredBridgeEvent)eventArgs);
					else if (OnUnhandledEvent != null) OnUnhandledEvent(sender, (Event) eventArgs);
					break;
			
			
				case "ChannelLeftBridge":
					if(OnChannelLeftBridgeEvent != null)
						OnChannelLeftBridgeEvent(sender, (ChannelLeftBridgeEvent)eventArgs);
					else if (OnUnhandledEvent != null) OnUnhandledEvent(sender, (Event) eventArgs);
					break;
			
			
				case "ChannelStateChange":
					if(OnChannelStateChangeEvent != null)
						OnChannelStateChangeEvent(sender, (ChannelStateChangeEvent)eventArgs);
					else if (OnUnhandledEvent != null) OnUnhandledEvent(sender, (Event) eventArgs);
					break;
			
			
				case "ChannelDtmfReceived":
					if(OnChannelDtmfReceivedEvent != null)
						OnChannelDtmfReceivedEvent(sender, (ChannelDtmfReceivedEvent)eventArgs);
					else if (OnUnhandledEvent != null) OnUnhandledEvent(sender, (Event) eventArgs);
					break;
			
			
				case "ChannelDialplan":
					if(OnChannelDialplanEvent != null)
						OnChannelDialplanEvent(sender, (ChannelDialplanEvent)eventArgs);
					else if (OnUnhandledEvent != null) OnUnhandledEvent(sender, (Event) eventArgs);
					break;
			
			
				case "ChannelCallerId":
					if(OnChannelCallerIdEvent != null)
						OnChannelCallerIdEvent(sender, (ChannelCallerIdEvent)eventArgs);
					else if (OnUnhandledEvent != null) OnUnhandledEvent(sender, (Event) eventArgs);
					break;
			
			
				case "ChannelUserevent":
					if(OnChannelUsereventEvent != null)
						OnChannelUsereventEvent(sender, (ChannelUsereventEvent)eventArgs);
					else if (OnUnhandledEvent != null) OnUnhandledEvent(sender, (Event) eventArgs);
					break;
			
			
				case "ChannelHangupRequest":
					if(OnChannelHangupRequestEvent != null)
						OnChannelHangupRequestEvent(sender, (ChannelHangupRequestEvent)eventArgs);
					else if (OnUnhandledEvent != null) OnUnhandledEvent(sender, (Event) eventArgs);
					break;
			
			
				case "ChannelVarset":
					if(OnChannelVarsetEvent != null)
						OnChannelVarsetEvent(sender, (ChannelVarsetEvent)eventArgs);
					else if (OnUnhandledEvent != null) OnUnhandledEvent(sender, (Event) eventArgs);
					break;
			
			
				case "ChannelHold":
					if(OnChannelHoldEvent != null)
						OnChannelHoldEvent(sender, (ChannelHoldEvent)eventArgs);
					else if (OnUnhandledEvent != null) OnUnhandledEvent(sender, (Event) eventArgs);
					break;
			
			
				case "ChannelUnhold":
					if(OnChannelUnholdEvent != null)
						OnChannelUnholdEvent(sender, (ChannelUnholdEvent)eventArgs);
					else if (OnUnhandledEvent != null) OnUnhandledEvent(sender, (Event) eventArgs);
					break;
			
			
				case "ChannelTalkingStarted":
					if(OnChannelTalkingStartedEvent != null)
						OnChannelTalkingStartedEvent(sender, (ChannelTalkingStartedEvent)eventArgs);
					else if (OnUnhandledEvent != null) OnUnhandledEvent(sender, (Event) eventArgs);
					break;
			
			
				case "ChannelTalkingFinished":
					if(OnChannelTalkingFinishedEvent != null)
						OnChannelTalkingFinishedEvent(sender, (ChannelTalkingFinishedEvent)eventArgs);
					else if (OnUnhandledEvent != null) OnUnhandledEvent(sender, (Event) eventArgs);
					break;
			
			
				case "ContactStatusChange":
					if(OnContactStatusChangeEvent != null)
						OnContactStatusChangeEvent(sender, (ContactStatusChangeEvent)eventArgs);
					else if (OnUnhandledEvent != null) OnUnhandledEvent(sender, (Event) eventArgs);
					break;
			
			
				case "PeerStatusChange":
					if(OnPeerStatusChangeEvent != null)
						OnPeerStatusChangeEvent(sender, (PeerStatusChangeEvent)eventArgs);
					else if (OnUnhandledEvent != null) OnUnhandledEvent(sender, (Event) eventArgs);
					break;
			
			
				case "EndpointStateChange":
					if(OnEndpointStateChangeEvent != null)
						OnEndpointStateChangeEvent(sender, (EndpointStateChangeEvent)eventArgs);
					else if (OnUnhandledEvent != null) OnUnhandledEvent(sender, (Event) eventArgs);
					break;
			
			
				case "Dial":
					if(OnDialEvent != null)
						OnDialEvent(sender, (DialEvent)eventArgs);
					else if (OnUnhandledEvent != null) OnUnhandledEvent(sender, (Event) eventArgs);
					break;
			
			
				case "StasisEnd":
					if(OnStasisEndEvent != null)
						OnStasisEndEvent(sender, (StasisEndEvent)eventArgs);
					else if (OnUnhandledEvent != null) OnUnhandledEvent(sender, (Event) eventArgs);
					break;
			
			
				case "StasisStart":
					if(OnStasisStartEvent != null)
						OnStasisStartEvent(sender, (StasisStartEvent)eventArgs);
					else if (OnUnhandledEvent != null) OnUnhandledEvent(sender, (Event) eventArgs);
					break;
			
			
				case "TextMessageReceived":
					if(OnTextMessageReceivedEvent != null)
						OnTextMessageReceivedEvent(sender, (TextMessageReceivedEvent)eventArgs);
					else if (OnUnhandledEvent != null) OnUnhandledEvent(sender, (Event) eventArgs);
					break;
			
			
				case "ChannelConnectedLine":
					if(OnChannelConnectedLineEvent != null)
						OnChannelConnectedLineEvent(sender, (ChannelConnectedLineEvent)eventArgs);
					else if (OnUnhandledEvent != null) OnUnhandledEvent(sender, (Event) eventArgs);
					break;
				default:
					if(OnUnhandledEvent!=null)
						OnUnhandledEvent(this, (Event)eventArgs);
					else if (OnUnhandledEvent != null) OnUnhandledEvent(sender, (Event) eventArgs);
					break;
			}
		}
	}
}
