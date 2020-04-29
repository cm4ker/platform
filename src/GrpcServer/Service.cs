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
        
     + RootOfTheService
        + MyNamedService
            +Method1
                + ReturnsType
                    + Type
                + Arguments
                    + Arg1
                        + Type
                    + Arg2
                        + Type
                    + Arg3
                        + Type
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


    [ServiceContract]
    public interface ISampleService
    {
        [OperationContract]
        string Ping(string s);

        [OperationContract]
        ComplexModelResponse PingComplexModel(ComplexModelInput inputModel);

        [OperationContract]
        void VoidMethod(out string s);

        [OperationContract]
        Task<int> AsyncMethod();

        [OperationContract]
        int? NullableMethod(bool? arg);

        [OperationContract]
        void XmlMethod(System.Xml.Linq.XElement xml);
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


    public class SampleService : ISampleService
    {
        public string Ping(string s)
        {
            Console.WriteLine("Exec ping method");
            return s;
        }

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

        public void VoidMethod(out string s)
        {
            s = "Value from server";
        }

        public Task<int> AsyncMethod()
        {
            return Task.Run(() => 42);
        }

        public int? NullableMethod(bool? arg)
        {
            return null;
        }

        public void XmlMethod(XElement xml)
        {
            Console.WriteLine(xml.ToString());
        }
    }
}