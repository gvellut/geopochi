Ceci est GEopochi v2. C'est un proxy entre Google Earth et la deuxi�me version du G�oportail de l'IGN (http://visubeta.geoportail.fr). Compar� � la version 1, il devrait �tre beaucoup plus rapide pour afficher les tuiles (les images de la version 2 �tant dans une projection similaire � celle requise par Google Earth, il n'y a plus de calcul de reprojection � effectuer). Il utilise d�sormais le langage Erlang et le serveur web Yaws pour servir les fichiers. C'est pourquoi le programme devrait fonctionner sur les plateformes principales (Windows, Linux, Mac OS X) sans modification, bien que je n'aie pas pu v�rifier par moi-m�me.

*** Installation ***
- T�l�charger Erlang/OTP :
	- Pour les utilisateurs de Windows, aller � http://erlang.org/download.html et t�l�charger le premier lien dans la colonne "Windows binary" puis installer.
	- Pour les utilisateurs de Linux, votre distribution a probablement un package pr�compil� pour Erlang.
	- Pour les utilisateurs de Mac OS X, le projet Darwin Ports a un package Erlang.
	- Dans tous les cas, les sources de Erlang peuvent �tre t�l�charg�s depuis http://erlang.org/download.html et compil�es � la main
- Installer Google Earth 4 ou une version ult�rieure.
- Double cliquer sur launch.bat � la racine de l'archive. Ca devrait cr�er une fen�tre Dos.
- Si vous obtenez une erreur � propos de 'erl non trouv�', ajoutez le dossier o� se trouve le programme erl.exe (qui vient de Erlang/OTP) dans votre variable d'environnement PATH.
- Double cliquer sur geopochiv2.x.kml � la racine de l'archive. GE devrait s'ouvrir. Aller � l'un des territoires support�s par la version actuelle de GEopochi. Zoomer un peu (voire beaucoup, dans le cas des cartes de SPM) si aucune image n'apparait.

*** Conseil d'utilisations ***
- Les images t�l�charg�es depuis le g�oportail sont mises dans le r�pertoire cache pour ne pas faire plusieurs fois la m�me requ�te. Vous pouvez le vider de temps en temps mais n'effacez pas le fichier empty.png car il est n�cessaire pour la bonne marche de l'application.

*** Limitations actuelles ***
- Seule la France m�tropolitaine (photos a�riennes et scans) et Saint-Pierre-et-Miquelon (idem) sont actuellement support�s, bien qu'il ne soit pas trop difficile d'ajouter les autres territoires/couches. Ce sera peut-�tre fait plus tard.
