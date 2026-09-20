public class FeatureCollection
{
    public EarthquakeFeature[] Features { get; set; } = [];
}

public class EarthquakeFeature
{
    public EarthquakeProperties Properties { get; set; } = new();
}

public class EarthquakeProperties
{
    public string Place { get; set; }
    public double? Mag { get; set; }
}
