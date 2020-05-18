using System;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Aquila.WebServiceCore
{
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