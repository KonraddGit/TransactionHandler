using EventHandler.Domain.Models.Configuration.Enums;
using System;
using System.Xml.Serialization;

namespace EventHandler.Domain.Models.Configuration
{
    [XmlRoot(ElementName = "Socket")]
    public class EventHandlerRawSocket
    {
        [XmlAttribute]
        public string? Name { get; set; }

        [XmlAttribute]
        public string? CustomName { get; set; }

        [XmlAttribute(AttributeName = "IPPort")]
        public int Port { get; set; }

        [XmlIgnore]
        public ProtocolType ProtocolType { get; set; }

        [XmlAttribute("ProtocolType")]
        public string ProtocolTypeXml
        {
            get => ProtocolType.ToString().ToUpperInvariant();
            set => ProtocolType = Enum.Parse<ProtocolType>(value, true);
        }

        [XmlAttribute(AttributeName = "AddressIP")]
        public string? IpAddress { get; set; }

        [XmlAttribute(AttributeName = "ConnectionString")]
        public string? DbConnectionString { get; set; }

        [XmlAttribute(AttributeName = "EOFCharacter")]
        public string? EOFCharacter { get; set; }

        public string[] EOFCharacters { get; set; } = Array.Empty<string>();

        [XmlAttribute(AttributeName = "Length")]
        public int BufferLength { get; set; }

        [XmlAttribute]
        public string? StoredProcedureName { get; set; }

        [XmlAttribute(AttributeName = "ClientRecognitionType")]
        public ClientRecognitionType ClientRecognitionType { get; set; }

        [XmlIgnore]
        public bool EnablePortOnFirewall { get; set; }

        [XmlAttribute("EnablePortOnFirewall")]
        public string EnablePortOnFirewallSerialize
        {
            get => EnablePortOnFirewall ? "True" : "False";
            set => EnablePortOnFirewall = Convert.ToBoolean(value);
        }

        [XmlIgnore]
        public bool SocketConfigurationEnabled { get; set; }

        [XmlAttribute("enabled")]
        public string SocketConfigurationEnabledSerialize
        {
            get => SocketConfigurationEnabled ? "True" : "False";
            set => SocketConfigurationEnabled = Convert.ToBoolean(value);
        }

        [XmlIgnore]
        public bool ClientMode { get; set; }

        [XmlAttribute("ClientMode")]
        public string ClientModeSerialize
        {
            get => ClientMode ? "True" : "False";
            set => ClientMode = Convert.ToBoolean(value);
        }

        [XmlIgnore]
        public bool CutClientAfterFirstFrame { get; set; }

        [XmlAttribute("CutClientAfterFirstFrame")]
        public string CutClientAfterFirstFrameSerialize
        {
            get => CutClientAfterFirstFrame ? "True" : "False";
            set => CutClientAfterFirstFrame = Convert.ToBoolean(value);
        }

        [XmlIgnore]
        public bool HandleImmediately
        {
            get => !string.IsNullOrWhiteSpace(HandleImmediatelySerialize)
               ? Convert.ToBoolean(HandleImmediatelySerialize)
               : Convert.ToBoolean(HandleImmidiatelySerialize);
        }

        [XmlAttribute("HandleImmidiately")]
        public string? HandleImmidiatelySerialize { get; set; }

        [XmlAttribute("HandleImmediately")]
        public string? HandleImmediatelySerialize { get; set; }
    }
}
