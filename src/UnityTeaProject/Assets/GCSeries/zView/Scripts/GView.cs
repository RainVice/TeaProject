using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using GCSeries.Core;
using F3Device.Device;

namespace GCSeries.zView
{
    public partial class GView : MonoBehaviour
    {
        //////////////////////////////////////////////////////////////////
        // Events
        //////////////////////////////////////////////////////////////////

        public delegate void EventHandler(GView sender, IntPtr connection);

        /// <summary>
        /// Event dispatched when a connection has been accepted.
        /// </summary>
        public event EventHandler ConnectionAccepted;

        /// <summary>
        /// Event dispatched when a connection has switched modes.
        /// </summary>
        public event EventHandler ConnectionModeSwitched;

#pragma warning disable 0067
        /// <summary>
        /// Event dispatched when a connection has transitioned
        /// to the ModeActive connection state.
        /// </summary>
        public event EventHandler ConnectionModeActive;

        /// <summary>
        /// Event dispatched when a connection has transitioned
        /// to the ModePaused connection state.
        /// </summary>
        public event EventHandler ConnectionModePaused;

        /// <summary>
        /// Event dispatched when a connection has been closed.
        /// </summary>
        public event EventHandler ConnectionClosed;

        /// <summary>
        /// Event dispatched when a connection error has occurred.
        /// </summary>
        public event EventHandler ConnectionError;

        /// <summary>
        /// Event dispatched when video recording has transitioned to
        /// an inactive state.
        /// </summary>
        public event EventHandler VideoRecordingInactive;

        /// <summary>
        /// Event dispatched when video recording has transitioned to 
        /// an active recording state.
        /// </summary>
        public event EventHandler VideoRecordingActive;

        /// <summary>
        /// Event dispatched when video recording has finished.
        /// </summary>
        public event EventHandler VideoRecordingFinished;

        /// <summary>
        /// Event dispatched when video recording has paused.
        /// </summary>
        public event EventHandler VideoRecordingPaused;

        /// <summary>
        /// Event dispatched when a video recording error has occurred.
        /// </summary>
        public event EventHandler VideoRecordingError;

        /// <summary>
        /// Event dispatched when the video recording quality has changed.
        /// </summary>
        public event EventHandler VideoRecordingQualityChanged;


        //////////////////////////////////////////////////////////////////
        // Enumerations
        //////////////////////////////////////////////////////////////////

        /// <summary>
        /// Defines the types of zView nodes that can exist.
        /// </summary>
        public enum NodeType
        {
            Presenter = 0,
            Viewer = 1,
        }

        /// <summary>
        /// Defines the availability states for zView modes.
        /// </summary>
        public enum ModeAvailability
        {
            /// <summary>
            /// Mode is available.
            /// </summary>
            Available = 0,

            /// <summary>
            /// Mode is not available.
            /// </summary>
            NotAvailable = 1,

            /// <summary>
            /// Mode is not available because no webcam hardware is available.
            /// </summary>
            NotAvailableNoWebcam = 2,

            /// <summary>
            /// Mode is not available because necessary calibration has not been
            /// performed.
            /// </summary>
            NotAvailableNotCalibrated = 3,
        }

        /// <summary>
        /// Defines the optional capabilities that may be implemented by a zView
        /// node.
        /// </summary>
        public enum Capability
        {
            /// <summary>
            /// The node supports video recording via the video recording APIs.
            /// </summary>
            /// 
            /// <remarks>
            /// A zView connection will support this capability as long as the viewer
            /// node in the connection supports this capability.  The presenter node
            /// need not support this capability.
            /// </remarks>
            VideoRecording = 0,

            /// <summary>
            /// The node supports responding to requests to exit the application
            /// hosting the node when a zView connection is closed.
            /// </summary>
            /// 
            /// <remarks>
            /// For a zView node to support this capability, the application hosting
            /// the node must call GetConnectionCloseAction() whenever a zView
            /// connection enters the ConnectionState.Closed state and then exit
            /// if the close action is ConnectionCloseAction.ExitApplication.
            /// </remarks>
            RemoteApplicationExit = 1,
        }

        /// <summary>
        /// Defines the keys for all mode and mode spec attributes.
        /// </summary>
        public enum ModeAttributeKey
        {
            /// <summary>
            /// The version of the mode. 
            /// Datatype: UInt32.
            /// </summary>
            /// 
            /// <remarks>
            /// Different versions of a mode may function differently and may have
            /// different settings and frame data/buffers.
            /// </remarks>
            Version = 0,

            /// <summary>
            /// The mode's compositing mode. 
            /// Datatype: CompositingMode (get/set as UInt32).
            /// </summary>
            /// 
            /// <remarks>
            /// This indicates how the viewer node should composite the images it
            /// receives from the presenter node.
            /// </remarks>
            CompositingMode = 1,

            /// <summary>
            /// The mode's presenter camera mode. 
            /// Datatype: CameraMode (get/set as UInt32).
            /// </summary>
            /// 
            /// <remarks>
            /// This indicates how the presenter node's camera should function when the
            /// mode is active.
            /// </remarks>
            PresenterCameraMode = 2,

            /// <summary>
            /// The order of rows of pixels in images generated for the mode.
            /// Datatype: ImageRowOrder (get/set as UInt32).
            /// </summary>
            /// 
            /// <remarks>
            /// Any images generated by either the presenter or viewer node for the
            /// mode should have their rows of pixels in this order.
            /// </remarks>
            ImageRowOrder = 3,

            /// <summary>
            /// The format of pixels in color images generated for the mode.
            /// Datatype: PixelFormat (get/set as UInt32).
            /// </summary>
            /// 
            /// <remarks>
            /// Any color images generated by either the presenter or viewer node for
            /// the mode should use this pixel format.  The pixel format specifies the
            /// number, type, order, and size of the channels in a pixel.
            /// </remarks>
            ColorImagePixelFormat = 4,
        }

        /// <summary>
        /// Defines the possible zView mode compositing modes.
        /// </summary>
        /// 
        /// <remarks>
        /// This specifies the valid values that can be used to set the
        /// ModeAttributeKey.CompositingMode mode/mode spec attribute.
        /// </remarks>
        public enum CompositingMode
        {
            /// <summary>
            /// No compositing will be performed.
            /// </summary>
            None = 0,

            /// <summary>
            /// Images will be composited on top of images from an augmented
            /// reality camera video stream.
            /// </summary>
            AugmentedRealityCamera = 1,
        }

        /// <summary>
        /// Defines the possible zView mode camera modes.
        /// </summary>
        /// 
        /// <remarks>
        /// This currently specifies the valid values that can be used to set the
        /// ModeAttributeKey.PresenterCameraMode mode/mode spec attribute.
        /// </remarks>
        public enum CameraMode
        {
            /// <summary>
            /// The camera should have a fixed pose that never changes.
            /// </summary>
            Fixed = 0,

            /// <summary>
            /// The camera's pose should change according to head tracking
            /// information obtained on the local zView node.
            /// </summary>
            LocalHeadTracked = 1,

            /// <summary>
            /// The camera's pose should change according to information sent
            /// from the remote zView node.
            /// </summary>
            RemoteMovable = 2,
        }

        /// <summary>
        /// Defines the possible image pixel formats.
        /// </summary>
        /// 
        /// <remarks>
        /// This currently specifies the valid values that can be used to set the
        /// ModeAttributeKey.ColorImagePixelFormat mode/mode spec attribute.
        /// </remarks>
        public enum PixelFormat
        {
            /// <summary>
            /// Pixel format with four 8-bit channels in the following order:
            /// red, green, blue, and alpha.
            /// </summary>
            R8G8B8A8 = 0,
        }

        /// <summary>
        /// Defines the possible orderings for pixel rows within images.
        /// </summary>
        /// 
        /// <remarks>
        /// This currently specifies the valid values that can be used to set the
        /// ModeAttributeKey.ImageRowOrder mode/mode spec attribute.
        /// </remarks>
        public enum ImageRowOrder
        {
            /// <summary>
            /// The top row of pixels occurs first in the image data and the
            /// remaining rows are ordered from top to bottom.
            /// </summary>
            TopToBottom = 0,

            /// <summary>
            /// The bottom row of pixels occurs first in the image data and the
            /// remaining rows are ordered from bottom to top.
            /// </summary>
            BottomToTop = 1,
        }

        /// <summary>
        /// Defines the possible zView connection states.
        /// </summary>
        public enum ConnectionState
        {
            /// <summary>
            /// The connection is initializing.
            /// </summary>
            /// 
            /// <remarks>
            /// In this state, client code should not perform any zView operations
            /// using the connection.
            ///
            /// The connection will automatically transition to the
            /// AwaitingConnectionAcceptance state when it has
            /// finished initializing.
            /// </remarks>
            ConnectionInitialization = 0,

            /// <summary>
            /// The connection is waiting to be locally accepted or rejected.
            /// </summary>
            /// 
            /// <remarks>
            /// If the connection is accepted, it will transition to the
            /// SwitchingModes state.  If the connection is rejected, it will 
            /// transition to the Closed state.
            /// </remarks>
            AwaitingConnectionAcceptance = 1,

            /// <summary>
            /// The connection is internally switching between zView modes.
            /// </summary>
            /// 
            /// <remarks>
            /// In this state, client code is not required to perform any zView
            /// operations using the connection.
            ///
            /// The connection will automatically transition to the NoMode state (if 
            /// a switch to the NULL mode was requested) or to the ModeSetup state 
            /// (if a switch a non-NULL mode was requested) when it has finished switching
            /// modes internally.
            /// </remarks>
            SwitchingModes = 2,

            /// <summary>
            /// The connection is not currently in any zView mode.
            /// </summary>
            /// 
            /// <remarks>
            /// In this state, client code is not required to perform any zView
            /// operations using the connection.
            ///
            /// The connection can be switched into a mode by calling
            /// SetConnectionMode().
            /// </remarks>
            NoMode = 3,

            /// <summary>
            /// The connection is setting up the current zView mode.
            /// </summary>
            /// 
            /// <remarks>
            /// The connection will transition to the ModeActive state once all 
            /// mode setup phases for the current mode have been completed by both 
            /// the local and remote nodes.
            /// </remarks>
            ModeSetup = 4,

            /// <summary>
            /// The connection's current zView mode is active.
            /// </summary>
            /// 
            /// <remarks>
            /// In this state, client code should be sending frames to and/or receiving
            /// frames from the remote zView node.
            ///
            /// The connection's current zView mode can be paused by calling PauseMode(). 
            /// This will transition the connection into the ModePaused state.
            /// </remarks>
            ModeActive = 5,

            /// <summary>
            /// The connection's current zView mode is paused.
            /// </summary>
            /// 
            /// <remarks>
            /// In this state, client code should not be sending frames to nor
            /// receiving frames from the remote zView node. Client code is not
            /// required to perform any zView operations while the connection is in
            /// this state.
            ///
            /// The connection's current zView mode can be resumed by calling
            /// ResumeMode(). This will transition the connection into the
            /// ModeResuming state.
            /// </remarks>
            ModePaused = 6,

            /// <summary>
            /// The connection's current zView mode is resuming.
            /// </summary>
            /// 
            /// <remarks>
            /// The connection will automatically transition to the ModeActive state 
            /// or to the ModeSetup state when it has finished resuming the current mode. 
            /// The ModeSetup state is only transitioned to if there changes have been 
            /// made to mode-specific settings (while the mode was paused) that require 
            /// one or more mode setup phases to be rerun.
            /// </remarks>
            ModeResuming = 7,

            /// <summary>
            /// The connection is internally processing a change to a
            /// mode-specific setting that will require one or more mode setup phases
            /// to be rerun.
            /// </summary>
            /// 
            /// <remarks>
            /// The connection will automatically transition to the ModeSetup state when it 
            /// has finished internally processing the mode-specific settings change.
            /// </remarks>
            ProcessingModeSettingsChange = 8,

            /// <summary>
            /// The connection is closed and should be cleaned up and destroyed.
            /// </summary>
            /// 
            /// <remarks>
            /// In this state, client code may call GetConnectionCloseReason() to
            /// determine why the connection was closed.
            ///
            /// In this state client code may call GetConnectionCloseAction() and
            /// then optionally perform the requested action.
            /// </remarks>
            Closed = 9,

            /// <summary>
            /// An error has occurred and the connection is no longer usable.
            /// </summary>
            /// 
            /// <remarks>
            /// In this state, client code may call GetConnectionError() to
            /// determine the type of error that occurred.
            /// </remarks>
            Error = 10,
        }

        /// <summary>
        /// Defines the possible zView connection close actions.
        /// </summary>
        /// 
        /// <remarks>
        /// When closing a connection by calling CloseConnection(), one of these
        /// actions must be specified as the action that the remote zView node should
        /// take after the connection is closed.
        ///
        /// Once a connection enters the ConnectionState.Closed state, the close 
        /// action for the connection can be queried by calling GetConnectionCloseAction().
        /// </remarks>
        public enum ConnectionCloseAction
        {
            /// <summary>
            /// The application hosting the zView node should not perform any
            /// additional action after the connection is closed.
            /// </summary>
            None = 0,

            /// <summary>
            /// The application hosting the zView node should exit after the
            /// connection is closed.
            /// </summary>
            /// 
            /// <remarks>
            /// zView nodes may not perform this action is they do not support the
            /// Capability.RemoteApplicationExit capability.
            /// </remarks>
            ExitApplication = 1,
        }

        /// <summary>
        /// Defines the possible reasons why a zView connection was closed.
        /// </summary>
        /// 
        /// <remarks>
        /// When closing a connection by calling CloseConnection(), one of these
        /// reasons must be specified to indicate why the connection is being closed.
        ///
        /// Once a connection enters the ConnectionState.Closed state, the close
        /// reason for the connection can be queried by calling
        /// GetConnectionCloseReason().
        /// </remarks>
        public enum ConnectionCloseReason
        {
            /// <summary>
            /// The connection was closed for an unknown reason.
            /// </summary>
            Unknown = 0,

            /// <summary>
            /// The connection was closed because the remote zView node's zView
            /// API context was shut down.
            /// </summary>
            ShutDown = 1,

            /// <summary>
            /// The connection was closed because a user requested it to be
            /// closed.
            /// </summary>
            UserRequested = 2,

            /// <summary>
            /// The connection was closed because is was rejected by one of the
            /// zView nodes involved in the connection.
            /// </summary>
            ConnectionRejected = 3,
        }

        /// <summary>
        /// Defines the possible phases that the setup of a zView mode can go
        /// through.
        /// </summary>
        /// 
        /// <remarks>
        /// In this version of the zView API, all modes use all of the mode setup
        /// phases defined by this enum. However, future versions of the zView API may
        /// introduce mode setup phases that are only used by some modes.
        /// </remarks>
        public enum ModeSetupPhase
        {
            /// <summary>
            /// Mode setup is initializing.
            /// </summary>
            /// 
            /// <remarks>
            /// When in this mode setup phase, client code must set any settings that
            /// the remote node will need to complete mode setup. Exactly which
            /// settings must be set depends on the current mode. Client code may also
            /// begin performing other setup tasks related to the current mode during
            /// this setup phase (e.g. client code might begin creating or loading
            /// resources needed for the current mode).
            /// </remarks>
            Initialization = 0,

            /// <summary>
            /// Mode setup is completing.
            /// </summary>
            /// 
            /// <remarks>
            /// When in this mode setup phase, client code must finish any setup that
            /// is necessary prior to the current mode becoming active.  Exactly what
            /// setup must be performed depends on the current mode.
            /// </remarks>
            Completion = 1,
        }

        /// <summary>
        /// Defines the possible states that a connection setting can be in.
        /// </summary>
        /// 
        /// <remarks>
        /// A setting's state can be queried by calling GetSettingState().
        /// </remarks>
        public enum SettingState
        {
            /// <summary>
            /// Setting's value is up to date.
            /// </summary>
            UpToDate = 0,

            /// <summary>
            /// Setting's value was changed during the most recent call to
            /// LateUpdate(). State will transition to UpToDate on the next
            /// frame's LateUpdate().
            /// </summary>
            Changed = 1,

            /// <summary>
            /// Setting's value was changed locally, but the change has not 
            /// yet been accepted by the other side of the connection.
            /// </summary>
            ChangePending = 2,
        }

        /// <summary>
        /// Defines the keys for all possible zView connection settings.
        /// </summary>
        public enum SettingKey
        {
            /// <summary>
            /// The width, in pixels, of the primary images for the connection's
            /// current mode. 
            /// Datatype: UInt16.
            /// </summary>
            /// 
            /// <remarks>
            /// In standard mode family modes, this should be set by the presenter node
            /// during the ModeSetupPhase.Initialization mode setup phase. In
            /// augmented reality mode family modes, this should be set by the viewer
            /// node during the ModeSetupPhase.Initialization mode setup phase.
            ///
            /// If this setting is set after the ModeSetupPhase.Initialization
            /// mode setup phase (i.e. in a later mode setup phase or while the mode is
            /// active or paused), then the connection will automatically transition
            /// back to the ConnectionState.ModeSetup state in the
            /// ModeSetupPhase.Completion mode setup phase in order to allow
            /// both nodes to take into account the new setting value (e.g. by
            /// reallocating image buffers to use the new width).
            ///
            /// This setting should generally be set in a batch (by calling
            /// BeginSettingsBatch() and EndSettingsBatch()) with the the
            /// ImageHeight setting.  Doing this ensures that remote
            /// nodes will see image width and height changes at the same time (instead
            /// of possibly seeing one of these settings change during one frame and
            /// then the other change in the next frame).
            /// </remarks>
            ImageWidth = 0,

            /// <summary>
            /// The height, in pixels, of the primary images for the connection's current mode. 
            /// Datatype: UInt16.
            /// </summary>
            /// 
            /// <remarks>
            /// In standard mode family modes, this should be set by the presenter node
            /// during the ModeSetupPhase.Initialization mode setup phase. In
            /// augmented reality mode family modes, this should be set by the viewer
            /// node during the ::ModeSetupPhase.Initialization mode setup phase.
            ///
            /// If this setting is set after the ModeSetupPhase.Initialization
            /// mode setup phase (i.e. in a later mode setup phase or while the mode is
            /// active or paused), then the connection will automatically transition
            /// back to the ConnectionState.ModeSetup state in the
            /// ModeSetupPhase.Completion mode setup phase in order to allow
            /// both nodes to take into account the new setting value (e.g. by
            /// reallocating image buffers to use the new height).
            ///
            /// This setting should generally be set in a batch (by calling
            /// BeginSettingsBatch() and EndSettingsBatch()) with the the
            /// ImageWidth setting.  Doing this ensures that remote
            /// nodes will see image width and height changes at the same time (instead
            /// of possibly seeing one of these settings change during one frame and
            /// then the other change in the next frame).
            /// </remarks>
            ImageHeight = 1,

            /// <summary>
            /// The connection's current video recording quality. 
            /// Datatype: VideoRecordingQuality (get/set as UInt32).
            /// </summary>
            /// 
            /// <remarks>
            /// This setting may only be set if the connection's video recording state
            /// is currently VideoRecordingState.NotRecording.
            ///
            /// Whenever a video recording is started, the current value of this
            /// setting is used as the quality level for the new video recording.
            /// </remarks>
            VideoRecordingQuality = 2,

            /// <summary>
            /// The current horizontal offset, in pixels, of the primary image
            /// overlay displayed by the viewer node (default 0.0f). 
            /// Datatype: float.
            /// </summary>
            /// 
            /// <remarks>
            /// This is only available for augmented reality mode family modes. For
            /// these modes the viewer node will use this value to offset the position
            /// of the presenter's color images from the augmented reality mode camera
            /// video stream images when the two are composited.
            /// </remarks>
            OverlayOffsetX = 3,

            /// <summary>
            /// The current vertical offset, in pixels, of the primary image
            /// overlay displayed by the viewer node (default 0.0f). 
            /// Datatype: float.
            /// </summary>
            /// 
            /// <remarks>
            /// This is only available for augmented reality mode family modes. For
            /// these modes the viewer node will use this value to offset the position
            /// of the presenter's color images from the augmented reality mode camera
            /// video stream images when the two are composited.
            /// </remarks>
            OverlayOffsetY = 4,

            /// <summary>
            /// The current horizontal scale factor for the primary image
            /// overlay displayed by the viewer node (default 1.0f). 
            /// Datatype: float.
            /// </summary>
            /// 
            /// <remarks>
            /// This is only available for augmented reality mode family modes. For
            /// these modes the viewer node will use this value to scale the
            /// presenter's color images before they are composited with the augmented
            /// reality mode camera video stream images.
            /// </remarks>
            OverlayScaleX = 5,

            /// <summary>
            /// The current vertical scale factor for the primary image overlay
            /// displayed by the viewer node (default 1.0f).
            /// Datatype: float.
            /// </summary>
            /// 
            /// <remarks>
            /// This is only available for augmented reality mode family modes. For
            /// these modes the viewer node will use this value to scale the
            /// presenter's color images before they are composited with the augmented
            /// reality mode camera video stream images.
            /// </remarks>
            OverlayScaleY = 6,
        }

        /// <summary>
        /// Defines the possible streams that may be used by a zView mode for
        /// sending frame data between the nodes involved in a zView connection.
        /// </summary>
        public enum Stream
        {
            /// <summary>
            /// Stream used sending the primary image data between nodes for a
            /// mode. May also be used to send metadata related to the images being
            /// sent or data necessary for generating images to be sent.
            /// </summary>
            Image = 0,
        }

        /// <summary>
        /// Defines the keys for all possible pieces of frame data for a zView
        /// mode.
        /// </summary>
        public enum FrameDataKey
        {
            /// <summary>
            /// The frame's frame number.
            /// Datatype: UInt64.
            /// </summary>
            /// 
            /// <remarks>
            /// This is used for Stream.Image frames in both standard mode family
            /// and augmented reality mode family modes.
            /// </remarks>
            FrameNumber = 0,

            /// <summary>
            /// The camera pose matrix (position and orientation) to use for rendering the current 
            /// mode's primary images. 
            /// Datatype: Matrix4x4.
            /// </summary>
            /// 
            /// <remarks>
            /// This is only used for Stream.Image frames sent from the viewer
            /// node to the presenter node in augmented reality mode family modes.
            /// </remarks>
            CameraPose = 1,

            /// <summary>
            /// The camera focal length to use for rendering the current mode's
            /// primary images.
            /// Datatype: float.
            /// </summary>
            /// 
            /// <remarks>
            /// This is only used for Stream.Image frames sent from the viewer
            /// node to the presenter node in augmented reality mode family modes.
            /// </remarks>
            CameraFocalLength = 2,

            /// <summary>
            /// The horizontal offset of the camera's principal point to use for
            /// rendering the current mode's primary images. 
            /// Datatype: float.
            /// </summary>
            /// 
            /// <remarks>
            /// This is only used for Stream.Image frames sent from the viewer
            /// node to the presenter node in augmented reality mode family modes.
            /// </remarks>
            CameraPrincipalPointOffsetX = 3,

            /// <summary>
            /// The vertical offset of the camera's principal point to use for
            /// rendering the current mode's primary images.
            /// Datatype: float.
            /// </summary>
            /// 
            /// <remarks>
            /// This is only used for Stream.Image frames sent from the viewer
            /// node to the presenter node in augmented reality mode family modes.
            /// </remarks>
            CameraPrincipalPointOffsetY = 4,

            /// <summary>
            /// The camera pixel aspect ratio to use for rendering the current
            /// mode's primary images.
            /// Datatype: float.
            /// </summary>
            /// 
            /// <remarks>
            /// This is only used for Stream.Image frames sent from the viewer
            /// node to the presenter node in augmented reality mode family modes.
            /// </remarks>
            CameraPixelAspectRatio = 5,

            /// <summary>
            /// The camera axis skew to use for rendering the current mode's
            /// primary images.
            /// Datatype: float.
            /// </summary>
            /// 
            /// <remarks>
            /// This is only used for Stream.Image frames sent from the viewer
            /// node to the presenter node in augmented reality mode family modes.
            /// </remarks>
            CameraAxisSkew = 6,
        }

        /// <summary>
        /// Defines the keys for all possible frame buffers for a zView mode.
        /// </summary>
        public enum FrameBufferKey
        {
            /// <summary>
            /// Frame buffer for storing the first color image associated with a
            /// zView mode.
            /// </summary>
            ImageColor0 = 0,
        }

        /// <summary>
        /// Defines the possible zView connection video recording states.
        /// </summary>
        /// 
        /// <remarks>
        /// A connection's current video recording state can be queried by calling
        /// GetVideoRecordingState().
        /// </remarks>
        public enum VideoRecordingState
        {
            /// <summary>
            /// Video recording capability is not currently available and cannot be used.
            /// </summary>
            /// 
            /// <remarks>
            /// A connection's video recording state will automatically transition from
            /// this state to the NotRecording state if the connection supports the 
            /// Capability.VideoRecording capability. This transition will occur once the 
            /// connection is fully initialized.
            /// </remarks>
            NotAvailable = 0,

            /// <summary>
            /// Not actively recording and no current recording exists.
            /// </summary>
            /// 
            /// <remarks>
            /// A video recording can be started by calling StartVideoRecording().
            /// This will transition the video recording state to the Starting state.
            /// </remarks>
            NotRecording = 1,

            /// <summary>
            /// Video recording is in the process of starting.
            /// </summary>
            /// 
            /// <remarks>
            /// A connection's video recording state will automatically transition from
            /// this state to the Recording state once video recording has fully started.
            /// </remarks>
            Starting = 2,

            /// <summary>
            /// Acively recording.
            /// </summary>
            /// 
            /// <remarks>
            /// Recording can be finished by calling FinishVideoRecording(), which will 
            /// transition the video recording state to the Finishing state. Recording 
            /// can be paused by calling PauseVideoRecording(), which will transition the 
            /// video recording state to the Pausing state.
            /// </remarks>
            Recording = 3,

            /// <summary>
            /// Video recording is in the process of finishing.
            /// </summary>
            /// 
            /// <remarks>
            /// A connection's video recording state will automatically transition 
            /// fromthis state to the Finished state once video recording has completed 
            /// finishing.
            /// </remarks>
            Finishing = 4,

            /// <summary>
            /// Not actively recording; current finished recording exists.
            /// </summary>
            /// 
            /// <remarks>
            /// The finished recording can be saved by calling SaveVideoRecording(),
            /// which will transition the video recording state to the Saving state. 
            /// The finished recording can be discarded by calling DiscardVideoRecording(), 
            /// which will transition the video recording state to the Discarding state.
            /// </remarks>
            Finished = 5,

            /// <summary>
            /// Video recording is in the process of pausing.
            /// </summary>
            /// 
            /// <remarks>
            /// A connection's video recording state will automatically transition from
            /// this state to the Paused state once video recording has completed pausing.
            /// </remarks>
            Pausing = 6,

            /// <summary>
            /// Not actively recording; current resumable recording exists.
            /// </summary>
            /// 
            /// <remarks>
            /// Recording can be resumed by calling ResumeVideoRecording(), which
            /// will transition the video recording state to the Resuming state. 
            /// Recording can be finished by calling FinishVideoRecording(), which 
            /// will transition the video recording state to the Finishing state.
            /// </remarks>
            Paused = 7,

            /// <summary>
            /// Video recording is in the process of resuming.
            /// </summary>
            /// 
            /// <remarks>
            /// A connection's video recording state will automatically transition from
            /// this state to the Recording state once video recording has completed resuming.
            /// </remarks>
            Resuming = 8,

            /// <summary>
            /// The current finished video recording is in the process of being
            /// saved.
            /// </summary>
            /// 
            /// <remarks>
            /// A connection's video recording state will automatically transition from
            /// this state to the NotRecording state once saving is complete.
            /// </remarks>
            Saving = 9,

            /// <summary>
            /// The current finished video recording is in the process of being
            /// discarded.
            /// </summary>
            /// 
            /// <remarks>
            /// A connection's video recording state will automatically transition from
            /// this state to the NotRecording state once discarding is complete.
            /// </remarks>
            Discarding = 10,

            /// <summary>
            /// A recoverable video-recording-related error occurred.
            /// </summary>
            /// 
            /// <remarks>
            /// In this state, client code should call ClearVideoRecordingError() to
            /// clear the error and allow new video recordings to be started. This
            /// will transition the video recording state to the ClearingError state.
            ///
            /// In this state, client code may call GetVideoRecordingError() to
            /// determine the type of error that occurred.
            /// </remarks>
            Error = 11,

            /// <summary>
            /// The most recent video recording error is in the process of being
            /// cleared.
            /// </summary>
            /// 
            /// <remarks>
            /// A connection's video recording state will automatically transition from
            /// this state to the NotRecording state once clearing of the video recording 
            /// error is complete.
            /// </remarks>
            ClearingError = 12,
        }

        /// <summary>
        /// Defines the possible video recording quality levels.
        /// </summary>
        public enum VideoRecordingQuality
        {
            Unknown = -1,

            /// <summary>
            /// Video recording with 854 x 480 pixel resolution.
            /// </summary>
            Resolution480p = 0,

            /// <summary>
            /// Video recording with 1280 x 720 pixel resolution.
            /// </summary>
            Resolution720p = 1,

            /// <summary>
            /// Video recording with 1920 x 1080 pixel resolution.
            /// </summary>
            Resolution1080p = 2,
        }


        //////////////////////////////////////////////////////////////////
        // Compound Types
        //////////////////////////////////////////////////////////////////

        /// <summary>
        /// Class representing a mode that is supported by a zView node along
        /// with the current availability of that mode.
        /// </summary>
        public class SupportedMode
        {
            /// <summary>
            /// The handle of the mode that is supported.
            /// </summary>
            public IntPtr Mode { get; private set; }

            /// <summary>
            /// The supported mode's current availability.
            /// </summary>
            public ModeAvailability ModeAvailability { get; private set; }

            public SupportedMode(IntPtr mode, ModeAvailability modeAvailability)
            {
                this.Mode = mode;
                this.ModeAvailability = modeAvailability;
            }
        }


        //////////////////////////////////////////////////////////////////
        // Unity Inspector Fields
        //////////////////////////////////////////////////////////////////

        /// <summary>
        /// Layers to be ignored by the standard mode's default
        /// virtual camera.
        /// </summary>
        public LayerMask StandardModeIgnoreLayers;

        /// <summary>
        /// Layers to be ignored by the augmented reality mode's
        /// default virtual camera.
        /// </summary>
        public LayerMask ARModeIgnoreLayers = 0;

        /// <summary>
        /// Layers to cull out by the augmented reality mode's
        /// default virtual camera if geometry protrudes into
        /// negative parallax outside the bounds of the viewport.
        /// </summary>
        public LayerMask ARModeEnvironmentLayers = 0;

        /// <summary>
        /// Layer (0 - 31) to be assigned to the augmented reality mode's
        /// box mask. This layer must be unique and should not be used for 
        /// any objects in the scene other than the box mask.
        /// </summary>
        public int ARModeMaskLayer = 31;

        /// <summary>
        /// The render queue priority for the augmented reality mode's 
        /// box mask. This is only used when ARModeEnableTransparency
        /// is enabled and should generally be assigned values less than
        /// 2000 (opaque geometry) to ensure that its depth will be rendered
        /// prior to rendering any opaque geometry.
        /// </summary>
        public int ARModeMaskRenderQueue = 1900;

        /// <summary>
        /// The size of the augmented reality mode's box mask in meters.
        /// </summary>
        public Vector3 ARModeMaskSize = Vector3.one * 2.0f;

        /// <summary>
        /// Enables debug visualization for the augmented reality mode's
        /// box mask in the Unity Editor's SceneView window.
        /// </summary>
        public bool ARModeShowMask = false;

        /// <summary>
        /// If not enabled, force all non-mask pixels rendered by the augmented 
        /// reality mode's virtual camera to have an alpha value of 1. By default,
        /// this is disabled due to the fact that most standard shaders associated
        /// with both opaque and transparent geometry either have incorrect values
        /// in their alpha channels, or do not write their alpha channels to the
        /// frame buffer.
        /// </summary>
        public bool ARModeEnableTransparency = false;

        /// <summary>
        /// ZView requires a reference to the active ZCamera. ZView will try
        /// to find an instance of ZCamera on awake if left unassigned. If the
        /// ZCamera is destroyed during the life of the current scene, this
        /// value must be assigned manually.
        /// </summary>
        public ZCamera ActiveZCamera = null;

        //////////////////////////////////////////////////////////////////
        // Public API
        //////////////////////////////////////////////////////////////////

        /// <summary>
        /// Checks whether the GCSeries zView SDK was properly initialized.
        /// </summary>
        /// 
        /// <returns>
        /// True if initialized. False otherwise.
        /// </returns>
        public bool IsInitialized()
        {
            return GlobalState.Instance.IsInitialized;
        }

        /// <summary>
        /// Gets the current version of the GCSeries zView Unity plugin.
        /// </summary>
        /// 
        /// <returns>
        /// The plugin version in major.minor.patch string format.
        /// </returns>
        public string GetPluginVersion()
        {
            int major = 0;
            int minor = 0;
            int patch = 1;
            return string.Format("{0}.{1}.{2}", major, minor, patch);
        }

        /// <summary>
        /// Gets the current runtime version of the GCSeries zView SDK.
        /// </summary>
        /// 
        /// <returns>
        /// The runtime version in major.minor.patch string format.
        /// </returns>
        public string GetRuntimeVersion()
        {
            int major = 0;
            int minor = 0;
            int patch = 1;
            return string.Format("{0}.{1}.{2}", major, minor, patch);
        }

        /// <summary>
        /// Get the node type of the current context.
        /// </summary>
        /// 
        /// <returns>
        /// The node type of the current context.
        /// </returns>
        public NodeType GetNodeType()
        {
            NodeType type = NodeType.Presenter;
            return type;
        }

        /// <summary>
        /// Get the node ID of the current context.
        /// </summary>
        /// 
        /// <returns>
        /// The byte buffer to fill with the node ID of the current context.
        /// </returns>
        public byte[] GetNodeId()
        {
            // Get the node id's size in bytes.
            int size = 4;
            byte[] id = new byte[size];
            Array.Clear(id, 0, size);
            Debug.LogWarning("GView.GetNodeId():这个函数目前不使用!");
            return id;
        }

        /// <summary>
        /// Get the node name string associated with the current context.
        /// </summary>
        /// 
        /// <returns>
        /// The node name string associated with the current context.
        /// </returns>
        public string GetNodeName()
        {
            Debug.LogWarning("GView.GetNodeName():这个函数目前不使用!");
            return "UnmannedNode";
        }

        /// <summary>
        /// Set the node name string associated with the current context.
        /// </summary>
        /// 
        /// <param name="name">
        /// The new node name to use.
        /// </param>
        public void SetNodeName(string name)
        {
            Debug.LogWarning("GView.SetNodeName():这个函数目前不使用!name=" + name);
        }

        /// <summary>
        /// Get the node status string associated with the current context.
        /// </summary>
        /// 
        /// <returns>
        /// The node status string associated with the current context.
        /// </returns>
        public string GetNodeStatus()
        {
            Debug.LogWarning("GView.GetNodeStatus():这个函数目前不使用!");
            return "OK";
        }

        /// <summary>
        /// Set the node status string associated with the current context.
        /// </summary>
        /// 
        /// <param name="status">
        /// The new node status to use.
        /// </param>
        public void SetNodeStatus(string status)
        {
            Debug.LogWarning("GView.SetNodeStatus():这个函数目前不使用!");
        }

        /// <summary>
        /// Get the number of modes supported by the current context.
        /// </summary>
        /// 
        /// <returns>
        /// The number of modes supported by the current context.
        /// </returns>
        public int GetNumSupportedModes()
        {
            Debug.LogWarning("GView.GetNumSupportedModes():这个函数目前不使用!");
            int numModes = 0;
            numModes = 1;
            return numModes;
        }

        /// <summary>
        /// Get a supported mode from the list of supported modes for the
        /// current context.
        /// </summary>
        /// 
        /// <param name="modeIndex">
        /// The index of the supported mode to get. This must be
        /// greater than or equal to 0 and less than the number of
        /// supported modes queried via GetNumSupportedModes().
        /// </param>
        /// 
        /// <returns>
        /// The requested supported mode.
        /// </returns>
        /// 
        /// <exception cref="GCSeries.zView.InvalidParameterException">
        /// Thrown if the mode index is out of range.
        /// </exception>
        public SupportedMode GetSupportedMode(int modeIndex)
        {
            Debug.LogWarning("GView.GetSupportedMode():这个函数目前不使用!");
            return null;
        }

        /// <summary>
        /// Set the modes supported by the current context.
        /// </summary>
        /// 
        /// <param name="modes">
        /// The list of supported modes.
        /// </param>
        /// 
        /// <exception cref="GCSeries.zView.InvalidModeException">
        /// Thrown if any supported mode references an invalid mode.
        /// </exception>
        public void SetSupportedModes(IList<SupportedMode> modes)
        {
            Debug.LogWarning("GView.SetSupportedModes():这个函数目前不使用!");
        }

        /// <summary>
        /// Get the number of capabilities supported by the current context.
        /// </summary>
        /// 
        /// <returns>
        /// The number of capabilities supported by the current context.
        /// </returns>
        public int GetNumSupportedCapabilities()
        {
            Debug.LogWarning("GView.GetNumSupportedCapabilities():这个函数目前不使用!");
            int numCapabilities = 0;
            return numCapabilities;
        }

        /// <summary>
        /// Get a supported capability from the list of supported capabilities for 
        /// the current context.
        /// </summary>
        /// 
        /// <param name="capabilityIndex">
        /// The index of the supported capability to get. This must be greater than 
        /// or equal to 0 and less than the number of supported capabilities queried 
        /// via GetNumSupportedCapabilities(). 
        /// </param>
        ///
        /// <returns>
        /// The requested supported capability.
        /// </returns>
        /// 
        /// <exception cref="GCSeries.zView.InvalidParameterException">
        /// Thrown if the capability index is out of range.
        /// </exception>
        public Capability GetSupportedCapability(int capabilityIndex)
        {
            Debug.LogWarning("GView.GetSupportedCapability():这个函数目前不使用!");
            Capability capability = Capability.VideoRecording;
            return capability;
        }

        /// <summary>
        /// Get the value of the specified mode attribute of type UInt32 for the
        /// specified mode.
        /// </summary>
        /// 
        /// <param name="mode">
        /// The mode to get the attribute value of.
        /// </param>
        /// <param name="key">
        /// The mode attribute key to get the value of.
        /// </param>
        /// 
        /// <returns>
        /// The value associated with specified mode and mode attribute key.
        /// </returns>
        /// 
        /// <exception cref="GCSeries.zView.InvalidParameterException">
        /// Thrown if the mode is null (IntPtr.Zero).
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidModeException">
        /// Thrown if the mode is invalid.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidModeAttributeKeyException">
        /// Thrown if the mode attribute key is invalid.
        /// </exception>
        public UInt32 GetModeAttributeUInt32(IntPtr mode, ModeAttributeKey key)
        {
            Debug.LogWarning("GView.GetModeAttributeUInt32():这个函数目前不使用!");
            UInt32 value = 0;
            return value;
        }

        /// <summary>
        /// Connect to the default viewer using the current context.
        /// </summary>
        /// 
        /// <remarks>
        /// This method performs its work asynchronously. Once a connection
        /// to the default viewer is created, it will be accessible via 
        /// GetConnection() or GetCurrentActiveConnection() after the next 
        /// LateUpdate().
        /// </remarks>
        public void ConnectToDefaultViewer()
        {
            Debug.Log("GView.ConnectToDefaultViewer():进入!");
            IntPtr connection;
            xxConnectToDefaultViewer(out connection);
            //因为我们永远连接成功.所以直接赋值
            GlobalState.Instance.Connection = connection;

            //这里是否需要进入一个模式呢?

            //因为永远是连上的,所以这里发个事件算了
            if (this.ConnectionAccepted != null)
            {
                this.ConnectionAccepted(this, GlobalState.Instance.Connection);
            }
        }

        /// <summary>
        /// Close the specified connection.
        /// </summary>
        /// 
        /// <remarks>
        /// The close action will be queriable by the remote node
        /// once the connection has entered the Closed state. Note: The node 
        /// that calls this function for a connection will not be able to query 
        /// the action it specifies via GetConnectionCloseAction(), since the 
        /// action is meant for the remote node. Instead, GetConnectionCloseAction() 
        /// will always return None when called by the node that called this 
        /// method.
        /// </remarks>
        /// 
        /// <param name="connection">
        /// The connection to close.
        /// </param>
        /// <param name="action">
        /// The action that should be performed by the remote node after the 
        /// connection is closed.
        /// </param>
        /// <param name="reason">
        /// The reason why the connection is being closed. This will be queriable 
        /// via the GetConnectionCloseReason() function once the connection has 
        /// entered the Closed state.
        /// </param>
        /// <param name="reasonDetails">
        /// Additional details on the reason why the connection is being closed. 
        /// This is purely for logging purposes and will not be displayed to the
        /// user.
        /// </param>
        /// 
        /// <exception cref="GCSeries.zView.InvalidParameterException">
        /// Thrown if the connection is null (IntPtr.Zero).
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionException">
        /// Thrown if the connection is invalid.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionStateException">
        /// Thrown if the connection is already in a Closed or Error state.
        /// </exception>
        public void CloseConnection(IntPtr connection, ConnectionCloseAction action, ConnectionCloseReason reason, string reasonDetails)
        {
            Debug.Log("GView.CloseConnection():进入!");

            CurVirtualCameraMode = IntPtr.Zero;
            GlobalState.Instance.Connection = IntPtr.Zero;//这里关闭了
        }

        /// <summary>
        /// Get the number of currently visible connections for the current context.
        /// </summary>
        /// 
        /// <remarks>
        /// The number of currently visible connections will change over the lifetime
        /// of a context. However, the number connections queried by this function
        /// will only change during LateUpdate() (i.e. the value will remain 
        /// stable between calls to LateUpdate()).
        /// </remarks>
        /// 
        /// <returns>
        /// The number of visible connections for the specified context.
        /// </returns>
        public int GetNumConnections()
        {
            if (GlobalState.Instance.Connection == IntPtr.Zero)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }

        /// <summary>
        /// Get a connection from the list of currently visible connections for the
        /// current context.
        /// </summary>
        /// 
        /// <remarks>
        /// The list of currently visible connections will change over the lifetime of
        /// a context. However, the list of connections that is accessible via this
        /// function will only change during LateUpdate() (i.e. the list will 
        /// remain stable between calls to LateUpdate()).
        /// </remarks>
        /// 
        /// <param name="connectionIndex">
        /// The index of the connection to get. This must be greater than or equal 
        /// to 0 and less than the number of connections queried via GetNumConnections().
        /// </param>
        /// 
        /// <returns>
        /// The requested connection.
        /// </returns>
        /// 
        /// <exception cref="GCSeries.zView.InvalidParameterException">
        /// Thrown if the connection index is out of range.
        /// </exception>
        public IntPtr GetConnection(int connectionIndex)
        {
            return GlobalState.Instance.Connection;
        }

        /// <summary>
        /// Get the current active connection if it exists.
        /// </summary>
        /// 
        /// <returns>
        /// The current active connection if it exists. Otherwise IntPtr.Zero.
        /// </returns>
        public IntPtr GetCurrentActiveConnection()
        {
            return GlobalState.Instance.Connection;
        }

        /// <summary>
        /// Get the state of the specified connection.
        /// </summary>
        /// 
        /// <remarks>
        /// The state of a connection will change over the lifetime of the connection.
        /// However, the state queried via this function will only change during
        /// LateUpdate() (i.e. the state will remain stable between calls to LateUpdate()).
        /// </remarks>
        /// 
        /// <param name="connection">
        /// The connection to get the state of.
        /// </param>
        /// 
        /// <returns>
        /// The state of the specified connection.
        /// </returns>
        /// 
        /// <exception cref="GCSeries.zView.InvalidParameterException">
        /// Thrown if the connection is null (IntPtr.Zero).
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionException">
        /// Thrown if the connection is invalid.
        /// </exception>
        public ConnectionState GetConnectionState(IntPtr connection)
        {
            return ConnectionState.ModeActive;
        }

        /// <summary>
        /// Get the error code associated with the specified connection.
        /// </summary>
        /// 
        /// <remarks>
        /// It is only valid to query the error code associated with a connection when it 
        /// is in the Error state (i.e. through callbacks registered against the 
        /// ConnectionError event).
        /// </remarks>
        /// 
        /// <param name="connection">
        /// The connection to get the error code of.
        /// </param>
        /// 
        /// <returns>
        /// The error code associated with the specified connection.
        /// </returns>
        /// 
        /// <exception cref="GCSeries.zView.InvalidParameterException">
        /// Thrown if the connection is null (IntPtr.Zero).
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionException">
        /// Thrown if the connection is invalid.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionStateException">
        /// Thrown if the connection is not in the Error state.
        /// </exception>
        public PluginError GetConnectionError(IntPtr connection)
        {
            Debug.LogWarning("GView.GetConnectionError():这个函数目前未实现!");
            PluginError connectionError = PluginError.Ok;
            return connectionError;
        }

        /// <summary>
        /// Check whether the specified connection was initiated locally or remotely.
        /// </summary>
        /// 
        /// <param name="connection">
        /// The connection to check whether it was locally initiated.
        /// </param>
        /// 
        /// <returns>
        /// Whether the connection was locally initiated.
        /// </returns>
        /// 
        /// <exception cref="GCSeries.zView.InvalidParameterException">
        /// Thrown if the connection is null (IntPtr.Zero).
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionException">
        /// Thrown if the connection is invalid.
        /// </exception>
        public bool WasConnectionLocallyInitiated(IntPtr connection)
        {
            Debug.LogWarning("GView.WasConnectionLocallyInitiated():这个函数目前未实现!");
            bool wasLocallyInitiated = false;
            return wasLocallyInitiated;
        }

        /// <summary>
        /// Gets the node ID of the remote node that the specified connection is
        /// connected to.
        /// </summary>
        /// 
        /// <param name="connection">
        /// The connection to get the remote node ID of.
        /// </param>
        /// 
        /// <returns>
        /// The byte buffer to fill with the node ID of the remote node that the 
        /// specified connection is connected to.
        /// </returns>
        /// 
        /// <exception cref="GCSeries.zView.InvalidParameterException">
        /// Thrown if the connection is null (IntPtr.Zero).
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionException">
        /// Thrown if the connection is invalid.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionStateException">
        /// Thrown if the connection is in the Closed or Error state.
        /// </exception>
        public byte[] GetConnectedNodeId(IntPtr connection)
        {
            Debug.LogWarning("GView.GetConnectedNodeId():这个函数目前未实现!");
            // Get the node id's size in bytes.
            int size = 4;
            byte[] id = new byte[size];
            Array.Clear(id, 0, size);
            return id;
        }

        /// <summary>
        /// Gets the node name string of the remote node that the specified connection
        /// is connected to.
        /// </summary>
        /// 
        /// <param name="connection">
        /// The connection to get the remote node name of.
        /// </param>
        /// 
        /// <returns>
        /// The node name string of the remote node that the specified connection
        /// is connected to.
        /// </returns>
        /// 
        /// <exception cref="GCSeries.zView.InvalidParameterException">
        /// Thrown if the connection is null (IntPtr.Zero).
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionException">
        /// Thrown if the connection is invalid.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionStateException">
        /// Thrown if the connection is in the Closed or Error state.
        /// </exception>
        public string GetConnectedNodeName(IntPtr connection)
        {
            Debug.LogWarning("GView.GetConnectedNodeName():这个函数目前未实现!");
            return "unnamed";
        }

        /// <summary>
        /// Gets the node status string of the remote node that the specified
        /// connection is connected to.
        /// </summary>
        /// 
        /// <param name="connection">
        /// The connection to get the remote node status of.
        /// </param>
        /// 
        /// <returns>
        /// The node status string of the remote node that the specified
        /// connection is connected to.
        /// </returns>
        /// 
        /// <exception cref="GCSeries.zView.InvalidParameterException">
        /// Thrown if the connection is null (IntPtr.Zero).
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionException">
        /// Thrown if the connection is invalid.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionStateException">
        /// Thrown if the connection is in the Closed or Error state.
        /// </exception>
        public string GetConnectedNodeStatus(IntPtr connection)
        {
            Debug.LogWarning("GView.GetConnectedNodeStatus():这个函数目前未实现!");
            return "OK";
        }

        /// <summary>
        /// Check if the specified connection supports the specified capability.
        /// </summary>
        /// 
        /// <param name="connection">
        /// The connection to check the capability support of.
        /// </param>
        /// <param name="capability">
        /// The capability to check for support of.
        /// </param>
        /// 
        /// <returns>
        /// Whether the specified capability is supported by the specified 
        /// connection or not.
        /// </returns>
        /// 
        /// <exception cref="GCSeries.zView.InvalidParameterException">
        /// Thrown if the connection is null (IntPtr.Zero).
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionException">
        /// Thrown if the connection is invalid.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionStateException">
        /// Thrown if the connection is in the Closed or Error state.
        /// </exception>
        public bool DoesConnectionSupportCapability(IntPtr connection, Capability capability)
        {
            if (capability == Capability.RemoteApplicationExit)
            {
                return false;
            }
            if (capability == Capability.VideoRecording)
            {
                bool isSupported = xxGetVideoRecordingCapability();
                return isSupported;

            }
            return false;
        }

        /// <summary>
        /// Get the number of modes supported by the specified connection.
        /// </summary>
        /// 
        /// <remarks>
        /// The number of modes supported by a connection may change over the lifetime
        /// of the connection. However, the number of supported modes returned by this
        /// function will only change during LateUpdate() (i.e. the value will remain 
        /// stable between calls to LateUpdate()).
        /// </remarks>
        /// 
        /// <param name="connection">
        /// The connection to get the number of supported modes for.
        /// </param>
        ///
        /// <returns>
        /// The number of modes supported by the specified connection. 
        /// </returns>
        /// 
        /// <exception cref="GCSeries.zView.InvalidParameterException">
        /// Thrown if the connection is null (IntPtr.Zero).
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionException">
        /// Thrown if the connection is invalid.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionStateException">
        /// Thrown if the connection is in the Closed or Error state.
        /// </exception>
        public int GetNumConnectionSupportedModes(IntPtr connection)
        {
            if (connection != IntPtr.Zero &&
                GlobalState.Instance.Connection == connection)
                return 3;//支持两个模式
            return 0;
        }

        /// <summary>
        /// Get a supported mode and associated mode availability information from the
        /// list of modes supported by the specified connection.
        /// </summary>
        /// 
        /// <remarks>
        /// The list of modes supported by a connection may change over the lifetime of
        /// the connection. However, the list of supported modes accessible via this
        /// function will only change during LateUpdate() (i.e. the value will remain 
        /// stable between calls to LateUpdate()).
        /// </remarks>
        /// 
        /// <param name="connection">
        /// The connection to get a supported mode from.
        /// </param>
        /// <param name="supportedModeIndex">
        /// The index of the supported mode to get. This must be greater than or equal 
        /// to 0 and less than the number of supported modes queried via the 
        /// GetNumConnectionSupportedModes() function.
        /// </param>
        /// 
        /// <returns>
        /// The requested supported mode with associated mode availability information.
        /// </returns>
        /// 
        /// <exception cref="GCSeries.zView.InvalidParameterException">
        /// Thrown if the connection is null (IntPtr.Zero) or the supported mode index
        /// is out of range.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionException">
        /// Thrown if the connection is invalid.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionStateException">
        /// Thrown if the connection is in the Closed or Error state.
        /// </exception>
        public SupportedMode GetConnectionSupportedMode(IntPtr connection, int supportedModeIndex)
        {
            //Debug.LogWarning("GView.GetConnectionSupportedMode():这个函数目前未实现!");
            if (connection != IntPtr.Zero)
            {
                int screenNum = F3Device.DeviceManager.Instance.AllMonitors.Count;

                if (supportedModeIndex == 0)
                {
                    if (screenNum < 2)
                    {
                        return new SupportedMode(GlobalState.Instance.ModeStandard, ModeAvailability.NotAvailable);
                    }

                    return new SupportedMode(GlobalState.Instance.ModeStandard, ModeAvailability.Available);
                }
                else if (supportedModeIndex == 1)
                {
                    if (screenNum < 2)
                    {
                        return new SupportedMode(GlobalState.Instance.ModeThreeD, ModeAvailability.NotAvailable);
                    }

                    return new SupportedMode(GlobalState.Instance.ModeThreeD, ModeAvailability.Available);
                }
                else if (supportedModeIndex == 2)
                {
                    if (screenNum < 2)
                    {
                        return new SupportedMode(GlobalState.Instance.ModeAugmentedReality, ModeAvailability.NotAvailable);
                    }

                    //如果有罗技相机那么返回可用
                    var devices = WebCamTexture.devices;
                    foreach (var item in devices)
                    {
                        if (item.name.Contains("C920"))
                        {
                            return new SupportedMode(GlobalState.Instance.ModeAugmentedReality, ModeAvailability.Available);
                        }
                    }

                    //如果没有罗技相机那么返回不可用
                    return new SupportedMode(GlobalState.Instance.ModeAugmentedReality, ModeAvailability.NotAvailableNoWebcam);
                }
            }
            return new SupportedMode(IntPtr.Zero, ModeAvailability.NotAvailable);
        }

        /// <summary>
        /// Get the standard mode handle.
        /// </summary>
        /// 
        /// <returns>
        /// The standard mode handle.
        /// </returns>
        public IntPtr GetStandardMode()
        {
            return GlobalState.Instance.ModeStandard;
        }

        /// <summary>
        /// Get the 3D mode handle.
        /// </summary>
        /// 
        /// <returns>
        /// The 3D mode handle.
        /// </returns>
        public IntPtr GetThreeDMode()
        {
            return GlobalState.Instance.ModeThreeD;
        }

        /// <summary>
        /// Get the augmented reality mode handle.
        /// </summary>
        /// 
        /// <returns>
        /// The augmented reality mode.
        /// </returns>
        public IntPtr GetAugmentedRealityMode()
        {
            return GlobalState.Instance.ModeAugmentedReality;
        }

        /// <summary>
        /// Get the current mode of the specified connection.
        /// </summary>
        /// 
        /// <remarks>
        /// The current mode of a connection will change over the lifetime of the
        /// connection.  However, the mode queried via this function will only change
        /// during LateUpdate() (i.e. the value will remain stable between calls to 
        /// LateUpdate()).
        /// </remarks>
        /// 
        /// <param name="connection">
        /// The connection to set the current mode of.
        /// </param>
        /// 
        /// <returns>
        /// The current mode of the specified connection.
        /// </returns>
        /// 
        /// <exception cref="GCSeries.zView.InvalidParameterException">
        /// Thrown if the connection is null (IntPtr.Zero).
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionException">
        /// Thrown if the connection is invalid.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionStateException">
        /// Thrown if the connection is in the Closed or Error state.
        /// </exception>
        public IntPtr GetConnectionMode(IntPtr connection)
        {
            return this.CurVirtualCameraMode;
        }

        /// <summary>
        /// Set the current mode of the specified connection.
        /// </summary>
        /// 
        /// <remarks>
        /// If the current mode of the specified connection is not equal to the mode
        /// specified when this function is called, then this function will initiate a
        /// mode switch (the actual mode switch will occur asynchronously and will only
        /// become visible after the next LateUpdate() is called).
        /// </remarks>
        /// 
        /// <param name="connection">
        /// The connection to set the current mode of.
        /// </param>
        /// <param name="mode">
        /// The new mode to use as the current mode of the specified connection. 
        /// Passing IntPtr.Zero for this argument makes it so that there is no current 
        /// mode and transitions the connection into the NodeMode state.
        /// </param>
        /// 
        /// <exception cref="GCSeries.zView.InvalidParameterException">
        /// Thrown if the connection or the mode is null (IntPtr.Zero). Also thrown
        /// if the mode is not supported or not available.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionException">
        /// Thrown if the connection is invalid.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidModeException">
        /// Thrown if the mode is invalid.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionStateException">
        /// Thrown if the connection is in the Closed or Error state.
        /// </exception>
        public void SetConnectionMode(IntPtr connection, IntPtr mode)
        {
            Debug.Log("GView.SetConnectionMode():进入函数!");
            if (connection != IntPtr.Zero)
            {
                CurVirtualCameraMode = mode;
            }
        }

        /// <summary>
        /// Get the user data associated with the specified connection.
        /// </summary>
        /// 
        /// <param name="connection">
        /// The connection to get the user data of.
        /// </param>
        /// 
        /// <returns>
        /// The user data of the specified connection.
        /// </returns>
        /// 
        /// <exception cref="GCSeries.zView.InvalidParameterException">
        /// Thrown if the connection is null (IntPtr.Zero).
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionException">
        /// Thrown if the connection is invalid.
        /// </exception>
        public IntPtr GetConnectionUserData(IntPtr connection)
        {
            Debug.LogWarning("GView.GetConnectionUserData():这个函数目前未实现!");
            return IntPtr.Zero;
        }

        /// <summary>
        /// Set the user data associated with the specified connection.
        /// </summary>
        /// 
        /// <param name="connection">
        /// The connection to get the user data of.
        /// </param>
        /// <param name="userData">
        /// The user data of the specified connection.
        /// </param>
        /// 
        /// <exception cref="GCSeries.zView.InvalidParameterException">
        /// Thrown if the connection is null (IntPtr.Zero).
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionException">
        /// Thrown if the connection is invalid.
        /// </exception>
        public void SetConnectionUserData(IntPtr connection, IntPtr userData)
        {
            Debug.LogWarning("GView.SetConnectionUserData():这个函数目前未实现!");
        }

        /// <summary>
        /// Get the close action associated with the specified connection.
        /// </summary>
        /// 
        /// <remarks>
        /// The client code should perform this action if possible after a connection
        /// enters the Closed state.
        /// 
        /// It is only valid to call this function for a connection that is in the
        /// Closed state or in a callback registered against the ConnectedClosed event.
        /// </remarks>
        /// 
        /// <param name="connection">
        /// The connection to get the close action of.
        /// </param>
        /// 
        /// <returns>
        /// The close action of the specified connection.
        /// </returns>
        /// 
        /// <exception cref="GCSeries.zView.InvalidParameterException">
        /// Thrown if the connection is null (IntPtr.Zero).
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionException">
        /// Thrown if the connection is invalid.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionStateException">
        /// Thrown if the connection is not in the Closed state.
        /// </exception>
        public ConnectionCloseAction GetConnectionCloseAction(IntPtr connection)
        {
            Debug.LogWarning("GView.GetConnectionCloseAction():这个函数目前未实现!");
            return ConnectionCloseAction.None;
        }

        /// <summary>
        /// Get the close reason associated with the specified connection.
        /// </summary>
        /// 
        /// <remarks>
        /// It is only valid to call this function for a connection that is in the
        /// Closed state or in a callback registered against the ConnectedClosed event.
        /// </remarks>
        /// 
        /// <param name="connection">
        /// The connection to get the close reason of.
        /// </param>
        /// 
        /// <returns>
        /// The close reason of the specified connection.
        /// </returns>
        /// 
        /// <exception cref="GCSeries.zView.InvalidParameterException">
        /// Thrown if the connection is null (IntPtr.Zero).
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionException">
        /// Thrown if the connection is invalid.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionStateException">
        /// Thrown if the connection is not in the Closed state.
        /// </exception>
        public ConnectionCloseReason GetConnectionCloseReason(IntPtr connection)
        {
            Debug.LogWarning("GView.GetConnectionCloseReason():这个函数目前未实现!");
            ConnectionCloseReason reason = ConnectionCloseReason.Unknown;
            return reason;
        }

        /// <summary>
        /// Request that current mode be paused for the specified connection.
        /// </summary>
        /// 
        /// <remarks>
        /// Pausing will occur asynchronously and eventually become visible after a
        /// call to LateUpdate().
        ///
        /// It is only valid to call this function for a connection that is in the
        /// ModeActive state.
        /// </remarks>
        /// 
        /// <param name="connection">
        /// The connection to pause frame sending for.
        /// </param>
        /// 
        /// <exception cref="GCSeries.zView.InvalidParameterException">
        /// Thrown if the connection is null (IntPtr.Zero).
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionException">
        /// Thrown if the connection is invalid.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionStateException">
        /// Thrown if the connection is not in the ModeActive state.
        /// </exception>
        public void PauseMode(IntPtr connection)
        {
            Debug.LogWarning("GView.PauseMode():这个函数目前未实现!");
        }

        /// <summary>
        /// Request that the current mode be resumed for the specified connection.
        /// </summary>
        /// 
        /// <remarks>
        /// Resuming will occur asynchronously and eventually become visible after a
        /// call to LateUpdate().
        ///
        /// It is only valid to call this function for a connection that is in the
        /// ModePaused state.
        /// </remarks>
        /// 
        /// <param name="connection">
        /// The connection to resume frame sending for.
        /// </param>
        /// 
        /// <exception cref="GCSeries.zView.InvalidParameterException">
        /// Thrown if the connection is null (IntPtr.Zero).
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionException">
        /// Thrown if the connection is invalid.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionStateException">
        /// Thrown if the connection is not in the ModePaused state.
        /// </exception>
        public void ResumeMode(IntPtr connection)
        {
            Debug.LogWarning("GView.ResumeMode():这个函数目前未实现!");
        }

        /// <summary>
        /// Begin a settings batch for the specified connection.
        /// </summary>
        /// 
        /// <remarks>
        /// While a settings batch is active for a connection, changes to setting
        /// values will not be sent over the connection until the settings batch is
        /// ended (via a call to EndSettingsBatch()). This allows multiple settings
        /// value changes to be sent as an atomic unit. This is necessary when a group
        /// of settings are interrelated and changing one setting in the group requires
        /// other settings in the group to also be changed in order to keep all
        /// settings in the group in a consistent state.
        /// 
        /// At most one settings batch can be active at any time for a particular
        /// connection. Attempting to begin a settings batch for a connection when the
        /// connection already has an active settings batch will result in an error.
        /// </remarks>
        /// 
        /// <param name="connection">
        /// The connection to begin a settings batch for.
        /// </param>
        /// 
        /// <exception cref="GCSeries.zView.InvalidParameterException">
        /// Thrown if the connection is null (IntPtr.Zero).
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionException">
        /// Thrown if the connection is invalid.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionStateException">
        /// Thrown if the connection is in the Closed or Error state.
        /// </exception>
        public void BeginSettingsBatch(IntPtr connection)
        {
            Debug.LogWarning("GView.BeginSettingsBatch():这个函数目前未实现!");
        }

        /// <summary>
        /// End a settings batch for the specified connection.
        /// </summary>
        /// 
        /// <remarks>
        /// Attempting to end a setting batch for a connection that does not have an
        /// active settings batch will result in an error.
        /// </remarks>
        /// 
        /// <param name="connection">
        /// The connection to end a settings batch for.
        /// </param>
        /// 
        /// <exception cref="GCSeries.zView.InvalidParameterException">
        /// Thrown if the connection is null (IntPtr.Zero).
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionException">
        /// Thrown if the connection is invalid.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionStateException">
        /// Thrown if the connection is in the Closed or Error state.
        /// </exception>
        public void EndSettingsBatch(IntPtr connection)
        {
            Debug.LogWarning("GView.EndSettingsBatch():这个函数目前未实现!");
        }

        /// <summary>
        /// Get the value of the specified setting of type bool for the specified
        /// connection.
        /// </summary>
        /// 
        /// <param name="connection">
        /// The connection to get the setting value for.
        /// </param>
        /// <param name="key">
        /// The setting key to get the value of.
        /// </param>
        /// 
        /// <returns>
        /// The value associated with specified connection and setting key.
        /// </returns>
        /// 
        /// <exception cref="GCSeries.zView.InvalidParameterException">
        /// Thrown if the connection is null (IntPtr.Zero).
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionException">
        /// Thrown if the connection is invalid.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionStateException">
        /// Thrown if the connection is in the Closed or Error state.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidSettingKeyException">
        /// Thrown if the setting key is invalid.
        /// </exception>
        public bool GetSettingBool(IntPtr connection, SettingKey key)
        {
            Debug.LogWarning("GView.GetSettingBool():这个函数目前未实现!key=" + key);
            bool value = false;
            return value;
        }

        /// <summary>
        /// Get the value of the specified setting of type sbyte (Int8) for the 
        /// specified connection.
        /// </summary>
        /// 
        /// <param name="connection">
        /// The connection to get the setting value for.
        /// </param>
        /// <param name="key">
        /// The setting key to get the value of.
        /// </param>
        /// 
        /// <returns>
        /// The value associated with specified connection and setting key.
        /// </returns>
        /// 
        /// <exception cref="GCSeries.zView.InvalidParameterException">
        /// Thrown if the connection is null (IntPtr.Zero).
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionException">
        /// Thrown if the connection is invalid.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionStateException">
        /// Thrown if the connection is in the Closed or Error state.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidSettingKeyException">
        /// Thrown if the setting key is invalid.
        /// </exception>
        public sbyte GetSettingInt8(IntPtr connection, SettingKey key)
        {
            Debug.LogWarning("GView.GetSettingInt8():这个函数目前未实现!key=" + key);
            sbyte value = 0;
            return value;
        }

        /// <summary>
        /// Get the value of the specified setting of type Int16 for the specified
        /// connection.
        /// </summary>
        /// 
        /// <param name="connection">
        /// The connection to get the setting value for.
        /// </param>
        /// <param name="key">
        /// The setting key to get the value of.
        /// </param>
        /// 
        /// <returns>
        /// The value associated with specified connection and setting key.
        /// </returns>
        /// 
        /// <exception cref="GCSeries.zView.InvalidParameterException">
        /// Thrown if the connection is null (IntPtr.Zero).
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionException">
        /// Thrown if the connection is invalid.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionStateException">
        /// Thrown if the connection is in the Closed or Error state.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidSettingKeyException">
        /// Thrown if the setting key is invalid.
        /// </exception>
        public Int16 GetSettingInt16(IntPtr connection, SettingKey key)
        {
            Debug.LogWarning("GView.GetSettingInt16():这个函数目前未实现!key=" + key);
            Int16 value = 0;
            return value;
        }

        /// <summary>
        /// Get the value of the specified setting of type Int32 for the specified
        /// connection.
        /// </summary>
        /// 
        /// <param name="connection">
        /// The connection to get the setting value for.
        /// </param>
        /// <param name="key">
        /// The setting key to get the value of.
        /// </param>
        /// 
        /// <returns>
        /// The value associated with specified connection and setting key.
        /// </returns>
        /// 
        /// <exception cref="GCSeries.zView.InvalidParameterException">
        /// Thrown if the connection is null (IntPtr.Zero).
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionException">
        /// Thrown if the connection is invalid.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionStateException">
        /// Thrown if the connection is in the Closed or Error state.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidSettingKeyException">
        /// Thrown if the setting key is invalid.
        /// </exception>
        public Int32 GetSettingInt32(IntPtr connection, SettingKey key)
        {
            Debug.LogWarning("GView.GetSettingInt32():这个函数目前未实现!key=" + key);
            Int32 value = 0;
            return value;
        }

        /// <summary>
        /// Get the value of the specified setting of type Int64 for the specified
        /// connection.
        /// </summary>
        /// 
        /// <param name="connection">
        /// The connection to get the setting value for.
        /// </param>
        /// <param name="key">
        /// The setting key to get the value of.
        /// </param>
        /// 
        /// <returns>
        /// The value associated with specified connection and setting key.
        /// </returns>
        /// 
        /// <exception cref="GCSeries.zView.InvalidParameterException">
        /// Thrown if the connection is null (IntPtr.Zero).
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionException">
        /// Thrown if the connection is invalid.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionStateException">
        /// Thrown if the connection is in the Closed or Error state.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidSettingKeyException">
        /// Thrown if the setting key is invalid.
        /// </exception>
        public Int64 GetSettingInt64(IntPtr connection, SettingKey key)
        {
            Debug.LogWarning("GView.GetSettingInt64():这个函数目前未实现!key=" + key);
            Int64 value = 0;
            return value;
        }

        /// <summary>
        /// Get the value of the specified setting of type byte (UInt8) for the 
        /// specified connection.
        /// </summary>
        /// 
        /// <param name="connection">
        /// The connection to get the setting value for.
        /// </param>
        /// <param name="key">
        /// The setting key to get the value of.
        /// </param>
        /// 
        /// <returns>
        /// The value associated with specified connection and setting key.
        /// </returns>
        /// 
        /// <exception cref="GCSeries.zView.InvalidParameterException">
        /// Thrown if the connection is null (IntPtr.Zero).
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionException">
        /// Thrown if the connection is invalid.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionStateException">
        /// Thrown if the connection is in the Closed or Error state.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidSettingKeyException">
        /// Thrown if the setting key is invalid.
        /// </exception>
        public byte GetSettingUInt8(IntPtr connection, SettingKey key)
        {
            Debug.LogWarning("GView.GetSettingUInt8():这个函数目前未实现!key=" + key);
            byte value = 0;
            return value;
        }

        /// <summary>
        /// Get the value of the specified setting of type UInt16 for the specified
        /// connection.
        /// </summary>
        /// 
        /// <param name="connection">
        /// The connection to get the setting value for.
        /// </param>
        /// <param name="key">
        /// The setting key to get the value of.
        /// </param>
        /// 
        /// <returns>
        /// The value associated with specified connection and setting key.
        /// </returns>
        /// 
        /// <exception cref="GCSeries.zView.InvalidParameterException">
        /// Thrown if the connection is null (IntPtr.Zero).
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionException">
        /// Thrown if the connection is invalid.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionStateException">
        /// Thrown if the connection is in the Closed or Error state.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidSettingKeyException">
        /// Thrown if the setting key is invalid.
        /// </exception>
        public UInt16 GetSettingUInt16(IntPtr connection, SettingKey key)
        {
            Debug.LogWarning("GView.GetSettingUInt16():这个函数目前未实现!key=" + key);
            UInt16 value = 0;
            return value;
        }

        /// <summary>
        /// Get the value of the specified setting of type UInt32 for the specified
        /// connection.
        /// </summary>
        /// 
        /// <param name="connection">
        /// The connection to get the setting value for.
        /// </param>
        /// <param name="key">
        /// The setting key to get the value of.
        /// </param>
        /// 
        /// <returns>
        /// The value associated with specified connection and setting key.
        /// </returns>
        /// 
        /// <exception cref="GCSeries.zView.InvalidParameterException">
        /// Thrown if the connection is null (IntPtr.Zero).
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionException">
        /// Thrown if the connection is invalid.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionStateException">
        /// Thrown if the connection is in the Closed or Error state.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidSettingKeyException">
        /// Thrown if the setting key is invalid.
        /// </exception>
        public UInt32 GetSettingUInt32(IntPtr connection, SettingKey key)
        {
            Debug.LogWarning("GView.GetSettingUInt32():这个函数目前未实现!key=" + key);
            UInt32 value = 0;
            return value;
        }

        /// <summary>
        /// Get the value of the specified setting of type UInt64 for the specified
        /// connection.
        /// </summary>
        /// 
        /// <param name="connection">
        /// The connection to get the setting value for.
        /// </param>
        /// <param name="key">
        /// The setting key to get the value of.
        /// </param>
        /// 
        /// <returns>
        /// The value associated with specified connection and setting key.
        /// </returns>
        /// 
        /// <exception cref="GCSeries.zView.InvalidParameterException">
        /// Thrown if the connection is null (IntPtr.Zero).
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionException">
        /// Thrown if the connection is invalid.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionStateException">
        /// Thrown if the connection is in the Closed or Error state.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidSettingKeyException">
        /// Thrown if the setting key is invalid.
        /// </exception>
        public UInt64 GetSettingUInt64(IntPtr connection, SettingKey key)
        {
            Debug.LogWarning("GView.GetSettingUInt64():这个函数目前未实现!key=" + key);
            UInt64 value = 0;
            return value;
        }

        /// <summary>
        /// Get the value of the specified setting of type float for the specified
        /// connection.
        /// </summary>
        /// 
        /// <param name="connection">
        /// The connection to get the setting value for.
        /// </param>
        /// <param name="key">
        /// The setting key to get the value of.
        /// </param>
        /// 
        /// <returns>
        /// The value associated with specified connection and setting key.
        /// </returns>
        /// 
        /// <exception cref="GCSeries.zView.InvalidParameterException">
        /// Thrown if the connection is null (IntPtr.Zero).
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionException">
        /// Thrown if the connection is invalid.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionStateException">
        /// Thrown if the connection is in the Closed or Error state.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidSettingKeyException">
        /// Thrown if the setting key is invalid.
        /// </exception>
        public float GetSettingFloat(IntPtr connection, SettingKey key)
        {
            Debug.LogWarning("GView.GetSettingFloat():这个函数目前未实现!key=" + key);
            float value = 0;
            return value;
        }

        /// <summary>
        /// Get the value of the specified setting of type double for the specified
        /// connection.
        /// </summary>
        /// 
        /// <param name="connection">
        /// The connection to get the setting value for.
        /// </param>
        /// <param name="key">
        /// The setting key to get the value of.
        /// </param>
        /// 
        /// <returns>
        /// The value associated with specified connection and setting key.
        /// </returns>
        /// 
        /// <exception cref="GCSeries.zView.InvalidParameterException">
        /// Thrown if the connection is null (IntPtr.Zero).
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionException">
        /// Thrown if the connection is invalid.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionStateException">
        /// Thrown if the connection is in the Closed or Error state.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidSettingKeyException">
        /// Thrown if the setting key is invalid.
        /// </exception>
        public double GetSettingDouble(IntPtr connection, SettingKey key)
        {
            Debug.LogWarning("GView.GetSettingDouble():这个函数目前未实现!key=" + key);
            double value = 0;
            return value;
        }

        /// <summary>
        /// Get the value of the specified setting of type Vector3 for the specified
        /// connection.
        /// </summary>
        /// 
        /// <param name="connection">
        /// The connection to get the setting value for.
        /// </param>
        /// <param name="key">
        /// The setting key to get the value of.
        /// </param>
        /// 
        /// <returns>
        /// The value associated with specified connection and setting key.
        /// </returns>
        /// 
        /// <exception cref="GCSeries.zView.InvalidParameterException">
        /// Thrown if the connection is null (IntPtr.Zero).
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionException">
        /// Thrown if the connection is invalid.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionStateException">
        /// Thrown if the connection is in the Closed or Error state.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidSettingKeyException">
        /// Thrown if the setting key is invalid.
        /// </exception>
        public Vector3 GetSettingVector3(IntPtr connection, SettingKey key)
        {
            Debug.LogWarning("GView.GetSettingVector3():这个函数目前未实现!key=" + key);
            return Vector3.zero;
        }

        /// <summary>
        /// Get the value of the specified setting of type Matrix4x4 for the specified
        /// connection.
        /// </summary>
        /// 
        /// <param name="connection">
        /// The connection to get the setting value for.
        /// </param>
        /// <param name="key">
        /// The setting key to get the value of.
        /// </param>
        /// 
        /// <returns>
        /// The value associated with specified connection and setting key.
        /// </returns>
        /// 
        /// <exception cref="GCSeries.zView.InvalidParameterException">
        /// Thrown if the connection is null (IntPtr.Zero).
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionException">
        /// Thrown if the connection is invalid.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionStateException">
        /// Thrown if the connection is in the Closed or Error state.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidSettingKeyException">
        /// Thrown if the setting key is invalid.
        /// </exception>
        public Matrix4x4 GetSettingMatrix4x4(IntPtr connection, SettingKey key)
        {
            Debug.LogWarning("GView.GetSettingMatrix4x4():这个函数目前未实现!key=" + key);
            return Matrix4x4.zero;
        }

        /// <summary>
        /// Set the value of the specified setting of type bool for the specified
        /// connection.
        /// </summary>
        /// 
        /// <remarks>
        /// When the value of a setting is set, its state will transition to
        /// ChangePending until the new value has been accepted by the other side 
        /// of the associated connection.
        /// </remarks>
        /// 
        /// <param name="connection">
        /// The connection to set the setting value for.
        /// </param>
        /// <param name="key">
        /// The setting key to set the value of.
        /// </param>
        /// <param name="value">
        /// The new value for the specified setting key for the specified connection.
        /// </param>
        /// 
        /// <exception cref="GCSeries.zView.InvalidParameterException">
        /// Thrown if the connection is null (IntPtr.Zero).
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionException">
        /// Thrown if the connection is invalid.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionStateException">
        /// Thrown if the connection is in the Closed or Error state.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidSettingKeyException">
        /// Thrown if the setting key is invalid.
        /// </exception>
        public void SetSetting(IntPtr connection, SettingKey key, bool value)
        {
            Debug.LogWarning("GView.SetSetting():这个函数目前未实现!key=" + key + ",value=" + value);
        }

        /// <summary>
        /// Set the value of the specified setting of type sbyte (Int8) for the 
        /// specified connection.
        /// </summary>
        /// 
        /// <remarks>
        /// When the value of a setting is set, its state will transition to
        /// ChangePending until the new value has been accepted by the other side 
        /// of the associated connection.
        /// </remarks>
        /// 
        /// <param name="connection">
        /// The connection to set the setting value for.
        /// </param>
        /// <param name="key">
        /// The setting key to set the value of.
        /// </param>
        /// <param name="value">
        /// The new value for the specified setting key for the specified connection.
        /// </param>
        /// 
        /// <exception cref="GCSeries.zView.InvalidParameterException">
        /// Thrown if the connection is null (IntPtr.Zero).
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionException">
        /// Thrown if the connection is invalid.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionStateException">
        /// Thrown if the connection is in the Closed or Error state.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidSettingKeyException">
        /// Thrown if the setting key is invalid.
        /// </exception>
        public void SetSetting(IntPtr connection, SettingKey key, sbyte value)
        {
            Debug.LogWarning("GView.SetSetting():这个函数目前未实现!key=" + key + ",value=" + value);
        }

        /// <summary>
        /// Set the value of the specified setting of type Int16 for the specified
        /// connection.
        /// </summary>
        /// 
        /// <remarks>
        /// When the value of a setting is set, its state will transition to
        /// ChangePending until the new value has been accepted by the other side 
        /// of the associated connection.
        /// </remarks>
        /// 
        /// <param name="connection">
        /// The connection to set the setting value for.
        /// </param>
        /// <param name="key">
        /// The setting key to set the value of.
        /// </param>
        /// <param name="value">
        /// The new value for the specified setting key for the specified connection.
        /// </param>
        /// 
        /// <exception cref="GCSeries.zView.InvalidParameterException">
        /// Thrown if the connection is null (IntPtr.Zero).
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionException">
        /// Thrown if the connection is invalid.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionStateException">
        /// Thrown if the connection is in the Closed or Error state.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidSettingKeyException">
        /// Thrown if the setting key is invalid.
        /// </exception>
        public void SetSetting(IntPtr connection, SettingKey key, Int16 value)
        {
            Debug.LogWarning("GView.SetSetting():这个函数目前未实现!key=" + key + ",value=" + value);
        }

        /// <summary>
        /// Set the value of the specified setting of type Int32 for the specified
        /// connection.
        /// </summary>
        /// 
        /// <remarks>
        /// When the value of a setting is set, its state will transition to
        /// ChangePending until the new value has been accepted by the other side 
        /// of the associated connection.
        /// </remarks>
        /// 
        /// <param name="connection">
        /// The connection to set the setting value for.
        /// </param>
        /// <param name="key">
        /// The setting key to set the value of.
        /// </param>
        /// <param name="value">
        /// The new value for the specified setting key for the specified connection.
        /// </param>
        /// 
        /// <exception cref="GCSeries.zView.InvalidParameterException">
        /// Thrown if the connection is null (IntPtr.Zero).
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionException">
        /// Thrown if the connection is invalid.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionStateException">
        /// Thrown if the connection is in the Closed or Error state.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidSettingKeyException">
        /// Thrown if the setting key is invalid.
        /// </exception>
        public void SetSetting(IntPtr connection, SettingKey key, Int32 value)
        {
            Debug.LogWarning("GView.SetSetting():这个函数目前未实现!key=" + key + ",value=" + value);
        }

        /// <summary>
        /// Set the value of the specified setting of type Int64 for the specified
        /// connection.
        /// </summary>
        /// 
        /// <remarks>
        /// When the value of a setting is set, its state will transition to
        /// ChangePending until the new value has been accepted by the other side 
        /// of the associated connection.
        /// </remarks>
        /// 
        /// <param name="connection">
        /// The connection to set the setting value for.
        /// </param>
        /// <param name="key">
        /// The setting key to set the value of.
        /// </param>
        /// <param name="value">
        /// The new value for the specified setting key for the specified connection.
        /// </param>
        /// 
        /// <exception cref="GCSeries.zView.InvalidParameterException">
        /// Thrown if the connection is null (IntPtr.Zero).
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionException">
        /// Thrown if the connection is invalid.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionStateException">
        /// Thrown if the connection is in the Closed or Error state.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidSettingKeyException">
        /// Thrown if the setting key is invalid.
        /// </exception>
        public void SetSetting(IntPtr connection, SettingKey key, Int64 value)
        {
            Debug.LogWarning("GView.SetSetting():这个函数目前未实现!key=" + key + ",value=" + value);
        }

        /// <summary>
        /// Set the value of the specified setting of type byte (UInt8) for the 
        /// specified connection.
        /// </summary>
        /// 
        /// <remarks>
        /// When the value of a setting is set, its state will transition to
        /// ChangePending until the new value has been accepted by the other side 
        /// of the associated connection.
        /// </remarks>
        /// 
        /// <param name="connection">
        /// The connection to set the setting value for.
        /// </param>
        /// <param name="key">
        /// The setting key to set the value of.
        /// </param>
        /// <param name="value">
        /// The new value for the specified setting key for the specified connection.
        /// </param>
        /// 
        /// <exception cref="GCSeries.zView.InvalidParameterException">
        /// Thrown if the connection is null (IntPtr.Zero).
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionException">
        /// Thrown if the connection is invalid.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionStateException">
        /// Thrown if the connection is in the Closed or Error state.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidSettingKeyException">
        /// Thrown if the setting key is invalid.
        /// </exception>
        public void SetSetting(IntPtr connection, SettingKey key, byte value)
        {
            Debug.LogWarning("GView.SetSetting():这个函数目前未实现!key=" + key + ",value=" + value);
        }

        /// <summary>
        /// Set the value of the specified setting of type UInt16 for the specified
        /// connection.
        /// </summary>
        /// 
        /// <remarks>
        /// When the value of a setting is set, its state will transition to
        /// ChangePending until the new value has been accepted by the other side 
        /// of the associated connection.
        /// </remarks>
        /// 
        /// <param name="connection">
        /// The connection to set the setting value for.
        /// </param>
        /// <param name="key">
        /// The setting key to set the value of.
        /// </param>
        /// <param name="value">
        /// The new value for the specified setting key for the specified connection.
        /// </param>
        /// 
        /// <exception cref="GCSeries.zView.InvalidParameterException">
        /// Thrown if the connection is null (IntPtr.Zero).
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionException">
        /// Thrown if the connection is invalid.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionStateException">
        /// Thrown if the connection is in the Closed or Error state.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidSettingKeyException">
        /// Thrown if the setting key is invalid.
        /// </exception>
        public void SetSetting(IntPtr connection, SettingKey key, UInt16 value)
        {
            Debug.LogWarning("GView.SetSetting():这个函数目前未实现!key=" + key + ",value=" + value);
        }

        /// <summary>
        /// Set the value of the specified setting of type UInt32 for the specified
        /// connection.
        /// </summary>
        /// 
        /// <remarks>
        /// When the value of a setting is set, its state will transition to
        /// ChangePending until the new value has been accepted by the other side 
        /// of the associated connection.
        /// </remarks>
        /// 
        /// <param name="connection">
        /// The connection to set the setting value for.
        /// </param>
        /// <param name="key">
        /// The setting key to set the value of.
        /// </param>
        /// <param name="value">
        /// The new value for the specified setting key for the specified connection.
        /// </param>
        /// 
        /// <exception cref="GCSeries.zView.InvalidParameterException">
        /// Thrown if the connection is null (IntPtr.Zero).
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionException">
        /// Thrown if the connection is invalid.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionStateException">
        /// Thrown if the connection is in the Closed or Error state.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidSettingKeyException">
        /// Thrown if the setting key is invalid.
        /// </exception>
        public void SetSetting(IntPtr connection, SettingKey key, UInt32 value)
        {
            Debug.LogWarning("GView.SetSetting():这个函数目前未实现!key=" + key + ",value=" + value);
        }

        /// <summary>
        /// Set the value of the specified setting of type UInt64 for the specified
        /// connection.
        /// </summary>
        /// 
        /// <remarks>
        /// When the value of a setting is set, its state will transition to
        /// ChangePending until the new value has been accepted by the other side 
        /// of the associated connection.
        /// </remarks>
        /// 
        /// <param name="connection">
        /// The connection to set the setting value for.
        /// </param>
        /// <param name="key">
        /// The setting key to set the value of.
        /// </param>
        /// <param name="value">
        /// The new value for the specified setting key for the specified connection.
        /// </param>
        /// 
        /// <exception cref="GCSeries.zView.InvalidParameterException">
        /// Thrown if the connection is null (IntPtr.Zero).
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionException">
        /// Thrown if the connection is invalid.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionStateException">
        /// Thrown if the connection is in the Closed or Error state.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidSettingKeyException">
        /// Thrown if the setting key is invalid.
        /// </exception>
        public void SetSetting(IntPtr connection, SettingKey key, UInt64 value)
        {
            Debug.LogWarning("GView.SetSetting():这个函数目前未实现!key=" + key + ",value=" + value);
        }

        /// <summary>
        /// Set the value of the specified setting of type float for the specified
        /// connection.
        /// </summary>
        /// 
        /// <remarks>
        /// When the value of a setting is set, its state will transition to
        /// ChangePending until the new value has been accepted by the other side 
        /// of the associated connection.
        /// </remarks>
        /// 
        /// <param name="connection">
        /// The connection to set the setting value for.
        /// </param>
        /// <param name="key">
        /// The setting key to set the value of.
        /// </param>
        /// <param name="value">
        /// The new value for the specified setting key for the specified connection.
        /// </param>
        /// 
        /// <exception cref="GCSeries.zView.InvalidParameterException">
        /// Thrown if the connection is null (IntPtr.Zero).
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionException">
        /// Thrown if the connection is invalid.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionStateException">
        /// Thrown if the connection is in the Closed or Error state.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidSettingKeyException">
        /// Thrown if the setting key is invalid.
        /// </exception>
        public void SetSetting(IntPtr connection, SettingKey key, float value)
        {
            Debug.LogWarning("GView.SetSetting():这个函数目前未实现!key=" + key + ",value=" + value);
        }

        /// <summary>
        /// Set the value of the specified setting of type double for the specified
        /// connection.
        /// </summary>
        /// 
        /// <remarks>
        /// When the value of a setting is set, its state will transition to
        /// ChangePending until the new value has been accepted by the other side 
        /// of the associated connection.
        /// </remarks>
        /// 
        /// <param name="connection">
        /// The connection to set the setting value for.
        /// </param>
        /// <param name="key">
        /// The setting key to set the value of.
        /// </param>
        /// <param name="value">
        /// The new value for the specified setting key for the specified connection.
        /// </param>
        /// 
        /// <exception cref="GCSeries.zView.InvalidParameterException">
        /// Thrown if the connection is null (IntPtr.Zero).
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionException">
        /// Thrown if the connection is invalid.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionStateException">
        /// Thrown if the connection is in the Closed or Error state.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidSettingKeyException">
        /// Thrown if the setting key is invalid.
        /// </exception>
        public void SetSetting(IntPtr connection, SettingKey key, double value)
        {
            Debug.LogWarning("GView.SetSetting():这个函数目前未实现!key=" + key + ",value=" + value);
        }

        /// <summary>
        /// Set the value of the specified setting of type Vector3 for the specified
        /// connection.
        /// </summary>
        /// 
        /// <remarks>
        /// When the value of a setting is set, its state will transition to
        /// ChangePending until the new value has been accepted by the other side 
        /// of the associated connection.
        /// </remarks>
        /// 
        /// <param name="connection">
        /// The connection to set the setting value for.
        /// </param>
        /// <param name="key">
        /// The setting key to set the value of.
        /// </param>
        /// <param name="value">
        /// The new value for the specified setting key for the specified connection.
        /// </param>
        /// 
        /// <exception cref="GCSeries.zView.InvalidParameterException">
        /// Thrown if the connection is null (IntPtr.Zero).
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionException">
        /// Thrown if the connection is invalid.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionStateException">
        /// Thrown if the connection is in the Closed or Error state.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidSettingKeyException">
        /// Thrown if the setting key is invalid.
        /// </exception>
        public void SetSetting(IntPtr connection, SettingKey key, Vector3 value)
        {
            Debug.LogWarning("GView.SetSetting():这个函数目前未实现!key=" + key + ",value=" + value);
        }

        /// <summary>
        /// Set the value of the specified setting of type Matrix4x4 for the specified
        /// connection.
        /// </summary>
        /// 
        /// <remarks>
        /// When the value of a setting is set, its state will transition to
        /// ChangePending until the new value has been accepted by the other side 
        /// of the associated connection.
        /// </remarks>
        /// 
        /// <param name="connection">
        /// The connection to set the setting value for.
        /// </param>
        /// <param name="key">
        /// The setting key to set the value of.
        /// </param>
        /// <param name="value">
        /// The new value for the specified setting key for the specified connection.
        /// </param>
        /// 
        /// <exception cref="GCSeries.zView.InvalidParameterException">
        /// Thrown if the connection is null (IntPtr.Zero).
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionException">
        /// Thrown if the connection is invalid.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionStateException">
        /// Thrown if the connection is in the Closed or Error state.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidSettingKeyException">
        /// Thrown if the setting key is invalid.
        /// </exception>
        public void SetSetting(IntPtr connection, SettingKey key, Matrix4x4 value)
        {
            Debug.LogWarning("GView.SetSetting():这个函数目前未实现!key=" + key + ",value=" + value);
        }

        /// <summary>
        /// Get the state of the specified setting for the specified connection
        /// </summary>
        /// 
        /// <param name="connection">
        /// The connection to get the setting state for.
        /// </param>
        /// <param name="key">
        /// The setting key to get the state of.
        /// </param>
        /// 
        /// <returns>
        /// The state of the specified setting state for the specified connection.
        /// </returns>
        /// 
        /// <exception cref="GCSeries.zView.InvalidParameterException">
        /// Thrown if the connection is null (IntPtr.Zero).
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionException">
        /// Thrown if the connection is invalid.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionStateException">
        /// Thrown if the connection is in the Closed or Error state.
        /// </exception>
        public SettingState GetSettingState(IntPtr connection, SettingKey key)
        {
            Debug.LogWarning("GView.GetSettingState():这个函数目前未实现!key=" + key);
            SettingState state;
            state = SettingState.UpToDate;
            return state;
        }

        /// <summary>
        /// Get the value of the specified frame data of type bool for the 
        /// specified frame.
        /// </summary>
        /// 
        /// <param name="frame">
        /// The frame to get frame data from.
        /// </param>
        /// <param name="key">
        /// The frame data key to get the value of.
        /// </param>
        /// 
        /// <returns>
        /// The value associated with specified frame and frame data key.
        /// </returns>
        /// 
        /// <exception cref="GCSeries.zView.InvalidParameterException">
        /// Thrown if the frame is null (IntPtr.Zero).
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidFrameException">
        /// Thrown if the frame is invalid.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidFrameDataKeyException">
        /// Thrown if the frame data key is invalid.
        /// </exception>
        public bool GetFrameDataBool(IntPtr frame, FrameDataKey key)
        {
            Debug.LogWarning("GView.GetFrameDataBool():这个函数目前未实现!key=" + key);
            bool value = false;
            return value;
        }

        /// <summary>
        /// Get the value of the specified frame data of type sbyte (Int8) for the 
        /// specified frame.
        /// </summary>
        /// 
        /// <param name="frame">
        /// The frame to get frame data from.
        /// </param>
        /// <param name="key">
        /// The frame data key to get the value of.
        /// </param>
        /// 
        /// <returns>
        /// The value associated with specified frame and frame data key.
        /// </returns>
        /// 
        /// <exception cref="GCSeries.zView.InvalidParameterException">
        /// Thrown if the frame is null (IntPtr.Zero).
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidFrameException">
        /// Thrown if the frame is invalid.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidFrameDataKeyException">
        /// Thrown if the frame data key is invalid.
        /// </exception>
        public sbyte GetFrameDataInt8(IntPtr frame, FrameDataKey key)
        {
            Debug.LogWarning("GView.GetFrameDataInt8():这个函数目前未实现!key=" + key);
            sbyte value = 0;
            return value;
        }

        /// <summary>
        /// Get the value of the specified frame data of type Int16 for the 
        /// specified frame.
        /// </summary>
        /// 
        /// <param name="frame">
        /// The frame to get frame data from.
        /// </param>
        /// <param name="key">
        /// The frame data key to get the value of.
        /// </param>
        /// 
        /// <returns>
        /// The value associated with specified frame and frame data key.
        /// </returns>
        /// 
        /// <exception cref="GCSeries.zView.InvalidParameterException">
        /// Thrown if the frame is null (IntPtr.Zero).
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidFrameException">
        /// Thrown if the frame is invalid.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidFrameDataKeyException">
        /// Thrown if the frame data key is invalid.
        /// </exception>
        public Int16 GetFrameDataInt16(IntPtr frame, FrameDataKey key)
        {
            Debug.LogWarning("GView.GetFrameDataInt16():这个函数目前未实现!key=" + key);
            Int16 value = 0;
            return value;
        }

        /// <summary>
        /// Get the value of the specified frame data of type Int32 for the 
        /// specified frame.
        /// </summary>
        /// 
        /// <param name="frame">
        /// The frame to get frame data from.
        /// </param>
        /// <param name="key">
        /// The frame data key to get the value of.
        /// </param>
        /// 
        /// <returns>
        /// The value associated with specified frame and frame data key.
        /// </returns>
        /// 
        /// <exception cref="GCSeries.zView.InvalidParameterException">
        /// Thrown if the frame is null (IntPtr.Zero).
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidFrameException">
        /// Thrown if the frame is invalid.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidFrameDataKeyException">
        /// Thrown if the frame data key is invalid.
        /// </exception>
        public Int32 GetFrameDataInt32(IntPtr frame, FrameDataKey key)
        {
            Debug.LogWarning("GView.GetFrameDataInt32():这个函数目前未实现!key=" + key);
            Int32 value = 0;
            return value;
        }

        /// <summary>
        /// Get the value of the specified frame data of type Int64 for the 
        /// specified frame.
        /// </summary>
        /// 
        /// <param name="frame">
        /// The frame to get frame data from.
        /// </param>
        /// <param name="key">
        /// The frame data key to get the value of.
        /// </param>
        /// 
        /// <returns>
        /// The value associated with specified frame and frame data key.
        /// </returns>
        /// 
        /// <exception cref="GCSeries.zView.InvalidParameterException">
        /// Thrown if the frame is null (IntPtr.Zero).
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidFrameException">
        /// Thrown if the frame is invalid.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidFrameDataKeyException">
        /// Thrown if the frame data key is invalid.
        /// </exception>
        public Int64 GetFrameDataInt64(IntPtr frame, FrameDataKey key)
        {
            Debug.LogWarning("GView.GetFrameDataInt64():这个函数目前未实现!key=" + key);
            Int64 value = 0;
            return value;
        }

        /// <summary>
        /// Get the value of the specified frame data of type byte (UInt8) for the 
        /// specified frame.
        /// </summary>
        /// 
        /// <param name="frame">
        /// The frame to get frame data from.
        /// </param>
        /// <param name="key">
        /// The frame data key to get the value of.
        /// </param>
        /// 
        /// <returns>
        /// The value associated with specified frame and frame data key.
        /// </returns>
        /// 
        /// <exception cref="GCSeries.zView.InvalidParameterException">
        /// Thrown if the frame is null (IntPtr.Zero).
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidFrameException">
        /// Thrown if the frame is invalid.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidFrameDataKeyException">
        /// Thrown if the frame data key is invalid.
        /// </exception>
        public byte GetFrameDataUInt8(IntPtr frame, FrameDataKey key)
        {
            Debug.LogWarning("GView.GetFrameDataUInt8():这个函数目前未实现!key=" + key);
            byte value = 0;
            return value;
        }

        /// <summary>
        /// Get the value of the specified frame data of type UInt16 for the 
        /// specified frame.
        /// </summary>
        /// 
        /// <param name="frame">
        /// The frame to get frame data from.
        /// </param>
        /// <param name="key">
        /// The frame data key to get the value of.
        /// </param>
        /// 
        /// <returns>
        /// The value associated with specified frame and frame data key.
        /// </returns>
        /// 
        /// <exception cref="GCSeries.zView.InvalidParameterException">
        /// Thrown if the frame is null (IntPtr.Zero).
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidFrameException">
        /// Thrown if the frame is invalid.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidFrameDataKeyException">
        /// Thrown if the frame data key is invalid.
        /// </exception>
        public UInt16 GetFrameDataUInt16(IntPtr frame, FrameDataKey key)
        {
            Debug.LogWarning("GView.GetFrameDataUInt16():这个函数目前未实现!key=" + key);
            UInt16 value = 0;
            return value;
        }

        /// <summary>
        /// Get the value of the specified frame data of type UInt32 for the 
        /// specified frame.
        /// </summary>
        /// 
        /// <param name="frame">
        /// The frame to get frame data from.
        /// </param>
        /// <param name="key">
        /// The frame data key to get the value of.
        /// </param>
        /// 
        /// <returns>
        /// The value associated with specified frame and frame data key.
        /// </returns>
        /// 
        /// <exception cref="GCSeries.zView.InvalidParameterException">
        /// Thrown if the frame is null (IntPtr.Zero).
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidFrameException">
        /// Thrown if the frame is invalid.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidFrameDataKeyException">
        /// Thrown if the frame data key is invalid.
        /// </exception>
        public UInt32 GetFrameDataUInt32(IntPtr frame, FrameDataKey key)
        {
            Debug.LogWarning("GView.GetFrameDataUInt32():这个函数目前未实现!key=" + key);
            UInt32 value = 0;
            return value;
        }

        /// <summary>
        /// Get the value of the specified frame data of type UInt64 for the 
        /// specified frame.
        /// </summary>
        /// 
        /// <param name="frame">
        /// The frame to get frame data from.
        /// </param>
        /// <param name="key">
        /// The frame data key to get the value of.
        /// </param>
        /// 
        /// <returns>
        /// The value associated with specified frame and frame data key.
        /// </returns>
        /// 
        /// <exception cref="GCSeries.zView.InvalidParameterException">
        /// Thrown if the frame is null (IntPtr.Zero).
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidFrameException">
        /// Thrown if the frame is invalid.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidFrameDataKeyException">
        /// Thrown if the frame data key is invalid.
        /// </exception>
        public UInt64 GetFrameDataUInt64(IntPtr frame, FrameDataKey key)
        {
            Debug.LogWarning("GView.GetFrameDataUInt64():这个函数目前未实现!key=" + key);
            UInt64 value = 0;
            return value;
        }

        /// <summary>
        /// Get the value of the specified frame data of type float for the 
        /// specified frame.
        /// </summary>
        /// 
        /// <param name="frame">
        /// The frame to get frame data from.
        /// </param>
        /// <param name="key">
        /// The frame data key to get the value of.
        /// </param>
        /// 
        /// <returns>
        /// The value associated with specified frame and frame data key.
        /// </returns>
        /// 
        /// <exception cref="GCSeries.zView.InvalidParameterException">
        /// Thrown if the frame is null (IntPtr.Zero).
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidFrameException">
        /// Thrown if the frame is invalid.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidFrameDataKeyException">
        /// Thrown if the frame data key is invalid.
        /// </exception>
        public float GetFrameDataFloat(IntPtr frame, FrameDataKey key)
        {
            Debug.LogWarning("GView.GetFrameDataFloat():这个函数目前未实现!key=" + key);
            float value = 0;
            return value;
        }

        /// <summary>
        /// Get the value of the specified frame data of type double for the 
        /// specified frame.
        /// </summary>
        /// 
        /// <param name="frame">
        /// The frame to get frame data from.
        /// </param>
        /// <param name="key">
        /// The frame data key to get the value of.
        /// </param>
        /// 
        /// <returns>
        /// The value associated with specified frame and frame data key.
        /// </returns>
        /// 
        /// <exception cref="GCSeries.zView.InvalidParameterException">
        /// Thrown if the frame is null (IntPtr.Zero).
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidFrameException">
        /// Thrown if the frame is invalid.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidFrameDataKeyException">
        /// Thrown if the frame data key is invalid.
        /// </exception>
        public double GetFrameDataDouble(IntPtr frame, FrameDataKey key)
        {
            Debug.LogWarning("GView.GetFrameDataDouble():这个函数目前未实现!key=" + key);
            double value = 0;
            return value;
        }

        /// <summary>
        /// Get the value of the specified frame data of type Vector3 for the 
        /// specified frame.
        /// </summary>
        /// 
        /// <param name="frame">
        /// The frame to get frame data from.
        /// </param>
        /// <param name="key">
        /// The frame data key to get the value of.
        /// </param>
        /// 
        /// <returns>
        /// The value associated with specified frame and frame data key.
        /// </returns>
        /// 
        /// <exception cref="GCSeries.zView.InvalidParameterException">
        /// Thrown if the frame is null (IntPtr.Zero).
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidFrameException">
        /// Thrown if the frame is invalid.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidFrameDataKeyException">
        /// Thrown if the frame data key is invalid.
        /// </exception>
        public Vector3 GetFrameDataVector3(IntPtr frame, FrameDataKey key)
        {
            Debug.LogWarning("GView.GetFrameDataVector3():这个函数目前未实现!key=" + key);
            //ZSVector3 value;
            return Vector3.zero;
        }

        /// <summary>
        /// Get the value of the specified frame data of type Matrix4x4 for the 
        /// specified frame.
        /// </summary>
        /// 
        /// <param name="frame">
        /// The frame to get frame data from.
        /// </param>
        /// <param name="key">
        /// The frame data key to get the value of.
        /// </param>
        /// 
        /// <returns>
        /// The value associated with specified frame and frame data key.
        /// </returns>
        /// 
        /// <exception cref="GCSeries.zView.InvalidParameterException">
        /// Thrown if the frame is null (IntPtr.Zero).
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidFrameException">
        /// Thrown if the frame is invalid.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidFrameDataKeyException">
        /// Thrown if the frame data key is invalid.
        /// </exception>
        public Matrix4x4 GetFrameDataMatrix4x4(IntPtr frame, FrameDataKey key)
        {
            Debug.LogWarning("GView.GetFrameDataMatrix4x4():这个函数目前未实现!key=" + key);
            //ZSMatrix4 value;
            return Matrix4x4.zero;
        }

        /// <summary>
        /// Get the current video recording state of the specified connection.
        /// </summary>
        /// 
        /// <param name="connection">
        /// The connection to get the video recording state of.
        /// </param>
        /// 
        /// <returns>
        /// The video recording state of the specified connection.
        /// </returns>
        /// 
        /// <exception cref="GCSeries.zView.InvalidParameterException">
        /// Thrown if the connection is null (IntPtr.Zero).
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionException">
        /// Thrown if the connection is invalid.
        /// </exception>
        public VideoRecordingState GetVideoRecordingState(IntPtr connection)
        {
            VideoRecordingState state = VideoRecordingState.NotAvailable;
            if (connection == GlobalState.Instance.Connection)
            {
                try
                {
                    xxGetVideoRecordingState(out state);
                }
                catch (Exception)
                {
                }
            }
            return state;
        }

        /// <summary>
        /// Get the current video recording error code of the specified connection.
        /// </summary>
        /// 
        /// <remarks>
        /// Calling this method will fail unless the current video recording state is
        /// Error.
        /// </remarks>
        /// 
        /// <param name="connection">
        /// The connection to get the video recording error code of.
        /// </param>
        /// 
        /// <returns>
        /// The video recording error code of the specified connection.
        /// </returns>
        /// 
        /// <exception cref="GCSeries.zView.InvalidParameterException">
        /// Thrown if the connection is null (IntPtr.Zero).
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionException">
        /// Thrown if the connection is invalid.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionStateException">
        /// Thrown if the connection is in the Closed or Error state.
        /// </exception>
        /// <exception cref="GCSeries.zView.UnsupportedCapabilityException">
        /// Thrown if the connection does not support the video recording capability.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidVideoRecordingStateException">
        /// Thrown if the video recording is not in the Error state.
        /// </exception>
        public PluginError GetVideoRecordingError(IntPtr connection)
        {
            Debug.LogWarning("GView.GetVideoRecordingError():这个函数目前未实现!");
            PluginError videoRecordingError = PluginError.Ok;
            return videoRecordingError;
        }

        /// <summary>
        /// Clear the video recording error code of the specified connection.
        /// </summary>
        /// 
        /// <remarks>
        /// Transitions the video recording state of the connection from Error to some 
        /// other non-error state. Exactly which video recording state is transitioned 
        /// to depends on the video recording state that the connection was in prior to 
        /// it entering the Error state. In most cases, this function will transition the 
        /// video recording state to NotRecording. A notable exception is when the
        /// video recording state of the connection was Saving prior to it entering the 
        /// Error state. In this case, the video recording state may transition back to
        /// Finished after calling this function if a recoverable save error occurred and 
        /// it is still possible that the current video recording could be saved (e.g. with 
        /// a different file name or after some disk space has been freed).
        ///
        /// Calling this function will fail unless the current video recording state is
        /// Error.
        /// </remarks>
        /// 
        /// <param name="connection">
        /// The connection to clear the video recording error code of.
        /// </param>
        /// 
        /// <exception cref="GCSeries.zView.InvalidParameterException">
        /// Thrown if the connection is null (IntPtr.Zero).
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionException">
        /// Thrown if the connection is invalid.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionStateException">
        /// Thrown if the connection is in the Closed or Error state.
        /// </exception>
        /// <exception cref="GCSeries.zView.UnsupportedCapabilityException">
        /// Thrown if the connection does not support the video recording capability.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidVideoRecordingStateException">
        /// Thrown if the video recording is not in the Error state.
        /// </exception>
        public void ClearVideoRecordingError(IntPtr connection)
        {
            Debug.LogWarning("GView.ClearVideoRecordingError():这个函数目前未实现!");
        }

        /// <summary>
        /// Start video recording on the specified connection.
        /// </summary>
        /// 
        /// <remarks>
        /// Transitions the video recording state of the connection to Recording. While the 
        /// video recording state transition is taking place, the video recording state will be
        /// Starting.
        ///
        /// Calling this function will fail unless the current video recording state is
        /// NotRecording.
        /// </remarks>
        /// 
        /// <param name="connection">
        /// The connection to start video recording on.
        /// </param>
        /// 
        /// <exception cref="GCSeries.zView.InvalidParameterException">
        /// Thrown if the connection is null (IntPtr.Zero).
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionException">
        /// Thrown if the connection is invalid.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionStateException">
        /// Thrown if the connection is in the Closed or Error state.
        /// </exception>
        /// <exception cref="GCSeries.zView.UnsupportedCapabilityException">
        /// Thrown if the connection does not support the video recording capability.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidVideoRecordingStateException">
        /// Thrown if the video recording is not in the NotRecording state.
        /// </exception>
        public void StartVideoRecording(IntPtr connection)
        {
            Debug.Log("GView.StartVideoRecording():启动录屏!");
            try
            {
                Debug.Log("GView.StartVideoRecording():临时文件路径:" + tempRecFile);
                FARError err = xxStartScreenRec(tempRecFile);
                if (err != FARError.Ok)
                {
                    Debug.LogError("GView.StartVideoRecording():启动录屏失败! err=" + err);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("GView.StartVideoRecording():启动录屏异常e=" + e.Message);
            }

        }

        /// <summary>
        /// Finish video recording on the specified connection.
        /// </summary>
        /// 
        /// <remarks>
        /// Transitions the video recording state of the connection to Finished. While 
        /// the video recording state transition is taking place, the video recording state 
        /// will be Finishing.
        ///
        /// Calling this function will fail unless the current video recording state is
        /// Recording or Paused.
        /// </remarks>
        /// 
        /// <param name="connection">
        /// The connection to finish video recording on.
        /// </param>
        /// 
        /// <exception cref="GCSeries.zView.InvalidParameterException">
        /// Thrown if the connection is null (IntPtr.Zero).
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionException">
        /// Thrown if the connection is invalid.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionStateException">
        /// Thrown if the connection is in the Closed or Error state.
        /// </exception>
        /// <exception cref="GCSeries.zView.UnsupportedCapabilityException">
        /// Thrown if the connection does not support the video recording capability.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidVideoRecordingStateException">
        /// Thrown if the video recording is not in the Recording or Paused state.
        /// </exception>
        public void FinishVideoRecording(IntPtr connection)
        {
            Debug.Log("GView.FinishVideoRecording():停止录屏!");
            try
            {
                FARError err = xxStopScreenRec();
                if (err != FARError.Ok)
                {
                    Debug.LogError("GView.FinishVideoRecording():停止录屏失败! err=" + err);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("GView.FinishVideoRecording():停止录屏异常e=" + e.Message);
            }

        }

        /// <summary>
        /// Pause video recording on the specified connection.
        /// </summary>
        /// 
        /// <remarks>
        /// Transitions the video recording state of the connection to Paused. While the 
        /// video recording state transition is taking place, the video recording state will 
        /// be Pausing.
        ///
        /// Calling this function will fail unless the current video recording state is
        /// Recording.
        /// </remarks>
        /// 
        /// <param name="connection">
        /// The connection to pause video recording on.
        /// </param>
        /// 
        /// <exception cref="GCSeries.zView.InvalidParameterException">
        /// Thrown if the connection is null (IntPtr.Zero).
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionException">
        /// Thrown if the connection is invalid.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionStateException">
        /// Thrown if the connection is in the Closed or Error state.
        /// </exception>
        /// <exception cref="GCSeries.zView.UnsupportedCapabilityException">
        /// Thrown if the connection does not support the video recording capability.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidVideoRecordingStateException">
        /// Thrown if the video recording is not in the Recording state.
        /// </exception>
        public void PauseVideoRecording(IntPtr connection)
        {
            Debug.LogWarning("GView.PauseVideoRecording():这个函数目前未实现!");
        }

        /// <summary>
        /// Resume video recording on the specified connection.
        /// </summary>
        /// 
        /// <remarks>
        /// Transitions the video recording state of the connection to Recording. While the 
        /// video recording state transition is taking place, the video recording state will be
        /// Resuming.
        ///
        /// Calling this function will fail unless the current video recording state is
        /// Paused.
        /// </remarks>
        /// 
        /// <param name="connection">
        /// The connection to resume video recording on.
        /// </param>
        /// 
        /// <exception cref="GCSeries.zView.InvalidParameterException">
        /// Thrown if the connection is null (IntPtr.Zero).
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionException">
        /// Thrown if the connection is invalid.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionStateException">
        /// Thrown if the connection is in the Closed or Error state.
        /// </exception>
        /// <exception cref="GCSeries.zView.UnsupportedCapabilityException">
        /// Thrown if the connection does not support the video recording capability.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidVideoRecordingStateException">
        /// Thrown if the video recording is not in the Paused state.
        /// </exception>
        public void ResumeVideoRecording(IntPtr connection)
        {
            Debug.LogWarning("GView.ResumeVideoRecording():这个函数目前未实现!");
        }

        /// <summary>
        /// Begin saving the specified connection's current video recording to the
        /// specified file name.
        /// </summary>
        /// 
        /// <remarks>
        /// Transitions the video recording state of the connection to Saving. Saving occurs 
        /// asynchronously and the video recording state of the connection will automatically 
        /// transition to NotRecording if saving finishes successfully. If saving fails, then 
        /// the video recording state will automatically transition to Error.
        ///
        /// Calling this function will fail unless the current video recording state is
        /// Finished.
        /// </remarks>
        /// 
        /// <param name="connection">
        /// The connection to save the current video recording of.
        /// </param>
        /// <param name="fileName">
        /// The file name to save the video recording to.
        /// </param>
        /// 
        /// <exception cref="GCSeries.zView.InvalidParameterException">
        /// Thrown if the connection is null (IntPtr.Zero).
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionException">
        /// Thrown if the connection is invalid.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionStateException">
        /// Thrown if the connection is in the Closed or Error state.
        /// </exception>
        /// <exception cref="GCSeries.zView.UnsupportedCapabilityException">
        /// Thrown if the connection does not support the video recording capability.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidVideoRecordingStateException">
        /// Thrown if the video recording is not in the Finished state.
        /// </exception>
        public void SaveVideoRecording(IntPtr connection, string fileName)
        {
            if (!File.Exists(tempRecFile))
            {
                Debug.LogWarning("GView.SaveVideoRecording():不存在录屏的临时文件,无法作移动!临时文件路径:" + tempRecFile);
                return;
            }
            if (File.Exists(fileName))
            {
                Debug.Log("GView.SaveVideoRecording():移动路径上已经存在文件,删除!");
                File.Delete(fileName);
            }
            try
            {
                //注意这里实际上需要等待ffmpeg.exe完成退出才能移动这个文件.
                Debug.Log("GView.SaveVideoRecording():移动临时录屏文件到" + fileName);
                File.Move(tempRecFile, fileName);

                xxSaveVideoRecording(fileName);
            }
            catch (Exception e)
            {
                Debug.LogError("GView.SaveVideoRecording():移动文件异常e=" + e.Message);
            }


        }

        /// <summary>
        /// Begin discarding the specified connection's current video recording.
        /// </summary>
        /// 
        /// <remarks>
        /// Transitions the video recording state of the connection to Discarding. 
        /// Discarding occurs asynchronously and the video recording state of the connection 
        /// will automatically transition to Recording when discarding is finished.
        ///
        /// Calling this function will fail unless the current video recording state is
        /// Finished.
        /// </remarks>
        /// 
        /// <param name="connection">
        /// Transitions the video recording state of the connection to Discarding. 
        /// Discarding occurs asynchronously and the video recording state of the 
        /// connection will automatically transition to NotRecording when discarding is 
        /// finished.
        ///
        /// Calling this function will fail unless the current video recording state is
        /// Finished.
        /// </param>
        /// 
        /// <exception cref="GCSeries.zView.InvalidParameterException">
        /// Thrown if the connection is null (IntPtr.Zero).
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionException">
        /// Thrown if the connection is invalid.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionStateException">
        /// Thrown if the connection is in the Closed or Error state.
        /// </exception>
        /// <exception cref="GCSeries.zView.UnsupportedCapabilityException">
        /// Thrown if the connection does not support the video recording capability.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidVideoRecordingStateException">
        /// Thrown if the video recording is not in the Finished state.
        /// </exception>
        public void DiscardVideoRecording(IntPtr connection)
        {
            Debug.Log("GView.DiscardVideoRecording():中止录屏!");
            try
            {
                FARError err = xxStopScreenRec();
                if (err != FARError.Ok)
                {
                    Debug.LogError("GView.FinishVideoRecording():中止录屏! err=" + err);
                }
                //删除已经存在的录制文件
                if (!File.Exists(tempRecFile))
                {
                    File.Delete(tempRecFile);
                }

                xxDiscardVideoRecording();
            }
            catch (Exception e)
            {
                Debug.Log("GView.DiscardVideoRecording():异常e=" + e.Message);
            }

        }

        /// <summary>
        /// Get the amount of time that has elapsed, in milliseconds, since the
        /// specified connection's current video recording began.
        /// </summary>
        /// 
        /// <param name="connection">
        /// The connection to get the current video recording time of.
        /// </param>
        /// 
        /// <returns>
        /// The current video recording time of the specified connection, 
        /// in milliseconds.
        /// </returns>
        /// 
        /// <exception cref="GCSeries.zView.InvalidParameterException">
        /// Thrown if the connection is null (IntPtr.Zero).
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionException">
        /// Thrown if the connection is invalid.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidConnectionStateException">
        /// Thrown if the connection is in the Closed or Error state.
        /// </exception>
        /// <exception cref="GCSeries.zView.UnsupportedCapabilityException">
        /// Thrown if the connection does not support the video recording capability.
        /// </exception>
        /// <exception cref="GCSeries.zView.InvalidVideoRecordingStateException">
        /// Thrown if the video recording is in the NotAvailable, NotRecording, Starting, 
        /// Error, or ClearingError state.
        /// </exception>
        public UInt64 GetVideoRecordingTime(IntPtr connection)
        {
            Debug.LogWarning("GView.GetVideoRecordingTime():这个函数目前未实现!");
            UInt64 timeInMilliseconds = 0;
            return timeInMilliseconds;
        }

        /// <summary>
        /// Set the specified mode's current active virtual camera.
        /// </summary>
        /// 
        /// <remarks>
        /// If null is specified for the virtual camera, the mode's virtual camera
        /// will be reset to its default implementation.
        /// </remarks>
        /// 
        /// <param name="mode">
        /// The mode to specify the virtual camera.
        /// </param>
        /// <param name="virtualCamera">
        /// The virtual camera associated with the specified mode.
        /// </param>
        public void SetVirtualCamera(IntPtr mode, GVirtualCamera virtualCamera)
        {
            Debug.LogWarning("GView.SetVirtualCamera():这个函数目前未实现!");
        }

        /// <summary>
        /// Get the current active virtual camera associated with the specified mode.
        /// </summary>
        /// 
        /// <param name="mode">
        /// The mode to get the virtual camera for.
        /// </param>
        /// 
        /// <returns>
        /// The current active virtual camera associated with the specified mode.
        /// </returns>
        public GVirtualCamera GetVirtualCamera(IntPtr mode)
        {
            GVirtualCamera virtualCamera = null;
            if (!_virtualCameras.TryGetValue(mode, out virtualCamera))
            {
                Debug.LogError(string.Format("Failed to find virtual camera for mode: {0}.", mode));
            }

            return virtualCamera;
        }

        /// <summary>
        /// Start ignoring (i.e. not processing) any connections with the
        /// specified user data.
        /// </summary>
        /// <param name="userData">
        /// The user data for which to ignore connections for.
        /// </param>
        public void RegisterConnectionUserDataToIgnore(IntPtr userData)
        {
            Debug.Log("GView.RegisterConnectionUserDataToIgnore():进入!");
            _connectionUserDatasToIgnore.Add(userData);
        }

        /// <summary>
        /// Stop ignoring (i.e. not processing) any connections with the
        /// specified user data.
        /// </summary>
        /// <param name="userData">
        /// The user data for which to no longer ignore connections for.
        /// </param>
        public void UnregisterConnectionUserDataToIgnore(IntPtr userData)
        {
            Debug.Log("GView.UnregisterConnectionUserDataToIgnore():进入!");
            _connectionUserDatasToIgnore.Remove(userData);
        }

        //////////////////////////////////////////////////////////////////
        // Private Compound Types
        //////////////////////////////////////////////////////////////////

        private class ConnectionInfo
        {
            public IntPtr Connection { get; private set; }
            public ConnectionState ConnectionState { get; set; }
            public IntPtr Mode { get; set; }
            public IntPtr ReceivedFrame { get; set; }
            public IntPtr FrameToSend { get; set; }
            public VideoRecordingState VideoRecordingState { get; set; }
            public VideoRecordingQuality VideoRecordingQuality { get; set; }

            public ConnectionInfo(IntPtr connection)
            {
                this.Connection = connection;
                this.ConnectionState = ConnectionState.Error;
                this.Mode = IntPtr.Zero;
                this.ReceivedFrame = IntPtr.Zero;
                this.FrameToSend = IntPtr.Zero;
                this.VideoRecordingState = VideoRecordingState.NotAvailable;
                this.VideoRecordingQuality = VideoRecordingQuality.Unknown;
            }
        }


        //////////////////////////////////////////////////////////////////
        // Private Members
        //////////////////////////////////////////////////////////////////

        // private static readonly Matrix4x4 s_flipHandednessMap = Matrix4x4.Scale(new Vector4(1.0f, 1.0f, -1.0f));

        // private Dictionary<IntPtr, ConnectionInfo> _connectionInfos = new Dictionary<IntPtr, ConnectionInfo>();
        private Dictionary<IntPtr, GVirtualCamera> _defaultVirtualCameras = new Dictionary<IntPtr, GVirtualCamera>();
        private Dictionary<IntPtr, GVirtualCamera> _virtualCameras = new Dictionary<IntPtr, GVirtualCamera>();

        private HashSet<IntPtr> _connectionUserDatasToIgnore = new HashSet<IntPtr>();

        // private bool _wasFullScreen = false;


        //////////////////////////////////////////////////////////////////
        // Unity Monobehaviour Callbacks
        //////////////////////////////////////////////////////////////////

        //void Awake()
        //{
        //    // Force initialization of the global state.
        //    GlobalState globalState = GlobalState.Instance;
        //    if (globalState == null || !globalState.IsInitialized)
        //    {
        //        Debug.LogWarning("Failed to initialize global state. Disabling zView GameObject.");
        //        this.gameObject.SetActive(false);
        //        return;
        //    }

        //    // Continue initialization.
        //    this.InitializeVirtualCameras();

        //    // Initialize whether the application is in windowed or fullscreen mode.
        //    _wasFullScreen = Screen.fullScreen;

        //    // Kick off the end of frame update coroutine.
        //    this.StartCoroutine(EndOfFrameUpdate());
        //}

        //void Start()
        //{
        //    // Handle transitioning from a zView-enabled scene while there
        //    // is currently an active connection.
        //    this.HandleSceneTransition();
        //}

        // void LateUpdate()
        // {
        //     // Cache whether the application is in windowed or fullscreen mode.
        //     _wasFullScreen = Screen.fullScreen;
        // }

        void OnDrawGizmos()
        {
            // Draw the bounds of the AR mode box mask.
            if (this.ARModeShowMask)
            {
                if (this.ActiveZCamera == null)
                    return;

                // Cache original color and matrix to be restored after
                // drawing is finished.
                Color originalGizmosColor = Gizmos.color;
                Matrix4x4 originalGizmosMatrix = Gizmos.matrix;

                Gizmos.color = Color.white;

                // ZCamera's parent represents viewport center and
                // therefore where we align the box mask to it.
                // If ZCamera has no parent, then it will align the 
                // box mask to world center as is done with ZCamera.
                Gizmos.matrix =
                    this.ActiveZCamera.transform.parent?.localToWorldMatrix ??
                    Matrix4x4.identity;

                Vector3 center = new Vector3(0.0f, 0.0f, -this.ARModeMaskSize.z * 0.5f);
                Vector3 size = this.ARModeMaskSize;

                Gizmos.DrawWireCube(center, size);

                // Restore original color and matrix.
                Gizmos.color = originalGizmosColor;
                Gizmos.matrix = originalGizmosMatrix;
            }
        }
        #region -----------------------------------------------  GC一体机的实现  -----------------------------------------------
        /// <summary>
        /// 投屏纹理的宽度
        /// </summary>
        public UInt16 imageWidth = 1920;

        /// <summary>
        /// 投屏纹理高度
        /// </summary>
        public UInt16 imageHeight = 1080;

        /// <summary>
        /// CurVirtualCameraMode属性使用的字段
        /// </summary>
        private IntPtr _curVirtualCameraMode = IntPtr.Zero;

        /// <summary>
        /// 当前的工作模式
        /// </summary>
        private IntPtr CurVirtualCameraMode
        {
            get { return _curVirtualCameraMode; }
            set
            {
                if (value != _curVirtualCameraMode || value == IntPtr.Zero)
                {
                    try
                    {
                        Debug.Log("GView.CurVirtualCameraMode():切换相机模式! mode=" + value);
                        GVirtualCamera gVirtualCamera = null;
                        if (_defaultVirtualCameras.TryGetValue(_curVirtualCameraMode, out gVirtualCamera))
                        {
                            gVirtualCamera.TearDown();//关闭老的相机
                        }

                        if (_defaultVirtualCameras.TryGetValue(value, out gVirtualCamera))
                        {
                            gVirtualCamera.SetUp(this, GlobalState.Instance.Connection, ModeSetupPhase.Initialization);

                            _workingCamera = gVirtualCamera;//记录当前相机
                            FARStartRenderingView(_workingCamera);
                        }
                        else
                        {
                            //None模式这个就找不到
                            //Debug.LogError($"GView.CurVirtualCameraMode():没找到要打开的新相机,将无法投屏! mode={value}");
                            _workingCamera = null;//没找到要置空
                            FARDll.CloseDown();
                        }


                    }
                    catch (Exception e)
                    {
                        Debug.LogError("GView.CurVirtualCameraMode():异常!Message=" + e.Message);
                        FARDll.CloseDown();
                    }
                    _curVirtualCameraMode = value;
                    GlobalState.Instance.virtualCameraMode = value;//记录到全局状态中

                    //事件要在virtualCameraMode值被修改了之后发出
                    if (this.ConnectionModeSwitched != null)
                    {
                        this.ConnectionModeSwitched(this, GlobalState.Instance.Connection);
                    }
                }
            }
        }

        //当前工作相机
        private GVirtualCamera _workingCamera = null;

        void Awake()
        {
            // Force initialization of the global state.
            GlobalState globalState = GlobalState.Instance;
            if (globalState == null || !globalState.IsInitialized)
            {
                Debug.LogWarning("Failed to initialize global state. Disabling zView GameObject.");
                this.gameObject.SetActive(false);
                return;
            }

            // Get the first found instance of ZCamera if a reference has not
            // been manually assigned.
            if (this.ActiveZCamera == null)
            {
                this.ActiveZCamera = GameObject.FindObjectOfType<ZCamera>();

                if (this.ActiveZCamera == null)
                {
                    Debug.LogWarning("No instance of ZCamera has been found " +
                        "in the scene. ZView will not work correctly unless " +
                        "ZView.Instance.ActiveZCamera is not null");
                }
            }


            InitializeVirtualCameras();

            //从streamingAssets里面寻找辅助exe
            string ffmpegPath = Path.Combine(Application.streamingAssetsPath, "ffmpeg.exe");

            FARError err = xxSetEXE(ffmpegPath);
            if (err != FARError.Ok)
            {
                //去调试路径里拿一个
                err = xxSetEXE("C:\\Test\\ffmpeg.exe");
            }

            if (err != FARError.Ok)
            {
                Debug.LogError("GView.Awake():设置录屏辅助程序路径失败!");
            }
        }

        void Start()
        {
            HandleSceneTransition();
        }

        void Update()
        {
            if (_workingCamera != null)
            {
                _workingCamera.Render(this, IntPtr.Zero, IntPtr.Zero);
            }

            VideoRecordingState videoRecordingState = GetVideoRecordingState(GlobalState.Instance.Connection);
            if (videoRecordingState != GlobalState.Instance.videoRecordingState)
            {
                switch (videoRecordingState)
                {
                    case VideoRecordingState.NotRecording:
                        if (this.VideoRecordingInactive != null)
                        {
                            this.VideoRecordingInactive(this, GlobalState.Instance.Connection);
                        }
                        break;

                    case VideoRecordingState.Recording:
                        if (this.VideoRecordingActive != null)
                        {
                            this.VideoRecordingActive(this, GlobalState.Instance.Connection);
                        }
                        break;

                    case VideoRecordingState.Finished:
                        if (this.VideoRecordingFinished != null)
                        {
                            this.VideoRecordingFinished(this, GlobalState.Instance.Connection);
                        }
                        break;

                    case VideoRecordingState.Paused:
                        if (this.VideoRecordingPaused != null)
                        {
                            this.VideoRecordingPaused(this, GlobalState.Instance.Connection);
                        }
                        break;

                    case VideoRecordingState.Error:
                        if (this.VideoRecordingError != null)
                        {
                            this.VideoRecordingError(this, GlobalState.Instance.Connection);
                        }
                        break;

                    default:
                        break;
                }
                GlobalState.Instance.videoRecordingState = videoRecordingState;
            }

        }

        void OnDestroy()
        {
            _virtualCameras.Clear();
            _defaultVirtualCameras.Clear();
            _workingCamera = null;
        }

        void OnApplicationQuit()
        {
            this.CloseConnection(
                        this.GetCurrentActiveConnection(),
                        GView.ConnectionCloseAction.None,
                        GView.ConnectionCloseReason.UserRequested,
                        string.Empty);

            GlobalState.DestroyInstance();
            FARDll.CloseDown();
        }

        IntPtr _hViewClient = IntPtr.Zero;

        FARDll.FAR_Status status = FARDll.FAR_Status.FAR_NotInitialized;


        ///<summary> 
        ///通过f-ar接口读取屏幕信息后设置窗口位置 
        ///</summary> 
        ///<param name="farwin">投屏窗口句柄</param> 
        ///<param name="preview">是否为3D预览窗口</param> 
        public void UpdateWindowPos(IntPtr farwin)
        {
            F3Device.Screen.Monitor monitor = F3Device.DeviceManager.Instance.FindProjectionMonitor(F3DService.Instance.mainWindowHandle);
            if (monitor != null)
            {
                F3Device.Screen.RECT rect = monitor.m_MonitorInfo.rcMonitor;
                FARDll.MoveWindow(farwin, rect.Left, rect.Top, rect.Width, rect.Height, true);
            }
            else
            {
                UnityEngine.Debug.Log("Direct3DWin.UpdateWindows 没有找到投屏目标显示器");
            }
        }

        private IEnumerator CreateFARWindow(GVirtualCamera virtualCamera)
        {
            yield return new WaitForEndOfFrame();
            _hViewClient = FARDll.FindWindow(null, "ClientWinCpp");
            status = FARDll.FAR_Status.FAR_NotInitialized;
            if (_hViewClient == IntPtr.Zero)
            {
                string _path = Path.Combine(Application.streamingAssetsPath, "ClientWin.exe");
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.FileName = _path;
                FARDll.viewProcess = new System.Diagnostics.Process();
                FARDll.viewProcess.StartInfo = startInfo;
                FARDll.viewProcess.Start();
                FARDll.viewProcess.WaitForInputIdle();
                _hViewClient = FARDll.FindWindow(null, "ClientWinCpp");
            }
            if (FARDll.viewProcess != null)
            {
                if (_hViewClient != IntPtr.Zero)
                {
                    //全屏到非GC显示器 
                    UpdateWindowPos(_hViewClient);
                    int RTcount = 0;
                    RenderTexture[] tempRTs = virtualCamera.GetRenderTexture(out RTcount);

                    if (RTcount == 1)
                    {
                        status = (FARDll.FAR_Status)FARDll.StartView(_hViewClient, tempRTs[0].GetNativeTexturePtr(), IntPtr.Zero);
                    }
                    else if (RTcount == 2)
                    {
                        FARDll.SwitchProjector(FARDll.ProjectorType.LeftRight);
                        status = (FARDll.FAR_Status)FARDll.StartView(_hViewClient, tempRTs[0].GetNativeTexturePtr(), tempRTs[1].GetNativeTexturePtr());
                    }
                }
            }
            else
                status = FARDll.FAR_Status.FAR_GraphicsCardUnsupported;

            if (status <= 0)
            {
                UnityEngine.Debug.LogError("Direct3DWin.CreateFARWindow 投屏启动失败" + status);
                FARDll.CloseDown();
            }

            if (_GLIssueHandle != null)
                this.StopCoroutine(_GLIssueHandle);
            _GLIssueHandle = this.StartCoroutine(CallPluginAtEndOfFrames());
        }

        private Coroutine _GLIssueHandle;
        private IEnumerator CallPluginAtEndOfFrames()
        {
            while (true)
            {
                yield return new WaitForEndOfFrame();
                GL.IssuePluginEvent(FARDll.GetRenderEventFunc(), 1);
            }
        }

        /// <summary> 
        /// 启动FAR投屏窗口 
        /// </summary> 
        /// <param name="workmMode">投屏单张纹理或两张纹理</param> 
        private void FARStartRenderingView(GVirtualCamera virtualCamera)
        {
            StartCoroutine(CreateFARWindow(virtualCamera));
        }

        /// <summary>
        /// 初始化两个相机
        /// </summary>
        private void InitializeVirtualCameras()
        {
            //CameraMode.Fixed, CameraMode目前都是固定的,不是跟踪头部的 
            _virtualCameras.Clear();
            _defaultVirtualCameras.Clear();
            _workingCamera = null;

            // Create the default virtual camera for standard mode.
            GameObject virtualCameraStandardObject = new GameObject("GVirtualCameraStandard");
            virtualCameraStandardObject.transform.parent = this.transform;
            _cameraStandard = virtualCameraStandardObject.AddComponent<GVirtualCameraStandard>();
            _defaultVirtualCameras[GlobalState.Instance.ModeStandard] = _cameraStandard;
            _virtualCameras[GlobalState.Instance.ModeStandard] = _cameraStandard;

            GameObject virtualCameraThreeDObject = new GameObject("GVirtualCameraThreeD");
            virtualCameraThreeDObject.transform.parent = this.transform;
            _cameraThreeD = virtualCameraThreeDObject.AddComponent<GVirtualCameraThreeD>();
            _defaultVirtualCameras[GlobalState.Instance.ModeThreeD] = _cameraThreeD;
            _virtualCameras[GlobalState.Instance.ModeThreeD] = _cameraThreeD;

            // Create the default virtual camera for AR mode.
            GameObject virtualCameraARObject = new GameObject("GVirtualCameraAR");
            virtualCameraARObject.transform.parent = this.transform;
            _cameraAR = virtualCameraARObject.AddComponent<GVirtualCameraAR>();
            _defaultVirtualCameras[GlobalState.Instance.ModeAugmentedReality] = _cameraAR;
            _virtualCameras[GlobalState.Instance.ModeAugmentedReality] = _cameraAR;

        }

        /// <summary>
        /// Set up the current mode's virtual camera if there is
        /// a connection in the ModeActive state. This logic is necessary
        /// for the purpose of handling scene transitions between two zView-enabled
        /// scenes while a connection is currently active.
        /// </summary>
        private void HandleSceneTransition()
        {
            IntPtr connection = GlobalState.Instance.Connection;
            if (connection == IntPtr.Zero)
            {
                return;
            }
            CurVirtualCameraMode = GlobalState.Instance.virtualCameraMode;
        }

        private GVirtualCameraStandard _cameraStandard = null;
        private GVirtualCameraThreeD _cameraThreeD = null;
        private GVirtualCameraAR _cameraAR = null;

        /// <summary>
        /// 默认的录屏临时文件存放目录放桌面算了
        /// </summary>
        private string tempRecFile
        {
            get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "FARRecording.mp4"); }
        }

        public enum FARError
        {
            Unknown = -1,
            Ok = 0,
            NoCalib = 1,     // 没标定
            NoLicense = 2,   // 没证书
            NoFFmpeg = 3,    //没有ffmpeg程序
            NoSendsignal = 4 //没有Sendsignal程序
        }

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        public static extern FARError xxGetARCameraPose(out Matrix4x4 matrix);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern FARError xxConnectToDefaultViewer(out IntPtr connect);

        /// <summary>
        /// 设置辅助exe的路径,其中ffmpeg用来开启录屏进程,sendsignal用来关闭录屏进程.
        /// 注,目前sendsignal.exe已经不需要了.
        /// </summary>
        /// <param name="ffmpeg">ffmpeg.exe的路径</param>
        /// <param name="sendsignal">sendsignal.exe的路径</param>
        /// <returns></returns>
        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern FARError xxSetEXE(string ffmpeg, string sendsignal = "sendsignal.exe");

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern bool xxGetVideoRecordingCapability();

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern FARError xxGetVideoRecordingState(out VideoRecordingState state);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern FARError xxStartScreenRec(string filePath);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern FARError xxStopScreenRec();

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern FARError xxSaveVideoRecording(string filePath);

        [DllImport("zCoreUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern FARError xxDiscardVideoRecording();

        #endregion
    }
}

