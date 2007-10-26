require 'proj4'
include Proj4

module GeoportailStruct
  
  TILE_SIZE = 384
  WGS84_TILE_SIZE = 500
  
  class Extent
    attr_accessor :top_left,:bottom_right
    
    def initialize(*params)
      if params.length == 2
        @top_left = params[0]
        @bottom_right = params[1]
      else
        @top_left = UV.new(params[0],params[1])
        @bottom_right = UV.new(params[2],params[3])
      end
    end
    
    def intersects?(extent)
      @top_left.u < extent.bottom_right.u and 
        @bottom_right.u > extent.top_left.u and
        @top_left.v > extent.bottom_right.v and
        @bottom_right.v < extent.top_left.v 
    end
    
    def contains?(point)
      point.u > @top_left.u and 
        point.u < @bottom_right.u and 
        point.v > @bottom_right.v and
        point.v < @top_left.v
    end
  end
    
  class GeoportailTerritory < Struct.new(:full_name,:projection,:scale_min,:scale_max,:layers)
    attr_accessor :projection_string
    
    def initialize(*params)
      super(*params)
      
      @projection_string = "+" + projection.join(" +")
      self.projection = Projection.new(self.projection)  #transform the string array into a projection object 
      
      layers.each_value do |layer|
        layer.fill_wgs84(projection)
      end
    end

    def [](layer)
      layers[layer]
    end
  end
 
  class GeoportailLayer < Struct.new(:full_name,:extent,:cell,:width,:height,:copyright, :components, :default)
    attr_accessor :levels, :wgs84_extent, :wgs84_levels
    attr_reader :top_level
    
    def initialize(*params)
      super(*params)
            
      @levels = []
      
      size = width < height ? width : height
            
      while size > TILE_SIZE
        @levels << GeoportailLayerLevel.new(@levels.length,cell,TILE_SIZE,extent)
        size = size / 2
      end
      
      @top_level = @levels.length - 1
    end

    def fill_wgs84(projection)
      #get the axis-aligned extent in wgs84
      uv_start = projection.inverse(extent.top_left)
      corners = []
      corners << projection.inverse(UV.new(extent.bottom_right.u,extent.top_left.v))
      corners << projection.inverse(UV.new(extent.top_left.u,extent.bottom_right.v))
      corners << projection.inverse(extent.bottom_right)

            
      uv_wn = uv_start.clone
      uv_es = uv_start.clone
      corners.each do |uv|
        uv_wn.u = uv.u if uv.u < uv_wn.u
        uv_wn.v = uv.v if uv.v > uv_wn.v
        uv_es.u = uv.u if uv.u > uv_es.u
        uv_es.v = uv.v if uv.v < uv_es.v
      end
      
      #define a square extent so it is easier to manage later on
      wgs84_width = uv_es.u - uv_wn.u
      wgs84_height = uv_wn.v - uv_es.v
      
      diffd2 = (wgs84_width - wgs84_height)/2
      if diffd2 > 0
        uv_wn.v += diffd2
        uv_es.v -= diffd2
        top_size = wgs84_width
      else
        uv_wn.u += diffd2
        uv_es.u -= diffd2
        top_size = wgs84_height
      end
      @wgs84_extent = Extent.new(uv_wn,uv_es)
      
      @wgs84_levels = []
      @levels.each do |dt_level|
        @wgs84_levels << GeoportailLayerWgs84Level.new(dt_level.level,@top_level,top_size,WGS84_TILE_SIZE)
      end
    end
      
    def find_components_for_scale_extent(scale,extent)
      results = []
      components.each do |component|
        if component.valid_for_scale?(scale) && component.intersects?(extent)
          results << component.url
        end
      end
      results
    end
          
    #not sure about this one...
    def level_by_arc_extent(arc_extent)
      arc_distance = 6378137.0 * arc_extent * Math::PI / 180.0 #in rads
      levels.each do |dt_level|
        return dt_level.level if arc_distance < dt_level.world_per_tile.u
      end
      return top_level
    end
  end
  
  class GeoportailLayerLevel
    attr_accessor :level,:world_per_tile, :world_per_pixel, :tile_size, :max_tile_x, :max_tile_y
    
    def initialize(level,cell,tile_size,extent)
      @level = level
      @tile_size = tile_size
      wppu = cell.u * (1 << level)
      wppv = cell.v * (1 << level)
      @world_per_pixel = UV.new(wppu,wppv)
      wptu = tile_size * wppu
      wptv = tile_size * wppv
      @world_per_tile = UV.new(wptu, wptv)
      @max_tile_x = ((extent.bottom_right.u - extent.top_left.u) / wptu).floor
      @max_tile_y = ((extent.top_left.v - extent.bottom_right.v) / wptv).floor
    end
  end

  class GeoportailLayerWgs84Level
    attr_accessor :level,:world_per_tile, :world_per_pixel, :tile_size

    def initialize(level,top_level, top_size,tile_size)
      @level = level
      @tile_size = tile_size
      #square so same unit in both directions
      wpt = top_size / (1 << (top_level - level))
      @world_per_tile = UV.new(wpt,wpt)
      wpp = wpt / tile_size
      @world_per_pixel = UV.new(wpp,wpp)
    end

  end
  
  class GeoportailLayerComponent < Struct.new(:url,:extent,:scale_min,:scale_max)
    def intersects?(extent)
      self.extent.intersects?(extent)
    end

    def valid_for_scale?(scale)
      scale >= scale_min and scale <= scale_max
    end
  end

  class GeoportailTileLink < Struct.new(:x,:y,:level,:extent,:min_lod,:max_lod,:url_root)
    def url
      url_root + "/#{x}/#{y}"
    end
  end
end
