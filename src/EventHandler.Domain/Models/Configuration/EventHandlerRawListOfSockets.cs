using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace EventHandler.Domain.Models.Configuration
{
    [XmlRoot(ElementName = "ezUniverse")]
    public class EventHandlerConfiguration
    {
        public static EventHandlerConfiguration Create(
            EventHandlerRawListOfSockets sockets,
            ControlCenterServiceSettings ccsSettings)
        {
            var config = new EventHandlerConfiguration
            {
                EzEventHandler = new EventHandlerEzEventHandler
                {
                    ControlCenterService = new ControlCenterServiceSettings
                    {
                        ControlCenterServiceURL = ccsSettings.ControlCenterServiceURL,
                        ControlCenterServiceAddress = ccsSettings.ControlCenterServiceAddress,
                        HubName = ccsSettings.HubName,
                        HubReconnectionDelay = ccsSettings.HubReconnectionDelay,
                        HubReconnectionTries = ccsSettings.HubReconnectionTries,
                        UseWebSocket = ccsSettings.UseWebSocket
                    },

                    EventHandlerRawListOfSockets = new EventHandlerRawListOfSockets
                    {
                        Name = sockets.Name,
                        Sockets = sockets.Sockets
                    }
                }
            };

            return config;
        }

        [XmlAttribute(AttributeName = "id")]
        public string? Id { get; set; }

        [XmlAttribute]
        public string? Version { get; set; }

        [XmlAttribute]
        public string? Timestamp { get; set; }

        [XmlElement(ElementName = "ezEventHandler")]
        public EventHandlerEzEventHandler? EzEventHandler { get; set; }
    }

    public class EventHandlerEzEventHandler
    {
        [XmlElement(ElementName = "ListOfSocketsToWatch")]
        public EventHandlerRawListOfSockets? EventHandlerRawListOfSockets { get; set; }

        [XmlElement(ElementName = "ControlCenterService")]
        public ControlCenterServiceSettings? ControlCenterService { get; set; }
    }

    public class ControlCenterServiceSettings
    {
        public string ServiceName { get; set; } = "EZEventHandler";

        [XmlAttribute]
        public string? ControlCenterServiceURL { get; set; }

        [XmlAttribute]
        public string? ControlCenterServiceAddress { get; set; }

        [XmlAttribute]
        public string? HubName { get; set; }

        [XmlAttribute]
        public int HubReconnectionTries { get; set; }

        [XmlAttribute]
        public int HubReconnectionDelay { get; set; }

        [XmlIgnore]
        public bool UseWebSocket { get; set; }

        [XmlAttribute("UseWebSocket")]
        public string UseWebSocketSerialize
        {
            get => UseWebSocket ? "True" : "False";
            set => UseWebSocket = Convert.ToBoolean(value);
        }
    }

    public class EventHandlerRawListOfSockets
    {
        [XmlElement(ElementName = "Socket")]
        public List<EventHandlerRawSocket> Sockets = new List<EventHandlerRawSocket>();

        [XmlAttribute]
        public string? Name { get; set; }
    }
}
