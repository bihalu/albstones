# Albstones
Albstones sind bemalte Steine, die auf der Schw�bischen Alb "ausgewildert" werden.  
Auf Facebook gibt es eine gro�e Fangemeinde mit der man jeden Fund eines Albstones teilen kann.  
Hierbei kann jeder mittmachen und es gibt nur ein paar Regeln:  

* Finde einen passenden Stein in der Natur
* Bemale ihn nach belieben
* Lege den Albstone wieder in der Natur aus
* Freue Dich, wenn jemand Deinen Albstone findet
* Wenn Du selbst einen Albstone findest, mache ein Foto, poste es auf Facebook in der Gruppe #albstones und lege den Albstone wieder woanders aus
* Wenn der Albstone besonders f�r Dich ist, darfst Du ihn auch behalten ;-)

# Wozu nun dieses Projekt?
Wenn es doch schon eine gro�e Albstones Community auf Facebook gibt wozu dient dann dieses Projekt.  

## "besondere" Albstones
Die Idee ist, dass es "besondere" Albstones gibt.  
Zu jedem besonderen Albstone gibt es eine Digitale Abbildung auf der Webseite [https://albstones.de](https://albstones.de)  

Zus�tzlich soll es noch eine Geschichte auf der Webseite geben die sich um diese besonderen Albstones dreht.  

## Digitaler Albstone
Die digitale Abbildung eines Albstones enth�lt folgende Informationen:  

* Addresse -> eindeutige Addresse abgeleitet �ber 12 W�rter nach dem BIP-39 und BIP-32 Verfahren
* Datum -> Geburtsdatum bzw. Funddatum
* Name -> Name des Albstones
* Lokation (L�ngengrad, Breitengrad) -> Geburtsort bzw. letzter Fundort
* Nachricht -> Kurzbeschreibung bzw. Lebenslauf des Albstones lyrisch ausgedr�ckt
* Bild -> Bild des Albstones

F�r jeden Albstone gibt es einen initialen Datensatz (Geburt) .
Folgeeintr�ge entstehen, wenn jemand den Albstone findet und einscannt.  
Das Einscannen ist f�lschungssicher, da nur �ber die 12 W�rter der Stein eindeutig identifiziert werden kann.

## Physischer Albstone
Der physische Albstone hingegen enth�lt ein Identifizierungsmerkmal.  
In diesem Fall sind das 12 W�rter, die als QR Code auf dem Stein angebracht sind.  
Besser w�re ein RFID Chip, der in den Stein integriert ist.  

Beide Methoden m�ssen noch getestet werden, da es schwierig ist einen QR Code auf einen Stein zu drucken.  
Auch der RFID Chip ist kompliziert, da ein Loch gebohrt werden muss.  
Au�erdem muss sichergestellt werden, dass die 12 W�rter z.B. mit dem Handy eingescannt werden k�nnen.

Und nat�rlich ist der Albstone k�nstlerisch bemalt ;-)

## M�gliche Verwendung
a) als Schatzsuche auf der schw�bischen Alb  
Bei dieser Variante werden die Albstones weit auseinander auf der schw�bischen Alb ausgewildert.  
Sie m�ssen sehr gut versteckt werden, da ggf. noch ein Anreiz �ber Bitcoins hinterlegt ist.  
Die Lokation an der sie ausgewildert wurden wird nat�rlich nicht verraten.  
Man kann ja den Albstone im Umkreis von 5Km zum Geburtsort auslegen.  
Somit besteht eine sehr geringe Chance den Stein zu finden.

b) als Suchspiel auf einem begrenzten Gel�nde (Schnitzeljagd)   
Bei dieser Variante sind die Albstones genau an der Lokation "versteckt", die auch auf der Webseite steht.  
Die Herausforderung ist den Stein �ber die Koordinaten (L�ngengrad, Breitengrad) zu finden und dann einen Scan durchzuf�hren.  
Bei dem Scan kann man eine pers�nliche Nachricht hinterlegen. Der Stein bleibt jedoch an der Lokation.  
Man kann daf�r auch einen gro�en Stein w�hlen (> 100KG) auf dem der QR Code hinterlegt ist.  
Somit ist sichergestellt, dass der Stein nicht bewegt wird.  

## Anreiz
Um den Anreiz f�r die Suche nach den Albstones zu erh�hen kann auf die jeweilige Addresse des Albstones ein Bitcoin Betrag geschickt werden.  
Damit kann man auf der Blockchain verifizieren, dass der Albstone noch nicht gefunden wurde.  
Jemand mit dem technischen Wissen und dem Zugriff auf einen physischen Albstone k�nnte den Bitcoin Betrag einl�sen bzw. transferieren.  
Der Prim�re Anreiz sollte jedoch sein, den Albstone zu finden und mit einem Eintrag auf der Webseite den Fund zu dokumentieren.

# Container Image bauen

```
docker login -u bihalu docker.io

docker build --build-arg VERSION=0.1.0 --build-arg ASSEMBLY_VERSION=0.1.0 -t docker.io/bihalu/albstones:0.1.0 -f WebApp/Dockerfile .

docker push docker.io/bihalu/albstones:0.1.0
```
