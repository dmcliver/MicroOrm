This is a crappy micro orm, and as such I do not recommend using it in a production environment.

Usage example:
SqlManager manager = new SqlManager("ConnectionStringName");
IEnumerable<MyEntity> entities = manager.ExecQuery("SELECT * FROM MyEntity");