module WmsData
  WMS_DATA = {
    :format => {
      "image/png" => "png",
      "image/jpeg" => "jpeg"},
    :crs => {
      "CRS:84" => :wgs84_latlon,
      "EPSG:4326" => :wgs84_latlon,
      "EPSG:54004" => ["proj=merc","lat_ts=0", "lon_0=0","k=1.000000", "x_0=0", "y_0=0", "ellps=WGS84", "datum=WGS84", "units=m"], #simple mercator
      "EPSG:41001" => ["proj=merc","lat_ts=0", "lon_0=0","k=1.000000", "x_0=0", "y_0=0", "ellps=WGS84", "datum=WGS84", "units=m"] #simple mercator
    }
  }
end
