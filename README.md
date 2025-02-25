# MagicMaze
Konsolenbasiertes Labyrinth das sich beim Erforschen selbst entwickelt und in den Räumen einige Rätsel zum Lösen enthält. Erstellt mit ChatGPT. Ursprünglich zur Erprobung des Fabrik-Pattern.

<b><h2>Schnellstart</h2></b><br>
Der ganze Code befindet sich in der Datei Program.cs.<br>
Sie kann im Microsoft Visual Studio direkt mit F5 gestartet werden.
<b><h2>Beschreibung</h2></b><br>
Das Labyrinth entwickelt sich beim Erforschen nach dem Zufallsprinzip.<br>
Es setzt sich aus folgenden Elementen zusammen:<br>
* Räume (Chamber)
* Korridore (Corridor)
* T-Abzweigungen (TieIntersection)
* Kreuzungen (Crossintersection)

Die Räume enthalten die Rätsel und deren Schlüssel.<br>
Die Räume müssen durchsucht und die Schlüssel zu den Rätsel gebracht werden.<br>
Wird in einem Raum ein Schlüssel gefunden, kann er in den Rucksack gepackt und in den Raum mit dem Rätsel gebracht werden.<br>
Beim Erkunden verbindet das Labyrinth manchmal einen Gang mit einem bestehenden Element, so das man sich unverhofft dort wiederfindet wo man gestartet ist.<br>
Ebenso hat es in den Korridoren Wände die keine Räume enthalten oder Verzweigungen die in einer Sackgasse münden.<br>
Dann wird man auf das Element zurückgeworfen wo man herkam. Einfach nicht beirren lassen.<br>
Es gibt aber keine Garantie das alle Wege des Labyrinths zu allen Rätsel führen.<br>
Hat der Spieler Einfluss auf das Labyrinth? Damned if I'd know!

<b><h2>Anfänger-Tips</h2></b><br>
* <h4>Rätsel</h4>Die Rätselaufgaben bestehen darin, Dinge die man mitnehmen kann in den richtigen Raum zu bringen. Z.B.: Den Bezinkanister zum Auto. Schon ist ein Rätsel gelöst.
* <h4>Fortschritt handschriftlich notieren.</h4>Da sich die Optionen für die Fortbewegung über ein Element, an den Verbindungspunkten orientieren und nicht an den Richtungen, ändern sie sich, einmal verbunden, für den Rest des Spiels nicht mehr. Ebenso die ID's der Elemente. So können die Elemente leicht als Quadrate mit ID und Verbindungspunkte notiert werden. Tip: Immer im Gegenuhrzeigersinn.<br>
&#160;&#160;&#160;&#160;&#160;&#160;1&#160;&#124;<br>
&#160;&#160;&#160;&#160;&#45;&#124;&#45;&#45;&#45;&#45;&#124;&#45;<br>
&#45;&#160;2&#160;&#124;&#160;ID&#160;&#124;&#160;4&#160;&#45;<br>
&#160;&#160;&#160;&#160;&#45;&#124;&#45;&#45;&#45;&#45;&#124;&#45;<br>
&#160;&#160;&#160;&#160;&#160;&#160;3&#160;&#124;<br>

* <h4>Gefühl für den Zufallsgenerator</h4>Kann das Labyrinth durch Autosuggestion beeinflusst werden? Ich sage ja. Richtungsänderungen können dafür sorgen, dass die Korridore nicht irgendwann wieder dort münden wo man begonnen hat.  
