#include "gdal_priv.h"
#include "gdalwarper.h"
#include "ogr_spatialref.h"
#include "ruby.h"

typedef VALUE (ruby_method)(...);

static VALUE mCreateImage;

//Arguments : 
//The path to the resulting image,
//the number of tiles in i, 
//the number of tiles in j,
//an array with the fs location of the gp tiles taking part in the mosaic (ordered; should be i*j tiles),
//an array with the affine parameters to go from pixel (of the mosaic to be created) to world (in a non wgs84 lat/lon proj) 
//the Proj4 string of the projection of the local coordinate
//an array with the the affine parameters to go from pixel to WGS84 lat/lon coordinate in the result image
//the size of the gp tiles,
//the size of the WGS84 tiles
static VALUE ci_create(VALUE self,
		       VALUE path,VALUE format,
		       VALUE num_i, VALUE num_j,
		       VALUE gp_tiles, 
		       VALUE corresp, VALUE proj,
		       VALUE output_corresp, VALUE output_proj,		   
		       VALUE tile_size, VALUE width, VALUE height){
  
  GDALDataset* hGeopDstDS = NULL; //Will contain the mosaic of gp tiles
  GDALDataset* hGeopSrcDS = NULL; //Will temporarily contain all the original gp tiles
  GDALDataset* hGEDstDS = NULL; //Will contain the raster to send back to ge in tiff
  GDALDataset* hGEDstDS_output = NULL; //Will contain the raster to send back to ge in JPEG format
  int numI,numJ;
  char * tile_path;
  int tileSize,outputWidth,outputHeight;
  char * projection;
  char *outputProjection;
  GDALDriver *tiffDriver;
  GDALDriver *outputDriver;
  
  VALUE *ptr;
  CPLErr err;
  
  tiffDriver = GetGDALDriverManager()->GetDriverByName("GTiff");
  char* outputFormat = STR2CSTR(format);
  if(!strcmp(outputFormat,"image/jpeg"))
    outputDriver = GetGDALDriverManager()->GetDriverByName("JPEG");
  else if(!strcmp(outputFormat,"image/png"))
    outputDriver = GetGDALDriverManager()->GetDriverByName("PNG");
  else
    return Qfalse;
  
  
  numI = FIX2INT(num_i);
  numJ = FIX2INT(num_j);
  tile_path = STR2CSTR(path);
  projection = STR2CSTR(proj);
  outputProjection = STR2CSTR(output_proj);
  tileSize = FIX2INT(tile_size);
  outputWidth = FIX2INT(width);
  outputHeight = FIX2INT(height);

  char * tmpFilePathA = (char*) CPLMalloc((RSTRING(path)->len + 7) * sizeof(char));
  sprintf(tmpFilePathA,"%s.a.tif",tile_path);
     
  int numbands;
  int size = RARRAY(gp_tiles)->len;
  ptr = RARRAY(gp_tiles)->ptr;

  GByte *dataBuffer = (GByte*) CPLMalloc(sizeof(GByte)* tileSize * tileSize * 3);
 
  for(int i = 0 ; i < size ; i++,ptr++){
    //copy to the dst file the tiles whose path are in the array 
    int row,col;
    row = i % numJ;
    col = (int) i / numJ;

    hGeopSrcDS = (GDALDataset *) GDALOpen(STR2CSTR(*ptr), GA_ReadOnly );

    if(hGeopSrcDS == NULL)
      return Qfalse;
    
  
    //The xSize and ySize are smaller than tileSize. So the tile can get into the buffer
    int xSize = hGeopSrcDS->GetRasterXSize();
    int ySize = hGeopSrcDS->GetRasterYSize();
    numbands = hGeopSrcDS->GetRasterCount();
    
    if(numbands > 3)
      numbands = 3;
        
    if(hGeopDstDS == NULL){
      //Create the temp image, which is a mosaic of all the local tiles,
      //downloaded from the geoportail
      hGeopDstDS = tiffDriver->Create(tmpFilePathA,numI*tileSize,numJ*tileSize,numbands,GDT_Byte,0);

      if(hGeopDstDS == NULL)
	return Qfalse;
      
    }



    err = hGeopSrcDS->RasterIO(GF_Read,0,0,xSize,ySize,
			       dataBuffer,xSize,ySize,
			       GDT_Byte,numbands,0,0,0,0);
    
    if(err == CE_Failure)
      return Qfalse;
            
    err = hGeopDstDS->RasterIO(GF_Write,col * tileSize,row * tileSize,
			       xSize,ySize,dataBuffer,xSize,ySize,
			       GDT_Byte,numbands,0,0,0,0);

    if(err == CE_Failure)
      return Qfalse;
    
    GDALClose(hGeopSrcDS);
  }
  CPLFree(dataBuffer);
    
  //Add an affine transform
  ptr = RARRAY(corresp)->ptr;

  double afTransform[6];
  afTransform[0] = NUM2DBL(ptr[0]);
  afTransform[1] = NUM2DBL(ptr[1]);
  afTransform[2] = NUM2DBL(ptr[2]);
  afTransform[3] = NUM2DBL(ptr[3]);
  afTransform[4] = NUM2DBL(ptr[4]);
  afTransform[5] = NUM2DBL(ptr[5]);
    
  OGRSpatialReference oSRS;
  oSRS.importFromProj4(projection);
  char *localProjWKT = NULL;
  oSRS.exportToWkt(&localProjWKT);

  hGeopDstDS->SetProjection(localProjWKT);
  hGeopDstDS->SetGeoTransform(afTransform);
  
  //open the dest file and warp the mosaic into it
  char * tmpFilePathB = (char*) CPLMalloc((RSTRING(path)->len + 7) * sizeof(char)); 
  sprintf(tmpFilePathB,"%s.b.tif",tile_path); 
  hGEDstDS = tiffDriver->Create(tmpFilePathB,outputWidth,outputHeight,numbands,GDT_Byte,0); 
      
  if(hGEDstDS == NULL)
    return Qfalse;
     
  //Add the affine geotransform
  ptr = RARRAY(output_corresp)->ptr;
  afTransform[0] = NUM2DBL(ptr[0]);
  afTransform[1] = NUM2DBL(ptr[1]);
  afTransform[2] = NUM2DBL(ptr[2]);
  afTransform[3] = NUM2DBL(ptr[3]);
  afTransform[4] = NUM2DBL(ptr[4]);
  afTransform[5] = NUM2DBL(ptr[5]);
  
  OGRSpatialReference oSRSOutput;
  oSRSOutput.importFromProj4(outputProjection);
  char * outputProjWKT = NULL;
  oSRSOutput.exportToWkt(&outputProjWKT);

  hGEDstDS->SetProjection(outputProjWKT);
  hGEDstDS->SetGeoTransform(afTransform);
  
  //Do the warping
  GDALWarpOptions *psWarpOptions = GDALCreateWarpOptions(); 

  psWarpOptions->hSrcDS = hGeopDstDS;
  psWarpOptions->hDstDS = hGEDstDS;
  
  psWarpOptions->nBandCount = 0;

  // Establish reprojection transformer. 
  psWarpOptions->pTransformerArg = 
    GDALCreateGenImgProjTransformer( hGeopDstDS, 
				     GDALGetProjectionRef(hGeopDstDS), 
				     hGEDstDS,
				     GDALGetProjectionRef(hGEDstDS), 
				     FALSE, 0.0, 1 );
    
  psWarpOptions->pfnTransformer = GDALGenImgProjTransform;

  // Initialize and execute the warp operation. 
  GDALWarpOperation oOperation;
  oOperation.Initialize( psWarpOptions );
  oOperation.ChunkAndWarpImage( 0, 0, 
				hGEDstDS->GetRasterXSize() , 
				hGEDstDS->GetRasterYSize() );
 
  GDALDestroyGenImgProjTransformer( psWarpOptions->pTransformerArg );
  GDALDestroyWarpOptions( psWarpOptions );
  
  //Clean up the mosaic'ed file
  GDALClose( hGeopDstDS );
 
  //export the temp tiff file into the jpeg file
  hGEDstDS_output = outputDriver->CreateCopy(tile_path,hGEDstDS,TRUE,0,0,0);
  
  if(hGEDstDS_output == NULL){
    return Qfalse;
  }

  //Clean up the still open datasets
  GDALClose(hGEDstDS); 
  GDALClose(hGEDstDS_output);

  //Clean up the temp tiff files
  VSIUnlink(tmpFilePathA);
  CPLFree(tmpFilePathA);
  VSIUnlink(tmpFilePathB);
  CPLFree(tmpFilePathB);

  //Clean up the WKT strings
  CPLFree(localProjWKT);
  CPLFree(outputProjWKT);

  return Qtrue;
}

void Init_CI(void) {
  mCreateImage = rb_define_module("CreateImage");
  rb_define_module_function(mCreateImage,"create",(ruby_method*) &ci_create,12);
  GDALAllRegister(); //Register the GDAL drivers
}
