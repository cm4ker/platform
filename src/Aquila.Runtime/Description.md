### Description table

| MD Key           | Descriptor container    | 
| ---------------- | ----------------------- |
| Entity_id        | Container               |
| Entity_id2       | Container               |

Has predefined structure

```
{
 Metadata
    - Descriptor root
       - Descriptor child 1 (Main descriptor)
       - Descriptor child 2 (Some payload table)     - has no type id
       - Descriptor child 3 (Some 2nd payload table) - has no type id
       - Descriptor child 4 (Else?)                  - has type id
}
```

1. Load metadata
2. Load descriptors
3. Load assembly with solution
4. Compile Queries (SQL -> AQL) etc

```
var table_name = get_table_name(type_id);
var list = list<Entity.Link>();
var arr = array();  

var list = new List<string>;
var dict = new Dict<T,K>;
```