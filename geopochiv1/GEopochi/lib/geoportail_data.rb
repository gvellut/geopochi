module GeoportailData
  UB = 10000000000000 #UnBounded
  
  DATA = {
    :spm => GeoportailTerritory.new("Saint-Pierre-et-Miquelon",
                                    ["proj=utm","zone=21","units=m", "no.defs"], 1500, 300000,
                                    :scan =>
                                      GeoportailLayer.new("Scan",Extent.new(5.430e+5,5.22500e+6,5.73e+5,5.1750e+6),UV.new(2.5,2.5), 12000, 20000, "", [
                                                            GeoportailLayerComponent.new("/f/d.ecw",Extent.new(543000, 5225000, 573000, 5175000),0, UB) #always valid
                                                          ],false),
                                    :ortho =>
                                      GeoportailLayer.new("Aerial",Extent.new(5.440e5,5.2230e6,5.680e5,5.1760e6),UV.new(0.5,0.5),48000,94000,"", [
                                                            GeoportailLayerComponent.new("/g/9752005.ecw",Extent.new(5.440e5,5.2230e6,5.680e5,5.1760e6),0,UB)
                                                          ],true)
                                    ),
    :kerg => GeoportailTerritory.new("Kerguelen",
                                     ["proj=utm","zone=42","south","units=m", "no.defs"],5000, 300000,
                                     :scan => #/f/...
                                       GeoportailLayer.new("Scan",Extent.new(4.2935e+5,4.6467e+6,6.4571e+5,4.4523e+6),UV.new(10,10),21636, 19440,"dmyctokerbr.jp2,dmyctokertl.jp2",[
                                                             GeoportailLayerComponent.new("/f/6.ecw",Extent.new(451750, 4628550, 632200, 4470700),0, 50000),
                                                             GeoportailLayerComponent.new("/f/8.ecw",Extent.new(429350, 4646700, 645710, 4452300),50000, UB)
                                                           ],true)
                                     ),
    :guad => GeoportailTerritory.new("Guadeloupe",
                                     ["proj=utm","zone=20","units=m", "no.defs"],1500, 300000,
                                     :ortho => 
                                       GeoportailLayer.new("Aerial",Extent.new(4.820e+5,2.0060e+6,7.150e+5,1.7500e+6),UV.new(0.5,0.5),466000, 512000,"",[
                                                             GeoportailLayerComponent.new("/e/50.ecw",Extent.new(482000, 2006000, 715000, 1750000),0, UB)
                                                           ],true),
                                     :scan => #cright /f/....
                                       GeoportailLayer.new("Scan",Extent.new(4.8e+5,2.01e+6,7.2022e+5,1.74306e+6), UV.new(2.5,2.5), 96088, 106776,"dmyctoguabr.jp2,dmyctoguatl.jp2",[
                                                             GeoportailLayerComponent.new("/f/b.ecw",Extent.new(480000, 2010000, 720000, 1750000),0,UB),
                                                             GeoportailLayerComponent.new("/f/3.ecw",Extent.new(481480, 2005370, 720220, 1743060),12500, UB)
                                                           ],false),
                                     :alti =>
                                       GeoportailLayer.new("Altitude",Extent.new(4.797342201845560e+5,2.011538517009150e+6,7.476842201845560e+5,1.733438517009150e+6), UV.new(25,25),10718, 11124,"",[
                                                             GeoportailLayerComponent.new("/a/2.ecw",Extent.new(480000, 2010000, 720000, 1750000),0,UB)
                                                           ],false)
                                     ),
    :mart => GeoportailTerritory.new("Martinique",
                                     ["proj=utm","zone=20","units=m", "no.defs"],1500, 300000,
                                     :ortho =>
                                       GeoportailLayer.new("Aerial",Extent.new(6.890e+5,1.6470e+6,7.380e+5,1.590e+6) , UV.new(0.5,0.5), 98000, 114000,"",[
                                                             GeoportailLayerComponent.new("/e/51.ecw",Extent.new(689000, 1647000, 738000, 1590000),0, UB)
                                                           ],true),
                                     :scan =>#/f/....
                                       GeoportailLayer.new("Scan", Extent.new(6.871e+5,1.6540e+6,7.53e+5,1.58250e+6),UV.new(2.5,2.5), 26360, 28600,"dmyctomarbr.jp2,dmyctomartl.jp2",[
                                                             GeoportailLayerComponent.new("/f/c.ecw",Extent.new(690000, 1650000, 740000, 1590000),0, UB),
                                                             GeoportailLayerComponent.new("/f/4.ecw",Extent.new(687100, 1654000, 753000, 1582500),12500, UB)
                                                           ],false),
                                     :alti =>
                                       GeoportailLayer.new("Altitude",Extent.new(6.818436794941037e+5,1.657912564942177e+6,7.453422509622261e+5,1.583639361437386e+6), UV.new(25,25), 2540, 2971,"",[
                                                             GeoportailLayerComponent.new("/a/1.ecw",Extent.new(690000, 1650000, 740000, 1590000),0, UB)
                                                           ],false)
                                     ),
    :guya => GeoportailTerritory.new("Guyane",
                                     ["proj=utm","zone=22","units=m", "no.defs"],1500, 500000,
                                     :ortho =>
                                       GeoportailLayer.new("Aerial",Extent.new(1.51e+5,6.38e+5,3.65e+5,4.98e+5), UV.new(0.5,0.5), 427999, 280000,"",[
                                                             GeoportailLayerComponent.new("/e/52.ecw",Extent.new(151000, 638000, 365000, 498000),0, UB)
                                                           ],true),
                                     :scan =>#/f/...
                                       GeoportailLayer.new("Scan",Extent.new(8.325192613901058e+4,6.783609006634455e+005,4.832519261390106e+005,1.783609000000000e+005), UV.new(2.5,2.5), 160000, 200000,"dmyctoguybr2.jp2,dmyctoguytl2.jp2",[
                                                             GeoportailLayerComponent.new("/f/e.ecw",Extent.new(150000, 640000, 420000, 460000),0, UB),
                                                             GeoportailLayerComponent.new("/f/1.ecw",Extent.new(100000, 643000, 450000, 343000),12500, 50000),
                                                             GeoportailLayerComponent.new("/f/12.ecw",Extent.new(83250, 678475, 483250, 178475),50000, UB)
                                                           ],false),
                                     :alti =>
                                       GeoportailLayer.new("Altitude",Extent.new(1.46e+5,6.40e+5,4.46e+5,4.50e+5), UV.new(25,25),12000, 8000,"",[
                                                             GeoportailLayerComponent.new("/a/3.ecw",Extent.new(150000, 640000, 420000, 460000),0,UB)
                                                           ],false)
                                     ),
    :wafu => GeoportailTerritory.new("Wallis & Futuna",
                                     ["proj=utm","zone=1","south","units=m", "no.defs"],1500, 300000,
                                     :ortho => 
                                       GeoportailLayer.new("Aerial",Extent.new(3.7200e+5,8.5440e+6,5.9600e+5,8.4110e+6), UV.new(0.5,0.5), 448000, 266000,"",[
                                                             GeoportailLayerComponent.new("/e/55.ecw",Extent.new(372000, 8544000, 596000, 8411000),0,UB)
                                                           ],true),
                                     :scan => 
                                       GeoportailLayer.new("Scan", Extent.new(3.7119125e+5,8.543685e+6,5.9679125e+5,8.410245e+6), UV.new(2.5,2.5), 90240, 53376,"",[
                                                             GeoportailLayerComponent.new("/f/f.ecw",Extent.new(371191, 8426565, 392951, 8410245),0,UB)
                                                           ],false)
                                     ),
    :reun => GeoportailTerritory.new("Réunion",
                                     ["proj=utm","zone=40","south","units=m", "no.defs"],1500, 300000,
                                     :ortho => 
                                       GeoportailLayer.new("Aerial", Extent.new(3.13e+5,7.693e+6,3.810e+5,7.6330e+6), UV.new(0.5,0.5), 136000, 120000,"",[
                                                             GeoportailLayerComponent.new("/e/53.ecw",Extent.new(313000, 7693000, 381000, 7633000),0, UB)
                                                           ],true),
                                     :scan => #/f/...
                                       GeoportailLayer.new("Scan", Extent.new(3.08377e+5,7.6979890e+6,3.853770e+5,7.626489e+6), UV.new(2.5,2.5), 30800, 28600,"dmyctoreubr.jp2,dmyctoreutl.jp2",[
                                                             GeoportailLayerComponent.new("/f/11.ecw",Extent.new(312000, 7692000, 382000, 7632000),0, UB),
                                                             GeoportailLayerComponent.new("/f/5.ecw",Extent.new(308377, 7697989, 385377, 7626489),12500, UB)
                                                           ],false),
                                     :alti =>
                                       GeoportailLayer.new("Altitude",Extent.new(3.05e+5,7.70e+6,3.95e+5,7.625e+6),UV.new(25,25), 3600, 3000,"",[
                                                             GeoportailLayerComponent.new("/a/5.ecw",Extent.new(312000, 7692000, 382000, 7632000),0, UB)
                                                           ],false)
                                     ),
    :mayo => GeoportailTerritory.new("Mayotte",
                                     ["proj=utm","zone=38","south","units=m", "no.defs"],1500, 300000,
                                     :ortho =>
                                       GeoportailLayer.new("Aerial",Extent.new(4.93e+5,8.61200e+6,5.340e+5,8.5520e+6), UV.new(0.5,0.5), 82000, 120000,"",[
                                                             GeoportailLayerComponent.new("/e/54.ecw",Extent.new(493000, 8612000, 534000, 8552000),0,UB)
                                                           ],true),
                                     :scan =>
                                       GeoportailLayer.new("Scan",Extent.new(5.00e+5,8.610e+6,5.400e+5,8.560e+6),UV.new(2.5,2.5), 16000, 20000,"",[
                                                             GeoportailLayerComponent.new("/f/10.ecw",Extent.new(500000, 8610000, 540000, 8560000),0,UB)
                                                           ],false),
                                     :alti =>
                                       GeoportailLayer.new("Altitude",Extent.new(5.00e+5,8.605e+6,5.40e+5,8.56e+6), UV.new(25,25),1600, 1800,"",[
                                                             GeoportailLayerComponent.new("/a/4.ecw",Extent.new(500000, 8610000, 540000, 8560000),0,UB)
                                                           ],false)
                                     ),
    :ncal => GeoportailTerritory.new("Nouvelle-Calédonie",
                                     ["proj=utm","zone=58","south","units=m", "no.defs"],5000, 500000,
                                     :scan => #/f/...
                                       GeoportailLayer.new("Scan",Extent.new(3.40e+5,7.86e+6,8.40e+5,7.46e+6), UV.new(5,5), 100000, 80000,"dmyctoncabr.jp2,dmyctoncatl.jp2",[
                                                             GeoportailLayerComponent.new("/f/17.ecw",Extent.new(340000, 7850000, 840000, 7470000),0, UB),
                                                             GeoportailLayerComponent.new("/f/9.ecw",Extent.new(40000, 7860000, 840000, 7460000),25000, UB),
                                                             GeoportailLayerComponent.new("/f/14.ecw",Extent.new(340000, 7860000, 840000, 7460000),150000, UB)
                                                           ],true)
                                     ),
    :croz => GeoportailTerritory.new("Crozet",
                                     ["proj=utm","zone=39","south","units=m", "no.defs"],5000, 300000,
                                     :scan => #/f/...
                                       GeoportailLayer.new("Scan",Extent.new(4.152e+5,4.94255e+6,6.21e+5,4.81405e+6) , UV.new(5,5), 41160, 25700,"dmyctocrobr.jp2,dmyctocrotl.jp2",[
                                                             GeoportailLayerComponent.new("/f/16.ecw",Extent.new(547850, 4870000, 596000, 4851000),0, UB),
                                                             GeoportailLayerComponent.new("/f/7.ecw",Extent.new(421000, 4936000, 615000, 4835000),25000, UB)
                                                           ],true)
                                     ),
    :metr => GeoportailTerritory.new("France",
                                     #["proj=lcc","a=6378249.2","b=6356515.0","lat_0=46d48'0.0\"N","lon_0=2d20'14.025\"E","lat_1=45d53'56.108\"N","lat_2=47d41'45.652\"N","x_0=600000.0","y_0=2200000.0","towgs84=-168,-60,+320","units=m"],
["proj=lcc","towgs84=-168,-60,320,0,0,0,0","ellps=clrk80","lat_0=46.80000","lat_1=45.8989194400","lat_2=47.6960138900","lon_0=2.33722917","x_0=600000","y_0=2200000"],
                                     1500,1000000,
                                     :ortho => 
                                       GeoportailLayer.new("Aerial",Extent.new(45000,2700000,1200000,1616000), UV.new(0.5,0.5), 2310000, 2168000,"/c/dmybdobr.jp2,/c/dmybdotl2.jp2",[
                                                             GeoportailLayerComponent.new("/c/10.ecw",Extent.new(245000, 2600000, 345000, 2500000),0,14000),                
                                                             GeoportailLayerComponent.new("/c/11.ecw",Extent.new(345000, 1800000, 445000, 1700000),0,14000),
                                                             GeoportailLayerComponent.new("/c/12.ecw",Extent.new(345000, 1900000, 445000, 1800000),0,14000),
                                                             GeoportailLayerComponent.new("/c/13.ecw",Extent.new(345000, 2000000, 445000, 1900000),0,14000),
                                                             GeoportailLayerComponent.new("/c/14.ecw",Extent.new(345000, 2100000, 445000, 2000000),0,14000),
                                                             GeoportailLayerComponent.new("/c/15.ecw",Extent.new(345000, 2200000, 445000, 2100000),0,14000),
                                                             GeoportailLayerComponent.new("/c/16.ecw",Extent.new(345000, 2300000, 445000, 2200000),0,14000),
                                                             GeoportailLayerComponent.new("/c/17.ecw",Extent.new(345000, 2400000, 445000, 2300000),0,14000),
                                                             GeoportailLayerComponent.new("/c/18.ecw",Extent.new(345000, 2500000, 445000, 2400000),0,14000),
                                                             GeoportailLayerComponent.new("/c/19.ecw",Extent.new(345000, 2600000, 445000, 2500000),0,14000),
                                                             GeoportailLayerComponent.new("/c/1a.ecw",Extent.new(445000, 1800000, 545000, 1700000),0,14000),
                                                             GeoportailLayerComponent.new("/c/1b.ecw",Extent.new(445000, 1900000, 545000, 1800000),0,14000),
                                                             GeoportailLayerComponent.new("/c/1c.ecw" ,Extent.new(445000, 2000000, 545000, 1900000),0,14000),
                                                             GeoportailLayerComponent.new("/c/1d.ecw",Extent.new(445000, 2100000, 545000, 2000000),0,14000),
                                                             GeoportailLayerComponent.new("/c/2.ecw",Extent.new(45000, 2400000, 145000, 2300000),0,14000),
                                                             GeoportailLayerComponent.new("/c/3.ecw",Extent.new(45000, 2500000, 145000, 2400000),0,14000),
                                                             GeoportailLayerComponent.new("/c/4.ecw",Extent.new(145000, 2200000, 245000, 2100000),0,14000),
                                                             GeoportailLayerComponent.new( "/c/5.ecw",Extent.new(145000, 2300000, 245000, 2200000),0,14000),
                                                             GeoportailLayerComponent.new("/c/6.ecw",Extent.new(145000, 2400000, 245000, 2300000),0,14000),
                                                             GeoportailLayerComponent.new("/c/7.ecw",Extent.new(145000, 2500000, 245000, 2400000),0,14000),
                                                             GeoportailLayerComponent.new("/c/8.ecw",Extent.new(245000, 1800000, 345000, 1700000),0,14000),
                                                             GeoportailLayerComponent.new("/c/9.ecw",Extent.new(245000, 1900000, 345000, 1800000),0,14000),
                                                             GeoportailLayerComponent.new("/c/a.ecw",Extent.new(245000, 2000000, 345000, 1900000),0,14000),
                                                             GeoportailLayerComponent.new("/c/b.ecw",Extent.new(245000, 2100000, 345000, 2000000),0,14000),
                                                             GeoportailLayerComponent.new("/c/c.ecw",Extent.new(245000, 2200000, 345000, 2100000),0,14000),
                                                             GeoportailLayerComponent.new("/c/d.ecw",Extent.new(245000, 2300000, 345000, 2200000),0,14000),
                                                             GeoportailLayerComponent.new("/c/e.ecw",Extent.new(245000, 2400000, 345000, 2300000),0,14000),
                                                             GeoportailLayerComponent.new("/c/f.ecw",Extent.new(245000, 2500000, 345000, 2400000),0,14000),
                                                             GeoportailLayerComponent.new("/d/1e.ecw",Extent.new(445000, 2200000, 545000, 2100000),0,14000),
                                                             GeoportailLayerComponent.new("/d/1f.ecw",Extent.new(445000, 2300000, 545000, 2200000),0,14000),
                                                             GeoportailLayerComponent.new("/d/20.ecw",Extent.new(445000, 2400000, 545000, 2300000),0,14000),
                                                             GeoportailLayerComponent.new("/d/21.ecw",Extent.new(445000, 2500000, 545000, 2400000),0,14000),
                                                             GeoportailLayerComponent.new("/d/22.ecw",Extent.new(445000, 2600000, 545000, 2500000),0,14000),
                                                             GeoportailLayerComponent.new("/d/23.ecw",Extent.new(445000, 2700000, 545000, 2600000),0,14000),
                                                             GeoportailLayerComponent.new("/d/24.ecw",Extent.new(545000, 1800000, 645000, 1700000),0,14000),
                                                             GeoportailLayerComponent.new("/d/25.ecw",Extent.new(545000, 1900000, 645000, 1800000),0,14000),
                                                             GeoportailLayerComponent.new("/d/26.ecw",Extent.new(545000, 2000000, 645000, 1900000),0,14000),
                                                             GeoportailLayerComponent.new("/d/27.ecw",Extent.new(545000, 2100000, 645000, 2000000),0,14000),
                                                             GeoportailLayerComponent.new("/d/28.ecw",Extent.new(545000, 2200000, 645000, 2100000),0,14000),
                                                             GeoportailLayerComponent.new("/d/29.ecw",Extent.new(545000, 2300000, 645000, 2200000),0,14000),
                                                             GeoportailLayerComponent.new("/d/2a.ecw",Extent.new(545000, 2400000, 645000, 2300000),0,14000),
                                                             GeoportailLayerComponent.new("/d/2b.ecw",Extent.new(545000, 2500000, 645000, 2400000),0,14000),
                                                             GeoportailLayerComponent.new("/d/2c.ecw",Extent.new(545000, 2600000, 645000, 2500000),0,14000),
                                                             GeoportailLayerComponent.new("/d/2d.ecw",Extent.new(545000, 2700000, 645000, 2600000),0,14000),
                                                             GeoportailLayerComponent.new("/d/2e.ecw",Extent.new(645000, 1800000, 745000, 1700000),0,14000),
                                                             GeoportailLayerComponent.new("/d/2f.ecw",Extent.new(645000, 1900000, 745000, 1800000),0,14000),
                                                             GeoportailLayerComponent.new("/d/30.ecw",Extent.new(645000, 2000000, 745000, 1900000),0,14000),
                                                             GeoportailLayerComponent.new("/d/31.ecw",Extent.new(645000, 2100000, 745000, 2000000),0,14000),
                                                             GeoportailLayerComponent.new("/d/32.ecw",Extent.new(645000, 2200000, 745000, 2100000),0,14000),
                                                             GeoportailLayerComponent.new("/d/33.ecw",Extent.new(645000, 2300000, 745000, 2200000),0,14000),
                                                             GeoportailLayerComponent.new("/d/34.ecw",Extent.new(645000, 2400000, 745000, 2300000),0,14000),
                                                             GeoportailLayerComponent.new("/d/35.ecw",Extent.new(645000, 2500000, 745000, 2400000),0,14000),
                                                             GeoportailLayerComponent.new("/d/36.ecw",Extent.new(645000, 2600000, 745000, 2500000),0,14000),
                                                             GeoportailLayerComponent.new("/e/37.ecw",Extent.new(645000, 2700000, 745000, 2600000),0,14000),
                                                             GeoportailLayerComponent.new("/e/38.ecw",Extent.new(745000, 1900000, 845000, 1800000),0,14000),
                                                             GeoportailLayerComponent.new("/e/39.ecw",Extent.new(745000, 2000000, 845000, 1900000),0,14000),
                                                             GeoportailLayerComponent.new("/e/3a.ecw",Extent.new(745000, 2100000, 845000, 2000000),0,14000),
                                                             GeoportailLayerComponent.new("/e/3b.ecw",Extent.new(745000, 2200000, 845000, 2100000),0,14000),
                                                             GeoportailLayerComponent.new("/e/3c.ecw",Extent.new(745000, 2300000, 845000, 2200000),0,14000),
                                                             GeoportailLayerComponent.new("/e/3d.ecw",Extent.new(745000, 2400000, 845000, 2300000),0,14000),
                                                             GeoportailLayerComponent.new("/e/3e.ecw",Extent.new(745000, 2500000, 845000, 2400000),0,14000),
                                                             GeoportailLayerComponent.new("/e/3f.ecw",Extent.new(745000, 2600000, 845000, 2500000),0,14000),
                                                             GeoportailLayerComponent.new("/e/40.ecw",Extent.new(845000, 1800000, 945000, 1700000),0,14000),
                                                             GeoportailLayerComponent.new("/e/41.ecw",Extent.new(845000, 1900000, 945000, 1800000),0,14000),
                                                             GeoportailLayerComponent.new("/e/42.ecw",Extent.new(845000, 2000000, 945000, 1900000),0,14000),
                                                             GeoportailLayerComponent.new("/e/43.ecw",Extent.new(845000, 2100000, 945000, 2000000),0,14000),
                                                             GeoportailLayerComponent.new("/e/44.ecw",Extent.new(845000, 2200000, 945000, 2100000),0,14000),
                                                             GeoportailLayerComponent.new("/e/45.ecw",Extent.new(845000, 2300000, 945000, 2200000),0,14000),
                                                             GeoportailLayerComponent.new("/e/46.ecw",Extent.new(845000, 2400000, 945000, 2300000),0,14000),
                                                             GeoportailLayerComponent.new("/e/47.ecw",Extent.new(845000, 2500000, 945000, 2400000),0,14000),
                                                             GeoportailLayerComponent.new("/e/48.ecw",Extent.new(845000, 2600000, 945000, 2500000),0,14000),
                                                             GeoportailLayerComponent.new("/e/49.ecw",Extent.new(945000, 1900000, 1045000, 1800000),0,14000),
                                                             GeoportailLayerComponent.new("/e/4a.ecw",Extent.new(945000, 2000000, 1045000, 1900000),0,14000),
                                                             GeoportailLayerComponent.new("/e/4b.ecw",Extent.new(945000, 2100000, 1045000, 2000000),0,14000),
                                                             GeoportailLayerComponent.new("/e/4c.ecw",Extent.new(945000, 2200000, 1045000, 2100000),0,14000),
                                                             GeoportailLayerComponent.new("/e/4d.ecw",Extent.new(945000, 2300000, 1045000, 2200000),0,14000),
                                                             GeoportailLayerComponent.new("/e/4e.ecw",Extent.new(945000, 2400000, 1045000, 2300000),0,14000),
                                                             GeoportailLayerComponent.new("/e/4f.ecw",Extent.new(945000, 2500000, 1045000, 2400000),0,14000),
                                                           GeoportailLayerComponent.new("/e/56.ecw",Extent.new(1110000, 1808000, 1200000, 1616000),0,14000),
#                                                              GeoportailLayerComponent.new("/g/0012000.ecw", Extent.new(  784000, 2174000, 896000, 2072000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0022001.ecw", Extent.new(  644000, 2565000, 740000, 2426000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0032002.ecw", Extent.new( 594000, 2202000, 730000, 2103000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0042004.ecw", Extent.new( 852000, 1974000, 972000, 1857000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0052003.ecw", Extent.new( 844000, 2024000, 977000, 1914000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0062004.ecw", Extent.new(  945000, 1941000, 1033000, 1840000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0072002.ecw", Extent.new(  719000, 2045000, 803000, 1919000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0082000.ecw", Extent.new( 721000, 2579000, 823000, 2471000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0092003.ecw", Extent.new( 475000, 1815000, 588000, 1728000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0102000.ecw", Extent.new( 676000, 2416000, 789000, 2325000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0112003.ecw", Extent.new( 546000, 1830000, 675000, 1737000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0122003.ecw", Extent.new( 559000, 1995000, 691000, 1853000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0132003.ecw", Extent.new( 752000, 1885000, 882000, 1799000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0142001.ecw", Extent.new( 342000, 2496000, 463000, 2421000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0152000.ecw", Extent.new( 577000, 2055000, 683000, 1956000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0162002.ecw", Extent.new( 381000, 2129000, 494000, 2022000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0172003.ecw", Extent.new( 298000, 2159000, 418000, 2011000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0182000.ecw", Extent.new( 556000, 2294000, 658000, 2156000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0192004.ecw", Extent.new( 511000, 2087000, 617000, 1990000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0202002.ecw", Extent.new(1110000, 1808000, 1200000, 1616000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0212002.ecw", Extent.new( 729000, 2341000, 842000, 2212000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0222003.ecw", Extent.new( 156000, 2451000, 288000, 2347000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0232000.ecw", Extent.new( 524000, 2163000, 623000, 2072000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0242001.ecw", Extent.new( 411000, 2082000, 531000, 1951000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0252001.ecw", Extent.new( 853000, 2297000, 958000, 2178000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0262001.ecw", Extent.new( 783000, 2043000, 879000, 1905000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0272000.ecw", Extent.new( 450000, 2502000, 563000, 2407000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0282002.ecw", Extent.new( 481000, 2440000, 576000, 2327000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0292000.ecw", Extent.new( 46000, 2438000, 174000, 2318000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0302001.ecw", Extent.new( 673000, 1943000, 803000, 1829000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0312002.ecw", Extent.new( 443000, 1882000, 578000, 1743000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0322004.ecw", Extent.new( 387000, 1901000, 510000, 1812000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0332004.ecw", Extent.new( 313000, 2074000, 442000, 1912000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0342001.ecw", Extent.new( 615000, 1888000, 752000, 1800000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0352001.ecw", Extent.new( 253000, 2422000, 351000, 2300000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0362004.ecw", Extent.new( 486000, 2255000, 591000, 2149000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0372002.ecw", Extent.new( 425000, 2304000, 528000, 2192000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0382003.ecw", Extent.new( 787000, 2104000, 919000, 1971000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0392001.ecw", Extent.new( 820000, 2263000, 897000, 2144000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0402002.ecw", Extent.new( 286000, 1955000, 425000, 1836000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0412002.ecw", Extent.new( 467000, 2351000, 595000, 2242000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0422001.ecw", Extent.new( 704000, 2145000, 791000, 2026000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0432000.ecw", Extent.new( 657000, 2050000, 771000, 1971000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0442004.ecw", Extent.new( 224000, 2324000, 355000, 2213000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0452001.ecw", Extent.new( 537000, 2373000, 661000, 2274000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0462000.ecw", Extent.new( 491000, 2007000, 592000, 1910000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0472004.ecw", Extent.new( 400000, 1978000, 502000, 1887000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0482004.ecw", Extent.new( 650000, 1999000, 734000, 1900000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0492002.ecw", Extent.new( 320000, 2320000, 443000, 2222000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0502002.ecw", Extent.new( 289000, 2536000, 375000, 2389000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0512004.ecw", Extent.new( 676000, 2493000, 799000, 2390000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0522001.ecw", Extent.new( 768000, 2415000, 867000, 2289000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0532001.ecw", Extent.new( 331000, 2401000, 425000, 2307000), 0, 14000),
# #                                                             GeoportailLayerComponent.new("/g/0541999.ecw", Extent.new( 823000, 2514000, 955000, 2377000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0542004.ecw", Extent.new( 823000, 2514000, 955000, 2377000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0552002.ecw", Extent.new(  786000, 2519000, 859000, 2382000), 0, 14000),
# #                                                             GeoportailLayerComponent.new("/g/0561999.ecw", Extent.new( 147000, 2372000, 274000, 2266000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0562004.ecw", Extent.new( 147000, 2372000, 274000, 2266000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0571999.ecw", Extent.new( 856000, 2510000, 989000, 2401000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0572004.ecw", Extent.new( 856000, 2510000, 989000, 2401000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0582002.ecw", Extent.new( 637000, 2289000, 745000, 2183000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0592004.ecw", Extent.new(580000, 2679000, 737000, 2552000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0602001.ecw", Extent.new( 552000, 2531000, 662000, 2450000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0612001.ecw", Extent.new( 362000, 2445000, 501000, 2353000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0622004.ecw", Extent.new( 543000, 2670000, 662000, 2557000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0632004.ecw", Extent.new( 602000, 2141000, 730000, 2031000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0642003.ecw", Extent.new( 264000, 1849000, 414000, 1756000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0652001.ecw", Extent.new( 381000, 1850000, 463000, 1742000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0662004.ecw", Extent.new( 548000, 1770000, 671000, 1702000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0672002.ecw", Extent.new(936000, 2467000, 1033000, 2358000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0682002.ecw", Extent.new( 936000, 2381000, 996000, 2279000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0692003.ecw", Extent.new( 746000, 2149000, 821000, 2052000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0702003.ecw", Extent.new(827000, 2344000, 939000, 2254000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0712002.ecw", Extent.new( 697000, 2243000, 840000, 2129000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0722000.ecw", Extent.new( 390000, 2391000, 496000, 2286000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0732001.ecw", Extent.new( 855000, 2112000, 981000, 2013000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0742003.ecw", Extent.new( 867000, 2167000, 966000, 2082000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0752003.ecw", Extent.new( 590000, 2435000, 611000, 2423000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0762003.ecw", Extent.new( 434000, 2566000, 562000, 2472000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0772003.ecw", Extent.new( 603000, 2459000, 692000, 2345000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0782003.ecw", Extent.new( 533000, 2456000, 594000, 2381000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0792002.ecw", Extent.new( 352000, 2239000, 438000, 2109000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0802001.ecw", Extent.new( 530000, 2599000, 664000, 2507000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0812003.ecw", Extent.new(  534000, 1913000, 650000, 1819000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0822000.ecw", Extent.new(  470000, 1935000, 575000, 1862000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0832003.ecw", Extent.new( 867000, 1878000, 974000, 1783000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0842001.ecw", Extent.new(783000, 1942000, 877000, 1855000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0852001.ecw", Extent.new(  237000, 2239000, 380000, 2145000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0862002.ecw", Extent.new(  413000, 2246000, 515000, 2117000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0872001.ecw", Extent.new( 466000, 2158000, 568000, 2048000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0882001.ecw", Extent.new(  825000, 2403000, 962000, 2321000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0892002.ecw", Extent.new( 637000, 2380000, 752000, 2257000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0902002.ecw", Extent.new( 930000, 2325000, 963000, 2279000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0912003.ecw", Extent.new(567000, 2421000, 620000, 2364000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0922003.ecw", Extent.new( 584000, 2441000, 602000, 2413000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0932003.ecw", Extent.new( 595000, 2448000, 621000, 2422000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0942003.ecw", Extent.new( 597000, 2431000, 622000, 2409000), 0, 14000),
#                                                              GeoportailLayerComponent.new("/g/0952003.ecw", Extent.new( 545000, 2473000, 620000, 2433000), 0, 14000),
                                                             GeoportailLayerComponent.new("/c/0.ecw",Extent.new(40000, 2680000, 1200000, 1610000),14000,UB)
                                                           ],true),
                                     :grk => 
                                       GeoportailLayer.new("Street",Extent.new(-140.0,2702605.10,1250000.0,1529970.46875), UV.new(1.32291666666667,1.32291666666667), 944987, 886400,"dmygrbr.jp2,dmygrtl.jp2", [
                                                             GeoportailLayerComponent.new("/b/7.ecw",Extent.new(45991, 2678892, 1200995, 1618888),0, 6250),
                                                             GeoportailLayerComponent.new("/b/2.ecw",Extent.new(42487, 2682597, 1222500, 1615382),6250, 12500),
                                                             GeoportailLayerComponent.new("/b/6.ecw",Extent.new(24985, 2680015, 1200014, 1604985),12500, 25000),
                                                             GeoportailLayerComponent.new("/b/1.ecw",Extent.new(-100, 2699960, 1222951, 1599900),25000, 50000),
                                                             GeoportailLayerComponent.new("/b/3.ecw",Extent.new(-140, 2700000, 1222994, 1599855),50000, 100000),
                                                             GeoportailLayerComponent.new("/b/5.ecw",Extent.new(22526, 2702572, 1222940, 1595356),100000, 200000),
                                                             GeoportailLayerComponent.new("/b/0.ecw",Extent.new(24259, 2700865, 1221234, 1597023),200000, 400000),
                                                             GeoportailLayerComponent.new("/b/4.ecw",Extent.new(26557, 2698534, 1219299, 1598925),400000, UB)
                                                           ],false),
                                     :bdp => 
                                       GeoportailLayer.new("Cadastre",Extent.new(0.0,2.70e6,1.40e6,1.50e6), UV.new(0.1,0.1), 13999999,11999999,"dmybdpbr.jp2,dmybdptl.jp2", [
                                       GeoportailLayerComponent.new("/h2/8_2-5.jp2",Extent.new(0.0,2.70e6,1.40e6,1.50e6),8000, 13000),
                                       GeoportailLayerComponent.new("/h2/8_5.jp2",Extent.new(0.0,2.70e6,1.40e6,1.50e6),13000,24000),
                                       GeoportailLayerComponent.new("/h2/8_10.jp2", Extent.new(0.0,2.70e6,1.40e6,1.50e6), 24000, 40000),
                                       GeoportailLayerComponent.new("/h2/8_25.jp2",Extent.new(0.0,2.70e6,1.40e6,1.50e6),40000, 100000),
                                       GeoportailLayerComponent.new("/h2/8_50.jp2", Extent.new(0.0,2.70e6,1.40e6,1.50e6),100000, 200000),
                                       GeoportailLayerComponent.new("/h2/8_100.jp2", Extent.new(0.0,2.70e6,1.40e6,1.50e6),200000, UB),
                                       GeoportailLayerComponent.new("/h2/03.jp2", Extent.new(595000, 2201000, 729000, 2104000), 0, 8000),
                                       GeoportailLayerComponent.new("/h2/09.jp2",  Extent.new(476000, 1814000, 587000, 1730000), 0, 8000),
                                       GeoportailLayerComponent.new("/h2/0a.jp2", Extent.new(677000, 2415000, 788000, 2326000),0, 8000),
                                       GeoportailLayerComponent.new("/h2/0c.jp2", Extent.new(560000, 1994000, 690000, 1854000), 0, 8000),
                                       GeoportailLayerComponent.new("/h2/19.jp2", Extent.new(854000, 2296000, 957000, 2179000), 0, 8000),
                                       GeoportailLayerComponent.new("/h2/1c.jp2", Extent.new(482000, 2439000, 575000, 2328000), 0, 8000),
                                       GeoportailLayerComponent.new("/h2/29.jp2", Extent.new(468000, 2347000, 594000, 2243000), 0, 8000),
                                       GeoportailLayerComponent.new("/h2/36.jp2", Extent.new(824000, 2510000, 954000, 2378000), 0, 8000),
                                       GeoportailLayerComponent.new("/h2/39.jp2", Extent.new(857000, 2507000, 988000, 2402000), 0, 8000),
                                       GeoportailLayerComponent.new("/h2/3c.jp2", Extent.new(553000, 2530000, 661000, 2451000), 0,8000),
                                       GeoportailLayerComponent.new("/h2/3d.jp2", Extent.new(363000, 2444000, 500000, 2354000), 0, 8000),
                                       GeoportailLayerComponent.new("/h2/3e.jp2", Extent.new(544000, 2669000, 661000, 2558000), 0, 8000),
                                       GeoportailLayerComponent.new("/h2/40.jp2", Extent.new(265000, 1848000, 413000, 1757000), 0, 8000),
                                       GeoportailLayerComponent.new("/h2/41.jp2", Extent.new(382000, 1849000, 462000, 1743000), 0, 8000),
                                       GeoportailLayerComponent.new("/h2/4e.jp2", Extent.new(534000, 2455000, 593000, 2382000), 0, 8000),
                                       GeoportailLayerComponent.new("/h2/53.jp2", Extent.new(868000, 1877000, 973000, 1783000), 0, 8000),
                                       GeoportailLayerComponent.new("/h2/5b.jp2", Extent.new(568000, 2420000, 619000, 2365000), 0, 8000),
                                       GeoportailLayerComponent.new("/h2/5d.jp2", Extent.new(596000, 2447000, 620000, 2423000), 0, 8000),
                                       GeoportailLayerComponent.new("/h2/5f.jp2", Extent.new(546000, 2472000, 619000, 2434000), 0, 8000)
                                       ],false),
                                       
                                     :scan =>
                                       GeoportailLayer.new("Scan", Extent.new(0.0000000,2700000.000,1300100.0,1549900.000), UV.new(2.50000,2.5000), 520040, 460040,"dmyctometrobr.jp2,dmyctometrotl.jp2",[                                                             
                                                             GeoportailLayerComponent.new("/f/18.ecw",Extent.new(30000, 2700000, 1230100, 1589900),0, 75000), #departemental
                                                             GeoportailLayerComponent.new("/f/19.ecw",Extent.new(0, 2700000, 1300100, 1549900),75000, 150000), #regional
                                                             GeoportailLayerComponent.new("/f/0.ecw",Extent.new(0, 2700000, 1215000, 1595000),150000, UB) #1000
                                                           ],false),
                                     :topo => 
                                       GeoportailLayer.new("Topographic", Extent.new(0.0000000,2700000.000,1300100.0,1549900.000), UV.new(2.50000,2.5000), 520040, 460040,"dmyctometrobr.jp2,dmyctometrotl.jp2",[
                                                             GeoportailLayerComponent.new("/f/a.ecw",Extent.new(40000, 2680000, 1200000, 1610000),0, 12500),
                                                             GeoportailLayerComponent.new("/f/15.ecw",Extent.new(25000, 2680000, 1200000, 1605000),12500, 25000),
                                                             GeoportailLayerComponent.new("/f/2.ecw",Extent.new(0, 2700000, 1250000, 1600000),25000, UB)
                                                           ],false),
                                     :edr =>
                                       GeoportailLayer.new("EDR", Extent.new(0.0000000,2700000.000,1300100.0,1549900.000), UV.new(2.50000,2.5000), 520040, 460040,"dmyctometrobr.jp2,dmyctometrotl.jp2",[
                                                             GeoportailLayerComponent.new("/f/1a.ecw",Extent.new(40000, 2680000, 1200000, 1610000),0, UB)
                                                           ],false),
                                     :alti =>
                                       GeoportailLayer.new("Altitude", Extent.new(-1.0347789e+5,2.809548e+6,1.256872e+6,1.6e+6) , UV.new(25.0000,25.000), 54014, 50200,"dmydtmbr.jp2,dmydtmtl.jp2",[
                                                             GeoportailLayerComponent.new("/a/0.ecw",Extent.new(-1.0347789e+5,2.809548e+6, 1.256872e+6,1.6e+6),0, UB)
                                                           ],false)
                                       )
                                       
  }
end
