using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using SqlPlusDbSync.Entity.EntityObject;


namespace SqlPlusDbSync.Platform.EntityObject
{
    //public class Mapper
    //{
    //    public DTO Map(DTO dto, SType sobject, SqlDataReader reader)
    //    {


    //        if (!reader.IsClosed && reader.HasRows)
    //        {
    //            while (reader.Read())
    //            {
    //                var path = new string[0];
    //                MapObject(dto, sobject, reader, path, false);
    //            }
    //        }
    //        else
    //        {
    //            dto.IsDeleted = true;
    //        }

    //        return dto;
    //    }

    //    public DTO MapGraph(SType sobject, SqlDataReader reader, Type type)
    //    {
    //        var entity = Activator.CreateInstance(type) as DTO;

    //        if (!reader.IsClosed && reader.HasRows)
    //        {
    //            while (reader.Read())
    //            {
    //                var path = new string[0];
    //                MapObject(entity, sobject, reader, path, true);
    //            }
    //        }

    //        return entity;
    //    }

    //    private void MapRelations(object entity, SType sobject, SqlDataReader reader, string[] path, bool graphOnly)
    //    {
    //        var entityType = entity.GetType();

    //        foreach (var rel in sobject.Relations)
    //        {
    //            var relObjectProp = entityType.GetProperty(rel.Type.Name);
    //            var relObjectType = relObjectProp.PropertyType;

    //            if (relObjectType.IsGenericType)
    //                relObjectType = relObjectType.GetGenericArguments()[0];

    //            var newPath = path.ToList();
    //            newPath.Add(sobject.Name);


    //            var collection = relObjectProp.GetValue(entity, null) as IList;
    //            if (collection is null)
    //            {
    //                collection = Activator.CreateInstance(relObjectProp.PropertyType) as IList;
    //                relObjectProp.SetValue(entity, collection, null);
    //            }

    //            var newInstance = Activator.CreateInstance(relObjectType) as DTO;
    //            MapObject(newInstance, rel.Type, reader, newPath.ToArray(), graphOnly);
    //            if (newInstance.Key is null) continue;
    //            var index = collection.IndexOf(newInstance);
    //            if (index >= 0)
    //                newInstance = collection[index] as DTO;
    //            else
    //                collection.Add(newInstance);
    //            if (!(rel.Type is TableType))
    //                MapObject(newInstance, rel.Type, reader, GetNewPath(path, sobject.Name), graphOnly);

    //        }
    //    }
    //    private void MapObject(object entity, SType sobject, SqlDataReader reader, string[] path, bool graphOnly)
    //    {
    //        DTO ent = entity as DTO;

    //        sobject = sobject ?? throw new ArgumentNullException("sobject");
    //        reader = reader ?? throw new ArgumentNullException("reader");

    //        ent.Register = true;

    //        var entityType = entity.GetType();
    //        var pathString = path.Aggregate("", (s, s1) => s += s1 + ".");
    //        foreach (var field in sobject.Fields.OrderByDescending(x => x.IsIdentifier))
    //        {
    //            var value = reader[pathString + field.GetFullName()];
    //            if (value is DBNull) value = null;
    //            var prop = entityType.GetProperty(field.Name);
    //            if (prop is null) throw new Exception($"Property {field.Name} not found in type {entityType.Name}");

    //            if (field.IsIdentifier)
    //            {
    //                if (value == GetDefault(prop.PropertyType) || value is null)
    //                {
    //                    return;
    //                }
    //                ent.Key = value;
    //            }
    //            prop.SetValue(entity, value, null);


    //            //First field is identifier (order by desc by bool field)
    //            //We can break cycle if  it is get graph only
    //            if (graphOnly)
    //                break;
    //        }

    //        if (sobject is TableType) return;

    //        MapRelations(entity, sobject, reader, path, graphOnly);
    //    }

    //    private object GetDefault(Type type)
    //    {
    //        if (type.IsValueType)
    //        {
    //            return Activator.CreateInstance(type);
    //        }
    //        return null;
    //    }
    //    private string GetPath(string[] path, string addToPath = "")
    //    {
    //        return path.Aggregate("", (s, s1) => s += s1 + ".", (s) => s += addToPath).Trim('.');
    //    }
    //    private string[] GetNewPath(string[] path, string addToPath)
    //    {
    //        List<string> listPath = path.ToList();
    //        listPath.Add(addToPath);
    //        return listPath.ToArray();
    //    }
    //}
}