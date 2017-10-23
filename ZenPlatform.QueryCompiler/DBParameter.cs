using ZenPlatform.QueryCompiler.Schema;

namespace ZenPlatform.QueryCompiler
{



    /// Специально не использовал DbParameter, чтобы убрать зависимость
    /// DbParameter - имплементация параметра для выполнения запроса на СУБД
    /// DBParameter - имплементация параметра для генерирования запроса внутри компилятора запросов
    /// Необходимо смотреть на производительность, наверняка есть резон связать компиляцию запросов и конструирование команды DbCommand

    public class DBParameter : DBClause
    {
        private readonly string _name;

        public DBParameter(string name, DBType type)
        {
            _name = name;
        }

        public DBParameter(string name, object value)
        {
            _name = name;
        }

        public DBParameter(string name, DBType type, bool isNullable)
        {
            _name = name;
        }

        public string Name => _name;

        public object Value { get; set; }

        public override string Compile(bool recompile = false)
        {
            return _name;
        }

        public override bool Equals(object obj)
        {
            var parameter = obj as DBParameter;
            if (parameter is null) return false;

            return parameter.Name.ToLower() == this.Name.ToLower();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}