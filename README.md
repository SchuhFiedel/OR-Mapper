# OR-Mapper - C# PostgreSQL Database - Zulli Maximiliano Marino

## Allgemeines



## Verwendung

### Speichern von Entitäten / Klassen in der Datenbank

Um eine Klasse als Tabelle zu deklarieren gibt es das [Table] Attribut. Automatisch wird hier der Name der Klasse dem Namen der Tabelle in der Datenbank gleichgestellt, allerdings kann der Name auch folgendermaßen explizit angegeben werden: [Table(TableName = "Student")].
Das Attribut muss in beiden Fällen vor dem namen der Klasse geschrieben werden.

Vererbung ist möglich indem die erbende Klasse und die Elternklasse jeweils das [Table] Attribut deklarieren. Wichtig hierbei ist allerdings, in der Datenbank (PostgreSQL) auch die Tabellen als Child und Parenttable zu definieren, da es sonst zu undefiniertem Verhalten kommen kann, vor allem wenn für die Kindertabelle der selbe Primarikey wie für die Elterntabelle verwendet wird.
