
namespace MonitoringConfig.Infrastructure.Data.Model {

    public enum AlertColor {
        RED,
        GREEN,
        BLUE,
        YELLOW,
        ORANGE,
        CUSTOM
    }

    public class Location {
        public double XCoord { get; set; }
        public double YCoord { get; set; }
    }

    public class ZoneSize {
        public double Width { get; set; }
        public double Height { get; set; }
    }

    public class FacilityZone {
        public int Id { get; set; }
        public string ZoneName { get; set; }
        public Location Location { get; set; }
        public ZoneSize ZoneSize { get; set; }
        public AlertColor Color { get; set; }
        public int ZoneParentId { get; set; }
        public FacilityZone ZoneParent { get; set; }
        public ICollection<FacilityZone> SubZones { get; set; } = new List<FacilityZone>();
        public ICollection<Channel> Channels { get; set; } = new List<Channel>();
        public ICollection<Device> Devices { get; set; } = new List<Device>();

    }
}
