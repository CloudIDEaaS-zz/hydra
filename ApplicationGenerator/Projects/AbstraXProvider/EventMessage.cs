using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.IO;

namespace AbstraX
{
    public enum Message
    {
        /// <summary>
        /// Params: (string uiComponentURL, string parms)
        /// </summary>
        ShowUI,
        /// <summary>
        /// Params: (string message)
        /// </summary>
        GeneralMessage,
        /// <summary>
        /// Params: (float percent)
        /// </summary>
        PercentComplete,
        /// <summary>
        /// Params: (string generatorName)
        /// </summary>
        RequestingBuild,
        /// <summary>
        /// Params: (string generatorName)
        /// </summary>
        BuildRequestCompleted,
        /// <summary>
        /// Params: (string fileName)
        /// </summary>
        FileGenerationStarted,
        /// <summary>
        /// Params: (string fileName)
        /// </summary>
        FileGenerationCompleted,
        /// <summary>
        /// Params: (Exception exception)
        /// </summary>
        /// <summary>
        /// Params: none
        /// </summary>
        GenerationStarted,
        /// <summary>
        /// Params: none
        /// </summary>
        GenerationCompleted,
        /// <summary>
        /// Params: (Exception exception)
        /// </summary>
        GeneralError,
        /// <summary>
        /// Params: (Exception exception)
        /// Client Params: (RemotableException exception)
        /// </summary>
        FatalError,
        /// <summary>
        /// Params: (string solutionName)
        /// </summary>
        SolutionOpened,
        /// <summary>
        /// Don not send as message
        /// </summary>
        /// 
        BuildMessageStart = 0x1000000,
        /// <summary>
        /// Params: (BuildFinishedEventArgs buildFinishedEventArgs)
        /// </summary>
        /// 
        BuildFinished,
        /// <summary>
        /// Params: (BuildStartedEventArgs buildStartedEventArgs)
        /// </summary>
        BuildStarted,
        /// <summary>
        /// Params: (CustomBuildEventArgs customBuildEventArgs)
        /// </summary>
        CustomEventRaised,
        /// <summary>
        /// Params: (BuildErrorEventArgs buildErrorEventArgs)
        /// </summary>
        ErrorRaised,
        /// <summary>
        /// Params: (BuildMessageEventArgs buildMessageEventArgs)
        /// </summary>
        MessageRaised,
        /// <summary>
        /// Params: (ProjectFinishedEventArgs projectFinishedEventArgs)
        /// </summary>
        ProjectFinished,
        /// <summary>
        /// Params: (ProjectStartedEventArgs projectStartedEventArgs)
        /// </summary>
        ProjectStarted,
        /// <summary>
        /// Params: (BuildStatusEventArgs buildStatusEventArgs)
        /// </summary>
        StatusEventRaised,
        /// <summary>
        /// Params: (TargetFinishedEventArgs targetFinishedEventArgs)
        /// </summary>
        TargetFinished,
        /// <summary>
        /// Params: (TargetStartedEventArgs targetStartedEventArgs)
        /// </summary>
        TargetStarted,
        /// <summary>
        /// Params: (TaskFinishedEventArgs taskFinishedEventArgs)
        /// </summary>
        TaskFinished,
        /// <summary>
        /// Params: (TaskStartedEventArgs taskStartedEventArgs)
        /// </summary>
        TaskStarted,
        /// <summary>
        /// Params: (BuildWarningEventArgs buildWarningEventArgs)
        /// </summary>
        WarningRaised,
    }

    [DataContract]
    public class EventMessage
    {
        [DataMember]
        public Message Message { get; set; }
        [DataMember]
        public byte[] EventArgs { get; set; }

        public EventMessage()
        {
        }

        public EventMessage(Message message, params object[] parms)
        {
            this.WriteTo(message, parms);
        }
    }

    public static class EventExtensions
    {
        public static bool IsBuildMessage(this Message message)
        {
            return (((int)message) & ((int)Message.BuildMessageStart)) == ((int)Message.BuildMessageStart);
        }
    }
}
