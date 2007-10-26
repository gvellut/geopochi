Ceci est GEopochi v2. C'est un proxy entre Google Earth et la deuxième version du Géoportail de l'IGN (http://visubeta.geoportail.fr). Comparé à la version 1, il devrait être beaucoup plus rapide pour afficher les tuiles (les images de la version 2 étant dans une projection similaire à celle requise par Google Earth, il n'y a plus de calcul de reprojection à effectuer). Il utilise désormais le langage Erlang et le serveur web Yaws pour servir les fichiers. C'est pourquoi le programme devrait fonctionner sur les plateformes principales (Windows, Linux, Mac OS X) sans modification, bien que je n'aie pas pu vérifier par moi-même.

*** Installation ***
- Télécharger Erlang/OTP :
	- Pour les utilisateurs de Windows, aller à http://erlang.org/download.html et télécharger le premier lien dans la colonne "Windows binary" puis installer.
	- Pour les utilisateurs de Linux, votre distribution a probablement un package précompilé pour Erlang.
	- Pour les utilisateurs de Mac OS X, le projet Darwin Ports a un package Erlang.
	- Dans tous les cas, les sources de Erlang peuvent être téléchargés depuis http://erlang.org/download.html et compilées à la main
- Installer Google Earth 4 ou une version ultérieure.
- Double cliquer sur launch.bat à la racine de l'archive. Ca devrait créer une fenêtre Dos.
- Si vous obtenez une erreur à propos de 'erl non trouvé', ajoutez le dossier où se trouve le programme erl.exe (qui vient de Erlang/OTP) dans votre variable d'environnement PATH.
- Double cliquer sur geopochiv2.x.kml à la racine de l'archive. GE devrait s'ouvrir. Aller à l'un des territoires supportés par la version actuelle de GEopochi. Zoomer un peu (voire beaucoup, dans le cas des cartes de SPM) si aucune image n'apparait.

*** Conseil d'utilisations ***
- Les images téléchargées depuis le géoportail sont mises dans le répertoire cache pour ne pas faire plusieurs fois la même requête. Vous pouvez le vider de temps en temps mais n'effacez pas le fichier empty.png car il est nécessaire pour la bonne marche de l'application.

*** Limitations actuelles ***
- Seule la France métropolitaine (photos aériennes et scans) et Saint-Pierre-et-Miquelon (idem) sont actuellement supportés, bien qu'il ne soit pas trop difficile d'ajouter les autres territoires/couches. Ce sera peut-être fait plus tard.
