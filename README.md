# OR-Mapper - C# PostgreSQL Database - Zulli Maximiliano Marino

## Allgemeines



## Verwendung

### Speichern von Entitäten / Klassen in der Datenbank

#### Tabellen Attribut / Table attribute
Um eine Klasse als Tabelle zu deklarieren gibt es das [Table] Attribut. Automatisch wird hier der Name der Klasse dem Namen der Tabelle in der Datenbank gleichgestellt, allerdings kann der Name auch folgendermaßen explizit angegeben werden: [Table(TableName = "Student")].
Das Attribut muss und kann in beiden Fällen vor dem namen der Klasse geschrieben werden.

Vererbung ist möglich indem die erbende Klasse und die Elternklasse jeweils das [Table] Attribut deklarieren. Wichtig hierbei ist allerdings, in der Datenbank (PostgreSQL) auch die Tabellen als Child und Parenttable zu definieren, da es sonst zu undefiniertem Verhalten kommen kann, vor allem wenn für die Kindertabelle der selbe Primarikey wie für die Elterntabelle verwendet wird.

#### Primärschlüssel Attribut / PrimaryKey Attribute
Um den Primärschlüssel einer Klasse / Entität / Tabelle zu definieren muss das [PrimaryKey] Attribut verwendet werden. Dieses Attribut wird vor die Eigenschaft der Klasse geschrieben und nimmt automatisch den Namen dieser Eigenschaft als name der Tabellenspalte in der Datenbank. Der Name der Spalte, der Typ dieser und ob sie Nullable ist kann allerdings auch spezifisch definiert werden; Beispielsweise: [PrimaryKey(ColumnName = "ID", Type = typeof(int), Nullable = false)]

#### Fremdschlüssel Attribut / ForeignField Attribute
Will man eine Klasseneigenschaft als Fremdschlüssel definieren schreibt man folgendes: [ForeignKey]. Um den Namen des Fremdschlüssels in der Tabelle dieser Entität zu definieren kann man folgendes schreiben: [ForeignKey(ColumnName)]. Um z.B. bei einer N:M Beziehung in der Datenbank eine Zwischentabelle anzusprechen muss man dies Folgendermaßen schreiben: [ForeignKey(ColumnName = "FK_Course", TargetTableName = "Student_Course" , TargetColumnName = "FK_Student")] 

#### SpaltenAttribut / Column Attribute / Field Attribute
Um eine Eigenschaft einer Klasse als Spalte zu definieren muss das [Column] Attribute verwendet werden. Dieses wird wie das PrimärschlüsselAttribut vor der Eigenschaft geschrieben und nimmt standardmäßig den Namen dieser Eigenschaft als Name der Spalte in der Datenbank. Dieser kann allerdings auch explizit definiert werden, wie folgt:
[Column(ColumnName = "FirstName", Type = typeof(sting), Nullable = false)]

#### Ignorieren Attribut / Ignore Attribute
