//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.Dynamic;
//using System.Linq;
//using System.Runtime.InteropServices;
//using Newtonsoft.Json.Converters;
//using Newtonsoft.Json.Linq;
//using SqlPlusDbSync.Configuration.Configuration;
//using SqlPlusDbSync.Platform.Configuration;


//namespace SqlPlusDbSync.Platform
//{
//    /// <summary>
//    /// Platform dynamic object
//    /// Like expando object, but have some features
//    /// </summary>
//    public class PObject
//    {
//        private readonly PType _sobject;
//        private readonly Dictionary<string, object> _properties;

//        public PObject()
//        {
//            _properties = new Dictionary<string, object>();
//        }

//        public PObject(PType sobject)
//        {
//            _sobject = sobject;
//            _properties = new Dictionary<string, object>();
//            Init();
//        }

//        public string ObjectName { get; set; }
//        public object this[string name]
//        {
//            get
//            {
//                return this.GetPObjectValue(name.ToLower());
//            }
//            set
//            {
//                _properties[name.ToLower()] = PlatformHelper.GetContainer(value);
//            }
//        }
//        public Dictionary<string, object> Properties
//        {
//            get { return _properties; }
//        }
//        public void CompareAndMark(PObject obj2)
//        {
//            var idOwner = this[PlatformHelper.IdServeceField]?.ToString();
//            var idCompare = obj2[PlatformHelper.IdServeceField]?.ToString();

//            if (string.IsNullOrEmpty(idOwner) && !string.IsNullOrEmpty(idCompare))
//            {
//                return;
//            }

//            if (!string.IsNullOrEmpty(idOwner) && string.IsNullOrEmpty(idCompare))
//            {
//                this[PlatformHelper.ActionServeceField] = PlatformHelper.InsertAction;
//            }

//            if (idOwner != idCompare) return;

//            this[PlatformHelper.ActionServeceField] = PlatformHelper.UpdateAction;
//            var keys = _properties.Keys.Union(obj2._properties.Keys).Distinct().ToList();
//            foreach (var pKey in keys)
//            {
//                if (this[pKey] == null)
//                {
//                    this[pKey] = obj2[pKey];

//                    if (this[pKey] is PObject)
//                        (this[pKey] as PObject)[PlatformHelper.ActionServeceField] =
//                            PlatformHelper.DeleteAction;

//                    else if (this[pKey] is List<PObject>)
//                    {
//                        List<PObject> list = this[pKey] as List<PObject>;

//                        foreach (var item in list)
//                        {
//                            item[PlatformHelper.ActionServeceField] = PlatformHelper.DeleteAction;
//                        }
//                    }
//                }
//                else if (obj2[pKey] == null)
//                {
//                    if (this[pKey] is PObject)
//                        (this[pKey] as PObject)[PlatformHelper.ActionServeceField] =
//                            PlatformHelper.InsertAction;
//                    else if (this[pKey] is List<PObject>)
//                    {
//                        List<PObject> list = this[pKey] as List<PObject>;

//                        foreach (var item in list)
//                        {
//                            item[PlatformHelper.ActionServeceField] = PlatformHelper.InsertAction;
//                        }
//                    }
//                }

//                else
//                {
//                    if (this[pKey] is PObject && obj2[pKey] is PObject)
//                    {
//                        var o1 = this[pKey] as PObject;
//                        var o2 = obj2[pKey] as PObject;

//                        o1.CompareAndMark(o2);
//                    }

//                    if (this[pKey] is List<PObject> && obj2[pKey] is List<PObject>)
//                    {
//                        var o1 = this[pKey] as List<PObject>;
//                        var o2 = obj2[pKey] as List<PObject>;

//                        var keysO1 = o1.Select(x => x[PlatformHelper.IdServeceField].ToString()).ToList();
//                        var keysO2 = o2.Select(x => x[PlatformHelper.IdServeceField].ToString()).ToList();

//                        var ao1 = o1.ToArray();
//                        var ao2 = o2.ToArray();

//                        foreach (var o1Item in ao1)
//                        {
//                            foreach (var o2Item in ao2)
//                            {
//                                o1Item.CompareAndMark(o2Item);

//                                var idO1 = o1Item[PlatformHelper.IdServeceField].ToString();
//                                var idO2 = o2Item[PlatformHelper.IdServeceField].ToString();

//                                if (string.IsNullOrEmpty(idO1) != null && !keysO2.Contains(idO1))
//                                {
//                                    o1Item[PlatformHelper.ActionServeceField] = PlatformHelper.InsertAction;
//                                }

//                                if (!string.IsNullOrEmpty(idO2) && !keysO1.Contains(idO2))
//                                {
//                                    o2Item[PlatformHelper.ActionServeceField] = PlatformHelper.DeleteAction;
//                                    o1.Add(o2Item);

//                                }
//                            }
//                        }
//                    }
//                }
//            }
//        }
//        private void Init()
//        {
//            if (_sobject is TableType)
//            {
//                this[PlatformHelper.ObjectKeyField] = _sobject.GetIdentity().Name;
//                return;
//            }

//            foreach (var relation in _sobject.Relations)
//            {
//                this[relation.Type.Name] = new List<PObject>();
//            }

//            var dynObj = new PObject(_sobject.Source);
//            this[_sobject.Source.Name] = dynObj;
//        }

//        public static PObject FromJson(JObject json, PType pType)
//        {
//            if (pType is null) throw new NullReferenceException();

//            var dynObj = new PObject(pType);

//            if (pType is TableType)
//            {
//                var to = pType as TableType;

//                foreach (JProperty token in json["Properties"])
//                {
//                    dynObj[token.Name] = token.Value;
//                }

//                var table = new PObject();
//                table.ObjectName = to.Table.Name;

//                dynObj[to.Table.Name] = table;

//                foreach (JProperty field in json["Properties"][to.Table.Name]["Properties"])
//                {
//                    table[field.Name] = field.Value.ToString();
//                }

//                return dynObj;
//            }

//            foreach (JProperty token in json["Properties"])
//            {
//                if (dynObj[token.Name] is List<PObject>)
//                    foreach (var item in token.Value)
//                    {
//                        (dynObj[token.Name] as List<PObject>).Add(FromJson(item as JObject,
//                            pType.Relations.Find(x => x.Type.Name == token.Name).Type));
//                    }
//                else
//                {
//                    dynObj[token.Name] = token.Value.ToString();
//                }

//            }

//            dynObj[pType.Source.Name] = FromJson(json["Properties"][pType.Source.Name] as JObject, pType.Source);

//            return dynObj;
//        }
//    }
//}