using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GrpcServer
{
    /*
     
     AvailableTypes
        string
        boolean
        byte[]
        datetime
        int
        long
        double
        complex
        
    + Root
        + SerializableTypes
             + MyComplexType
                    + MyProperty

        + Web-Services
            + MyNamedService
                +Method1
                    + ReturnsType
                        + MyComplexType
                    + Arguments
                        + Arg1
                            + string
                        + Arg2
                            + int
                        + Arg3
                            + MyComplexType
                +Method2
                    ...
                +Method3
                    ...
     */

    [DataContract]
    public class ComplexModelInput
    {
        [DataMember] public string StringProperty { get; set; }

        [DataMember] public int IntProperty { get; set; }

        [DataMember] public List<string> ListProperty { get; set; }

        [DataMember] public DateTimeOffset DateTimeOffsetProperty { get; set; }

        [DataMember] public List<ComplexObject> ComplexListProperty { get; set; }
    }

    [DataContract]
    public class ComplexObject
    {
        [DataMember] public string StringProperty { get; set; }

        [DataMember] public int IntProperty { get; set; }
    }

    [DataContract]
    public class ComplexModelResponse
    {
        [DataMember] public float FloatProperty { get; set; }

        [DataMember] public string StringProperty { get; set; }

        [DataMember] public List<string> ListProperty { get; set; }

        [DataMember] public DateTimeOffset DateTimeOffsetProperty { get; set; }

        [DataMember] public TestEnum TestEnum { get; set; }
    }

    public enum TestEnum
    {
        One,
        Two
    }


    [ServiceContract]
    public class SampleService
    {
        [OperationContract]
        public string Ping(string s)
        {
            Console.WriteLine("Exec ping method");
            return s;
        }

        [OperationContract]
        public ComplexModelResponse PingComplexModel(ComplexModelInput inputModel)
        {
            Console.WriteLine("Input data. IntProperty: {0}, StringProperty: {1}", inputModel.IntProperty,
                inputModel.StringProperty);

            return new ComplexModelResponse
            {
                FloatProperty = float.MaxValue / 2,
                StringProperty = inputModel.StringProperty,
                ListProperty = inputModel.ListProperty,
                DateTimeOffsetProperty = inputModel.DateTimeOffsetProperty
            };
        }

        [OperationContract]
        public void VoidMethod(out string s)
        {
            s = "Value from server";
        }

        [OperationContract]
        public Task<int> AsyncMethod()
        {
            return Task.Run(() => 42);
        }

        [OperationContract]
        public int? NullableMethod(bool? arg)
        {
            return null;
        }

        [OperationContract]
        public void XmlMethod(XElement xml)
        {
            Console.WriteLine(xml.ToString());
        }
    }
}