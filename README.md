# OR-Mapper - C# PostgreSQL Database - Zulli Maximiliano Marino

## Allgemeines

Um das Beispiel und die UnitTests ausführen zu können muss eine PostgreSQL Datenbank gestartet werden. Um diese zu erstellen liegt im Hauptordner die Datei "CREATE_DATABASE.sql". Durch das ausführen dieses SQL Scripts wird die Datenbank automatisch erstellt welche gebraucht wird. Für die DemoAnwendung und die Tests wir die selbe Datenbank verwendet - was nicht zu probleme führen sollte.

Dieses Framework unterstützt folgende Features:
- Persistieren von Objekten in der Datenbank
- Persistierung von Änderungen an Objekten in der Datenbank
- Caching beim speichern und auslesen von Objekten
- durch den TrackingCache Änderungsverfolgung
- Locking von Objekten, damit diese nicht von anderen Datenbankverbindungen verändert werden können während sie bearbeitetwerden
- 1:1 Beziehungen
- 1:n Beziehungen
- n:m Beziehungen
- Vererbung (hierbei müssen die Tabellen in der Datenbank richtig konfiguriert sein)

# Verwendung

## Speichern von Entitäten / Klassen in der Datenbank

### Tabellen Attribut / Table attribute
Um eine Klasse als Tabelle zu deklarieren gibt es das [Table] Attribut. Automatisch wird hier der Name der Klasse dem Namen der Tabelle in der Datenbank gleichgestellt, allerdings kann der Name auch folgendermaßen explizit angegeben werden: [Table(TableName = "Student")].
Das Attribut muss und kann in beiden Fällen vor dem namen der Klasse geschrieben werden.

Vererbung ist möglich indem die erbende Klasse und die Elternklasse jeweils das [Table] Attribut deklarieren. Wichtig hierbei ist allerdings, in der Datenbank (PostgreSQL) auch die Tabellen als Child und Parenttable zu definieren, da es sonst zu undefiniertem Verhalten kommen kann, vor allem wenn für die Kindertabelle der selbe Primarikey wie für die Elterntabelle verwendet wird.

### Primärschlüssel Attribut / PrimaryKey Attribute
Um den Primärschlüssel einer Klasse / Entität / Tabelle zu definieren muss das [PrimaryKey] Attribut verwendet werden. Dieses Attribut wird vor die Eigenschaft der Klasse geschrieben und nimmt automatisch den Namen dieser Eigenschaft als name der Tabellenspalte in der Datenbank. Der Name der Spalte, der Typ dieser und ob sie Nullable ist kann allerdings auch spezifisch definiert werden; Beispielsweise: [PrimaryKey(ColumnName = "ID", Type = typeof(int), Nullable = false)]

### Fremdschlüssel Attribut / ForeignField Attribute
Will man eine Klasseneigenschaft als Fremdschlüssel definieren schreibt man folgendes: [ForeignKey]. Um den Namen des Fremdschlüssels in der Tabelle dieser Entität zu definieren kann man folgendes schreiben: [ForeignKey(ColumnName)]. Um z.B. bei einer N:M Beziehung in der Datenbank eine Zwischentabelle anzusprechen muss man dies Folgendermaßen schreiben: [ForeignKey(ColumnName = "FK_Course", TargetTableName = "Student_Course" , TargetColumnName = "FK_Student")]  - Hier ist "Student_Course" eine N:M Zwischentabelle und TargetColumnName ist in dieser Zwischentabelle die Spalte der dieser ForeignKey korrespondiert.

### SpaltenAttribut / Column Attribute / Field Attribute
Um eine Eigenschaft einer Klasse als Spalte zu definieren muss das [Column] Attribute verwendet werden. Dieses wird wie das PrimärschlüsselAttribut vor der Eigenschaft geschrieben und nimmt standardmäßig den Namen dieser Eigenschaft als Name der Spalte in der Datenbank. Dieser kann allerdings auch explizit definiert werden, wie folgt: [Column(ColumnName = "FirstName", Type = typeof(sting), Nullable = false)]

### Ignorieren Attribut / Ignore Attribute
Das IgnoreAttribute [Ignore] kann vor einer Eigenschaft einer Klasse geschrieben werden damit diese Eigenschaft von dem ORMapper ignoriert wird, und somit keiner Datenbanktabellen entspricht. Dies ist nützlich wenn Teile einer Klasse nicht persistiert werden sollen.

## CRUD Opterationen im Programm

### Cacheing
Durch Caching können die Datenbankaufrufe reduziert werden. In diesem Cache werden alle Objekte gespeichert welche entweder von der Datenbank geholt wurden oder in die Datenbank gespeichert wurden. Nach lokaler änderung dieser werden sie auch im Cache aktualisiert. Datenbankzugriffe sind somit minimiert.
Um den Cache des ORMappers zu nutzen muss folgendes geschrieben werden bevor andere Operationen durchgeführt werden:

  ICache cache = new TrackingCache();
  ORMapper.Cache = cache;
ODER
  ICache cache = new BasicCache();
  ORMapper.Cache = cache;

### Speichern oder Aktualisieren eines Objektes in die/der Datenbank / Insert / Update Object
Um in Objekt zu speichern oder zu aktualisieren muss folgendes geschrieben werden: "ORMapper.Save(object);". Wenn das Objekt verändert wurde (und noch den selben Primärschlüssel besitzt wie zuvor) kann es mittels der Save Funktion in der Datenbank aktuallisiert werden. Das alte Objekt wird somit gänzlich überschrieben. 

### Laden eines Objektes aus der Datenbank / Read / Get Object
Soll ein Object aus der Datenbank geladen werden benötigt man hierfür den Primärschlüssel davon und schreibt folgendes: "ORMapper.Get<T>(primaryKey);".
  
### Löschen eines Objektes aus der Datenbank / Delete / Remove Object
Um ein Objekt aus der Datenbank und dem Cache zu entfernen muss folgendes geschrieben werden: "ORMapper.Delete(object);"
  
### Locking 
Um die Veränderung von Objekten durch parallele Datenbankverbindungen zu verhindern während ein objekt lokal bearbeitet wird kann Locking verwendet werden. Hierführ muss das Objekt was vor Veränderungen geschützt werden soll folgendermaßen gesperrt werden:
  ORMapper.Lock = new Locking();
  ORMapper.LockObject(object);
Um diese sperre wieder zu entfernen:
  ORMapper.Unlock(object);
